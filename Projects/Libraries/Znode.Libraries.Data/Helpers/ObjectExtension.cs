using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Znode.Libraries.Data.Helpers
{
    public static class ObjectExtension
    {
        /// <summary>
        ///   Searches for the public property with the specified name.
        /// </summary>
        /// <param id="name">The string containing the name of the public property to get.</param>
        /// <returns> An object representing the public property with the specified name, if found otherwise, null.</returns>
        /// <exception cref="System.Reflection.AmbiguousMatchException">if name already exist. "></exception>
        /// <exception cref="System.ArgumentNullException">name is null.</exception>
        public static object GetProperty(this object obj, string name)
        {
            object propValue = null;
            PropertyInfo propInfo = obj.GetType().GetProperty(name);
            if (!Equals(propInfo,null))
            {
                propValue = propInfo.GetValue(obj, null);
            }
            return propValue;
        }


        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            PropertyInfo propInfo = obj.GetType().GetProperty(propertyName);
            if (!Equals(propInfo, null))
            {
                propInfo.SetValue(obj, value);
            }
        }

        /// <summary>
        ///   Return Log of given object.
        /// </summary>
        /// <returns>Return Log of given object.</returns>
        public static string GetLog(this object target)
        {
            var properties =
            from property in target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            select new
            {
                Name = property.Name,
                Value = property.GetValue(target, null)
            };

            var builder = new StringBuilder();
            builder.Append("   ");
            builder.Append(target.GetType());
            builder.Append(Environment.NewLine);


            foreach (var property in properties)
            {
                if (property.Value != null)
                {
                    builder
                    .Append("   ")
                    .Append(property.Name)
                    .Append(" = ");
                    if (property.Value != null)
                    {
                        builder.Append(property.Value.ToString());
                    }
                    else
                    {
                        builder.Append("null");
                    }

                    builder.AppendLine();
                }
            }
            return builder.ToString();
        }

        //Map Data Table Values to Dynamic Model
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            List<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> rowList = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                var item = CreateItemFromRow<T>(row, properties);
                rowList.Add(item);
            }

            return rowList;
        }

        //Create Item Rows Based on the DataTable.
        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (row.Table.Columns.Contains(property.Name))
                {
                    object value = row[property.Name];
                    if (value == DBNull.Value)
                        value = null;
                    property.SetValue(item, value, null);
                }
            }
            return item;
        }
    }
}
