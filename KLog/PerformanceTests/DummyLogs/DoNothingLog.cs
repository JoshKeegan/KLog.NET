/*
 * KLog.NET: Performance Tests
 * DoNothingLog - a log implementation that will do nothing on write.
 *  Designed for testing the performance overheads of base Log, Log wrappers etc... by 
 *  excluding any runtime of the actual Log.write implementation
 * Authors:
 *  Josh Keegan 13/06/2016
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KLog;

namespace PerformanceTests.DummyLogs
{
    public class DoNothingLog : Log
    {
        public DoNothingLog(LogLevel logLevel) 
            : base(logLevel) {  }

        protected override void write(LogEntry entry) {  }
    }
}
