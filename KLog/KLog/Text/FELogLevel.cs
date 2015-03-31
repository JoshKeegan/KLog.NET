/*
 * KLog.NET
 * FELogLevel - Formatting Entity that gets a LogLevel as a string
 * Authors:
 *  Josh Keegan 30/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLog.Text
{
    public class FELogLevel : LogEntryFormattingEntity
    {
        public override string Eval(LogEntry entry)
        {
            return entry.LogLevel.ToString();
        }
    }
}
