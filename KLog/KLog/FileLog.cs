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

namespace KLog
{
    public class FileLog : TextLog, IDisposable
    {
        //Private variables
        private string filePath;
        private StreamWriter logWriter;
        private object logLock;

        //Log implementation
        protected override void write(string message)
        {
            //thread safety
            lock (logLock)
            {
                logWriter.WriteLine(message);
                logWriter.Flush(); //Flush after writes to guard log content against program crash
            }
        }

        //Constructors
        public FileLog(string filePath, LogLevel logLevel)
            : base(logLevel)
        {
            this.filePath = filePath;
            logWriter = new StreamWriter(filePath);
            logLock = new object();
        }

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
                logWriter.Close();
            }

            //Dispose or finaliser, free any native resources
        }
    }
}
