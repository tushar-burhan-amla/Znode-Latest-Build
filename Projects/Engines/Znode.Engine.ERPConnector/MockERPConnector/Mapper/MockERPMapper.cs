using System;
using System.Collections.Generic;
using System.Data;
using Znode.Engine.Api.Models;

namespace Znode.Engine.ERPConnector
{
    public static class MockERPMapper
    {

        /// <summary>
        /// Map datatable to Address Model
        /// </summary>
        /// <param name="dataTable">DataTable</param>
        /// <returns>return AddressModel</returns>
        public static AddressModel ToAddressModel(DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0)
                return null;

            AddressModel model = new AddressModel();
            foreach (DataRow row in dataTable.Rows)
            {
                model.AddressId = Convert.ToInt32(row["AddressId"]);
                model.AccountId = Convert.ToInt32(row["AccountId"].ToString());
                model.Address1 = Convert.ToString(row["Address1"].ToString());
                model.Address2 = Convert.ToString(row["Address2"].ToString());
                model.Address3 = Convert.ToString(row["Address3"].ToString());
                model.DisplayName = Convert.ToString(row["DisplayName"].ToString());
                model.FirstName = Convert.ToString(row["FirstName"].ToString());
            }
            return model;
        }

        /// <summary>
        /// This method will convert datatable to Address List Model
        /// </summary>
        /// <param name="dataTable">datatable dataTable</param>
        /// <returns>Returns the Address List Model</returns>
        public static List<AddressModel> ToAddressModelList(DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0)
                return null;

            List<AddressModel> modelList = new List<AddressModel>();

            foreach (DataRow row in dataTable.Rows)
            {
                AddressModel model = new AddressModel();
                model.AddressId = Convert.ToInt32(row["AddressId"]);
                model.AccountId = Convert.ToInt32(row["AccountId"].ToString());
                model.Address1 = Convert.ToString(row["Address1"].ToString());
                model.Address2 = Convert.ToString(row["Address2"].ToString());
                model.Address3 = Convert.ToString(row["Address3"].ToString());
                model.DisplayName = Convert.ToString(row["DisplayName"].ToString());
                model.FirstName = Convert.ToString(row["FirstName"].ToString());
                modelList.Add(model);
            }
            return modelList;
        }

        /// <summary>
        /// Map datatable to User Model
        /// </summary>
        /// <param name="dataTable">DataTable</param>
        /// <returns>return UserModel</returns>
        public static UserModel ToUserModel(DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0)
                return null;

            UserModel model = new UserModel();
            foreach (DataRow row in dataTable.Rows)
            {
                model.UserId = Convert.ToInt32(row["UserId"]);
                model.Email = Convert.ToString(row["Email"].ToString());
                model.UserName = Convert.ToString(row["UserName"].ToString());
                model.FullName = Convert.ToString(row["FullName"].ToString());
                model.PhoneNumber = Convert.ToString(row["PhoneNumber"].ToString());
                model.FirstName = Convert.ToString(row["FirstName"].ToString());
                model.LastName = Convert.ToString(row["LastName"].ToString());
                model.ProfileId = Convert.ToInt32(row["ProfileId"].ToString());
            }
            return model;
        }

        /// <summary>
        /// This method will convert datatable to User List Model
        /// </summary>
        /// <param name="dataTable">datatable dataTable</param>
        /// <returns>Returns the User List Model</returns>
        public static List<UserModel> ToUserModelList(DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0)
                return null;

            List<UserModel> modelList = new List<UserModel>();

            foreach (DataRow row in dataTable.Rows)
            {
                UserModel model = new UserModel();
                model.UserId = Convert.ToInt32(row["UserId"]);
                model.Email = Convert.ToString(row["Email"].ToString());
                model.UserName = Convert.ToString(row["UserName"].ToString());
                model.FullName = Convert.ToString(row["FullName"].ToString());
                model.PhoneNumber = Convert.ToString(row["PhoneNumber"].ToString());
                model.FirstName = Convert.ToString(row["FirstName"].ToString());
                model.LastName = Convert.ToString(row["LastName"].ToString());
                model.ProfileId = Convert.ToInt32(row["ProfileId"].ToString());
                modelList.Add(model);
            }
            return modelList;
        }
    }
}

