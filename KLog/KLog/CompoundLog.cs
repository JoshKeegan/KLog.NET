/*
 * KLog.NET
 * CompundLog - Implementation of Log that allows for multiple logs to be used as one
 * Authors:
 *  Josh Keegan 09/03/2015
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace KLog
{
    public class CompoundLog : Log
    {
        //Private variables
        private IEnumerable<Log> logs = null;

        //Log Implementation
        protected override void write(string message, LogLevel logLevel, StackFrame callingFrame, DateTime eventDate)
        {
            foreach(Log log in logs)
            {
                log.internalWrite(message, logLevel, callingFrame, eventDate);
            }
        }

        //Constructors
        public CompoundLog(IEnumerable<Log> logs)
            : base(LogLevel.All) //Allow all through the Compound Log. Each log within the compound log can have its own LogLevel
        {
            //Validation
            if(logs == null)
            {
                throw new ArgumentNullException("logs");
            }
            foreach(Log log in logs)
            {
                if(log == null)
                {
                    throw new ArgumentException("logs cannot contain null");
                }
            }

            this.logs = logs;
        }

        public CompoundLog(params Log[] args)
            : this((IEnumerable<Log>)args) {  }
    }
}
