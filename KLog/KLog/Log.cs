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

using KLog.Helpers;

namespace KLog
{
    public abstract class Log
    {
        //Constants
        private const int STACK_FRAME_SEARCH_FROM_IDX = 1;

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

        //Public Variables
        public LogLevel logLevel { get; protected set; }

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
            tryWriteLevel(LogLevel.Error, message, args);
        }

        public void Warn(string message)
        {
            tryWriteLevel(LogLevel.Warning, message);
        }

        public void Warn(string message, params object[] args)
        {
            tryWriteLevel(LogLevel.Warning, message, args);
        }

        public void Info(string message)
        {
            tryWriteLevel(LogLevel.Info, message);
        }

        public void Info(string message, params object[] args)
        {
            tryWriteLevel(LogLevel.Info, message, args);
        }

        public void Debug(string message)
        {
            tryWriteLevel(LogLevel.Debug, message);
        }

        public void Debug(string message, params object[] args)
        {
            tryWriteLevel(LogLevel.Debug, message, args);
        }

        /// <summary>
        /// Block the calling thread until all entries are written
        /// </summary>
        public virtual void BlockWhileWriting() 
        {
            //Do nothing, method here to allow implementations to implement
        }

        //Private methods
        private void tryWriteLevel(LogLevel logLevel, string message)
        {
            if (mayWriteLevel(logLevel))
            {
                write(new LogEntry(message, logLevel, getCallingFrame(), DateTime.Now));
            }
        }

        private void tryWriteLevel(LogLevel logLevel, string message, object[] args)
        {
            if(mayWriteLevel(logLevel))
            {
                write(new LogEntry(String.Format(message, args), logLevel, getCallingFrame(), DateTime.Now));
            }
        }

        internal void tryWriteLogLevel(LogEntry entry)
        {
            if(mayWriteLevel(entry.LogLevel))
            {
                write(entry);
            }
        }

        private bool mayWriteLevel(LogLevel logLevel)
        {
            //Use bitwise AND for checking specific rights
            return (this.logLevel & logLevel) == logLevel;
        }

        protected abstract void write(LogEntry entry);

        internal void internalWrite(LogEntry entry)
        {
            write(entry);
        }

        private static StackFrame getCallingFrame()
        {
            StackTrace trace = new StackTrace();

            //Find which method called the Log, excluding those in this namespace
            for (int i = STACK_FRAME_SEARCH_FROM_IDX; i < trace.FrameCount; i++)
            {
                StackFrame frame = trace.GetFrame(i);

                if (frame != null)
                {
                    Type callingType = frame.GetMethod().DeclaringType;

                    //TODO: First outside of Namespace isn't good enough here, since there could be external 
                    //  implementations of Log, or calls to Log.write from within KLog
                    if (callingType != null && !typesInNamespace.Contains(callingType))
                    {
                        //Found the first frame outside of this namespace
                        return frame;
                    }
                }
            }
            return null;
        }
    }
}
