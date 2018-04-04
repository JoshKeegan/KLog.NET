/*
 * KLog.NET WebAPI Demo
 * Unsafe Email Demo - demonstrates (unsafe) behaviour of a regular EmailLog when used in a website.
 *  See SafeEmailDemoController for correct implementation.
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

namespace Demo_KLog_WebAPI.Controllers
{
    public class UnsafeEmailDemoController : ApiController
    {
        [HttpGet]
        public bool Index()
        {
            // Log unsafely via a regular EmailLog
            //  This could work some of the time, but whenever the request completes before the task sending the email
            //  it will break.
            EmailLog emailLog = new EmailLog("test@visav.net",
                new string[] { "josh@visav.co.uk", "josh.keegan@gmx.com" },
                "mail.visav.net", "test@visav.net", "Qwerty1234",
                LogLevel.All)   
            {
                SubjectFormatter = new LogEntryTextFormatter("KLog EmailLog Demo Email")
            };

            emailLog.Info("Test message (unsafe)");

            return true;
        }
    }
}
