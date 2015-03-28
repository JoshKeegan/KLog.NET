/*
 * KLog.NET
 * DbLog - Implementation of Log that logs messages to a Database via ADO
 * Authors - Josh Keegan 28/03/2015
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KLog
{
    public class DbLog : Log, IDisposable
    {
        #region Private Variables

        private GetDbConnection getDbConnection;
        private GetDbCommand getDbCommand;
        IEnumerable<DbLogParameter> parameters;
        private bool closeConnections;
        private bool insertAsync;
        private int currentlyInserting = 0;

        #endregion

        #region Implement Log

        protected override void write(LogEntry entry)
        {
            using(DbConnection conn = getDbConnection())
            using(DbCommand command = getDbCommand(conn))
            {
                //If the connection we've been supplied with isn't already open, open it now
                if(conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                //Clear any parameters already on this DbCommand (for when reusing the same command)
                command.Parameters.Clear();

                //Add the parameters and their values to the command
                foreach(DbLogParameter parameter in parameters)
                {
                    DbParameter dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = parameter.Name;
                    dbParameter.Value = parameter.EvalValue(entry);
                    command.Parameters.Add(dbParameter);
                    //TODO: optimisation. Optional DbType parameter here, can then prepare the command (probs 
                    //  just on first run so it makes a difference if the command gets reused but doesn't if not)
                }

                if(insertAsync)
                {
                    currentlyInserting++;

                    //Note: this same functionality could be gottin in .NET 4.0 by spawning a new thread
                    Task<int> insertTask = command.ExecuteNonQueryAsync();
                    insertTask.ContinueWith((insertResult) =>
                    {
                        currentlyInserting--;

                        if(closeConnections)
                        {
                            conn.Close();
                        }
                    });
                }
                else
                {
                    command.ExecuteNonQuery();

                    if(closeConnections)
                    {
                        conn.Close();
                    }
                }
            }
        }

        #endregion

        #region Constructors

        public DbLog(LogLevel logLevel, GetDbConnection getDbConnection, GetDbCommand getDbCommand, 
            IEnumerable<DbLogParameter> parameters, bool closeConnections, bool insertAsync)
            : base(logLevel)
        {
            this.getDbConnection = getDbConnection;
            this.getDbCommand = getDbCommand;
            this.parameters = parameters;
            this.closeConnections = closeConnections;
            this.insertAsync = insertAsync;
        }

        //TODO: Constructor for simple single-threaded use. Once connection, one command, no delegates

        #endregion

        #region Public Delegates

        //Leave how the connection(s) and command(s) are generated to the end-user who will have 
        //  knowledge of hoe their application works and leave them to make decisions such as
        //  whether to reuse the same command
        public delegate DbConnection GetDbConnection();
        public delegate DbCommand GetDbCommand(DbConnection dbConnection);

        #endregion

        #region Public Methods

        public override void BlockWhileWriting()
        {
            if(insertAsync)
            {
                while(currentlyInserting != 0)
                {
                    Thread.Sleep(1);
                }
            }
        }

        #endregion

        #region Implement IDisposable

        public void Dispose()
        {
            this.Dispose(true);
        }

        ~DbLog()
        {
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            //Don't allow the object to be disposed of before log entries have finished being written
            BlockWhileWriting();

            //Calling Dispose(): Free managed resources
            if(disposing)
            {

            }

            //Dispose or finalizer, free any native resources
        }

        #endregion
    }
}
