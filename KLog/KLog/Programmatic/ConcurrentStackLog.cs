/*
 * KLog.NET
 * ConcurrentStackLog - Implementation of a Log that can have its messages queried programatically as a ConcurrentStack
 * Authors:
 *  Josh Keegan 04/10/2016
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KLog.Programmatic
{
    public class ConcurrentStackLog : Log
    {
        // Public Variables
        public ConcurrentStack<LogEntry> Stack = new ConcurrentStack<LogEntry>();

        // Implement Log
        protected override void write(LogEntry entry)
        {
            Stack.Push(entry);
        }

        // Constructors
        public ConcurrentStackLog(LogLevel logLevel)
            : base(logLevel) {  }
    }
}
