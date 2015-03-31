/*
 * KLog.NET
 * LogEntryFormattingEntity - abstract class defining how a class that formats some entity
 *  of a log entry as a string should be defined
 * Authors:
 *  Josh Keegan 30/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLog.Text
{
    public abstract class LogEntryFormattingEntity
    {
        public abstract string Eval(LogEntry entry);
    }
}
