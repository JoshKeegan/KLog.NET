/*
 * KLog.NET
 * ConcurrentQueueLog - Implementation of a Log that can have its messages queried programatically as a ConcurrentQueue
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
    public class ConcurrentQueueLog : Log
    {
        // Public Variables
        public readonly ConcurrentQueue<LogEntry> Queue = new ConcurrentQueue<LogEntry>();

        // Implement Log
        protected override void write(LogEntry entry)
        {
            Queue.Enqueue(entry);
        }

        // Constructors
        public ConcurrentQueueLog(LogLevel logLevel)
            : base(logLevel) {  }
    }
}
