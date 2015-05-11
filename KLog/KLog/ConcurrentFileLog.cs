/*
 * KLog.NET
 * ConcurrentFileLog - Implementation of Log that logs messages to a file,
 *  using a separate worker thread to perform the writes in order to not block the calling threads
 * Authors:
 *  Josh Keegan 30/03/2015
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KLog.Text;

namespace KLog
{
    public class ConcurrentFileLog : LogConcurrencyWrapper<FileLog>
    {
        //Constructors (should mirror those available in FileLog)
        public ConcurrentFileLog(FileLogNameTextFormatter feFilePath, bool rotate, LogLevel logLevel)
            : base(new FileLog(feFilePath, rotate, logLevel)) {  }

        public ConcurrentFileLog(string filePath, LogLevel logLevel)
            : base(new FileLog(filePath, logLevel)) {  }
    }
}
