/*
 * KLog.NET
 * Default Log - a static class that is used when an application wants to just send log 
 *  messages to some Default Log, whilst still allowing it to have separate logs for specific
 *  parts if desired.
 * Authors:
 *  Josh Keegan 16/02/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLog
{
    public static class DefaultLog
    {
        private static Log log = null;

        public static Log Log
        {
            get
            {
                //If no log has been made yet, create a null one
                if (log == null)
                {
                    log = new NullLog();
                }

                return log;
            }
            set
            {
                log = value;
            }
        }

        //Log level writes (should reflect what is available on a Log object)
        public static void Error(string message)
        {
            Log.Error(message);
        }

        public static void Error(string message, params object[] args)
        {
            Log.Error(message, args);
        }

        public static void Warn(string message)
        {
            Log.Warn(message);
        }

        public static void Warn(string message, params object[] args)
        {
            Log.Warn(message, args);
        }

        public static void Info(string message)
        {
            Log.Info(message);
        }

        public static void Info(string message, params object[] args)
        {
            Log.Info(message, args);
        }

        public static void Debug(string message)
        {
            Log.Debug(message);
        }

        public static void Debug(string message, params object[] args)
        {
            Log.Debug(message, args);
        }

        public void BlockWhileWriting()
        {
            Log.BlockWhileWriting();
        }
    }
}
