/*
 * KLog.NET Unit Tests
 * Log Tests
 * Authors:
 *  Josh Keegan 11/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KLog;
using KLog.Programmatic;

using NUnit.Framework;

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
            Assert.AreEqual(DEBUG_MESSAGE, log.MessageStack.Pop());

            log.Info(INFO_MESSAGE);
            Assert.AreEqual(INFO_MESSAGE, log.MessageStack.Pop());

            log.Warn(WARNING_MESSAGE);
            Assert.AreEqual(WARNING_MESSAGE, log.MessageStack.Pop());

            log.Error(ERROR_MESSAGE);
            Assert.AreEqual(ERROR_MESSAGE, log.MessageStack.Pop());
        }

        [Test]
        public void TestWriteDebug()
        {
            StackLog log = new StackLog(LogLevel.Debug);

            log.Debug(DEBUG_MESSAGE);
            Assert.AreEqual(DEBUG_MESSAGE, log.MessageStack.Pop());
        }

        [Test]
        public void TestWriteInfo()
        {
            StackLog log = new StackLog(LogLevel.Info);

            log.Info(INFO_MESSAGE);
            Assert.AreEqual(INFO_MESSAGE, log.MessageStack.Pop());
        }

        [Test]
        public void TestWriteWarning()
        {
            StackLog log = new StackLog(LogLevel.Warning);

            log.Warn(WARNING_MESSAGE);
            Assert.AreEqual(WARNING_MESSAGE, log.MessageStack.Pop());
        }

        [Test]
        public void TestWriteError()
        {
            StackLog log = new StackLog(LogLevel.Error);

            log.Error(ERROR_MESSAGE);
            Assert.AreEqual(ERROR_MESSAGE, log.MessageStack.Pop());
        }

        #endregion
    }
}
