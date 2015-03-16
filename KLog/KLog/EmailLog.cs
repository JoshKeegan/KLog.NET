/*
 * KLog.NET
 * EmailLog - Implementation of Log that logs messages by emailing a specified address
 * Authors:
 *  Josh Keegan 16/03/2015
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace KLog
{
    public class EmailLog : Log, IDisposable
    {
        #region Private Variables

        private string fromAddress;
        private string toAddress;
        private SmtpClient smtpClient;
        private string subject = "Log Message (KLog.NET)";

        #endregion

        #region Log Implementation

        internal override void write(string message, LogLevel logLevel, StackFrame callingFrame, DateTime eventDate)
        {
            message = String.Format("{0}: {1}", logLevel.ToString(), message);

            //TODO: Allow a custom format to be specified, with placeholders for different bits of data to include
            string body = String.Format("A message was logged:\n\n{0} - {1}: {2}",
                eventDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                callingFrame.GetMethod().DeclaringType.FullName,
                message);

            MailMessage mailMessage = new MailMessage(fromAddress, toAddress, subject, body);

            smtpClient.SendAsync(mailMessage, null);
        }

        #endregion

        #region Constructors

        public EmailLog(string fromAddress, string toAddress, SmtpClient smtpClient, LogLevel logLevel)
            : base(logLevel)
        {
            //Validation
            if(fromAddress == null)
            {
                throw new ArgumentNullException("fromAddress");
            }
            if(toAddress == null)
            {
                throw new ArgumentNullException("toAddress");
            }
            if(smtpClient == null)
            {
                throw new ArgumentNullException("smtpClient");
            }
            //Validate fromAddress & toAddress are valid email addresses
            if(!isValidEmailAddress(fromAddress))
            {
                throw new ArgumentException("fromAddress is not a valid email address");
            }
            if(!isValidEmailAddress(toAddress))
            {
                throw new ArgumentException("toAddress is not a valid email address");
            }           

            this.fromAddress = fromAddress;
            this.toAddress = toAddress;
            this.smtpClient = smtpClient;
        }

        public EmailLog(string fromAddress, string toAddress, SmtpClient smtpClient, string subject, 
            LogLevel logLevel)
            : this(fromAddress, toAddress, smtpClient, logLevel)
        {
            //Only set the subject if we've been supplied with one, otherwise keep the default
            if(subject != null)
            {
                this.subject = subject;
            }
        }

        public EmailLog(string fromAddress, string toAddress, string smtpServerHostName, string subject, 
            LogLevel logLevel)
            : this(fromAddress, toAddress, new SmtpClient(smtpServerHostName), subject, logLevel) {  }

        public EmailLog(string fromAddress, string toAddress, string smtpServer, int smtpServerPort, 
            string subject, LogLevel logLevel)
            : this(fromAddress, toAddress, new SmtpClient(smtpServer, smtpServerPort), logLevel) {  }

        public EmailLog(string fromAddress, string toAddress, string smtpServer, NetworkCredential smtpCred, 
            LogLevel logLevel)
            : this(fromAddress, toAddress, applyCredential(new SmtpClient(smtpServer),
            smtpCred), logLevel) {  }

        public EmailLog(string fromAddress, string toAddress, string smtpServer, int smtpServerPort,
            NetworkCredential smtpCred, LogLevel logLevel)
            : this(fromAddress, toAddress, applyCredential(new SmtpClient(smtpServer, smtpServerPort), 
            smtpCred), logLevel) {  }

        public EmailLog(string fromAddress, string toAddress, string smtpServer, int smtpServerPort, 
            string smtpUsername, string smtpPassword, string subject, LogLevel logLevel)
            : this(fromAddress, toAddress, smtpServer, smtpServerPort, 
            new NetworkCredential(smtpUsername, smtpPassword), logLevel) {  }

        public EmailLog(string fromAddress, string toAddress, string smtpServer, string smtpUsername, 
            string smtpPassword, string subject, LogLevel logLevel)
            : this(fromAddress, toAddress, smtpServer, new NetworkCredential(smtpUsername, smtpPassword), 
            logLevel) {  }

        #endregion

        #region Implement IDisposable

        public void Dispose()
        {
            this.Dispose(true);
        }

        ~EmailLog()
        {
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            //Calling Dispose(): Free managed resources
            if(disposing)
            {
                smtpClient.Dispose();
            }

            //Dispose or finalizer, free any native resources
        }

        #endregion

        #region Private Helpers

        private static SmtpClient applyCredential(SmtpClient client, NetworkCredential cred)
        {
            client.Credentials = cred;
            return client;
        }

        private static bool isValidEmailAddress(string email)
        {
            try
            {
                MailAddress address = new MailAddress(email);
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
