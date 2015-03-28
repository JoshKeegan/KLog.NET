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
using System.Threading.Tasks;

using KLog;

using Npgsql;

namespace Demo_KLog
{
    class Program
    {
        //Constants
        private const string LOGS_DIR = "logs";
        private const KLog.LogLevel LOG_LEVEL = KLog.LogLevel.All;

        static void Main(string[] args)
        {
            //Initialise Console Logging
            Log consoleLog = new ConcurrentColouredConsoleLog(LOG_LEVEL);

            //Email logging
            EmailLog emailLog = new EmailLog("test@visav.net", 
                new string[] { "josh@visav.co.uk", "josh.keegan@gmx.com" }, 
                "mail.visav.net", "test@visav.net", "Qwerty1234", "KLog Demo Email", 
                KLog.LogLevel.Error);

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
                string logName = String.Format("{0}/{1}.{2}.{3:000}.log", LOGS_DIR, "Test Log", 
                    DateTime.Now.ToString("yyyy-MM-dd"), logAttempt);

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

            //Run the thread-safety demo of the ColouredConsoleLog
            threadSafeColouredConsoleLogDemo();

            //Run the DbLog demo
            threadSafeDbLogDemo();

            //Run the Internal log demo
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

        private static void threadSafeDbLogDemo()
        {
            DbLog.GetDbConnection getDbConnection = (() =>
            {
                //Have had to disable connection pooling, since it seems Npgsql implements this with a normal dictionary (which isn't thread-safe)
                return new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=klogDemoUser;Password=wow_much_security;Database=KLog;Pooling=false");
            });
            DbLog.GetDbCommand getDbCommand = ((conn) =>
            {
                return new NpgsqlCommand("INSERT INTO demo (\"fieldA\", \"fieldB\") VALUES (:fieldA, :fieldB)", (NpgsqlConnection)conn);
            });
            DbLogParameter[] parameters = new DbLogParameter[]
            {
                new DbLogParameter(":fieldA", "valA"),
                new DbLogParameter(":fieldB", "valB")
            };

            //TODO: Async needs some work with exception logging & the BlockWhileWriting() mechanism. So it's diabled here for now
            using(DbLog dbLog = new DbLog(LOG_LEVEL, getDbConnection, getDbCommand, parameters, true, false))
            {
                //Now the log's been made, lets abuse it
                Task[] tasks = new Task[10];

                for (int i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = Task.Factory.StartNew(() =>
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            dbLog.Debug("debug");
                            dbLog.Info("info");
                            dbLog.Warn("warn");
                            dbLog.Error("error");
                        }
                    });
                }

                //Wait for the worker threads to finish their business
                Task.WaitAll(tasks);

                //Block whilst messages are still being written out
                dbLog.BlockWhileWriting();
            }
        }

        private static void internalLogDemo()
        {
            Log internalLogBefore = InternalLog.Log;

            ConcurrentColouredConsoleLog internalLog = new ConcurrentColouredConsoleLog(LOG_LEVEL);
            InternalLog.Log = internalLog;

            //Use an EmailLog with incorrect SMTP server settings to trigger a call to the InternalLog
            EmailLog log = new EmailLog("test@made_up.com", "josh.keegan@also_made_up.com", "smtp.made_up.com",
                "test", "testPassword", KLog.LogLevel.All);
            log.Debug("test");

            InternalLog.Log.BlockWhileWriting();

            //Clean up
            InternalLog.Log = internalLogBefore;
            internalLog.Dispose();
        }
    }
}
