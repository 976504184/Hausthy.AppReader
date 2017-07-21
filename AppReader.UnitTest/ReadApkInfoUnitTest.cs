using Xunit;

namespace Hausthy.AppReader.UnitTest
{
    public class ReadApkInfoUnitTest
    {
        public static string apkPath = @"C:\Users\haiyang\Desktop\source-armeabi-debug.apk";

        public static string ipaPath = @"C:\Users\haiyang\Desktop\DisplayLayout2.ipa";

        [Fact]
        public void ReadApkInfo()
        {
            var readerApi = new ApkReader();
            var result = readerApi.ZipApk(apkPath);
            Assert.NotNull(result);
        }

        [Fact]
        public void ReadIpaInfo()
        {
            var readerApi = new IpaReader();
            var result = readerApi.ZipIpa(ipaPath);
            Assert.NotNull(result);
        }
    }
}
