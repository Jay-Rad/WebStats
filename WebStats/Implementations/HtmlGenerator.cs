using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebStats.Interfaces;

namespace WebStats.Implementations
{
    /// <summary>
    /// Generates the HTML content for the WebStats widget.
    /// </summary>
    public class HtmlGenerator : IHtmlGenerator
    {
        public HtmlGenerator(IMeasurements measurements)
        {
            Measurements = measurements;
        }

        private IMeasurements Measurements { get; }

        private string WidgetContent { get; set; }


        /// <summary>
        /// Returns an HTML string to insert into the response body that will display
        /// the info gathered by WebStats.
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public string GetStatsWidget(string requestID)
        {
            if (string.IsNullOrWhiteSpace(WidgetContent))
            {
                using (var mrs = Assembly.GetExecutingAssembly().GetManifestResourceStream("WebStats.Resources.StatsWidget.html"))
                {
                    using (var sr = new StreamReader(mrs))
                    {
                        WidgetContent = sr.ReadToEnd();
                    }
                }
            }

            var current = Measurements.GetCurrentResponseSize();
            var min = Measurements.GetMinResponseSize();
            var avg = Measurements.GetAverageResponseSize();
            var max = Measurements.GetMaxResponseSize();
            var moduleTime = Measurements.GetModuleProcessingTime(requestID);
            var requestTime = Measurements.GetRequestProcessingTime(requestID);

            return WidgetContent
                .Replace("{currentResponseSize}", string.Format("{0:n0}", current))
                .Replace("{minResponseSize}", string.Format("{0:n0}", min))
                .Replace("{averageResponseSize}", string.Format("{0:n}", avg))
                .Replace("{maxResponseSize}", string.Format("{0:n0}", max))
                .Replace("{moduleTime}", moduleTime)
                .Replace("{requestTime}", requestTime);
        }
    }
}
