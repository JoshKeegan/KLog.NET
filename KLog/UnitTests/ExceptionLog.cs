/*
    KLog.NET Unit Tests
    Exception Log - an Implementation of Log that always throws an Exception in write()
    Authors:
        Josh Keegan 08/02/2016
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KLog;

namespace UnitTests
{
    internal class ExceptionLog : Log
    {
        public ExceptionLog(LogLevel logLevel) 
            : base(logLevel) {  }

        protected override void write(LogEntry entry)
        {
            throw new Exception("Something that could go wrong . . .");
        }
    }
}
