/*
 * KLog.NET: Performance Tests
 * CompoundLog writing to underlying logs with level LogLevel.All Performance Tests
 * Authors:
 *  Josh Keegan 13/06/2016
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KLog;

using PerformanceTests.DummyLogs;

namespace PerformanceTests.LogPerfTests
{
    public class CompoundLogLogLevelAllPerfTest : CompoundLogPerfTest
    {
        public CompoundLogLogLevelAllPerfTest() : base("Compound Log, writing to logs with LogLevel.All") {  }

        protected override void run()
        {
            // Make the log to write to
            Log log = new CompoundLog(new DoNothingLog(LogLevel.All), new DoNothingLog(LogLevel.All));

            for (int i = 0; i < NUM_WRITES; i++)
            {
                log.Info("Testing a plain string log message");
            }
        }
    }
}
