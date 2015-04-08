/*
 * KLog.NET Unit Tests
 * LogRateLimiter Tests
 * Authors:
 *  Josh Keegan 08/04/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using KLog;
using KLog.Programmatic;

using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class LogRateLimiterTests
    {
        //Constants
        private const int WAIT_MS = 10;

        [Test]
        public void TestWrite()
        {
            StackLog log = new StackLog(KLog.LogLevel.All);
            LogRateLimiter<StackLog> rateLimiter = new LogRateLimiter<StackLog>(log, new TimeSpan(1, 0, 0), 100, 
                (entry) => { }, (entry) => { });

            rateLimiter.Debug("test message");

            Assert.AreEqual("test message", log.Stack.Pop().Message);
        }

        [Test]
        public void TestExceedRateLimitDoesntWrite()
        {
            //Write 110 entries with a limit of 100. Check that the last one written is #100
            StackLog log = new StackLog(KLog.LogLevel.All);
            LogRateLimiter<StackLog> rateLimiter = new LogRateLimiter<StackLog>(log, new TimeSpan(1, 0, 0), 100,
                (entry) => { }, (entry) => { });

            for(int i = 0; i < 110; i++)
            {
                rateLimiter.Debug("test message " + i);
            }

            Assert.AreEqual("test message 99", log.Stack.Pop().Message);
        }
        
        [Test]
        public void TestExceedRateLimitAndThenWait()
        {
            //Set rate limit to re1 message per 10ms
            StackLog log = new StackLog(KLog.LogLevel.All);
            LogRateLimiter<StackLog> rateLimiter = new LogRateLimiter<StackLog>(log, new TimeSpan(0, 0, 0, 0, WAIT_MS), 1,
                (entry) => { }, (entry) => { });

            rateLimiter.Debug("1");
            rateLimiter.Debug("2");
            Thread.Sleep(WAIT_MS);
            rateLimiter.Debug("3");

            //Only 3 & 1 should have been logged
            Assert.AreEqual("3", log.Stack.Pop().Message);
            Assert.AreEqual("1", log.Stack.Pop().Message);
        }

        [Test]
        public void TestExceedRateLimitRunsHandler()
        {
            //Set rate limit to re1 message per 10ms
            StackLog log = new StackLog(KLog.LogLevel.All);
            string exceededOn = null;
            LogRateLimiter<StackLog> rateLimiter = new LogRateLimiter<StackLog>(log, new TimeSpan(1, 0, 0), 1,
                (entry) => 
                {
                    exceededOn = entry.Message;
                }, (entry) => { });

            rateLimiter.Debug("blah");
            rateLimiter.Debug("too much");

            Assert.AreEqual("blah", log.Stack.Pop().Message);
            Assert.AreEqual("too much", exceededOn);
        }

        [Test]
        public void TestEnterExitRateLimitRunsHandler()
        {
            //Set rate limit to re1 message per 10ms
            StackLog log = new StackLog(KLog.LogLevel.All);
            string exceededOn = null;
            string exitedOn = null;
            LogRateLimiter<StackLog> rateLimiter = new LogRateLimiter<StackLog>(log, new TimeSpan(0, 0, 0, 0, WAIT_MS), 1,
                (entry) =>
                {
                    exceededOn = entry.Message;
                }, 
                (entry) => 
                {
                    exitedOn = entry.Message;
                });

            rateLimiter.Debug("blah");
            rateLimiter.Debug("too much");
            rateLimiter.Debug("ignore me");

            Thread.Sleep(WAIT_MS);
            rateLimiter.Debug("back in the game");

            Assert.AreEqual("back in the game", log.Stack.Pop().Message);
            Assert.AreEqual("blah", log.Stack.Pop().Message);
            Assert.AreEqual("too much", exceededOn);
            Assert.AreEqual("back in the game", exitedOn);
        }
    }
}
