/*
 * KLog.NET: Performance Tests
 * PerfTest abstract class
 * Authors:
 *  Josh Keegan 13/06/2016
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTests
{
    public abstract class PerfTest
    {
        // Public Variables
        public readonly string Description;

        // Constructors
        protected PerfTest(string description)
        {
            Description = description;
        }

        // Public Methods
        public TimeSpan Run()
        {
            // Start timing
            Stopwatch sw = Stopwatch.StartNew();

            // Actually do the work
            run();

            // Stop timing
            sw.Stop();
            return sw.Elapsed;
        }

        // Protected Methods
        protected abstract void run();
    }
}
