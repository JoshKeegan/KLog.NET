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
            if(o is IFormattingEntity fe)
            {
                object evaluated = fe.Eval();

                //Allow for Formatting Entity chaining
                return Eval(evaluated);
            }
            else if (o is LogEntryTextFormatter letf)
            {
                object evaluated = letf.Eval();

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
            if(o is LogEntryFormattingEntity lefe)
            {
                object evaluated = lefe.Eval(entry);

                // Allow for Formatting Entity chaining
                return Eval(evaluated, entry);
            }
            else if (o is LogEntryTextFormatter letf)
            {
                object evaluated = letf.Eval(entry);

                //Allow for Formatting Entity chaining
                return Eval(evaluated, entry);
            }
            else if(o is IFormattingEntity fe)
            {
                return Eval(fe);
            }
            else // No more evaluation to be performed
            {
                return o;
            }
        }
    }
}
