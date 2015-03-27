/*
 * KLog.NET
 * ConsoleLog - Implementation of Log that logs messages to a file (thread safe)
 * Authors:
 *  Josh Keegan 09/03/2015
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace KLog
{
    public class ConsoleLog : TextLog
    {
        //Log Implementation
        protected override void write(string message)
        {
            Console.WriteLine(message);
        }

        //Constructors
        public ConsoleLog(LogLevel logLevel)
            : base(logLevel) {  }
    }
}
