/*
 * KLog.NET
 * LogEntryTextFormatter - Formats a LogEntry as a string
 * Authors:
 *  Josh Keegan 30/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLog.Text
{
    public class LogEntryTextFormatter
    {
        //Private Variables
        private object[] format;

        //Constructor
        public LogEntryTextFormatter(params object[] format)
        {
            this.format = format;
        }

        /*
         * TODO: Specify format as a string (KISS), which then gets parsed into the corresponding 
         * array of strings & format entities.
         * It should be possible to specify the FormattingEntity by name, e.g. ${logLevel} and also have
         * formatting parameters accepted like ${dateTime:yyyy-MM-dd}
         * 
         * Try and be consistent with String.Format where possible for the additional parameters
         * 
         * Have Formatting Entities that accept an (optional) string formatting parameter implement some
         *  additional interface or abstract class
         *  
         * Consider hiding the params object[] format ctor in favour of tne new string based one in
         * order to keep the API KISS as is the project goal. However, it might be worth keeping since
         * it keeps the library extensible and also allows for the user to cut out the additional overhead 
         * of runtime parsing
         */

        public string Eval(LogEntry entry)
        {
            StringBuilder builder = new StringBuilder();

            foreach(object o in format)
            {
                builder.Append(evalObjectAsString(o, entry));
            }

            return builder.ToString();
        }

        //Private Helpers
        private static string evalObjectAsString(object o, LogEntry entry)
        {
            //If this is already a string, return it
            if(o.GetType() == typeof(string))
            {
                return (string)o;
            }
            //Else If this is a formatter entity, evaluate it
            else if(o is LogEntryFormattingEntity)
            {
                LogEntryFormattingEntity e = (LogEntryFormattingEntity)o;
                object evaluated = e.Eval(entry);

                //Allow for Formatting Entity Chaining
                return evalObjectAsString(evaluated, entry);
            }
            //Otherwise this is some other type, convert it to a string
            else
            {
                return o.ToString();
            }
        }
    }
}
