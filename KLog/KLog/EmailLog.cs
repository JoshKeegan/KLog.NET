/*
 * KLog.NET
 * EmailLog - Implementation of Log that logs messages by emailing a specified address
 * Authors:
 *  Josh Keegan 16/03/2015
 *  
 * Note: If the program is about to close completely, it should call emailLog.BlockWhileSending()
 *  in order to wait for any emails still being sent (async) to actually get sent.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;

using KLog.Text;

namespace KLog
{
    /// <summary>
    /// Sends log entries by email asynchronously.
    /// Note: Not suitable for use in web applications, use Klog.Web.WebEmailLog instead
    /// </summary>
    public class EmailLog : Log, IDisposable
    {
        #region Private Variables

        private readonly string fromAddress;
        private readonly string[] toAddresses;
        private readonly string smtpServerHostname;
        private readonly int? smtpServerPort = null;
        private readonly string smtpUsername = null;
        private readonly string smtpPassword = null;
        private readonly LogEntryTextFormatter bodyFormatter;
        private volatile int currentlySending = 0;
        private LogEntryTextFormatter subjectFormatter = new LogEntryTextFormatter("Log Message (KLog.NET)");

        #endregion

        #region Public Variables

        public LogEntryTextFormatter SubjectFormatter
        {
            get => subjectFormatter;
            set => subjectFormatter = value ?? throw new ArgumentNullException(nameof(value));
        }

        #endregion

        #region Log Implementation

        protected override void write(LogEntry entry)
        {
            // Format this entry as a string for both the subject & body
            string subject = SubjectFormatter.Eval(entry);
            string body = bodyFormatter.Eval(entry);

            // Send an email to each of the listed to addresses
            foreach (string toAddress in toAddresses)
            {
                // Construct the Mail Message
                MailMessage mailMessage = new MailMessage(fromAddress, toAddress, subject, body);

                // Construct the SMTP client
                SmtpClient smtpClient = smtpServerPort == null
                    ? new SmtpClient(smtpServerHostname)
                    : new SmtpClient(smtpServerHostname, smtpServerPort.GetValueOrDefault());

                if (smtpUsername != null)
                {
                    NetworkCredential creds = new NetworkCredential(smtpUsername, smtpPassword);
                    smtpClient.Credentials = creds;
                }

                // Send the message
                //  Note that how the message gets sent is up to the implementation of the email log we're using.
                //  Default (in this class) is smtpClient.SendAsync, but as sendEmail is virtual, it can be overridden.
                //  This is important for use in websites, where having a Task still running after the end of the request
                //  will mean it gets terminated, so this default implementation wouldn't work unless the tasks finished 
                //  before the request.
                sendEmail(mailMessage, smtpClient);
            }
        }

        #endregion

        #region Constructors

        public EmailLog(string fromAddress, string[] toAddresses, string smtpServerHostname, int? smtpServerPort,
            string smtpUsername, string smtpPassword, LogLevel logLevel, LogEntryTextFormatter bodyFormatter = null)
            : base(logLevel)
        {
            //Validation
            if (fromAddress == null)
            {
                throw new ArgumentNullException(nameof(fromAddress));
            }
            if (toAddresses == null)
            {
                throw new ArgumentNullException(nameof(toAddresses));
            }
            if (smtpServerHostname == null)
            {
                throw new ArgumentNullException(nameof(smtpServerHostname));
            }
            //Validate fromAddress & toAddress are valid email addresses
            if (!isValidEmailAddress(fromAddress))
            {
                throw new ArgumentException("fromAddress is not a valid email address");
            }
            if (!toAddresses.Any())
            {
                throw new ArgumentException("must specify at least one email in toAddresses");
            }
            foreach (string toAddress in toAddresses)
            {
                if (!isValidEmailAddress(toAddress))
                {
                    throw new ArgumentException(String.Format("An value specified in toAddresses ({0}) is not a valid email address", toAddress));
                }
            }
            //Validate smtpServerPort
            if (smtpServerPort != null)
            {
                int port = smtpServerPort.GetValueOrDefault();
                if (port <= 0 || port > 65535)
                {
                    throw new ArgumentOutOfRangeException(nameof(smtpServerPort), "smtpServerPort must be null or in the range 1-65,535 (inclusive)");
                }
            }

            this.fromAddress = fromAddress;
            this.toAddresses = toAddresses;
            this.smtpServerHostname = smtpServerHostname;
            this.smtpServerPort = smtpServerPort;
            this.smtpUsername = smtpUsername;
            this.smtpPassword = smtpPassword;
            this.bodyFormatter = bodyFormatter ??
                                 new LogEntryTextFormatter("A message was logged:\n\n", TextLog.DEFAULT_FORMATTER);
        }

        public EmailLog(string fromAddress, string[] toAddresses, string smtpServerHostname,
            string smtpUsername, string smtpPassword, LogLevel logLevel)
            : this(fromAddress, toAddresses, smtpServerHostname, null, smtpUsername,
                smtpPassword, logLevel) {  }

        //TODO: Add more constructors which don't require optional fields

        /*
         * Constructors with single "to" email address
         */
        public EmailLog(string fromAddress, string toAddress, string smtpServerHostname, int? smtpServerPort, 
            string smtpUsername, string smtpPassword, LogLevel logLevel)
            : this(fromAddress, new string[] { toAddress }, smtpServerHostname, smtpServerPort, smtpUsername, 
            smtpPassword, logLevel) {  }

        public EmailLog(string fromAddress, string toAddress, string smtpServerHostname,
            string smtpUsername, string smtpPassword, LogLevel logLevel)
            : this(fromAddress, new string[] { toAddress }, smtpServerHostname,
            smtpUsername, smtpPassword, logLevel) {  }

        #endregion

        #region Public Methods

        /// <summary>
        /// Block the calling thread until all messages are sent
        /// </summary>
        public override void BlockWhileWriting()
        {
            while (currentlySending != 0)
            {
                Thread.Sleep(1);
            }
        }

        #endregion

        #region Implement IDisposable

        public void Dispose()
        {
            Dispose(true);
        }

        ~EmailLog()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            //Don't allow the object to be disposed of before messages have finished sending
            BlockWhileWriting();

            //Calling Dispose(): Free managed resources
            if(disposing)
            {
                
            }

            //Dispose or finalizer, free any native resources
        }

        #endregion

        #region Protected Methods

        protected virtual void sendEmail(MailMessage mailMessage, SmtpClient smtpClient)
        {
            //smtpClient.Send(mailMessage);
            smtpClient.SendCompleted += (sender, eventArgs) =>
            {
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

                // Message sent
                decrementCurrentlySending();
            };
            incrementCurrentlySending();

            // Hide any exceptions when sending this email. Don't want to break the client application due to a logging error
            //  Instead they will get sent to the Internal Log which should be monitored during the development of an application
            try
            {
                smtpClient.SendAsync(mailMessage, null);
            }
            catch (Exception e)
            {
                InternalLog.Error("Error whilst sending email. Exception:\n{0}", e);

                // No longer sending message
                decrementCurrentlySending();
            }
        }

        // Have halper methods so that anyone extending this class doesn't need to worry about
        //  the thread safety of currentlySending
        protected void incrementCurrentlySending()
        {
#pragma warning disable 420
            Interlocked.Increment(ref currentlySending);
#pragma warning restore 420
        }

        protected void decrementCurrentlySending()
        {
#pragma warning disable 420
            Interlocked.Decrement(ref currentlySending);
#pragma warning restore 420
        }

        #endregion

        #region Private Helpers

        private static bool isValidEmailAddress(string email)
        {
            try
            {
                new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
