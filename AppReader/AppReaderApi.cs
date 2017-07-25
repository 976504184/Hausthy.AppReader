using Hausthy.AppReader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hausthy.AppReader
{
    public static class AppReaderApi
    {
        public static object GetAppInfo(string appPath)
        {
            object result = null;
            var strExtension = FileUtil.GetFileExtension(appPath);

            if (strExtension.isNullOrEmpty()) return null;

            switch (strExtension.ToLower())
            {
                case ".apk":
                    var readerApi = new ApkReader();
                    result = readerApi.ZipApk(appPath);
                    break;
                case ".ipa":
                    var readerIpaApi = new IpaReader();
                    result = readerIpaApi.ZipIpa(appPath);
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
