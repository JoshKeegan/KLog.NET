﻿/*
 * KLog.NET
 * FeMessage - Formatting Entity that gets a Log Entry Message as a string
 * Authors:
 *  Josh Keegan 30/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLog.Text
{
    public class FeMessage : LogEntryFormattingEntity
    {
        public override object Eval(LogEntry entry)
        {
            return entry.Message;
        }
    }
}
