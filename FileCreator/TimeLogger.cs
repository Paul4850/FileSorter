using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCreator
{
    internal class TimeLogger
    {
        public static TimeSpan MeasureExecutionTime(Action action, string methodName, ILogger logger)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                action();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Execution failed");
                throw;
            }
            stopwatch.Stop();
            logger.LogInformation($"{methodName} execution time is {TimeSpan.FromSeconds(Math.Round(stopwatch.Elapsed.TotalSeconds))}");
            return stopwatch.Elapsed;
        }
    }
}
