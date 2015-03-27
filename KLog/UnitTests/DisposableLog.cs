/*
 * KLog.NET Unit Tests
 * Disposable Log - an Implementation of Log that stores whether it's Dispose() method gets evaluated
 * Authors:
 *  Josh Keegan 27/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KLog;

namespace UnitTests
{
    internal class DisposableLog : Log, IDisposable
    {
        //Public variables
        public bool Disposed { get; private set; }
        public bool Finalized { get; private set; }

        //Implement Log
        protected override void write(LogEntry entry) {  }

        //Implement IDisposable
        public void Dispose()
        {
            this.Disposed = true;
        }

        ~DisposableLog()
        {
            this.Finalized = true;
        }

        //Constructors
        public DisposableLog(LogLevel logLevel)
            : base(logLevel) {  }
    }
}
