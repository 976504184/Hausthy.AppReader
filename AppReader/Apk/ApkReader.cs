using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Hausthy.AppReader
{
    public class ApkReader
    {
        private const int VerId = 0;
        private const int IcnId = 1;
        private const int LabelId = 2;
        private readonly string[] _verIcn = new string[3];

        // Some possible tags and attributes
        private readonly string[] _tags = { "manifest", "application", "activity" };

        public ApkInfo ZipApk(string apkPath)
        {
            byte[] manifestData = null;
            byte[] resourcesData = null;
            using (var zip = new ZipInputStream(File.OpenRead(apkPath)))
            {
                using (var filestream = new FileStream(apkPath, FileMode.Open, FileAccess.Read))
                {
                    var zipfile = new ZipFile(filestream);
                    ZipEntry item;
                    while ((item = zip.GetNextEntry()) != null)
                    {
                        if (string.IsNullOrEmpty(item.Name) && item.IsDirectory) continue;

                        if (item.Name.ToLower() != "androidmanifest.xml" && item.Name.ToLower() != "resources.arsc") continue;

                        if (item.Name.ToLower() == "androidmanifest.xml")
                        {
                            manifestData = item.Size <= 0 ? new byte[50 * 1024] : new byte[item.Size];
                            using (var strm = zipfile.GetInputStream(item))
                            {
                                strm.Read(manifestData, 0, manifestData.Length);
                            }
                        }
                        else
                        {
                            using (var strm = zipfile.GetInputStream(item))
                            {
                                using (var s = new BinaryReader(strm))
                                {
                                    resourcesData = s.ReadBytes((int)s.BaseStream.Length);

                                }
                            }
                        }
                    }
                }
            }
            var apkInfo = ExtractInfo(manifestData);
            var fileInfo = new FileInfo(apkPath);
            apkInfo.FileSize = fileInfo.Length;
            return apkInfo;
        }

        private string FuzzFindInDocument(XmlDocument doc, string tag, string attr)
        {
            foreach (var t in _tags)
            {
                var nodelist = doc.GetElementsByTagName(t);
                for (var i = 0; i < nodelist.Count; i++)
                {
                    var element = nodelist.Item(i);
                    if (element == null || element.NodeType != XmlNodeType.Element || element.Attributes == null) continue;

                    var map = element.Attributes;
                    for (var j = 0; j < map.Count; j++)
                    {
                        var element2 = map.Item(j);
                        if (element2.Name.EndsWith(attr))
                        {
                            return element2.Value;
                        }
                    }
                }
            }
            return null;
        }

        private ApkInfo ExtractInfo(byte[] manifestXml)
        {
            string strManifestXml;
            var manifest = new APKManifest();
            try
            {
                strManifestXml = manifest.ReadManifestFileIntoXml(manifestXml);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var doc = new XmlDocument();
            doc.LoadXml(strManifestXml);
            return ExtractInfo(doc);
        }

        private ApkInfo ExtractInfo(XmlDocument manifestXml)
        {
            var info = new ApkInfo();
            _verIcn[VerId] = "";
            _verIcn[IcnId] = "";
            _verIcn[LabelId] = "";
            try
            {
                var doc = manifestXml;
                if (doc == null)
                    throw new Exception("Document initialize failed");
                info.ResourcesFileName = "resources.arsx";

                // Fill up some basic fields
                info.MinSdkVersion = FindInDocument(doc, "uses-sdk", "minSdkVersion");
                info.TargetSdkVersion = FindInDocument(doc, "uses-sdk", "targetSdkVersion");
                info.VersionCode = FindInDocument(doc, "manifest", "versionCode");
                info.VersionName = FindInDocument(doc, "manifest", "versionName");
                info.PackageName = FindInDocument(doc, "manifest", "package");
                info.MainActivity = FindInDocument(doc, "activity", "name");

                int labelId;
                info.Label = FindInDocument(doc, "application", "label");
                if (info.Label.StartsWith("@"))
                    _verIcn[LabelId] = info.Label;
                else if (int.TryParse(info.Label, out labelId))
                    _verIcn[LabelId] = $"@{labelId:X4}";


                if (info.VersionCode == null)
                    info.VersionCode = FuzzFindInDocument(doc, "manifest", "versionCode");

                if (info.VersionName == null)
                    info.VersionName = FuzzFindInDocument(doc, "manifest", "versionName");
                else if (info.VersionName.StartsWith("@"))
                    _verIcn[VerId] = info.VersionName;

                var id = FindInDocument(doc, "application", "android:icon") ?? FuzzFindInDocument(doc, "manifest", "icon");

                if (id == null)
                {
                    Debug.WriteLine("icon resId Not Found!");
                    return info;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return info;
        }

        private static string FindInDocument(XmlDocument doc, string keyName, string attribName)
        {
            var usesPermissions = doc.GetElementsByTagName(keyName);

            for (var s = 0; s < usesPermissions.Count; s++)
            {
                var permissionNode = usesPermissions.Item(s);

                if (permissionNode == null || permissionNode.NodeType != XmlNodeType.Element) continue;

                var node = permissionNode.Attributes?.GetNamedItem(attribName);
                if (node != null)
                    return node.Value;

            }

            return null;
        }

    }
}
