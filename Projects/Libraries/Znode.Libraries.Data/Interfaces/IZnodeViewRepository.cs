using System.Collections.Generic;
using System.Data;

namespace Znode.Libraries.Data
{
    public interface IZnodeViewRepository<T>
    {
        /// <summary>
        /// Method the Execute the Stored Procedure using Entity Framework.
        /// </summary>
        /// <param name="commandText">Name of the Stored procedure with Parameters</param>
        /// <returns>Return the Stored Procedure Result set.</returns>
        IList<T> ExecuteStoredProcedureList(string commandText);

        /// <summary>
        /// Method the Execute the Stored Procedure using Entity Framework.
        /// </summary>
        /// <param name="commandText">Name of the Stored procedure with Parameters</param>
        /// <param name="indexOutParamater">Index for the Output Param, should start from 0</param>
        /// <param name="totalRowCount">Count of the total Records</param>
        /// <returns>Return the Stored Procedure Result set, along with OutPut param.</returns>
        IList<T> ExecuteStoredProcedureList(string commandText, int? indexOutParamater, out int totalRowCount);

        /// <summary>
        /// Method to Set the Stored Procedure Parameters.
        /// </summary>
        /// <param name="ParameterName">Name of the Parameter</param>
        /// <param name="ParameterValue">Value of the Parameter</param>
        /// <param name="Direction">Direction of the Param like Input or Output</param>
        /// <param name="dbType">Parameter Datatype</param>
        void SetParameter(string ParameterName, object ParameterValue, ParameterDirection Direction, DbType dbType);

        /// <summary>
        /// Method to set table Value parameter of a custom DB type.
        /// </summary>
        /// <param name="ParameterName">Name of the Parameter</param>
        /// <param name="ParameterValue">Value of the Parameter</param>
        /// <param name="Direction">Direction of the Param like Input or Output</param>
        /// <param name="dbType">Parameter Datatype</param>
        /// <param name="tableValueTypeName">Custom DB type name.</param>
        void SetTableValueParameter(string ParameterName, object ParameterValue, ParameterDirection Direction, SqlDbType dbType,string tableValueTypeName);

        /// <summary>
        /// Method to Set the Stored Procedure Parameters.
        /// </summary>
        /// <param name="ParameterName">Name of the Parameter</param>
        /// <param name="ParameterValue">Value of the Parameter</param>
        /// <param name="Direction">Direction of the Param like Input or Output</param>
        /// <param name="dbType">Parameter Datatype</param>
        /// <param name="predicate">The total number of digits to the left and right of the decimal point</param>
        /// <param name="scale">The total number of decimal places</param>
        void SetParameter(string ParameterName, object ParameterValue, ParameterDirection Direction, DbType dbType, byte predicate, byte scale);

        /// <summary>
        /// Method the Execute the Stored Procedure using Entity Framework.
        /// </summary>
        /// <param name="commandText">Name of the Stored procedure with Parameters</param>
        /// <param name="storedProcedureTimeout">Custom time out for stored procedure</param>
        /// <returns>Return the Stored Procedure Result set.</returns>
        IList<T> ExecuteStoredProcedureList(string commandText, int storedProcedureTimeOut);

        /// <summary>
        /// Method to Execute the Stored Procedure using Entity Framework.
        /// </summary>
        /// <param name="commandText">Name of the Stored procedure with Parameters</param>
        /// <param name="storedProcedureTimeout">Custom time out for stored procedure</param>
        /// <param name="indexOutParamater">Index for the Output Param, should start from 0</param>
        /// <param name="totalRowCount">Count of the total Records</param>
        /// <returns>Return the Stored Procedure Result set, along with OutPut parameter.</returns>
        IList<T> ExecuteStoredProcedureList(string commandText, int storedProcedureTimeOut, int? indexOutParamater, out int totalRowCount);

    }
}
