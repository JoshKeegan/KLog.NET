/*
 * KLog.NET
 * LogConcurrencyWrapper - Implementation of Log that wraps a log that should only be called from one thread
 *  and turns it into a log that can be called from many threads safely
 * Authors:
 *  Josh Keegan 27/03/2015
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KLog
{
    public class LogConcurrencyWrapper<T> : Log, IDisposable where T : Log
    {
        #region Private Variables

        private readonly Log underlyingLog;

        private bool processing = true;
        private readonly ConcurrentQueue<LogEntry> processingQueue = new ConcurrentQueue<LogEntry>();
        private readonly Task processingTask;
        private readonly CancellationTokenSource processingTaskCancellationTokenSource;

        #endregion

        #region Log Implementation

        protected override void write(LogEntry entry)
        {
            //Dont actually perform the write here. All writes get performed by the processing thread (so
            //  there is only ever one write going on at once, without locking the calling worker threads)
            processingQueue.Enqueue(entry);
        }

        #endregion

        #region Contructors

        public LogConcurrencyWrapper(T log)
            : base(log.LogLevel)
        {
            //Validation
            if(log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            underlyingLog = log;

            //Set up the processing
            processingTaskCancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = processingTaskCancellationTokenSource.Token;

            processingTask = Task.Factory.StartNew(() =>
            {
                //Infinite loop of checking for new log entries to write
                while (true)
                {
                    //Write any entries to be written
                    while (processingQueue.Any())
                    {
                        LogEntry entry;
                        if (processingQueue.TryDequeue(out entry))
                        {
                            underlyingLog.internalWrite(entry);
                        }
                    }

                    //Check if this task has been cancelled
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    //Small pause to prevent 100% CPU usage
                    Thread.Sleep(1);
                }
            }, cancellationToken);
        }

        #endregion

        #region Public Methods

        public override void BlockWhileWriting()
        {
            //Only wait for the writing to finish if we're still writing
            if (processing)
            {
                //Mark processing as having finished to prevent this from running in another thread
                processing = false;

                //Request the cancellation
                processingTaskCancellationTokenSource.Cancel();

                //Try catch since although we don't exit the Task by throwing a TaskCanceledException, it may still throw one if it isn't
                //  in the running state (and therefore isn't cancelled by the implementation here) ~ see bug #6
                try
                {
                    //Wait for the task to finish processing
                    processingTask.Wait();
                }
                catch(AggregateException aggregateException)
                {
                    //Exclude TaskCanceledException, since this is semi-expected ~ see bug #6
                    Exception[] nonTaskCanceledExceptions = aggregateException.InnerExceptions.Where(
                        e => e.GetType() != typeof (TaskCanceledException)).ToArray();

                    //If there were any exceptions (after excluding TaskCanceledException), throw them
                    if(nonTaskCanceledExceptions.Any())
                    {
                        throw new AggregateException(nonTaskCanceledExceptions);
                    }
                }
                finally
                {
                    //Clean up
                    processingTaskCancellationTokenSource.Dispose();
                }
            }
        }

        #endregion

        #region Implement IDisposable

        public void Dispose()
        {
            Dispose(true);
        }

        ~LogConcurrencyWrapper()
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
                processingTaskCancellationTokenSource.Dispose();
                processingTask.Dispose();

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
