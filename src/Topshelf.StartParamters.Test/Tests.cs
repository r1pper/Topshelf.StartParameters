using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using Xunit;

namespace Topshelf.StartParamters.Test
{ 

    public class Tests
    {
        [Fact]
        public void DirectRun()
        {
            string path = GetServicePath();
            var temp = Path.GetTempFileName();
            Process process = null;
            try
            {
                var args = $"-path \"{temp}\" -action hello -manual directManual -tsspauto directAuto -constant directConstant";

                process = Process.Start(path, args);
                process.WaitForExit(10000);

                var returnValues = File.ReadAllText(temp);
                string[] result = ParseResult(returnValues);

                Assert.Equal(4, result.Length);
                Assert.Equal("hello", result[0]);
                Assert.Equal("directManual", result[1]);
                Assert.Equal("directAuto", result[2]);
                Assert.Equal("directConstant", result[3]);
            }
            finally
            {
                File.Delete(temp);
            }
        }

        [Fact]
        public void InstallRun()
        {
            string path = GetServicePath();
            var temp = Path.GetTempFileName();
            Process process = null;

            var installArgs = $"install -instance test -setpath \"{temp}\" -setaction installedHello -setmanual installedManual -auto installedAuto";
            var uninstallArgs = "uninstall - instance test";

            try
            {

                process = Process.Start(path, installArgs);
                process.WaitForExit(10000);

                var controller = new ServiceController("TSSP$Test");
                controller.Start();
                controller.WaitForStatus(ServiceControllerStatus.Stopped);

                var returnValues = File.ReadAllText(temp);
                string[] result = ParseResult(returnValues);

                Assert.Equal(4, result.Length);
                Assert.Equal("INSTALLEDHELLO", result[0]);
                Assert.Equal("installedManual", result[1]);
                Assert.Equal("installedAuto", result[2]);
                Assert.Equal("hello", result[3]);
            }
            finally
            {
                File.Delete(temp);
                process = Process.Start(path, uninstallArgs);
                process.WaitForExit(10000);
            }
        }

        private static string GetServicePath()
        {
            return File.ReadAllText("dummyPath.txt").Trim();
        }

        private static string[] ParseResult(string returnValues)
        {
            return Environment.OSVersion.Platform.ToString().StartsWith("Win") ?
                returnValues.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries) :
                 returnValues.Split('\n');
        }
    }
}
