using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Znode.Libraries.Data.Helpers;


namespace Znode.Libraries.Data
{
    public class ZnodeViewRepository<T> : IZnodeViewRepository<T> where T : class
    {
        #region Declarations

        private readonly IDBContext _context;
        readonly List<SqlParameter> parameterList = new List<SqlParameter>();
        public string ReturnParameter = string.Empty;
        #endregion

        #region Constructor

        public ZnodeViewRepository()
        {
            _context = HelperMethods.CurrentContext;
        }

        public ZnodeViewRepository(IDBContext context)
        {
            _context = context;
        }

        #endregion

        #region Public Methods
        //Executes the Stored Procedure using the Entity Framework.
        public IList<T> ExecuteStoredProcedureList(string commandText)
        {
            int totalRowCount = 0;
            var result = ExecuteStoredProcedureList(commandText, null, out totalRowCount);
            return result;
        }

        //Executes the Stored Procedure using the Entity Framework, return the total row count for the mentioned index location.
        public IList<T> ExecuteStoredProcedureList(string commandText, int? indexOutParamater, out int totalRowCount)
        {
            var result = ExecuteStoredProcedureList(commandText, 0, indexOutParamater, out totalRowCount);
            return result;
        }

        //Set the Stored Procedure Parameters.
        public void SetParameter(string ParameterName, object ParameterValue, ParameterDirection Direction, DbType dbType)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = ParameterName;
            parameter.Value = ParameterValue;
            parameter.DbType = dbType;
            if (!Equals(Direction, ParameterDirection.Output))
                parameter.Direction = ParameterDirection.Input;
            else
            {
                parameter.Direction = ParameterDirection.Output;
                ReturnParameter = ParameterName;
            }
            parameterList.Add(parameter);
        }

        //Set the Stored Procedure Parameters.
        public void SetTableValueParameter(string ParameterName, object ParameterValue, ParameterDirection Direction, SqlDbType dbType, string tableValueTypeName)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = ParameterName;
            parameter.Value = ParameterValue;
            parameter.SqlDbType = dbType;
            parameter.TypeName = tableValueTypeName;
            if (!Equals(Direction, ParameterDirection.Output))
                parameter.Direction = ParameterDirection.Input;
            else
            {
                parameter.Direction = ParameterDirection.Output;
                ReturnParameter = ParameterName;
            }
            parameterList.Add(parameter);
        }

        //Set the Stored Procedure Parameters.
        public void SetParameter(string ParameterName, object ParameterValue, ParameterDirection Direction, DbType dbType, byte predicate, byte scale)
        {
            SqlParameter parameter = new SqlParameter
            {
                ParameterName = ParameterName,
                Value = ParameterValue,
                DbType = dbType
            };
            if (!Equals(predicate, null))
            {
                parameter.Precision = predicate;
                parameter.Scale = scale;
            }
            if (!Equals(Direction, ParameterDirection.Output))
                parameter.Direction = ParameterDirection.Input;
            else
            {
                parameter.Direction = ParameterDirection.Output;
                ReturnParameter = ParameterName;
            }
            parameterList.Add(parameter);
        }


        //Executes the Stored Procedure using the Entity Framework.
        public IList<T> ExecuteStoredProcedureList(string commandText, int storedProcedureTimeOut)
        {
            int totalRowCount = 0;
            var result = ExecuteStoredProcedureList(commandText, storedProcedureTimeOut, null, out totalRowCount);
            return result;
        }


        //Executes the Stored Procedure using the Entity Framework, return the total row count for the mentioned index location.
        public IList<T> ExecuteStoredProcedureList(string commandText, int storedProcedureTimeOut, int? indexOutParamater, out int totalRowCount)
        {
            totalRowCount = 0;
            try
            {
                Database database = this._context.GetDatabase();

                //Set the time out value for the stored procedure, in seconds base on parameter, instead of using default value of the underlying provider
                if(storedProcedureTimeOut > 0)
                {
                    database.CommandTimeout = storedProcedureTimeOut;
                }

                var result = database.SqlQuery<T>(commandText, parameterList.ToArray()).ToList<T>();
                if (!Equals(result, null) && indexOutParamater.HasValue)
                {
                    totalRowCount = Convert.ToInt32(parameterList[Convert.ToInt32(indexOutParamater)].Value);
                }

                return result;
            }
            catch (Exception ex)
            {
                EntityLogging.LogObject(typeof(T), commandText, ex);
                throw;
            }
            finally
            {
                ClearParameters();
            }
        }
        #endregion

        #region Private Methods
        //Clears the defined parameter.
        private void ClearParameters()
        {
            parameterList.Clear();
        }
        #endregion
    }
}
