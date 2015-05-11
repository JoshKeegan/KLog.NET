/*
 * KLog.NET
 * FileLog - Implementation of Log that logs messages to a file (thread-safe)
 * Authors:
 *  Josh Keegan 16/02/2015
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using KLog.Text;

namespace KLog
{
    public class FileLog : TextLog, IDisposable
    {
        //Private variables
        private readonly FileLogNameTextFormatter feFilePath;
        private string filePath;
        private StreamWriter logWriter;
        private readonly object logLock;

        //Public Variables
        public readonly bool Rotate;

        //Log implementation
        protected override void write(string message)
        {
            //Rotate the log file being used if necessary
            rotateIfNecessary();

            //thread safety
            lock (logLock)
            {
                logWriter.WriteLine(message);
                logWriter.Flush(); //Flush after writes to guard log content against program crash
            }
        }

        //Constructors
        public FileLog(FileLogNameTextFormatter feFilePath, bool rotate, LogLevel logLevel)
            : base(logLevel)
        {
            this.feFilePath = feFilePath;
            Rotate = rotate;
            filePath = feFilePath.Eval();
            logWriter = new StreamWriter(filePath);
            logLock = new object();
        }

        public FileLog(string filePath, LogLevel logLevel)
            : this(new FileLogNameTextFormatter(filePath), false, logLevel) {  }

        //Implement IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FileLog()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            //Called Dispose(): Free managed resources
            if (disposing)
            {
                // thread safety
                lock (logLock)
                {
                    logWriter.Close();
                }
            }

            //Dispose or finaliser, free any native resources
        }

        // Private methods
        private void rotateIfNecessary()
        {
            if (Rotate)
            {
                //Evaluate without incrementing any counters
                string newFilePath = feFilePath.Eval(false);

                //If the file path to write to has changed since the last item was written to the log, change the stream to the new file
                //  to the new file
                if (filePath != newFilePath)
                {
                    // thread safety
                    lock (logLock)
                    {
                        //Check that another thread hasn't rotated the stream whilst waiting for the lock
                        if (filePath != newFilePath)
                        {
                            logWriter.Close();

                            //TODO: Option to automatically compress old log files

                            //Now work out the new log file path by resetting the counters and evaluating the FileLogNameTextFormatter again
                            feFilePath.ResetCounters();
                            newFilePath = feFilePath.Eval();

                            logWriter = new StreamWriter(newFilePath);
                            filePath = newFilePath;
                        }
                    }
                }
            }
        }
    }
}
