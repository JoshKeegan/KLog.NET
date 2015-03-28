/*
 * KLog.NET
 * Internal Log - a static class that is used to log messages from internally within KLog
 * Authors:
 *  Josh Keegan 28/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLog
{
    public static class InternalLog
    {
        //Private variables
        private static Log log = null;

        //Public variables
        public static Log Log
        {
            get
            {
                //If no log has been made yet, create a null one
                if(log == null)
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
        internal static void Error(string message)
        {
            Log.Error(message);
        }

        internal static void Error(string message, params object[] args)
        {
            Log.Error(message, args);
        }

        internal static void Warn(string message)
        {
            Log.Warn(message);
        }

        internal static void Warn(string message, params object[] args)
        {
            Log.Warn(message, args);
        }

        internal static void Info(string message)
        {
            Log.Info(message);
        }

        internal static void Info(string message, params object[] args)
        {
            Log.Info(message, args);
        }

        internal static void Debug(string message)
        {
            Log.Debug(message);
        }

        internal static void Debug(string message, params object[] args)
        {
            Log.Debug(message, args);
        }

        public static void BlockWhileWriting()
        {
            Log.BlockWhileWriting();
        }
    }
}
