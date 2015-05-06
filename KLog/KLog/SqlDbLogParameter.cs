/*
 * KLog.NET
 * SqlDbLogParameter - Contains a parameter, its valud & type specific to MS SQL to be used by 
 *  DbLog when building inserts
 * Authors:
 *  Josh Keegan 06/05/2015
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KLog
{
    public class SqlDbLogParameter : DbLogParameter
    {
        //Protected variables
        protected readonly SqlDbType type;
        protected readonly int size;

        //Constructors
        public SqlDbLogParameter(string name, object value, SqlDbType type, int size = -1) 
            : base(name, value)
        {
            this.type = type;
            this.size = size;
        }

        //Internal methods
        internal override void addToCommand(DbCommand command, LogEntry entry)
        {
            SqlParameter param = new SqlParameter(name, type, size)
            {
                Value = evalValue(entry)
            };
            command.Parameters.Add(param);
        }
    }
}
