using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebStats.Interfaces;

namespace WebStats.Implementations
{
    /// <summary>
    /// Provides methods for handling HTTP requests and adding statistical info
    /// to the response body.
    /// </summary>
    public class RequestProcessor : IRequestProcessor
    {
        public RequestProcessor(HttpApplication context, IDictionary<string, object> stateStore, IMeasurements measurements, IHtmlGenerator htmlGenerator)
        {
            Context = context;
            Measurements = measurements;
            HtmlGenerator = htmlGenerator;
            StateStore = stateStore;
        }
        private HttpApplication Context { get; }
        private IMeasurements Measurements { get; }
        private IHtmlGenerator HtmlGenerator { get; }
        private IDictionary<string, object> StateStore { get; set; }

        public void ProcessRequestStart()
        {
            // Add request ID header to identify this request further in the pipeline.
            var requestID = Guid.NewGuid().ToString();
            Context.Request.Headers.Add("WebStatsRequestID", requestID);

            // Log the beginning of the request processing for this request.
            Measurements.LogRequestStart(requestID, Stopwatch.StartNew());
            Context.Response.Filter = new MeasurementFilter(Context.Response.Filter);
        }


        public void ProcessRequestEnd()
        {
            // Get request ID header and remove it.
            var requestID = Context.Request.Headers["WebStatsRequestID"];
            Measurements.LogModuleStart(requestID, Stopwatch.StartNew());

            Measurements.LogResponseSize((Context.Response.Filter as MeasurementFilter).BytesWritten);
            

            if (Context.Response.ContentType == "text/html")
            {
                var statsHTML = HtmlGenerator.GetStatsWidget(requestID);

                Context.Response.Write(statsHTML);
            }

            CleanupApplicationState(requestID);
        }

        private void CleanupApplicationState(string requestID)
        {
            // Remove custom header.
            Context.Request.Headers.Remove("WebStatsRequestID");
            StateStore.Remove($"ModuleStart{requestID}");
            StateStore.Remove($"RequestStart{requestID}");

            Measurements.TrimResponseSizeList();
        }
    }
}
