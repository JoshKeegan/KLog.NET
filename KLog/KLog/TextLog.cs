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

namespace KLog
{
    public abstract class TextLog : Log
    {
        protected override void write(LogEntry entry)
        {
            //Format this entry as a string
            string text = buildLogEntryString(entry);

            //Write the raw message
            write(text);
        }

        protected abstract void write(string message);

        //Constructors
        //TODO: Text formatting options
        public TextLog(LogLevel logLevel)
            : base(logLevel) {  }

        //Private Helpers
        private static string buildLogEntryString(LogEntry entry)
        {
            //TODO: Improve with a range of text formatting options
            string message = String.Format("{0} - {1}: {2}: {3}",
                entry.LogLevel,
                entry.EventDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                entry.CallingFrame.GetMethod().DeclaringType.FullName,
                entry.Message);

            return message;
        }
    }
}
