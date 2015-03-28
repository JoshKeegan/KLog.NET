﻿/*
 * KLog.NET
 * DbLogParameter - Contains a parameter and its value to be used by the DbLog when building inserts
 * Authors:
 *  Josh Keegan 28/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLog
{
    public class DbLogParameter
    {
        //Public variables
        public string Name { get; private set; }
        public object Value { get; private set; }

        //Constructors
        public DbLogParameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        //Public Methods
        public object EvalValue(LogEntry entry)
        {
            //TODO: if Value is a string, evaluate it with the text formatter (once implemented)
            return Value;
        }
    }
}