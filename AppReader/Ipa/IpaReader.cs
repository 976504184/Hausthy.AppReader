using Hausthy.AppReader.Utils;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Hausthy.AppReader
{
    public class IpaReader
    {
        private Dictionary<string, string> _keyValues = null;

        public IpaInfo ZipIpa(string apkPath)
        {
            byte[] resourcesData = null;
            try
            {
                using (var zip = new ZipInputStream(File.OpenRead(apkPath)))
                {
                    using (var filestream = new FileStream(apkPath, FileMode.Open, FileAccess.Read))
                    {
                        var zipfile = new ZipFile(filestream);
                        ZipEntry item;
                        while ((item = zip.GetNextEntry()) != null)
                        {
                            if (item.Name.isNullOrEmpty() && item.IsDirectory) continue;

                            if (!item.Name.ToLower().Contains("info.plist")) continue;
                            ;

                            using (var strm = zipfile.GetInputStream(item))
                            {
                                using (var mem = new MemoryStream())
                                {
                                    while (true)
                                    {
                                        var buffer = new byte[4096];
                                        var size = strm.Read(buffer, 0, buffer.Length);
                                        if (size > 0)
                                        {
                                            mem.Write(buffer, 0, size);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    resourcesData = mem.ToArray();
                                }
                            }
                        }
                    }
                }
                var plist = (Dictionary<string, object>)PlistCs.readPlist(resourcesData);
                var t = JsonConvert.SerializeObject(plist);
                var ipaInfo = new IpaInfo
                {
                    Name = GetDictionaryValue<string>(plist, "CFBundleName"),
                    DisplayName = GetDictionaryValue<string>(plist, "CFBundleDisplayName"),
                    Identifier = GetDictionaryValue<string>(plist, "CFBundleIdentifier"),
                    Version = GetDictionaryValue<string>(plist, "CFBundleVersion"),
                    ShortVersionString = GetDictionaryValue<string>(plist, "CFBundleShortVersionString")
                };
                return ipaInfo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static T GetDictionaryValue<T>(IReadOnlyDictionary<string, object> dictionary, string key)
        {
            if (dictionary == null || !dictionary.ContainsKey(key)) return default(T);
            object obj;
            return dictionary.TryGetValue(key, out obj) ? (T)obj : default(T);
        }
    }
}
