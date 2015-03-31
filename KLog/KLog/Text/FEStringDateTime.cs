﻿/*
 * KLog.NET
 * FEDateTime - Formatting Entity that gets a DateTime as a string
 *  Can be either DateTime.Now or the DateTime from a LogEntry, depending
 *  on the context in which it's used
 * Authors:
 *  Josh Keegan 30/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLog.Text
{
    public class FEStringDateTime : LogEntryFormattingEntity, IFormattingEntity
    {
        //Private variables
        private string formatter;

        //Constructors
        public FEStringDateTime(string formatter)
        {
            this.formatter = formatter;
        }

        //Public Methods
        public object Eval()
        {
            return getLogText(DateTime.Now);
        }

        public override object Eval(LogEntry entry)
        {
            return getLogText(entry.EventDate);
        }

        //Private methods
        private string getLogText(DateTime dateTime)
        {
            return dateTime.ToString(formatter);
        }
    }
}
