/*
 * KLog.NET
 * LogFilter - Implementation of Log that wraps around another log.
 *  Filters messages so not all messages that get passed to the LogFilter will be
 *  written to the underlying log
 * Authors:
 *  Josh Keegan 20/05/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KLog;

namespace KLog
{
    public class LogFilter<T> : Log, IDisposable where T : Log
    {
        #region Private Variables

        private readonly T underlyingLog;
        private readonly Filter filter;

        #endregion

        #region Log Implementation

        protected override void write(LogEntry entry)
        {
            if(filter(entry))
            {
                underlyingLog.internalWrite(entry);
            }
        }

        #endregion

        #region  Constructors

        public LogFilter(T log, Filter filter)
            : base(log.LogLevel)
        {
            //Validation
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            underlyingLog = log;
            this.filter = filter;
        }

        #endregion

        #region Public Delegates

        /// <summary>
        /// The Filter part of LogFilter. Method that determines whether a LogEntry should get written
        /// </summary>
        /// <param name="entry">The LogEntry that is trying to be written</param>
        /// <returns>Whether to write the supplied LogEntry</returns>
        public delegate bool Filter(LogEntry entry);

        #endregion

        #region Implement IDisposable

        public void Dispose()
        {
            Dispose(true);
        }

        ~LogFilter()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            //Write all of the log entries out before letting the log be disposed of
            BlockWhileWriting();

            //Callling Dispose(): free managed resources
            if (disposing)
            {
                //If there is an underlying log that is disposable, dispose of it now
                if (underlyingLog != null
                    && typeof(IDisposable).IsAssignableFrom(typeof(T)))
                {
                    IDisposable disposable = (IDisposable)underlyingLog;
                    disposable.Dispose();
                }
            }

            //Dispose or finalizer, free any native resources
        }

        #endregion
    }
}
