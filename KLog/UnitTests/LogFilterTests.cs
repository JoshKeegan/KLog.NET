/*
 * KLog.NET Unit Tests
 * LogFilter Tests
 * Authors:
 *  Josh Keegan 29/05/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KLog;
using KLog.Programmatic;

using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class LogFilterTests
    {
        [Test]
        public void TestUnfiltered()
        {
            StackLog log = new StackLog(LogLevel.All);
            LogFilter<StackLog> filter = new LogFilter<StackLog>(log, (entry => true));

            filter.Debug("test");

            Assert.AreEqual("test", log.Stack.Pop().Message);
        }

        [Test]
        public void TestBlocked()
        {
            StackLog log = new StackLog(LogLevel.All);
            LogFilter<StackLog> filter = new LogFilter<StackLog>(log, (entry => false));

            filter.Debug("test");

            Assert.IsFalse(log.Stack.Any());
        }

        [Test]
        public void TestEntryBasedWrite()
        {
            StackLog log = new StackLog(LogLevel.All);
            //Dont write messages that start with "test"
            LogFilter<StackLog> filter = new LogFilter<StackLog>(log, (entry => !entry.Message.StartsWith("test")));

            filter.Debug("Not a test");
            filter.Debug("test");

            Assert.AreEqual("Not a test", log.Stack.Pop().Message);
        }
    }
}
