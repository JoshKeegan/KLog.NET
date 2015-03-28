/*
 * KLog.NET Unit Tests
 * Log Tests
 * Authors:
 *  Josh Keegan 11/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using KLog;
using KLog.Programmatic;

using NUnit.Framework;
using SixPack.Reflection;


namespace UnitTests
{
    [TestFixture]
    public class LogTests
    {
        //Constants
        private const string DEBUG_MESSAGE = "debug message";
        private const string INFO_MESSAGE = "info message";
        private const string WARNING_MESSAGE = "warning message";
        private const string ERROR_MESSAGE = "error message";

        #region Test Object Evaluation

        [Test]
        public void TestObjectEvaluation()
        {
            //An object should only be evaluated if it is actually going to be written out
            EvaluatableObject obj = new EvaluatableObject();
            Log log = new ConsoleLog(LogLevel.All);

            Assert.IsFalse(obj.Evaluated);

            log.Error("Oh hi there {0}", obj);
            Assert.IsTrue(obj.Evaluated);
        }

        [Test]
        public void TestObjectEvaluationNotEvaluated()
        {
            //An object should not be evaluated if it isn't written out
            EvaluatableObject obj = new EvaluatableObject();
            Log log = new ConsoleLog(LogLevel.None);

            Assert.IsFalse(obj.Evaluated);

            log.Info("Sup {0}", obj);
            Assert.IsFalse(obj.Evaluated);
        }

        #endregion

        #region Test Write

        [Test]
        public void TestWriteAllLevelsOnAll()
        {
            StackLog log = new StackLog(LogLevel.All);

            log.Debug(DEBUG_MESSAGE);
            Assert.AreEqual(DEBUG_MESSAGE, log.Stack.Pop().Message);

            log.Info(INFO_MESSAGE);
            Assert.AreEqual(INFO_MESSAGE, log.Stack.Pop().Message);

            log.Warn(WARNING_MESSAGE);
            Assert.AreEqual(WARNING_MESSAGE, log.Stack.Pop().Message);

            log.Error(ERROR_MESSAGE);
            Assert.AreEqual(ERROR_MESSAGE, log.Stack.Pop().Message);
        }

        [Test]
        public void TestWriteDebug()
        {
            StackLog log = new StackLog(LogLevel.Debug);

            log.Debug(DEBUG_MESSAGE);
            Assert.AreEqual(DEBUG_MESSAGE, log.Stack.Pop().Message);
        }

        [Test]
        public void TestWriteInfo()
        {
            StackLog log = new StackLog(LogLevel.Info);

            log.Info(INFO_MESSAGE);
            Assert.AreEqual(INFO_MESSAGE, log.Stack.Pop().Message);
        }

        [Test]
        public void TestWriteWarning()
        {
            StackLog log = new StackLog(LogLevel.Warning);

            log.Warn(WARNING_MESSAGE);
            Assert.AreEqual(WARNING_MESSAGE, log.Stack.Pop().Message);
        }

        [Test]
        public void TestWriteError()
        {
            StackLog log = new StackLog(LogLevel.Error);

            log.Error(ERROR_MESSAGE);
            Assert.AreEqual(ERROR_MESSAGE, log.Stack.Pop().Message);
        }

        [Test]
        public void TestWriteDebugLogLevelError()
        {
            StackLog log = new StackLog(LogLevel.Error);

            log.Debug(DEBUG_MESSAGE);
            Assert.IsFalse(log.Stack.Any());
        }

        [Test]
        public void TestWriteInfoLogLevelError()
        {
            StackLog log = new StackLog(LogLevel.Error);

            log.Info(INFO_MESSAGE);
            Assert.IsFalse(log.Stack.Any());
        }

        [Test]
        public void TestWriteWarnLogLevelError()
        {
            StackLog log = new StackLog(LogLevel.Error);

            log.Warn(WARNING_MESSAGE);
            Assert.IsFalse(log.Stack.Any());
        }

        #endregion

        #region Test getCallingFrame

        [Test]
        public void TestGetCallingFrameCorrectMethod()
        {
            MethodBase expected = MethodBase.GetCurrentMethod();

            StackLog log = new StackLog(LogLevel.All);
            log.Debug("test");

            LogEntry entry = log.Stack.Pop();

            Assert.AreEqual(expected, entry.CallingFrame.GetMethod());
        }

        [Test]
        public void TestGetCallingFrameCorrectMethodFromExternalLogImplementation()
        {
            MethodBase expected = MethodBase.GetCurrentMethod();

            ExternalStackLog log = new ExternalStackLog(LogLevel.All);
            log.Debug("test");

            LogEntry entry = log.Stack.Pop();

            Assert.AreEqual(expected, entry.CallingFrame.GetMethod());
        }

        [Test]
        public void TestGetCallingFrameCorrectMethodThroughDefaultLog()
        {
            MethodBase expected = MethodBase.GetCurrentMethod();

            StackLog log = new StackLog(LogLevel.All);
            DefaultLog.Log = log;
            DefaultLog.Debug("test");

            LogEntry entry = log.Stack.Pop();

            Assert.AreEqual(expected, entry.CallingFrame.GetMethod());
        }

        [Test]
        public void TestGetCallingFrameCorrectTypeWhenCalledInternally()
        {
            Type expected = typeof(EmailLog);

            //Set up the internal log
            StackLog internalLog = new StackLog(LogLevel.All);
            InternalLog.Log = internalLog;

            //Use an EmailLog with incorrect SMTP server settings to trigger a call to the InternalLog
            EmailLog log = new EmailLog("test@made_up.com", "josh.keegan@also_made_up.com", "smtp.made_up.com", 
                "test", "testPassword", LogLevel.All);
            log.Debug("test");

            LogEntry entry = internalLog.Stack.Pop();

            Assert.AreEqual(expected, entry.CallingFrame.GetMethod().DeclaringType);
        }

        #endregion
    }
}
