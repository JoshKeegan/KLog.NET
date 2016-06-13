/*
 * KLog.NET: Performance Tests
 * Program entry point
 * Authors:
 *  Josh Keegan 13/06/2016
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using KLog;
using KLog.Text;

namespace PerformanceTests
{
    public static class Program
    {
        // Constants
        private const string LOGS_DIR = "logs";
        private const string INTERNAL_LOGS_DIR = LOGS_DIR + "/internal/";
        private const LogLevel LOG_LEVEL = LogLevel.All;

        private const int NUM_RUNS = 3;

        public static void Main(string[] args)
        {
            // Initialise Internal Logging
            Log internalFileLog = new FileLog(new FileLogNameTextFormatter(
                INTERNAL_LOGS_DIR,
                "Internal Test log.",
                new FeStringDateTime("yyyy-MM-dd"),
                ".",
                new FeStringEvalCounter(3),
                ".log"),
                true,
                LogLevel.All);

            InternalLog.Log = internalFileLog;
            InternalLog.Log.Info("Internal Log Initialised");

            //Initialise Console Logging
            Log consoleLog = new ConcurrentColouredConsoleLog(LOG_LEVEL);

            // Initialise file logging
            Log fileLog = new FileLog(new FileLogNameTextFormatter(
                LOGS_DIR,
                "/Performance Tests Log.",
                new FeStringDateTime("yyyy-MM-dd"),
                ".",
                new FeStringEvalCounter(3),
                ".log"),
                true,
                LOG_LEVEL);

            DefaultLog.Log = new CompoundLog(consoleLog, fileLog);
            DefaultLog.Info("Log Initialised");

            // Get all the perf tests to be run
            IEnumerable<PerfTest> perfTests = getPerfTestsInstances();

            foreach (PerfTest perfTest in perfTests)
            {
                runTest(perfTest);
            }

            //Wait for anything in the default log to be written out
            DefaultLog.BlockWhileWriting();
        }

        // Private Methods
        private static void runTest(PerfTest perfTest)
        {
            DefaultLog.Info("Running Perf Test: {0}", perfTest.Description);

            TimeSpan totalTime = new TimeSpan(0);
            for (int i = 1; i <= NUM_RUNS; i++)
            {
                TimeSpan ts = perfTest.Run();
                DefaultLog.Info("Run {0}: {1}", i, ts);

                totalTime += ts;
            }

            // Average the times
            TimeSpan avgTime = new TimeSpan(totalTime.Ticks / NUM_RUNS);
            DefaultLog.Info("Avg: {0}", avgTime);
        }

        private static IEnumerable<PerfTest> getPerfTestsInstances()
        {
            // Get the types of all the implementations of PerfTest
            IEnumerable<Type> types =
                Assembly.GetAssembly(typeof(PerfTest))
                    .GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(PerfTest)));

            // Create an instance of each type
            IEnumerable<PerfTest> instances = types.Select(t => (PerfTest) Activator.CreateInstance(t));

            // Sort them by description text
            IEnumerable<PerfTest> sorted = instances.OrderBy(pt => pt.Description);

            return sorted;
        }
    }
}
