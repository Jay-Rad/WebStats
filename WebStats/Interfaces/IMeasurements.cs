using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;

namespace WebStats.Interfaces
{
    public interface IMeasurements
    {
        double GetAverageResponseSize();
        long GetMaxResponseSize();
        long GetMinResponseSize();
        string GetModuleProcessingTime(string requestID);
        string GetRequestProcessingTime(string requestID);
        void LogModuleStart(string requestID, Stopwatch counter);
        void LogRequestStart(string requestID, Stopwatch counter);
        void LogResponseSize(long responseSize);
    }
}