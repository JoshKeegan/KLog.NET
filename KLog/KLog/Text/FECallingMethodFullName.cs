/*
 * KLog.NET
 * FECallingMethodFullName - Formatting Entity that gets the full name of the calling method
 *  in a LogEntry
 * Authors:
 *  Josh Keegan 30/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLog.Text
{
    public class FECallingMethodFullName : LogEntryFormattingEntity
    {
        public override string GetLogText(LogEntry entry)
        {
            return entry.CallingFrame.GetMethod().DeclaringType.FullName;
        }
    }
}
