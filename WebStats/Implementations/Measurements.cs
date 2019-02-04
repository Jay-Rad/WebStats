using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebStats.Interfaces;

namespace WebStats.Implementations
{
    /// <summary>
    /// Provides methods for measuring HTTP request processing times and response sizes.
    /// </summary>
    public class Measurements : IMeasurements
    {
        public Measurements(IDictionary<string, object> stateStore)
        {
            StateStore = stateStore;
        }
        private IDictionary<string, object> StateStore { get; }
         
        
        public double GetAverageResponseSize()
        {
            var responseSizeList = GetResponseSizesList();
            if (responseSizeList.Count == 0)
            {
                return 0;
            }
            return responseSizeList.Average();
        }

        public double GetCurrentResponseSize()
        {
            var responseSizeList = GetResponseSizesList();
            if (responseSizeList.Count == 0)
            {
                return 0;
            }
            return responseSizeList.Last();
        }

        public long GetMaxResponseSize()
        {
            var responseSizeList = GetResponseSizesList();
            if (responseSizeList.Count == 0)
            {
                return 0;
            }
            return responseSizeList.Max();
        }

        public long GetMinResponseSize()
        {
            var responseSizeList = GetResponseSizesList();
            if (responseSizeList.Count == 0)
            {
                return 0;
            }
            // Exclude any empty-bodied responses.
            return responseSizeList.Where(x => x > 0).Min();   
        }

        public string GetModuleProcessingTime(string requestID)
        {
            if (!StateStore.ContainsKey($"ModuleStart{requestID}"))
            {
                return "Not Found";
            }
            var counter = (Stopwatch)StateStore[$"ModuleStart{requestID}"];
            counter.Stop();
            return string.Format("{0:n4}", counter.Elapsed.TotalMilliseconds);
        }

        public string GetRequestProcessingTime(string requestID)
        {
            if (!StateStore.ContainsKey($"RequestStart{requestID}"))
            {
                return "Not Found";
            }
            var counter = (Stopwatch)StateStore[$"RequestStart{requestID}"];
            counter.Stop();
            return string.Format("{0:n4}", counter.Elapsed.TotalMilliseconds);
        }



        public void LogModuleStart(string requestID, Stopwatch counter)
        {
            StateStore[$"ModuleStart{requestID}"] = counter;
        }

        public void LogRequestStart(string requestID, Stopwatch counter)
        {
            StateStore[$"RequestStart{requestID}"] = counter;
        }

        public void LogResponseSize(long responseSize)
        {
            var responseSizeList = GetResponseSizesList();
            responseSizeList.Add(responseSize);
        }

        private List<long> GetResponseSizesList()
        {
            if (!StateStore.ContainsKey("ResponseSizes") ||
              !(StateStore["ResponseSizes"] is List<long>))
            {
                StateStore["ResponseSizes"] = new List<long>();
            }

            return StateStore["ResponseSizes"] as List<long>;
        }
    }
}
