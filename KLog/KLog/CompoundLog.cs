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
        private readonly Log[] logs = null;

        //Log Implementation
        protected override void write(LogEntry entry)
        {
            foreach(Log log in logs)
            {
                log.tryWriteLogLevel(entry);
            }
        }

        public override void BlockWhileWriting()
        {
            foreach(Log log in logs)
            {
                log.BlockWhileWriting();
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
            
            //Optimisation: use an array for logs
            this.logs = logs.ToArray();

            if (this.logs.Any(log => log == null))
            {
                throw new ArgumentException("logs cannot contain null");
            }
        }

        public CompoundLog(params Log[] args)
            : this((IEnumerable<Log>)args) {  }
    }
}
