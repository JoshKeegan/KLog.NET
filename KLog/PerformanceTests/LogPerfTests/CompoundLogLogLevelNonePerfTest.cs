/*
 * KLog.NET: Performance Tests
 * CompoundLog writing to underlying logs with level LogLevel.None Performance Tests
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
    public class CompoundLogLogLevelNonePerfTest : CompoundLogPerfTest
    {
        public CompoundLogLogLevelNonePerfTest() : base("Compound Log, writing to logs with LogLevel.None") {  }

        protected override void run()
        {
            // Make the log to write to
            Log log = new CompoundLog(new DoNothingLog(LogLevel.None), new DoNothingLog(LogLevel.None));

            for (int i = 0; i < NUM_WRITES; i++)
            {
                log.Info("Testing a plain string log message");
            }
        }
    }
}
