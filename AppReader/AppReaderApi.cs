using Hausthy.AppReader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppReader
{
    public static class Api
    {
        public static object GetAppInfo(string appPath)
        {
            object result = null;
            var strExtension = FileUtil.GetFileExtension(appPath);

            if (strExtension.isNullOrEmpty()) return null;
           
            switch (strExtension.ToLower())
            {
                case ".apk":
                    result = null;
                    break;
                case ".ipa":
                    result = null;
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
