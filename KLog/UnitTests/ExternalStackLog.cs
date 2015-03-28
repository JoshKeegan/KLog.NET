/*
 * KLog.NET Unit Tests
 * ExternalStackLog - an implementation of a Stack Log that is external to the KLog namespace
 * Authors:
 *  Josh Keegan 28/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KLog;

namespace UnitTests
{
    public class ExternalStackLog : Log
    {
        //Public Variables
        public Stack<LogEntry> Stack = new Stack<LogEntry>();

        //Implement Log
        protected override void write(LogEntry entry)
        {
            Stack.Push(entry);
        }

        //Constructors
        public ExternalStackLog(LogLevel logLevel)
            : base(logLevel) {  }
    }
}
