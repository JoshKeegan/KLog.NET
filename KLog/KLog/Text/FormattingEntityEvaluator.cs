/*
 * KLog.NET
 * FormattingEntityEvaluator - evaluates a Formatting Entity
 * Authors:
 *  Josh Keegan 08/04/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KLog.Text
{
    public static class FormattingEntityEvaluator
    {
        public static object Eval(object o)
        {
            if(o is IFormattingEntity)
            {
                IFormattingEntity e = (IFormattingEntity)o;
                object evaluated = e.Eval();

                //Allow for Formatting Entity chaining
                return Eval(evaluated);
            }
            else
            {
                return o;
            }
        }

        public static object Eval(object o, LogEntry entry)
        {
            if(o is LogEntryFormattingEntity)
            {
                LogEntryFormattingEntity e = (LogEntryFormattingEntity)o;
                object evaluated = e.Eval(entry);

                //Allow for Formatting Entity chaining
                return Eval(evaluated, entry);
            }
            else if(o is IFormattingEntity)
            {
                IFormattingEntity e = (IFormattingEntity)o;
                return Eval(e);
            }
            else //No more evaluation to be performed
            {
                return o;
            }
        }
    }
}
