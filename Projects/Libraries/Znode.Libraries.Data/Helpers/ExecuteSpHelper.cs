using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Znode.Libraries.Data.Helpers
{
    public class ExecuteSpHelper
    {
        #region Variables
        readonly List<SqlParameter> parameterList = new List<SqlParameter>();
        public string ReturnParameter { get; set; } = string.Empty;
        #endregion

        #region SetParameter
        /// <summary>
        /// Add given parameter and its info in parameter list
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="dbType"></param>
        /// <param name="Direction"></param>
        public void ClearParameters()
        {
            parameterList.Clear();
        }

        public void GetParameter(string ParameterName, object ParameterValue, ParameterDirection Direction, SqlDbType dbType)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = ParameterName;
            parameter.Value = ParameterValue;
            parameter.SqlDbType = dbType;
            if (Direction != ParameterDirection.Output)
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
        #endregion

        #region Get SP Result In dataset
        /// <summary>
        /// Execute Stored procedure.
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <returns>Return dataset for given stored procedure</returns>
        public DataSet GetSPResultInDataSet(string storedProcedureName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ZnodeECommerceDB"].ConnectionString;
            DataSet ds = new DataSet();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, connection))
                {
                    //TODO:will remove with new publish implementation.
                    cmd.CommandTimeout = 400;
                    cmd.Parameters.AddRange(parameterList.ToArray());
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    try
                    {
                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                        adapter.Fill(ds);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return ds;
        }

        /// <summary>
        /// Execute stored procedure with out parameter.
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="indexOutParamater"></param>
        /// <param name="status"></param>
        /// <returns>Return dataset for given stored procedure</returns>
        public DataSet GetSPResultInDataSet(string storedProcedureName, int indexOutParamater, out int status)
        {
            status = 0;
            string connectionString = ConfigurationManager.ConnectionStrings["ZnodeECommerceDB"].ConnectionString;
            DataSet ds = new DataSet();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, connection))
                {
                    //TODO:will remove with new publish implementation.
                    cmd.CommandTimeout = 400;
                    cmd.Parameters.AddRange(parameterList.ToArray());
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    try
                    {
                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                        adapter.Fill(ds);
                        if (ds.Tables.Count == 0)
                        {
                            status = Convert.ToInt32(parameterList[Convert.ToInt32(indexOutParamater)].Value);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return ds;
        }

        //Execute the query, return the result in DataSet 
        public DataSet GetQueryResultInDataSet(string connectionString, string query)
        {
            DataSet results = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                            dataAdapter.Fill(results);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return results;
        }

        public object GetSPResultInObject(string storedProcedureName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ZnodeECommerceDB"].ConnectionString;

            object ReturnValue = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(storedProcedureName, connection))
                {
                    cmd.Parameters.AddRange(parameterList.ToArray());
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    try
                    {
                        adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                        cmd.Connection.Open();
                        ReturnValue = cmd.ExecuteScalar();
                        cmd.Connection.Close();

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return ReturnValue;
        }
        #endregion

    }
}
