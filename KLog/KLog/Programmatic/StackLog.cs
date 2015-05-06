/*
 * KLog.NET
 * StackLog - Implementation of a Log that can have its messages queried programatically as a stack
 *  Authors:
 *      Josh Keegan 17/03/2015
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace KLog.Programmatic
{
    public class StackLog : Log
    {
        //Public Variables
        public readonly Stack<LogEntry> Stack = new Stack<LogEntry>();

        //Implement Log
        protected override void write(LogEntry entry)
        {
            Stack.Push(entry);
        }

        //Constructors
        public StackLog(LogLevel logLevel)
            : base(logLevel) {  }
    }
}
