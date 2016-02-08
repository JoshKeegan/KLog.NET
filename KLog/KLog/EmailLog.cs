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

namespace KLog
{
    public class EmailLog : TextLog, IDisposable
    {
        #region Private Variables

        private readonly string fromAddress;
        private readonly string[] toAddresses;
        private readonly string smtpServerHostname;
        private readonly int? smtpServerPort = null;
        private readonly string smtpUsername = null;
        private readonly string smtpPassword = null;
        private readonly string subject = "Log Message (KLog.NET)";
        private volatile int currentlySending = 0;

        #endregion

        #region Log Implementation

        protected override void write(string message)
        {
            //TODO: Once TextLog has custom formatting options, they should be used to format the email body text, rather than it being done here
            string body = String.Format("A message was logged:\n\n{0}", message);

            //Send an email to each of the listed to addresses
            foreach(string toAddress in toAddresses)
            {
                MailMessage mailMessage = new MailMessage(fromAddress, toAddress, subject, body);

                SmtpClient smtpClient;
                if (smtpServerPort == null)
                {
                    smtpClient = new SmtpClient(smtpServerHostname);
                }
                else
                {
                    smtpClient = new SmtpClient(smtpServerHostname, smtpServerPort.GetValueOrDefault());
                }

                if (smtpUsername != null)
                {
                    NetworkCredential creds = new NetworkCredential(smtpUsername, smtpPassword);
                    smtpClient.Credentials = creds;
                }

                //smtpClient.Send(mailMessage);
                smtpClient.SendCompleted += (sender, eventArgs) =>
                {
                    //Dispose of the SMTP Client, hiding any exceptions from the client application.
                    //  Instead they will get sent to the Internal Log which should be monitored during the development of an application
                    try
                    {
                        smtpClient.Dispose();
                    }
                    catch (Exception e)
                    {
                        InternalLog.Error("Error whilst disposing of SMTP Client. Exception\n{0}", e);
                    }

                    //Message sent
#pragma warning disable 420
                    Interlocked.Decrement(ref currentlySending);
#pragma warning restore 420
                };
#pragma warning disable 420
                Interlocked.Increment(ref currentlySending);
#pragma warning restore 420

                //Hide any exceptions when sending this email. Don't want to break the client application due to a logging error
                //  Instead they will get sent to the Internal Log which should be monitored during the development of an application
                try
                {
                    smtpClient.SendAsync(mailMessage, null);
                }
                catch(Exception e)
                {
                    InternalLog.Error("Error whilst sending email. Exception:\n{0}", e);

                    //No longer sending message
                    currentlySending--;
                }
            }
        }

        #endregion

        #region Constructors

        public EmailLog(string fromAddress, string[] toAddresses, string smtpServerHostname, int? smtpServerPort, 
            string smtpUsername, string smtpPassword, LogLevel logLevel)
            : base(logLevel)
        {
            //Validation
            if(fromAddress == null)
            {
                throw new ArgumentNullException(nameof(fromAddress));
            }
            if(toAddresses == null)
            {
                throw new ArgumentNullException(nameof(toAddresses));
            }
            if(smtpServerHostname == null)
            {
                throw new ArgumentNullException(nameof(smtpServerHostname));
            }
            //Validate fromAddress & toAddress are valid email addresses
            if(!isValidEmailAddress(fromAddress))
            {
                throw new ArgumentException("fromAddress is not a valid email address");
            }
            if(!toAddresses.Any())
            {
                throw new ArgumentException("must specify at least one email in toAddresses");
            }
            foreach(string toAddress in toAddresses)
            {
                if (!isValidEmailAddress(toAddress))
                {
                    throw new ArgumentException(String.Format("An value specified in toAddresses ({0}) is not a valid email address", toAddress));
                }
            }
            //Validate smtpServerPort
            if(smtpServerPort != null)
            {
                int port = smtpServerPort.GetValueOrDefault();
                if(port <= 0 || port > 65535)
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
        }

        public EmailLog(string fromAddress, string[] toAddresses, string smtpServerHostname, 
            string smtpUsername, string smtpPassword, LogLevel logLevel)
            : this(fromAddress, toAddresses, smtpServerHostname, (int?)null, smtpUsername, 
            smtpPassword, logLevel) {  }

        public EmailLog(string fromAddress, string[] toAddresses, string smtpServerHostname, int? smtpServerPort,
            string smtpUsername, string smtpPassword, string subject, LogLevel logLevel)
            : this(fromAddress, toAddresses, smtpServerHostname, smtpServerPort, smtpUsername, smtpPassword, logLevel)
        {
            //Only set the subject if we've been supplied with one, otherwise keep the default
            if(subject != null)
            {
                this.subject = subject;
            }
        }

        public EmailLog(string fromAddress, string[] toAddresses, string smtpServerHostname,
            string smtpUsername, string smtpPassword, string subject, LogLevel logLevel)
            : this(fromAddress, toAddresses, smtpServerHostname, null, smtpUsername, smtpPassword, 
            subject, logLevel) { }

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

        public EmailLog(string fromAddress, string toAddress, string smtpServerHostname, int? smtpServerPort,
            string smtpUsername, string smtpPassword, string subject, LogLevel logLevel)
            : this(fromAddress, new string[] { toAddress }, smtpServerHostname, smtpServerPort,
            smtpUsername, smtpPassword, subject, logLevel) {  }

        public EmailLog(string fromAddress, string toAddress, string smtpServerHostname,
            string smtpUsername, string smtpPassword, string subject, LogLevel logLevel)
            : this(fromAddress, new string[] { toAddress }, smtpServerHostname,
            smtpUsername, smtpPassword, subject, logLevel) {  }

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
