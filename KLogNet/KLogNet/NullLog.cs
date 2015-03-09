/*
 * KLog.NET
 * NullLog - an implementation of Log that doesn't actually log anything
 * Authors:
 *  Josh Keegan 16/02/2015
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace KLogNet
{
    public class NullLog : Log
    {
        public NullLog()
            : base(LogLevel.None) { }

        protected override void write(string message, LogLevel logLevel, StackFrame callingFrame) { }
    }
}
