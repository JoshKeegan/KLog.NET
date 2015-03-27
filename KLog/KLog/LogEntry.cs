/*
 * KLog.NET
 * LogEntry - a single entry in the log
 * Authors:
 *  Josh Keegan 27/03/2015
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace KLog
{
    public class LogEntry
    {
        #region Public Variables

        public string Message { get; private set; }
        public LogLevel LogLevel { get; private set; }
        public StackFrame CallingFrame { get; private set; }
        public DateTime EventDate { get; private set; }

        #endregion

        #region Constructors

        public LogEntry(string message, LogLevel logLevel, StackFrame callingFrame, DateTime eventDate)
        {
            this.Message = message;
            this.LogLevel = logLevel;
            this.CallingFrame = callingFrame;
            this.EventDate = eventDate;
        }

        #endregion
    }
}
