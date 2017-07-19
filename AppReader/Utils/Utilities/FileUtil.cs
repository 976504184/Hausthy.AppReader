using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hausthy.AppReader.Utils
{
    public static class FileUtil
    {
        /// <summary>
        /// 获取文件扩展名
        /// </summary>
        /// <param name="filePath">文件名称</param>
        /// <returns></returns>
        public static string GetFileExtension(string filePath)
        {
            if (filePath.isNullOrEmpty())
                return "";
            var strExtension = Path.GetExtension(filePath);
            return strExtension;
        }
    }
}
