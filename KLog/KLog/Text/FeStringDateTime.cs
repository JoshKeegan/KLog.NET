/*
 * KLog.NET
 * FeStringDateTime - Formatting Entity that gets a DateTime as a string.
 *  The act of fetching the DateTime is implemented by FEDateTime
 * Authors:
 *  Josh Keegan 30/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLog.Text
{
    public class FeStringDateTime : LogEntryFormattingEntity, IFormattingEntity
    {
        //Private variables
        private readonly string formatter;
        private readonly FeDateTime feDateTime;

        //Constructors
        public FeStringDateTime(string formatter)
        {
            this.formatter = formatter;
            feDateTime = new FeDateTime();
        }

        //Public Methods
        public object Eval()
        {
            return getLogText((DateTime)feDateTime.Eval());
        }

        public override object Eval(LogEntry entry)
        {
            return getLogText((DateTime)feDateTime.Eval(entry));
        }

        //Private methods
        private string getLogText(DateTime dateTime)
        {
            return dateTime.ToString(formatter);
        }
    }
}
