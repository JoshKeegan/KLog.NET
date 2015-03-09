﻿/*
 * KLog.NET
 * ColouredConsoleLog - Implementation of Log that logs messages to a file (thread safe), 
 *  highlighting different Log Levels in different colours
 * Authors:
 *  Josh Keegan 09/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLogNet
{
    public class ColouredConsoleLog : ConsoleLog
    {
        //Constants
        private static readonly LogLevel[] REQUIRED_LOG_LEVELS = new LogLevel[] { 
            LogLevel.Debug, LogLevel.Info, LogLevel.Warning, LogLevel.Error };

        private static readonly Dictionary<LogLevel, ConsoleColor> DEFAULT_FOREGROUND_COLOURS = 
            new Dictionary<LogLevel, ConsoleColor>()
        {
            { LogLevel.Debug, ConsoleColor.Gray },
            { LogLevel.Info, ConsoleColor.White },
            { LogLevel.Warning, ConsoleColor.Yellow },
            { LogLevel.Error, ConsoleColor.Red }
        };

        private const Dictionary<LogLevel, ConsoleColor> DEFAULT_BACKGROUND_COLOURS = null; // null => no change
        
        //Defaults used when filling in any missing log level colours in a specified set of colours to be used for the background
        private static readonly Dictionary<LogLevel, ConsoleColor> DEFAULT_BACKGROUND_COLOURS_FILLING_IN = 
            new Dictionary<LogLevel, ConsoleColor>()
        {
            { LogLevel.Debug, ConsoleColor.Black },
            { LogLevel.Info, ConsoleColor.Black },
            { LogLevel.Warning, ConsoleColor.Black },
            { LogLevel.Error, ConsoleColor.Black }
        };

        //Private variables
        private Dictionary<LogLevel, ConsoleColor> foregroundColours;
        private Dictionary<LogLevel, ConsoleColor> backgroundColours;

        //Log Implementation
        internal override void write(string message, LogLevel logLevel, System.Diagnostics.StackFrame callingFrame, DateTime eventDate)
        {
            //Get the current Console colours (to restore them once we've wrote the log message) 
            ConsoleColor foregroundBefore = Console.ForegroundColor;
            ConsoleColor backgroundBefore = Console.BackgroundColor;

            //Change the colour to the appropriate one for this Log Level

            //If we are changing the foreground colour
            if(foregroundColours != null)
            {
                Console.ForegroundColor = foregroundColours[logLevel];
            }
            //If we are changing the background colour
            if(backgroundColours != null)
            {
                Console.BackgroundColor = backgroundColours[logLevel];
            }

            //Write the message
            base.write(message, logLevel, callingFrame, eventDate);

            //Change the colours back
            Console.ForegroundColor = foregroundBefore;
            Console.BackgroundColor = backgroundBefore;
        }

        //Constructors
        public ColouredConsoleLog(LogLevel logLevel, Dictionary<LogLevel, ConsoleColor> foregroundColours,
            Dictionary<LogLevel, ConsoleColor> backgroundColours)
            : base(logLevel)
        {
            //Normalise the specified colours (to include a value for each of the Log Levels)
            this.foregroundColours = normaliseLogLevelColours(foregroundColours, DEFAULT_FOREGROUND_COLOURS);
            this.backgroundColours = normaliseLogLevelColours(backgroundColours, DEFAULT_BACKGROUND_COLOURS_FILLING_IN);
        }

        public ColouredConsoleLog(LogLevel logLevel, Dictionary<LogLevel, ConsoleColor> foregroundColours)
            : this(logLevel, foregroundColours, DEFAULT_BACKGROUND_COLOURS) {  }

        public ColouredConsoleLog(LogLevel logLevel)
            : this(logLevel, DEFAULT_FOREGROUND_COLOURS) {  }

        //Private helpers
        private static Dictionary<LogLevel, ConsoleColor> normaliseLogLevelColours(
            Dictionary<LogLevel, ConsoleColor> logLevelColours, Dictionary<LogLevel, ConsoleColor> defaults)
        {
            //null is valid (as it represents no change to the consoles existing colour)
            if(logLevelColours != null)
            {
                List<LogLevel> toAdd = new List<LogLevel>();
                foreach(LogLevel logLevel in REQUIRED_LOG_LEVELS)
                {
                    if(!logLevelColours.ContainsKey(logLevel))
                    {
                        toAdd.Add(logLevel);
                    }
                }

                foreach(LogLevel logLevel in toAdd)
                {
                    logLevelColours.Add(logLevel, defaults[logLevel]);
                }
            }
            return logLevelColours;
        }
    }
}