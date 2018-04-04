/*
 * KLog.NET
 * WebEmailLog - Implementation of an EmailLog that can be safely used within web applications
 * Authors:
 *  Josh Keegan 04/04/2018
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;

using KLog.Text;

namespace KLog.Web
{
    /// <summary>
    /// An implementation of EmailLog that can be safely used within web applications
    /// </summary>
    public class WebEmailLog : EmailLog
    {
        #region Constructors

        public WebEmailLog(string fromAddress, string[] toAddresses, string smtpServerHostname, int? smtpServerPort,
            string smtpUsername, string smtpPassword, LogLevel logLevel, LogEntryTextFormatter bodyFormatter = null) :
            base(fromAddress, toAddresses, smtpServerHostname, smtpServerPort, smtpUsername, smtpPassword, logLevel,
                bodyFormatter) {  }

        public WebEmailLog(string fromAddress, string[] toAddresses, string smtpServerHostname, string smtpUsername,
            string smtpPassword, LogLevel logLevel) : base(fromAddress, toAddresses, smtpServerHostname, smtpUsername,
            smtpPassword, logLevel) {  }

        public WebEmailLog(string fromAddress, string toAddress, string smtpServerHostname, int? smtpServerPort,
            string smtpUsername, string smtpPassword, LogLevel logLevel) : base(fromAddress, toAddress,
            smtpServerHostname, smtpServerPort, smtpUsername, smtpPassword, logLevel) {  }

        public WebEmailLog(string fromAddress, string toAddress, string smtpServerHostname, string smtpUsername,
            string smtpPassword, LogLevel logLevel) : base(fromAddress, toAddress, smtpServerHostname, smtpUsername,
            smtpPassword, logLevel) {  }

        #endregion

        #region Email Sending

        protected override void sendEmail(MailMessage mailMessage, SmtpClient smtpClient)
        {
            // Use QBWI to start a task that will be left running after a web request completes:
            //  https://blogs.msdn.microsoft.com/webdev/2014/06/04/queuebackgroundworkitem-to-reliably-schedule-and-run-background-processes-in-asp-net/
            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                incrementCurrentlySending();

                try
                {
                    smtpClient.Send(mailMessage);

                    // Dispose of the SMTP Client, hiding any exceptions from the client application.
                    //  Instead they will get sent to the Internal Log which should be monitored during the development of an application
                    try
                    {
                        smtpClient.Dispose();
                    }
                    catch (Exception e)
                    {
                        InternalLog.Error("Error whilst disposing of SMTP Client. Exception\n{0}", e);
                    }
                }
                catch (Exception e)
                {
                    InternalLog.Error("Error whilst sending email. Exception:\n{0}", e);
                }
                finally
                {
                    // Message finished being processed (either sent, or errored)
                    decrementCurrentlySending();
                }
            });
        }

        #endregion
    }
}
