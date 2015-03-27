/*
 * KLog.NET Unit Tests
 * LogConcurrencyWrapper Tests
 * Authors:
 *  Josh Keegan 27/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KLog;

using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class LogConcurrencyWrapperTests
    {
        [Test]
        public void TestUnderlyingLogDisposal()
        {
            DisposableLog disposableLog = new DisposableLog(LogLevel.All);
            LogConcurrencyWrapper<DisposableLog> concurrentDisposableLog = new LogConcurrencyWrapper<DisposableLog>(disposableLog);

            Assert.IsFalse(disposableLog.Disposed);

            concurrentDisposableLog.Dispose();
            Assert.IsTrue(disposableLog.Disposed);
        }
    }
}
