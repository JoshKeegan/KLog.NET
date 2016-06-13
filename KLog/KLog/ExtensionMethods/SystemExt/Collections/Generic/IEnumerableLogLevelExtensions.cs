/*
 * KLog.NET
 * IEnumerable<LogLevel> Extension methods
 * Authors:
 *  Josh Keegan 13/06/2016
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KLog.ExtensionMethods.SystemExt.Collections.Generic
{
    public static class IEnumerableLogLevelExtensions
    {
        /// <summary>
        /// Combine the given LogLevels into one that will log if it meets any of the underlying levels
        /// </summary>
        /// <param name="logLevels">LogLevels to combine</param>
        /// <returns>Combined LogLevel</returns>
        public static LogLevel Combine(this IEnumerable<LogLevel> logLevels)
        {
            LogLevel toRet = LogLevel.None;

            // Use bitwise OR to combine all of the LogLevels into one
            foreach (LogLevel l in logLevels)
            {
                toRet = toRet | l;
            }

            return toRet;
        }
    }
}
