using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hausthy.AppReader
{
    public class IpaInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 应用包名
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// 应用版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 应用版本名
        /// </summary>
        public string ShortVersionString { get; set; }
    }
}
