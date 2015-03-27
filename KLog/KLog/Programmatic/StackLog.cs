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
        public Stack<string> MessageStack = new Stack<string>();

        //Implement Log
        protected override void write(LogEntry entry)
        {
            //TODO: Switch to storing a stack of the actual Log Entries themelves
            MessageStack.Push(entry.Message);
        }

        //Constructors
        public StackLog(LogLevel logLevel)
            : base(logLevel) {  }
    }
}
