using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Multifront.PaymentApplication.Data.Interface;
using Znode.Multifront.PaymentApplication.Helpers;

namespace Znode.Multifront.PaymentApplication.Data.Repository
{
    public class ZnodePaymentViewRepository<T> : IZnodePaymentViewRepository<T> where T : class
    {
        #region Declarations

        private readonly IDBContext _context;
        readonly List<SqlParameter> parameterList = new List<SqlParameter>();
        public string ReturnParameter = string.Empty;
        #endregion

        #region Constructor

        public ZnodePaymentViewRepository()
        {
            _context = znode_multifront_paymentEntities.Current;
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
            totalRowCount = 0;
            try
            {
                var result = this._context.GetDatabase().SqlQuery<T>(commandText, parameterList.ToArray()).ToList<T>();
                if (!Equals(result, null) && indexOutParamater.HasValue)
                {
                    totalRowCount = Convert.ToInt32(parameterList[Convert.ToInt32(indexOutParamater)].Value);
                }

                return result;
            }
            catch (Exception ex)
            {
                Logging.LogMessage(commandText+" : "+ ex.Message, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw;
            }
            finally
            {
                ClearParameters();
            }
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
