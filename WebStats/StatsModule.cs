using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebStats.Implementations;
using WebStats.Interfaces;

namespace WebStats
{
    /// <summary>
    /// Gathers information about request processing times and response sizes.
    /// For web page requests, stats are added to the response body as an HTML widget.
    /// </summary>
    public class StatsModule : IHttpModule
    {
        private static IDictionary<string, object> StateStore { get; } = new Dictionary<string, object>();
        private IHtmlGenerator HtmlGenerator { get; set; }
        private IRequestProcessor RequestProcessor { get; set; }

        private IMeasurements Measurements { get; set; }


        public void Context_BeginRequest(object sender, EventArgs e)
        {
            // Avoid race conditions.
            lock (StateStore)
            {
                RequestProcessor.ProcessRequestStart();
            }
        }

        public void Context_EndRequest(object sender, EventArgs e)
        {
            // Avoid race conditions.
            lock (StateStore)
            {
                RequestProcessor.ProcessRequestEnd();
            }
        }

        public void Dispose()
        {
            RequestProcessor = null;
            HtmlGenerator = null;
            Measurements = null;
        }

        public void Init(HttpApplication context)
        {
            BuildDependencies(context);
            context.BeginRequest += Context_BeginRequest;
            context.EndRequest += Context_EndRequest;
        }

        private void BuildDependencies(HttpApplication context)
        {
            Measurements = new Measurements(StateStore);
            HtmlGenerator = new HtmlGenerator(Measurements);
            RequestProcessor = new RequestProcessor(context, StateStore, Measurements, HtmlGenerator);
        }
    }
}
