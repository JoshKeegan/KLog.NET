/*
 * KLog.NET - Formatting Entity that gets a DateTime
 *  Can be either DateTime.Now or the DateTime from a LogEntry,
 *  depending on the context in which it's used
 * Authors:
 *  Josh Keegan 31/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLog.Text
{
    public class FEDateTime : LogEntryFormattingEntity, IFormattingEntity
    {
        //Public Methods
        public override object Eval(LogEntry entry)
        {
            return entry.EventDate;
        }

        public object Eval()
        {
            return DateTime.Now;
        }
    }
}
