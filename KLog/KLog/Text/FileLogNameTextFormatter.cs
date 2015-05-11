/*
 * KLog.NET
 * FileLogNameTextFormatter
 * Authors:
 *  Josh Keegan 11/05/2015
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KLog.Text
{
    public class FileLogNameTextFormatter : LogEntryTextFormatter
    {
        //Constructor
        public FileLogNameTextFormatter(params object[] format)
            : base(format)
        {
            //Validation
            if (!format.OfType<FeEvalCounter>().Any())
            {
                throw new ArgumentException(
                    "format must contain an FeEvalCounter. This is to prevent a log file from ever being overwritten.",
                    "format");
            }
        }

        //Public Methods
        public new string Eval(bool incrementCounters = true)
        {
            if (!incrementCounters)
            {
                StringBuilder builder = new StringBuilder();

                foreach (object o in format)
                {
                    if (o is FeEvalCounter)
                    {
                        FeEvalCounter counter = (FeEvalCounter) o;
                        builder.Append(counter.Eval(false));
                    }
                    else
                    {
                        builder.Append(FormattingEntityEvaluator.Eval(o));
                    }
                }

                return builder.ToString();
            }
            else
            {
                while (true)
                {
                    string filePath = base.Eval();

                    if (!File.Exists(filePath))
                    {
                        return filePath;
                    }
                }
            }
        }

        public void ResetCounters()
        {
            foreach (FeEvalCounter counter in format.OfType<FeEvalCounter>())
            {
                counter.Reset();
            }
        }
    }
}
