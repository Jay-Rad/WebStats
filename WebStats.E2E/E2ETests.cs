using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Linq;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Chrome;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebStats.E2E
{
    [TestClass]
    public class E2ETests
    {
        private Process iisExpressProc { get; set; }
        private ChromeDriver ChromeDriver { get; set; }


        /// <summary>
        /// Start IIS Express with the sample site.
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            var iisExpress = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"IIS Express\iisexpress.exe");
            if (!File.Exists(iisExpress))
            {
                Assert.Fail("Couldn't find iisexpress.exe");
                return;
            }

            var sitePath = GetSampleSitePath();
            iisExpressProc = Process.Start(iisExpress, $"/path:\"{sitePath}\" /port:50578");

            ChromeDriver = new ChromeDriver();
        }

        [TestCleanup]
        public void Cleanup()
        {
            iisExpressProc?.Kill();
            ChromeDriver?.Quit();
        }


        /// <summary>
        /// Launches the sample site in IIS Express, downloads a 10,000,000-byte text file,
        /// and ensures the correct value for max size is displayed in the widget after navigating
        /// to another page.
        /// </summary>
        [TestMethod]
        public void FunctionalTest()
        {
            ChromeDriver.Url = "http://localhost:50578";
            ChromeDriver.Navigate();

            ChromeDriver.Url = "http://localhost:50578/Downloads/LargeFile.txt";
            ChromeDriver.Navigate();

            ChromeDriver.Url = "http://localhost:50578";
            ChromeDriver.Navigate();

            var maxSize = ChromeDriver.FindElementById("maxResponseSize").GetAttribute("innerHTML");

      
            Assert.AreEqual("10,000,000", maxSize);
        }


        /// <summary>
        /// Hit the web server with many requests to ensure they all get 200 codes.
        /// </summary>
        [TestMethod]
        public void TrafficTest()
        {
            var taskList = new List<Task<WebResponse>>();
            for (var i = 0; i < 500; i++)
            {
                var webClient = new WebClient();
                taskList.Add(WebRequest.CreateHttp("http://localhost:50578").GetResponseAsync());
            }
            // Wait for all tasks to finish.
            while (taskList.Any(x => !x.IsCompleted))
            {
                System.Threading.Thread.Sleep(100);
            }
            Assert.IsTrue(taskList.All(x => ((HttpWebResponse)x.Result).StatusCode == HttpStatusCode.OK));
        }

        private string GetSampleSitePath()
        {
            var currentDir = new DirectoryInfo(Environment.CurrentDirectory);
            while (!currentDir.GetFiles().Any(x => x.FullName.Contains(".sln")))
            {
                currentDir = currentDir.Parent;
            }
            return Path.Combine(currentDir.FullName, "WebStats.SampleSite");
        }
    }
}
