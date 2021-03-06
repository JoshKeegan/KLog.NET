﻿/*
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
        public LogLevel LogLevel { get; protected set; }

        //Constructors
        public Log(LogLevel logLevel)
        {
            LogLevel = logLevel;
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
                tryWrite(new LogEntry(message, logLevel, getCallingFrame(), DateTime.Now));
            }
        }

        private void tryWriteLevel(LogLevel logLevel, string message, object[] args)
        {
            if(mayWriteLevel(logLevel))
            {
                tryWrite(new LogEntry(String.Format(message, args), logLevel, getCallingFrame(), DateTime.Now));
            }
        }

        internal void tryWriteLogLevel(LogEntry entry)
        {
            if(mayWriteLevel(entry.LogLevel))
            {
                tryWrite(entry);
            }
        }

        private bool mayWriteLevel(LogLevel logLevel)
        {
            //Use bitwise AND for checking specific rights
            return (LogLevel & logLevel) == logLevel;
        }

        /// <summary>
        /// Try and write an Entry to this Log.
        /// Will catch any errors and log them to the InternalLog rather than propagating them up to the caller.
        /// </summary>
        private void tryWrite(LogEntry entry)
        {
            try
            {
                write(entry);
            }
            catch (Exception e)
            {
                // Check if we have already been through the InternalLog to prevent an infinite loop if the InternalLog writes are what's erroring
                if (!isCallThroughInternalLog())
                {
                    InternalLog.Error("An error occurred whilst attempting to write to a Log. Exception:\n{0}", e);
                }
            }
        }

        /// <summary>
        /// Write an Entry to this Log. 
        /// Need not handle errors. If automatic error handling is required, call Log.tryWrite
        /// </summary>
        protected abstract void write(LogEntry entry);

        internal void internalWrite(LogEntry entry)
        {
            tryWrite(entry);
        }

        private static bool isCallThroughInternalLog()
        {
            StackTrace trace = new StackTrace();

            // Find whether the calling frame has been through the InternalLog class
            for (int i = 0; i < trace.FrameCount; i++)
            {
                StackFrame frame = trace.GetFrame(i);
                Type callingType = frame?.GetMethod().DeclaringType;

                if (callingType != null)
                {
                    // If we've hit InternalLog, bingo!
                    if (callingType == typeof (InternalLog))
                    {
                        return true;
                    }
                }
            }
            // Checked all stack frames & couldn't find InternalLog anywhere in them
            return false;
        }

        private static StackFrame getCallingFrame()
        {
            StackTrace trace = new StackTrace();

            //Find which method called the Log, excluding those in this namespace
            for (int i = STACK_FRAME_SEARCH_FROM_IDX; i < trace.FrameCount; i++)
            {
                StackFrame frame = trace.GetFrame(i);
                Type callingType = frame?.GetMethod().DeclaringType;

                if(callingType != null)
                {
                    //If we've hit InternalLog, the next frame outside of InternalLog (which may be inside KLog)
                    //  is the calling StackFrame
                    if(callingType == typeof(InternalLog))
                    {
                        for (; i < trace.FrameCount; i++)
                        {
                            frame = trace.GetFrame(i);

                            if(frame != null)
                            {
                                callingType = frame.GetMethod().DeclaringType;

                                if(callingType != null)
                                {
                                    if(callingType != typeof(InternalLog))
                                    {
                                        return frame;
                                    }
                                }
                            }
                        }
                    }
                    //Else if this calling type is outside of the KLog namespace
                    else if(!typesInNamespace.Contains(callingType))
                    {
                        return frame;
                    }
                }
            }
            return null;
        }
    }
}
