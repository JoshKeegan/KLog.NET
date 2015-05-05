﻿/*
 * KLog.NET
 * LogRateLimiter - Implementation of Log that wraps another log. 
 *  Limits the number of entries that can get logged in a given time period.
 *  If the limit is exceeded, no log messages will get written until it dips back below the threshold
 * Authors:
 *  Josh Keegan 08/04/2015
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLog
{
    public class LogRateLimiter<T> : Log, IDisposable where T : Log
    {
        #region Private Variables

        private T underlyingLog;

        //TODO: Could alter queue to be lock-free and fixed length (combination of MS ConcurrentQueue & PiSearch FixedLengthQueue)
        //  if ever requiure that level of performance
        private Queue<DateTime> logTimes;
        private object logTimesLock = new object();
        private TimeSpan timeSpan;
        private int numEntries;
        private bool inRateLimit = false;
        private HandleEnterExitRateLimit onEnterRateLimit;
        private HandleEnterExitRateLimit onExitRateLimit;

        #endregion

        #region Log Implementation

        protected override void write(LogEntry entry)
        {
            //Update logTimes
            bool doWrite = true;
            bool enteredRateLimit = false;
            bool exitedRateLimit = false;

            lock(logTimesLock)
            {
                logTimes.Enqueue(entry.EventDate);

                //If we've written numEntries to the log, check if we're within the rate limit
                if(logTimes.Count > numEntries)
                {
                    DateTime oldest = logTimes.Dequeue();
                    TimeSpan loggedAgo = entry.EventDate - oldest;

                    //If we have logged numEntries items in less time than the rate limit
                    if(loggedAgo < timeSpan)
                    {
                        //Don't write this entry to the log
                        doWrite = false;

                        //If we weren't already in the rate limit, we just entered it
                        if(!inRateLimit)
                        {
                            inRateLimit = true;
                            enteredRateLimit = true;
                        }
                    }
                    //Otherwise, if we were in the rate limit we aren't any more
                    else if(inRateLimit)
                    {
                        inRateLimit = false;
                        exitedRateLimit = true;
                    }
                }
            }

            if(doWrite)
            {
                underlyingLog.internalWrite(entry);
            }
            
            if(enteredRateLimit)
            {
                onEnterRateLimit(entry);
            }

            if(exitedRateLimit)
            {
                onExitRateLimit(entry);
            }
        }

        #endregion

        #region Constructors

        public LogRateLimiter(T log, TimeSpan timeSpan, int numEntries,
            HandleEnterExitRateLimit onEnterRateLimit, HandleEnterExitRateLimit onExitRateLimit)
            : base(log.LogLevel)
        {
            //Validation
            if(log == null)
            {
                throw new ArgumentNullException("log");
            }
            if(timeSpan == null)
            {
                throw new ArgumentNullException("timeSpan");
            }
            if(numEntries < 0)
            {
                throw new ArgumentOutOfRangeException("numEntries", "numEntries must be >= 0");
            }
            if(onEnterRateLimit == null)
            {
                throw new ArgumentNullException("onEnterRateLimit");
            }
            if (onExitRateLimit == null)
            {
                throw new ArgumentNullException("onExitRateLimit");
            }
            underlyingLog = log;

            logTimes = new Queue<DateTime>();
            this.timeSpan = timeSpan;
            this.numEntries = numEntries;
            this.onEnterRateLimit = onEnterRateLimit;
            this.onExitRateLimit = onExitRateLimit;
        }

        #endregion

        #region Public Delegates

        public delegate void HandleEnterExitRateLimit(LogEntry entry);

        #endregion

        #region Implement IDisposable

        public void Dispose()
        {
            Dispose(true);
        }

        ~LogRateLimiter()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            //Write all of the log entries out before letting the log be disposed of
            BlockWhileWriting();

            //Callling Dispose(): free managed resources
            if(disposing)
            {
                //If there is an underlying log that is disposable, dispose of it now
                if(underlyingLog != null
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
