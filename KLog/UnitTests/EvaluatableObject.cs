/*
 * KLog.NET Unit Tests
 * Evaluatable Object - an object that stores whether it's ToString() method gets evaluated
 * Authors:
 * Josh Keegan 11/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests
{
    internal class EvaluatableObject
    {
        internal bool Evaluated { get; private set; }

        public EvaluatableObject()
        {
            Evaluated = false;
        }

        public override string ToString()
        {
            Evaluated = true;
            return "A wild Evaluatable Object appeared!";
        }
    }
}
