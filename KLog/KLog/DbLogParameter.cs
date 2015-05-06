/*
 * KLog.NET
 * DbLogParameter - Contains a parameter and its value to be used by the DbLog when building inserts
 * Authors:
 *  Josh Keegan 28/03/2015
 */

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

using KLog.Text;

namespace KLog
{
    public class DbLogParameter
    {
        //Public variables
        private readonly string name;
        private readonly object value;

        //Constructors
        public DbLogParameter(string name, object value)
        {
            this.name = name;
            this.value = value;
        }

        //Internal methods
        internal void addToCommand(DbCommand command, LogEntry entry)
        {
            DbParameter dbParameter = command.CreateParameter();
            dbParameter.ParameterName = name;
            dbParameter.Value = evalValue(entry);
            command.Parameters.Add(dbParameter);
        }

        //Private methods
        private object evalValue(LogEntry entry)
        {
            //Evaluate Value with the FE Evaluator
            return FormattingEntityEvaluator.Eval(value, entry);
        }
    }
}
