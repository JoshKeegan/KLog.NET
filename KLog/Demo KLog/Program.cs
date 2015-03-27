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
            Log consoleLog = new ColouredConsoleLog(LOG_LEVEL);

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
                    fileLog = new FileLog(logName, LOG_LEVEL);

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

            DefaultLog.Info("Waiting for Log Emails to be sent before application shutdown . . .");
            emailLog.BlockWhileWriting();

            //Run the thread-safety demo of the ColouredConsoleLog
            threadSafeColouredConsoleLogDemo();
        }

        private static void threadSafeColouredConsoleLogDemo()
        {
            ColouredConsoleLog log = new ColouredConsoleLog(LOG_LEVEL);

            Task[] tasks = new Task[100];

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

            //Flush out the contents of the log (i.e wait for all of the messages to actually get written to the console)
            log.BlockWhileWriting();
        }
    }
}
