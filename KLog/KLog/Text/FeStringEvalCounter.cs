/*
 * KLog.NET - Formatting Entity that counts the number of times it has been evaluated as a string
 * Authors:
 *  Josh Keegan 11/05/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KLog.Text
{
    public class FeStringEvalCounter : FeEvalCounter
    {
        private readonly string format;

        //Constructors
        public FeStringEvalCounter(int minNumSignificantFigures, long indexedFrom = DEFAULT_INDEXED_FROM)
            : base(indexedFrom)
        {
            format = "{0:" + new string('0', minNumSignificantFigures) + "}";
        }

        //Public methods
        public override object Eval(bool increment)
        {
            return String.Format(format, base.Eval(increment));
        }
    }
}
