using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebStats.Implementations;
using WebStats.Interfaces;

namespace WebStats.UnitTests
{
    [TestClass]
    public class UnitTests
    {
        private IDictionary<string,object> StateStore { get; set; }
        private IMeasurements Measurements { get; set; }

        [TestInitialize]
        public void Init()
        {
            StateStore = new Dictionary<string, object>();
            Measurements = new Measurements(StateStore);
        }

        [TestMethod]
        public void MeasureTimeTests()
        {
            // Should return "Not Found" if start time hasn't been logged for request.
            Assert.AreEqual(Measurements.GetModuleProcessingTime("test"), "Not Found");
            Assert.AreEqual(Measurements.GetRequestProcessingTime("test"), "Not Found");

            var requestCounter = Stopwatch.StartNew();
            var moduleCounter = Stopwatch.StartNew();

            Measurements.LogRequestStart("test", requestCounter);
            Measurements.LogModuleStart("test", moduleCounter);

            // Add some random wait time.
            System.Threading.Thread.Sleep((int)(new Random().NextDouble() * 500));
            moduleCounter.Stop();
            System.Threading.Thread.Sleep((int)(new Random().NextDouble() * 500));
            requestCounter.Stop();

            var requestTime = Measurements.GetRequestProcessingTime("test");
            var expectedRequestTime = string.Format("{0:n4}", requestCounter.Elapsed.TotalMilliseconds);
            Assert.AreEqual(expectedRequestTime, requestTime);

            var moduleTime = Measurements.GetModuleProcessingTime("test");
            var expectedModuleTime = string.Format("{0:n4}", moduleCounter.Elapsed.TotalMilliseconds);
            Assert.AreEqual(expectedModuleTime, moduleTime);
        }

        [TestMethod]
        public void MeasureSizesTests()
        {
            // Should all be 0 before any sizes are logged.
            Assert.AreEqual(Measurements.GetCurrentResponseSize(), 0);
            Assert.AreEqual(Measurements.GetMinResponseSize(), 0);
            Assert.AreEqual(Measurements.GetAverageResponseSize(), 0);
            Assert.AreEqual(Measurements.GetMaxResponseSize(), 0);


            Measurements.LogResponseSize(500);
            Assert.AreEqual(Measurements.GetCurrentResponseSize(), 500);
            Assert.AreEqual(Measurements.GetMinResponseSize(), 500);
            Assert.AreEqual(Measurements.GetAverageResponseSize(), 500);
            Assert.AreEqual(Measurements.GetMaxResponseSize(), 500);


            Measurements.LogResponseSize(1000);
            Assert.AreEqual(Measurements.GetCurrentResponseSize(), 1000);
            Assert.AreEqual(Measurements.GetMinResponseSize(), 500);
            Assert.AreEqual(Measurements.GetAverageResponseSize(), 750);
            Assert.AreEqual(Measurements.GetMaxResponseSize(), 1000);


            // Verify trimming of response size list works.
            for (var i = 0; i < 6000; i++)
            {
                Measurements.LogResponseSize(i);
            }

            Assert.IsTrue((StateStore["ResponseSizes"] as List<long>).Count > 5000);
            Measurements.TrimResponseSizeList();
            Assert.IsTrue((StateStore["ResponseSizes"] as List<long>).Count == 5000);
        }

        [TestMethod]
        public void HtmlGeneratorTests()
        {
            var generator = new HtmlGenerator(Measurements);

            var widgetHTML = generator.GetStatsWidget("test");

            Assert.IsTrue(widgetHTML.Contains("<span id=\"requestTime\">Not Found</span>"));
            Assert.IsTrue(widgetHTML.Contains("<span id=\"moduleTime\">Not Found</span>"));
            Assert.IsTrue(widgetHTML.Contains("<span id=\"currentResponseSize\">0</span>"));
            Assert.IsTrue(widgetHTML.Contains("<span id=\"minResponseSize\">0</span>"));
            Assert.IsTrue(widgetHTML.Contains("<span id=\"averageresponseSize\">0.00</span>"));
            Assert.IsTrue(widgetHTML.Contains("<span id=\"maxResponseSize\">0</span>"));
        }

        [TestMethod]
        public void MeasurementFilterTest()
        {
            using (var ms = new MemoryStream())
            {
                var filter = new MeasurementFilter(ms);
                filter.Write(new byte[50], 0, 50);
                Assert.AreEqual(50, filter.BytesWritten);

                // BytesWritten should stay the same even after setting length.
                filter.SetLength(500);
                Assert.AreEqual(50, filter.BytesWritten);
            }
        }
    }
}
