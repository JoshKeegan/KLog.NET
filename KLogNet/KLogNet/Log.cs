/*
 * KLog.NET
 * Log class (abstract) - provides the methods necessary to use the log
 * Authors:
 *  Josh Keegan 16/02/2015
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using KLogNet.Helpers;

namespace KLogNet
{
    public abstract class Log
    {
        //Constants
        private const int STACK_FRAME_SEARCH_FROM_IDX = 3;

        //Private Methods
        private static HashSet<Type> _typesInNamespace = null;
        private static HashSet<Type> typesInNamespace
        {
            get
            {
                if (_typesInNamespace == null)
                {
                    //Get this assembly and namespace (via reflection so it won't be broken if this code is moved or renamed)
                    Assembly assembly = Assembly.GetAssembly(typeof(Log));
                    string strNamespace = typeof(Log).Namespace;

                    //Find all of the Types that are in this namespace
                    IEnumerable<Type> typesInNamespace = ReflectionHelpers.GetTypesInNamespace(assembly, strNamespace);
                    _typesInNamespace = new HashSet<Type>(typesInNamespace);
                }
                return _typesInNamespace;
            }
        }

        //Protected Variables
        protected LogLevel logLevel;

        //Constructors
        public Log(LogLevel logLevel)
        {
            this.logLevel = logLevel;
        }

        //Public Methods
        //Log Level Writes
        public void Error(string message)
        {
            tryWriteLevel(LogLevel.Error, message);
        }

        public void Error(string message, params object[] args)
        {
            Error(String.Format(message, args));
        }

        public void Warn(string message)
        {
            tryWriteLevel(LogLevel.Warning, message);
        }

        public void Warn(string message, params object[] args)
        {
            Warn(String.Format(message, args));
        }

        public void Info(string message)
        {
            tryWriteLevel(LogLevel.Info, message);
        }

        public void Info(string message, params object[] args)
        {
            Info(String.Format(message, args));
        }

        public void Debug(string message)
        {
            tryWriteLevel(LogLevel.Debug, message);
        }

        public void Debug(string message, params object[] args)
        {
            Debug(String.Format(message, args));
        }

        //Private methods
        public void tryWriteLevel(LogLevel logLevel, string message)
        {
            if (mayWriteLevel(logLevel))
            {
                write(message, logLevel);
            }
        }

        private bool mayWriteLevel(LogLevel logLevel)
        {
            //Use bitwise AND for checking specific rights
            return (this.logLevel & logLevel) == logLevel;
        }

        protected abstract void write(string message, LogLevel logLevel);

        protected static string getCaller()
        {
            StackTrace trace = new StackTrace();
            string caller = "";

            //Find which method called the Log, excluding those in this namespace
            for (int i = STACK_FRAME_SEARCH_FROM_IDX; i < trace.FrameCount; i++)
            {
                StackFrame frame = trace.GetFrame(i);

                if (frame != null)
                {
                    Type callingType = frame.GetMethod().DeclaringType;

                    if (callingType != null && !typesInNamespace.Contains(callingType))
                    {
                        //Found the first frame outside of this namespace
                        caller = String.Format("{0}: ", frame.GetMethod().DeclaringType.FullName);
                        break;
                    }
                }
            }
            return caller;
        }
    }
}
