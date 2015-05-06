/*
 * KLog.NET
 * FeLogLevel - Formatting Entity that gets a LogLevel as a string
 * Authors:
 *  Josh Keegan 30/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLog.Text
{
    public class FeLogLevel : LogEntryFormattingEntity
    {
        public override object Eval(LogEntry entry)
        {
            return entry.LogLevel.ToString();
        }
    }
}
