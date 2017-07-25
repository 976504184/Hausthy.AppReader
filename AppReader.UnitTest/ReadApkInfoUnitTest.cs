using Xunit;

namespace Hausthy.AppReader.UnitTest
{
    public class ReadApkInfoUnitTest
    {
        public static string apkPath = @"C:\Users\haiyang\Desktop\source-armeabi-debug.apk";

        public static string ipaPath = @"C:\Users\haiyang\Desktop\Ecm.ipa";

        [Fact]
        public void ReadApkInfo()
        {
            var result = AppReaderApi.GetAppInfo(apkPath);
            Assert.NotNull(result);
        }

        [Fact]
        public void ReadIpaInfo()
        {
            var result = AppReaderApi.GetAppInfo(ipaPath);
            Assert.NotNull(result);
        }
    }
}
