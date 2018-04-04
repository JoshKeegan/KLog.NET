/*
 * KLog.NET WebAPI Demo
 * Safe Email Demo - demonstrates usage of the WebEmailLog
 * Authors:
 *  Josh Keegan 04/04/2018
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using KLog;
using KLog.Text;
using KLog.Web;

namespace Demo_KLog_WebAPI.Controllers
{
    public class SafeEmailDemoController : ApiController
    {
        [HttpGet]
        public bool Index()
        {
            // Log safely via a WebEmailLog
            WebEmailLog webEmailLog = new WebEmailLog("test@visav.net",
                new string[] { "josh@visav.co.uk", "josh.keegan@gmx.com" },
                "mail.visav.net", "test@visav.net", "Qwerty1234",
                LogLevel.All)
            {
                SubjectFormatter = new LogEntryTextFormatter("KLog WebEmailLog Demo Email")
            };

            webEmailLog.Info("Test message (safe)");

            return true;
        }
    }
}
