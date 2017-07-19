using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hausthy.AppReader
{
    public class ApkReader
    {
        public static void ZipApk(string apkPath)
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
                        if (item.Name.ToLower() == "androidmanifest.xml")
                        {
                            manifestData = new byte[50 * 1024];
                            using (Stream strm = zipfile.GetInputStream(item))
                            {
                                strm.Read(manifestData, 0, manifestData.Length);
                            }

                        }
                        if (item.Name.ToLower() == "resources.arsc")
                        {
                            using (Stream strm = zipfile.GetInputStream(item))
                            {
                                using (BinaryReader s = new BinaryReader(strm))
                                {
                                    resourcesData = s.ReadBytes((int)s.BaseStream.Length);

                                }
                            }
                        }
                    }
                }
            }




        }



    }
}
