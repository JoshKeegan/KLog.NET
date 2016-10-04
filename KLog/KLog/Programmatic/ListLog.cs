/*
 * KLog.NET
 * ListLog - Implementation of a Log that can have its messages queried programatically as a List
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
    public class ListLog : Log
    {
        // Public Variables
        public readonly List<LogEntry> List;

        // Implement Log
        protected override void write(LogEntry entry)
        {
            List.Add(entry);
        }

        // Constructors
        public ListLog(LogLevel logLevel, int listCapacity = 0)
            : base(logLevel)
        {
            List = new List<LogEntry>(listCapacity);
        }
    }
}
