/*
 * KLog.NET Demonstration Program
 * Authors:
 *  Josh Keegan 16/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using KLog;
using System.Threading.Tasks;

namespace Demo_KLog
{
    class Program
    {
        //Constants
        private const string LOGS_DIR = "logs";
        private const LogLevel LOG_LEVEL = LogLevel.All;

        static void Main(string[] args)
        {
            //Initialise Console Logging
            Log consoleLog = new ConcurrentColouredConsoleLog(LOG_LEVEL);

            //Email logging
            EmailLog emailLog = new EmailLog("test@visav.net", new string[] { "josh@visav.co.uk", "josh.keegan@gmx.com" }, "mail.visav.net", "test@visav.net", "Qwerty1234", "KLog Demo Email", LogLevel.Error);

            //Initialise file logging
            Log fileLog = null;
            bool dirCreated = false;
            if (!System.IO.Directory.Exists(LOGS_DIR))
            {
                dirCreated = true;
                System.IO.Directory.CreateDirectory(LOGS_DIR);
            }

            int logAttempt = 0;
            while (true)
            {
                string logName = String.Format("{0}/{1}.{2}.{3:000}.log", LOGS_DIR, "Test Log", DateTime.Now.ToString("yyyy-MM-dd"), logAttempt);

                if (!System.IO.File.Exists(logName))
                {
                    fileLog = new ConcurrentFileLog(logName, LOG_LEVEL);

                    break;
                }
                logAttempt++;
            }

            DefaultLog.Log = new CompoundLog(consoleLog, fileLog, emailLog);
            DefaultLog.Info("Log Initialised");

            if (dirCreated)
            {
                DefaultLog.Warn("Logs directory was not found, creating . . .");
            }

            DefaultLog.Info("test");

            DefaultLog.Debug("some debug message with super important stuff going on");
            DefaultLog.Info("some info for you sire...");
            DefaultLog.Warn("amber alert");
            DefaultLog.Error("shit son");

            consoleLog.Info("Test just console log");
            fileLog.Info("Test just file log");

            DefaultLog.Info("Waiting for all Logs to finish writing . . .");

            //Run the thread-safety demo of the ColouredConsoleLog
            threadSafeColouredConsoleLogDemo();

            internalLogDemo();

            //Wait for anything in the default log to be written out
            DefaultLog.BlockWhileWriting();
        }

        private static void threadSafeColouredConsoleLogDemo()
        {
            ConcurrentColouredConsoleLog log = new ConcurrentColouredConsoleLog(LOG_LEVEL);

            Task[] tasks = new Task[100];

            //Do some work in other threads
            for(int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    for(int j = 0; j < 10; j++)
                    {
                        log.Debug("debug");
                        log.Info("info");
                        log.Warn("warn");
                        log.Error("error");
                    }
                });
            }

            //Wait for the worker threads to finish their business
            Task.WaitAll(tasks);

            //Block whilst messages are still being writen out
            log.BlockWhileWriting();
        }

        private static void internalLogDemo()
        {
            Log internalLogBefore = InternalLog.Log;

            ConcurrentColouredConsoleLog internalLog = new ConcurrentColouredConsoleLog(LOG_LEVEL);
            InternalLog.Log = internalLog;

            //Use an EmailLog with incorrect SMTP server settings to trigger a call to the InternalLog
            EmailLog log = new EmailLog("test@made_up.com", "josh.keegan@also_made_up.com", "smtp.made_up.com",
                "test", "testPassword", LogLevel.All);
            log.Debug("test");

            InternalLog.Log.BlockWhileWriting();

            //Clean up
            InternalLog.Log = internalLogBefore;
            internalLog.Dispose();
        }
    }
}
