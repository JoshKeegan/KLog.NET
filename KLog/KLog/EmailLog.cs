﻿/*
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
using System.Threading;

namespace KLog
{
    public class EmailLog : Log, IDisposable
    {
        #region Private Variables

        private string fromAddress;
        private string toAddress;
        private string smtpServerHostname;
        private int? smtpServerPort = null;
        private string smtpUsername = null;
        private string smtpPassword = null;
        private string subject = "Log Message (KLog.NET)";
        private int currentlySending = 0;

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

            SmtpClient smtpClient;
            if(smtpServerPort == null)
            {
                smtpClient = new SmtpClient(smtpServerHostname);
            }
            else
            {
                smtpClient = new SmtpClient(smtpServerHostname, smtpServerPort.GetValueOrDefault());
            }

            if(smtpUsername != null)
            {
                NetworkCredential creds = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.Credentials = creds;
            }

            //smtpClient.Send(mailMessage);
            smtpClient.SendCompleted += (sender, eventArgs) =>
            {
                //TODO: Should the EmailLog take another log to log messages to in the event of failure??

                MailMessage callbackMessage = (MailMessage)eventArgs.UserState;

                //Dispose of the SMTP Client and MailMessage
                smtpClient.Dispose();
                callbackMessage.Dispose();

                currentlySending--;
            };
            currentlySending++;
            smtpClient.SendAsync(mailMessage, null);
        }

        #endregion

        #region Constructors

        public EmailLog(string fromAddress, string toAddress, string smtpServerHostname, int? smtpServerPort, 
            string smtpUsername, string smtpPassword, LogLevel logLevel)
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
            if(smtpServerHostname == null)
            {
                throw new ArgumentNullException("smtpServerHostname");
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
            //Validate smtpServerPort
            if(smtpServerPort != null)
            {
                int port = smtpServerPort.GetValueOrDefault();
                if(port <= 0 || port > 65535)
                {
                    throw new ArgumentOutOfRangeException("smtpServerPort must be null or in the range 1-65,535 (inclusive)");
                }
            }

            this.fromAddress = fromAddress;
            this.toAddress = toAddress;
            this.smtpServerHostname = smtpServerHostname;
            this.smtpServerPort = smtpServerPort;
            this.smtpUsername = smtpUsername;
            this.smtpPassword = smtpPassword;
        }

        public EmailLog(string fromAddress, string toAddress, string smtpServerHostname, 
            string smtpUsername, string smtpPassword, LogLevel logLevel)
            : this(fromAddress, toAddress, smtpServerHostname, (int?)null, smtpUsername, 
            smtpPassword, logLevel) {  }

        public EmailLog(string fromAddress, string toAddress, string smtpServerHostname, int? smtpServerPort,
            string smtpUsername, string smtpPassword, string subject, LogLevel logLevel)
            : this(fromAddress, toAddress, smtpServerHostname, smtpServerPort, smtpUsername, smtpPassword, logLevel)
        {
            //Only set the subject if we've been supplied with one, otherwise keep the default
            if(subject != null)
            {
                this.subject = subject;
            }
        }

        public EmailLog(string fromAddress, string toAddress, string smtpServerHostname,
            string smtpUsername, string smtpPassword, string subject, LogLevel logLevel)
            : this(fromAddress, toAddress, smtpServerHostname, null, smtpUsername, smtpPassword, 
            subject, logLevel) { }

        //TODO: Add more constructors which don't require optional fields

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
                
            }

            //Dispose or finalizer, free any native resources

            //Don't allow the object to be disposed of before messages have finished sending
            while(currentlySending != 0)
            {
                Thread.Sleep(1);
            }
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