/*
 * KLog.NET: Performance Tests
 * Compound Log Performance Test - abstract class
 * Authors:
 *  Josh Keegan 13/06/2016
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTests.LogPerfTests
{
    public abstract class CompoundLogPerfTest : PerfTest
    {
        // Constants
        protected const int NUM_WRITES = 100000;

        protected CompoundLogPerfTest(string description) 
            : base(description) {  }
    }
}
