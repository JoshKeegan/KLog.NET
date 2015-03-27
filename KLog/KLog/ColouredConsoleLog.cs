/*
 * KLog.NET
 * ColouredConsoleLog - Implementation of Log that logs messages to the Console, 
 *  highlighting different Log Levels in different colours
 * Authors:
 *  Josh Keegan 09/03/2015
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KLog
{
    public class ColouredConsoleLog : ConsoleLog, IDisposable
    {
        #region Constants

        private static readonly LogLevel[] REQUIRED_LOG_LEVELS = new LogLevel[] { 
            LogLevel.Debug, LogLevel.Info, LogLevel.Warning, LogLevel.Error };

        private static readonly Dictionary<LogLevel, ConsoleColor> DEFAULT_FOREGROUND_COLOURS = 
            new Dictionary<LogLevel, ConsoleColor>()
        {
            { LogLevel.Debug, ConsoleColor.Gray },
            { LogLevel.Info, ConsoleColor.White },
            { LogLevel.Warning, ConsoleColor.Yellow },
            { LogLevel.Error, ConsoleColor.Red }
        };

        private const Dictionary<LogLevel, ConsoleColor> DEFAULT_BACKGROUND_COLOURS = null; // null => no change
        
        //Defaults used when filling in any missing log level colours in a specified set of colours to be used for the background
        private static readonly Dictionary<LogLevel, ConsoleColor> DEFAULT_BACKGROUND_COLOURS_FILLING_IN = 
            new Dictionary<LogLevel, ConsoleColor>()
        {
            { LogLevel.Debug, ConsoleColor.Black },
            { LogLevel.Info, ConsoleColor.Black },
            { LogLevel.Warning, ConsoleColor.Black },
            { LogLevel.Error, ConsoleColor.Black }
        };

        #endregion

        #region Private Variables

        private Dictionary<LogLevel, ConsoleColor> foregroundColours;
        private Dictionary<LogLevel, ConsoleColor> backgroundColours;

        private bool processing = true;
        private ConcurrentQueue<LogEntry> processingQueue = new ConcurrentQueue<LogEntry>();
        private Task processingTask;
        private CancellationTokenSource processingTaskCancellationTokenSource;

        #endregion

        #region Log Implementation

        protected override void write(LogEntry entry)
        {
            //Don't actually do the write here. All writes are perfromed by the processing thread (so 
            //  there is only one thread setting the colours)
            processingQueue.Enqueue(entry);
        }

        #endregion

        #region Constructors

        public ColouredConsoleLog(LogLevel logLevel, Dictionary<LogLevel, ConsoleColor> foregroundColours,
            Dictionary<LogLevel, ConsoleColor> backgroundColours)
            : base(logLevel)
        {
            //Normalise the specified colours (to include a value for each of the Log Levels)
            this.foregroundColours = normaliseLogLevelColours(foregroundColours, DEFAULT_FOREGROUND_COLOURS);
            this.backgroundColours = normaliseLogLevelColours(backgroundColours, DEFAULT_BACKGROUND_COLOURS_FILLING_IN);

            //Set up the processing
            processingTaskCancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = processingTaskCancellationTokenSource.Token;

            processingTask = Task.Factory.StartNew(() =>
            {
                //Infinite loop of checking for new log entries to write
                while(true)
                {
                    //Write any entries to be written
                    while(processingQueue.Any())
                    {
                        LogEntry entry;
                        if(processingQueue.TryDequeue(out entry))
                        {
                            actuallyWrite(entry);
                        }
                    }

                    //Check if this task has been cancelled
                    cancellationToken.ThrowIfCancellationRequested();

                    //Small pause to prevent 100% CPU usage
                    Thread.Sleep(1);
                }
            }, cancellationToken);
        }

        public ColouredConsoleLog(LogLevel logLevel, Dictionary<LogLevel, ConsoleColor> foregroundColours)
            : this(logLevel, foregroundColours, DEFAULT_BACKGROUND_COLOURS) {  }

        public ColouredConsoleLog(LogLevel logLevel)
            : this(logLevel, DEFAULT_FOREGROUND_COLOURS) {  }

        #endregion

        #region Public Methods

        public override void BlockWhileWriting()
        {
            //Only wait for the writing to finish if we're still writing
            if(processing)
            {
                //Mark processing as having finished to prevent this from running in another thread
                processing = false;

                processingTaskCancellationTokenSource.Cancel();

                try
                {
                    processingTask.Wait();
                }
                catch (AggregateException aggregateException)
                {
                    foreach(Exception e in aggregateException.InnerExceptions)
                    {
                        if(e.GetType() != typeof(TaskCanceledException))
                        {
                            throw e;
                        }
                    }
                }
                finally
                {
                    processingTaskCancellationTokenSource.Dispose();
                }
            }
            
        }

        #endregion

        #region Implement IDisposable

        public void Dispose()
        {
            this.Dispose(true);
        }

        ~ColouredConsoleLog()
        {
            this.Dispose(false);
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
            }

            //Dispose or finalizer, free any native resources
        }

        #endregion

        #region Private Helpers

        private static Dictionary<LogLevel, ConsoleColor> normaliseLogLevelColours(
            Dictionary<LogLevel, ConsoleColor> logLevelColours, Dictionary<LogLevel, ConsoleColor> defaults)
        {
            //null is valid (as it represents no change to the consoles existing colour)
            if(logLevelColours != null)
            {
                List<LogLevel> toAdd = new List<LogLevel>();
                foreach(LogLevel logLevel in REQUIRED_LOG_LEVELS)
                {
                    if(!logLevelColours.ContainsKey(logLevel))
                    {
                        toAdd.Add(logLevel);
                    }
                }

                foreach(LogLevel logLevel in toAdd)
                {
                    logLevelColours.Add(logLevel, defaults[logLevel]);
                }
            }
            return logLevelColours;
        }

        private void actuallyWrite(LogEntry entry)
        {
            //Get the current Console colours (to restore them once we've wrote the log message) 
            ConsoleColor foregroundBefore = Console.ForegroundColor;
            ConsoleColor backgroundBefore = Console.BackgroundColor;

            //Change the colour to the appropriate one for this Log Level

            //If we are changing the foreground colour
            if (foregroundColours != null)
            {
                Console.ForegroundColor = foregroundColours[entry.LogLevel];
            }
            //If we are changing the background colour
            if (backgroundColours != null)
            {
                Console.BackgroundColor = backgroundColours[entry.LogLevel];
            }

            //Write the message
            base.write(entry);

            //Change the colours back
            Console.ForegroundColor = foregroundBefore;
            Console.BackgroundColor = backgroundBefore;
        }

        #endregion
    }
}
