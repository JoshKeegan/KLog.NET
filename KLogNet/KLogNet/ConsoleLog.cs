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

namespace KLogNet
{
    public class ConsoleLog : Log
    {
        //Log Implementation
        protected override void write(string message, LogLevel logLevel, StackFrame callingFrame, DateTime eventDate)
        {
            message = String.Format("{0}: {1}", logLevel.ToString(), message);

            String text = eventDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " - " + callingFrame.GetMethod().DeclaringType.FullName + message;

            Console.WriteLine(text);
        }

        //Constructors
        public ConsoleLog(LogLevel logLevel)
            : base(logLevel) {  }
    }
}
