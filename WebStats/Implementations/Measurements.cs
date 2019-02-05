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
         

        /// <summary>
        /// Returns the average response body size of responses seen since application start.
        /// </summary>
        /// <returns></returns>
        public double GetAverageResponseSize()
        {
            var responseSizeList = GetResponseSizesList();
            if (responseSizeList.Count == 0)
            {
                return 0;
            }
            return responseSizeList.Average();
        }

        /// <summary>
        /// Returns the current response body size.
        /// </summary>
        /// <returns></returns>
        public double GetCurrentResponseSize()
        {
            var responseSizeList = GetResponseSizesList();
            if (responseSizeList.Count == 0)
            {
                return 0;
            }
            return responseSizeList.Last();
        }


        /// <summary>
        /// Returns the largest response body size of responses seen since application start.
        /// </summary>
        /// <returns></returns>
        public long GetMaxResponseSize()
        {
            var responseSizeList = GetResponseSizesList();
            if (responseSizeList.Count == 0)
            {
                return 0;
            }
            return responseSizeList.Max();
        }


        /// <summary>
        /// Returns the smallest, non-zero response body size of responses seen since application start.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the amount of time, in milliseconds, since processing started for the WebStats module
        /// for the given request.
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public double GetModuleProcessingTime(string requestID)
        {
            if (!StateStore.ContainsKey($"ModuleStart{requestID}"))
            {
                return 0;
            }
            var counter = (Stopwatch)StateStore[$"ModuleStart{requestID}"];
            counter.Stop();
            return counter.Elapsed.TotalMilliseconds;
        }

        /// <summary>
        /// Gets the amount of time, in milliseconds, since processing started for the given request.
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public double GetRequestProcessingTime(string requestID)
        {
            if (!StateStore.ContainsKey($"RequestStart{requestID}"))
            {
                return 0;
            }
            var counter = (Stopwatch)StateStore[$"RequestStart{requestID}"];
            counter.Stop();
            return counter.Elapsed.TotalMilliseconds;
        }


        /// <summary>
        /// Begin counting elapsed time since processing started for the WebStats
        /// module for the given request.
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public Stopwatch LogModuleStart(string requestID)
        {
            var counter = Stopwatch.StartNew();
            StateStore[$"ModuleStart{requestID}"] = counter;
            return counter;
        }

        /// <summary>
        /// Begin counting elapsed time since processing started for the given request.
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public Stopwatch LogRequestStart(string requestID)
        {
            var counter = Stopwatch.StartNew();
            StateStore[$"RequestStart{requestID}"] = counter;
            return counter;
        }


        /// <summary>
        /// Adds the given response body size to the list in the StateStore.
        /// </summary>
        /// <param name="responseSize"></param>
        public void LogResponseSize(long responseSize)
        {
            var responseSizeList = GetResponseSizesList();
            responseSizeList.Add(responseSize);
        }

        /// <summary>
        /// Trims the length of the response sizes list in the StateStore
        /// to keep it from growing indefinitely.
        /// </summary>
        public void TrimResponseSizeList()
        {
            var responseSizeList = GetResponseSizesList();
            while (responseSizeList.Count > 5000)
            {
                responseSizeList.RemoveAt(responseSizeList.Count - 1);
            }
        }

        /// <summary>
        /// Returns the list of response sizes from the StateStore.
        /// </summary>
        /// <returns></returns>
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
