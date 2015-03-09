/*
 * KLog.NET
 * Reflection Helpers
 * Authors:
 *  Josh Keegan 16/02/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KLogNet.Helpers
{
    public static class ReflectionHelpers
    {
        public static IEnumerable<Type> GetTypesInNamespace(Assembly assembly, string strNamespace)
        {
            return assembly.GetTypes().Where(t => t.Namespace == strNamespace);
        }
    }
}
