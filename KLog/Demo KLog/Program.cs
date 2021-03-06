﻿/*
 * KLog.NET Demonstration Program
 * Authors:
 *  Josh Keegan 16/03/2015
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using KLog;
using KLog.Programmatic;
using KLog.Text;

using Npgsql;

namespace Demo_KLog
{
    public static class Program
    {
        //Constants
        private const string LOGS_DIR = "logs";
        private const string INTERNAL_LOGS_DIR = LOGS_DIR + "/internal/";
        private const LogLevel LOG_LEVEL = LogLevel.All;
        private const int CONCURRENCY_NUM_TASKS = 10;

        public static void Main(string[] args)
        {
            // Initialise Internal Logging
            Log internalFileLog = new FileLog(new FileLogNameTextFormatter(
                INTERNAL_LOGS_DIR,
                "Internal Test log.",
                new FeStringDateTime("yyyy-MM-dd"),
                ".",
                new FeStringEvalCounter(3),
                ".log"),
                true,
                LogLevel.All);

            InternalLog.Log = internalFileLog;
            InternalLog.Log.Info("Internal Log Initialised");

            //Initialise Console Logging
            Log consoleLog = new ConcurrentColouredConsoleLog(LOG_LEVEL);

            //Email logging
            EmailLog emailLog = new EmailLog("test@visav.net",
                new string[] { "josh@visav.co.uk", "josh.keegan@gmx.com" },
                "mail.visav.net", "test@visav.net", "Qwerty1234",
                LogLevel.Error)
            {
                SubjectFormatter = new LogEntryTextFormatter("KLog Demo Email")
            };


            //Initialise file logging
            Log fileLog = new FileLog(new FileLogNameTextFormatter(
                LOGS_DIR,
                "/Test Log.",
                new FeStringDateTime("yyyy-MM-dd"),
                ".",
                new FeStringEvalCounter(3),
                ".log"), 
                true, 
                LOG_LEVEL);

            DefaultLog.Log = new CompoundLog(consoleLog, fileLog, emailLog);
            DefaultLog.Info("Log Initialised");

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

            //Run the DbLog demo
            threadSafeDbLogDemo();

            //Run the Internal log demo
            internalLogDemo();

            //Run the File Log Rotation Demo
            fileLogRotationDemo();

            // Run the Programmatic Log Demo
            programmaticLogDemo();

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
            /*
             * PostgreSQL
             */
            DbLog.GetDbConnection getDbConnection =
                () =>
                    new NpgsqlConnection(
                        "Server=127.0.0.1;Port=5432;User Id=klogDemoUser;Password=wow_much_security;Database=KLog;Pooling=true");
            DbLog.GetDbCommand getDbCommand =
                conn =>
                    new NpgsqlCommand(
                        "INSERT INTO demo (\"message\", \"logLevel\", \"callingMethodFullName\", \"eventDate\") " +
                        "VALUES (:message, :logLevel, :callingMethodFullName, :eventDate)",
                        (NpgsqlConnection) conn);
            DbLogParameter[] parameters = new DbLogParameter[]
            {
                new DbLogParameter(":message", new FeMessage()),
                new DbLogParameter(":logLevel", new FeLogLevel()),
                new DbLogParameter(":callingMethodFullName", new FeCallingMethodFullName()),
                new DbLogParameter(":eventDate", new FeDateTime())
            };

            /*
             * MS SQL
             */

            /*DbLog.GetDbConnection getDbConnection = (() => new SqlConnection("Server=127.0.0.1; Database=KLog; User ID=klogDemoUser; pwd=wow_much_security"));
            DbLog.GetDbCommand getDbCommand = (conn => 
                new SqlCommand("INSERT INTO demo (message, logLevel, callingMethodFullName, eventDate) " +
                "VALUES (@message, @logLevel, @callingMethodFullName, @eventDate)",
                (SqlConnection)conn));
            DbLogParameter[] parameters = new DbLogParameter[]
            {
                //Can use the MS SQL-Specific implementation of DbLogParameter here and specify types & sizes to reduce DB load (only 
                //  really matters if there is is a huge amount of logging going on)
                new SqlDbLogParameter("@message", new FeMessage(), SqlDbType.NText),
                new SqlDbLogParameter("@logLevel", new FeLogLevel(), SqlDbType.VarChar, 100),
                new SqlDbLogParameter("@callingMethodFullName", new FeCallingMethodFullName(), SqlDbType.NText),
                new SqlDbLogParameter("@eventDate", new FeDateTime(), SqlDbType.DateTime2)
            };*/

            //MS SQL Notes:
            //async doesn't currently work when not running in the debugger (doesn't insert all entries).
            //also async is *much* slower for this rather extreme case

            using(DbLog dbLog = new DbLog(LOG_LEVEL, getDbConnection, getDbCommand, parameters, true, false))
            {
                //Now the log's been made, lets abuse it
                Task[] tasks = new Task[CONCURRENCY_NUM_TASKS];

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
            internalLog.Info("This method is about to put an error on the console. This is the intended behaviour.");
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

        private static void fileLogRotationDemo()
        {
            FileLog fileLog = new FileLog(new FileLogNameTextFormatter(
                LOGS_DIR,
                "/rotationDemo/Rotation.",
                new FeStringDateTime("yyyy-MM-dd_fff"),
                ".",
                new FeStringEvalCounter(3),
                ".log"),
                true,
                LOG_LEVEL);

            for (int i = 0; i < 10; i++)
            {
                fileLog.Debug("Message A {0}", i);
            }
            Thread.Sleep(1);
            for (int i = 0; i < 10; i++)
            {
                fileLog.Debug("Message B {0}", i);
            }
            Thread.Sleep(1);
            for (int i = 0; i < 10; i++)
            {
                fileLog.Debug("Message C {0}", i);
            }

            //Clean up
            fileLog.BlockWhileWriting();
            fileLog.Dispose();
        }

        private static void programmaticLogDemo()
        {
            ConcurrentQueueLog cqLog = new ConcurrentQueueLog(LOG_LEVEL);

            // Demo concurrency here as well
            Task[] tasks = new Task[CONCURRENCY_NUM_TASKS];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 100; j++)
                    {
                        cqLog.Debug("debug");
                        cqLog.Info("info");
                        cqLog.Warn("warn");
                        cqLog.Error("error");
                    }
                });
            }

            // Wait for the worker threads to finish their business
            Task.WaitAll(tasks);

            // Read out the contents of the log
            LogEntry logEntry;
            Dictionary<string, int> messageCounts = new Dictionary<string, int>();
            while (cqLog.Queue.TryDequeue(out logEntry))
            {
                // Can manipulate LogEntries as you please. 
                //  Here we'll just sum up the number of times each distinct message has been logged
                //  This could be running in another thread whilst the rest of the application runs as normal since the underlying data structure here is a ConcurrentQueue.
                if (!messageCounts.ContainsKey(logEntry.Message))
                {
                    messageCounts.Add(logEntry.Message, 0);
                }

                messageCounts[logEntry.Message]++;
            }

            StringBuilder builder = new StringBuilder();
            bool first = true;
            foreach (KeyValuePair<string, int> kvp in messageCounts)
            {
                if (!first)
                {
                    builder.AppendLine();
                }
                else
                {
                    first = false;
                }

                builder.AppendFormat("{0}: {1}", kvp.Key, kvp.Value);
            }

            DefaultLog.Info("Post-processed programmatic log results:\n{0}", builder);
        }
    }
}
