using Xunit;

namespace Hausthy.AppReader.UnitTest
{
    public class ReadApkInfoUnitTest
    {
        public static string ApkPath = @"C:\Users\haiyang\Downloads\com.happyelements.AndroidAnimal.qq_1.47_47.apk";

        public static string IpaPath = @"F:\i4Tools7\App\携程旅行-订票出行必备_7.6.0（正版）.ipa";

        [Fact]
        public void ReadApkInfo()
        {
            var result = AppReaderApi.GetAppInfo(ApkPath);
            Assert.NotNull(result);
        }

        [Fact]
        public void ReadIpaInfo()
        {
            var result = AppReaderApi.GetAppInfo(IpaPath);
            Assert.NotNull(result);
        }
    }
}
