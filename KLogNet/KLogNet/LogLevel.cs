/*
 * KLog.NET
 * LogLevel enum - Used in logging to determine which type of message is being logged
 * Authors:
 *  Josh Keegan 09/07/2012
 *  
 * Note on implementation: LogLeven gets checked with bitwise logic, so to log 
 *  events on multiple specific log levels, bitwise OR them together to create 
 *  the precise logging criteria desired.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLogNet
{
    public enum LogLevel
    {
        None = 0,
        Debug = 1,
        Info = 2,
        Warning = 4,
        Error = 8,
        All = 15
    }
}
