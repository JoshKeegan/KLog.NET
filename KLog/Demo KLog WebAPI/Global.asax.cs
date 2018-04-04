/*
 * KLog.NET WebAPI Demo
 * Authors:
 *  Josh Keegan 04/04/2018
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

using KLog;
using KLog.Text;
using KLog.Web;

namespace Demo_KLog_WebAPI
{
    public class WebApiApplication : HttpApplication
    {
        // Constants
        private const string LOGS_DIR = "C:/Demo KLog WebAPI Logs";
        private const string LOG_FILE_NAME = "Demo KLog WebAPI";
        private const LogLevel FILE_LOG_LEVEL = LogLevel.All;
        private const bool FILE_LOG_ROTATION = true;
        private const int FILE_LOG_COUNTER_NUM_SIGNIFICANT_FIGURES = 3;

        // Email Logging Rate Limits
#if !DEBUG
        private const int EMAIL_LOG_RATE_LIMIT_NUM_ENTRIES = 100;
        private static readonly TimeSpan EMAIL_LOG_RATE_LIMIT_TIMESPAN = new TimeSpan(1, 0, 0);
#endif

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // Initialise logging
            initLogging();
        }

        private static void initLogging()
        {
            // (optioinal) Set up an internal file log to debug KLog itself & any config issues (e.g. SMTP 
            //  credentials being incorrect)
            InternalLog.Log = new FileLog(new FileLogNameTextFormatter(
                    LOGS_DIR, "/internal/", LOG_FILE_NAME, "_internal.", new FeStringDateTime("yyyy-MM-dd"), ".",
                    new FeStringEvalCounter(FILE_LOG_COUNTER_NUM_SIGNIFICANT_FIGURES), ".log"),
                FILE_LOG_ROTATION, FILE_LOG_LEVEL);

            // Only log to email in release builds
#if DEBUG
            Log emailLog = new NullLog();
#else
            WebEmailLog actualEmailLog = new WebEmailLog("test@visav.net",
                new string[] { "josh@visav.co.uk", "josh.keegan@gmx.com" },
                "mail.visav.net", "test@visav.net", "Qwerty1234",
                LogLevel.Error)
            {
                SubjectFormatter = new LogEntryTextFormatter("KLog Demo Email")
            };
            // Rate limit email logging, so if a huge volume of errors need to be sent we don't spam the mail server
            LogRateLimiter<WebEmailLog> emailLog = new LogRateLimiter<WebEmailLog>(actualEmailLog,
                EMAIL_LOG_RATE_LIMIT_TIMESPAN, EMAIL_LOG_RATE_LIMIT_NUM_ENTRIES,
                // On Entering the rate limit
                (entry) =>
                {
                    // Log something to the email log as an error so anyone getting the emails is notified
                    actualEmailLog.Error("Email logging pausing due to exceeding the rate limit");
                },
                // On Exiting the rate limit
                (entry) =>
                {
                    // Log something to the email log as an error so anyone getting the emails is notified
                    actualEmailLog.Error("Email logging restarting as messages have now dropped below the rate limit");
                });
#endif

            FileLog fileLog = new FileLog(new FileLogNameTextFormatter(
                    LOGS_DIR, "/", LOG_FILE_NAME, ".", new FeStringDateTime("yyyy-MM-dd"), ".",
                    new FeStringEvalCounter(FILE_LOG_COUNTER_NUM_SIGNIFICANT_FIGURES), ".log"),
                FILE_LOG_ROTATION, FILE_LOG_LEVEL);

            DefaultLog.Log = new CompoundLog(emailLog, fileLog);

            DefaultLog.Info("Log Initialised");
        }
    }
}
