/*
 * KLog.NET
 * ListLog - Implementation of a Log that can have its messages queried programatically as a LinkedList
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
    public class LinkedListLog : Log
    {
        // Public Variables
        public readonly LinkedList<LogEntry> List = new LinkedList<LogEntry>();

        // Implement Log
        protected override void write(LogEntry entry)
        {
            List.AddLast(entry);
        }

        // Constructors
        public LinkedListLog(LogLevel logLevel)
            : base(logLevel) {  }
    }
}
