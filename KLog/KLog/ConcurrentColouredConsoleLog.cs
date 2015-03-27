/*
 * KLog.NET
 * ColouredConsoleLog - Implementation of Log that logs messages to the Console, 
 *  highlighting different Log Levels in different colours
 * Authors:
 *  Josh Keegan 09/03/2015
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KLog
{
    public class ConcurrentColouredConsoleLog : LogConcurrencyWrapper<ColouredConsoleLog>
    {
        //Constructors (should mirror those available in ColouredConsoleLog)
        public ConcurrentColouredConsoleLog(LogLevel logLevel, Dictionary<LogLevel, ConsoleColor> foregroundColours,
            Dictionary<LogLevel, ConsoleColor> backgroundColours)
            : base(new ColouredConsoleLog(logLevel, foregroundColours, backgroundColours)) {  }

        public ConcurrentColouredConsoleLog(LogLevel logLevel, Dictionary<LogLevel, ConsoleColor> foregroundColours)
            : base(new ColouredConsoleLog(logLevel, foregroundColours)) { }

        public ConcurrentColouredConsoleLog(LogLevel logLevel)
            : base(new ColouredConsoleLog(logLevel)) { }
    }
}
