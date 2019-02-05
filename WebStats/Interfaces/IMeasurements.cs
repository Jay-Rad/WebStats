using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;

namespace WebStats.Interfaces
{
    public interface IMeasurements
    {
        double GetAverageResponseSize();
        double GetCurrentResponseSize();
        long GetMaxResponseSize();
        long GetMinResponseSize();
        double GetModuleProcessingTime(string requestID);
        double GetRequestProcessingTime(string requestID);
        Stopwatch LogModuleStart(string requestID);
        Stopwatch LogRequestStart(string requestID);
        void LogResponseSize(long responseSize);
        void TrimResponseSizeList();
    }
}