/*
 * KLog.NET
 * QueueLog - Implementation of a Log that can have its messages queried programatically as a Queue
 * Authors:
 *  Josh Keegan 04/10/2016
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KLog.Programmatic
{
    public class QueueLog : Log
    {
        // Public Variables
        public readonly Queue<LogEntry> Queue = new Queue<LogEntry>();

        // Implement Log
        protected override void write(LogEntry entry)
        {
            Queue.Enqueue(entry);
        }

        // Constructors
        public QueueLog(LogLevel logLevel)
            : base(logLevel) {  }
    }
}
