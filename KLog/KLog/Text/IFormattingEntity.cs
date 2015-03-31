/*
 * KLog.NET
 * IFormattingEntity - Interface that should be implemented by formatting classes
 *  that can be used outside of formatting a log entry (e.g. for a log file name)
 * Authors:
 *  Josh Keegan 30/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KLog.Text
{
    public interface IFormattingEntity
    {
        object Eval();
    }
}
