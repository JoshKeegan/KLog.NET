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

using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class LogTests
    {
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
    }
}
