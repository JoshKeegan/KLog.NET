/*
 * KLog.NET
 * FileLog - Implementation of Log that logs messages to a file (thread-safe)
 * Authors:
 *  Josh Keegan 09/03/2015
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace KLogNet
{
    public class FileLog : Log, IDisposable
    {
        //Private variables
        private string filePath;
        private StreamWriter logWriter;
        private object logLock;

        //Log implementation
        protected override void write(string message, LogLevel logLevel, StackFrame callingFrame)
        {
            message = String.Format("{0}: {1}", logLevel.ToString(), message);

            String text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " - " + callingFrame.GetMethod().DeclaringType.FullName + message;

            Console.WriteLine(text);

            //thread safety
            lock (logLock)
            {
                this.logWriter.WriteLine(text);
                this.logWriter.Flush(); //Flush after writes to guard log content against program crash
            }
        }

        //Constructors
        public FileLog(string filePath, LogLevel logLevel)
            : base(logLevel)
        {
            this.filePath = filePath;
            this.logWriter = new StreamWriter(filePath);
            this.logLock = new object();
        }

        //Implement IDisposable
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FileLog()
        {
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            //Called Dispose(): Free managed resources
            if (disposing)
            {
                this.logWriter.Close();
            }

            //Dispose or finaliser, free any native resources
        }
    }
}
