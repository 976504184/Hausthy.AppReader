using Hausthy.AppReader.Utils;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

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

                            if (item.Name.ToLower().Contains("info.plist"))
                            {
                                var zipEntry = zipfile.GetEntry(item.Name);
                                using (Stream strm = zipfile.GetInputStream(item))
                                {
                                    using (MemoryStream mem = new MemoryStream())
                                    {
                                        int size = 0;
                                        while (true)
                                        {
                                            byte[] buffer = new byte[4096];
                                            size = strm.Read(buffer, 0, buffer.Length);
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
                }
                Stream ms = new MemoryStream(resourcesData);
                var docs = XDocument.Load(ms);
                var keyValues = GetDictInDocument(docs);
                var ipaInfo = new IpaInfo
                {
                    Name = keyValues["CFBundleName"],
                    DisplayName = keyValues["CFBundleDisplayName"],
                    Identifier = keyValues["CFBundleIdentifier"],
                    Version = keyValues["CFBundleVersion"],
                    ShortVersionString = keyValues["CFBundleShortVersionString"]
                };
                return ipaInfo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private Dictionary<string, string> GetDictInDocument(XDocument docs)
        {
            if (_keyValues.isEmpty())
            {
                var keyValues = docs.Descendants("dict")
                .SelectMany(d => d.Elements("key").Zip(d.Elements().Where(e => e.Name != "key"), (k, v) => new { Key = k, Value = v }))
                .ToDictionary(i => i.Key.Value, i => i.Value.Value);
                _keyValues = keyValues;
            }
            return _keyValues;
        }
    }
}
