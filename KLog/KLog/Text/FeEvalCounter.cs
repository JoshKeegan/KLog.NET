/*
 * KLog.NET - Formatting Entity that counts the number of 
 *  times it has been evaluated
 * Authors:
 *  Josh Keegan 11/05/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KLog.Text
{
    public class FeEvalCounter : LogEntryFormattingEntity, IFormattingEntity
    {
        private long count;

        //Constructors
        public FeEvalCounter(long indexedFrom = 0)
        {
            Interlocked.Exchange(ref count, indexedFrom - 1);
        }

        //Public Methods
        public override object Eval(LogEntry entry)
        {
            return Eval(true);
        }

        public object Eval()
        {
            return Eval(true);
        }

        public virtual object Eval(bool increment)
        {
            if (increment)
            {
                Interlocked.Increment(ref count);
            }
            return Interlocked.Read(ref count);
        }

        public void Reset()
        {
            Interlocked.Exchange(ref count, 0);
        }
    }
}
