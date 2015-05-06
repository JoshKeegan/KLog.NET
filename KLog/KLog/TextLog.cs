/*
 * KLog.NET
 * TextLog - abstract implemenatation of Log that represents logs that are purely textual
 * Authors:
 *  Josh Keegan 27/03/2015
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using KLog.Text;

namespace KLog
{
    public abstract class TextLog : Log
    {
        //Private Variables
        private readonly LogEntryTextFormatter formatter;

        //Implement Log
        protected override void write(LogEntry entry)
        {
            //Format this entry as a string
            string text = formatter.Eval(entry);

            //Write the raw message
            write(text);
        }

        protected abstract void write(string message);

        //Constructors
        public TextLog(LogLevel logLevel, LogEntryTextFormatter formatter)
            : base(logLevel) 
        {
            this.formatter = formatter;
        }

        public TextLog(LogLevel logLevel)
            : this(logLevel, 
            new LogEntryTextFormatter(
                new FeStringDateTime("yyyy-MM-dd HH:mm:ss"), " - ",
                new FeCallingMethodFullName(), ": ",
                new FeLogLevel(), ": ",
                new FeMessage())) {  }
    }
}
