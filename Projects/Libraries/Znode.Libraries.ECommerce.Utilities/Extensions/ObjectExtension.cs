using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Znode.Libraries.ECommerce.Utilities
{
    public static class ObjectExtension
    {

        /// <summary>
        /// Return Log of given object.
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
            if (propInfo != null)
            {
                propValue = propInfo.GetValue(obj, null);
            }
            return propValue;
        }

        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            PropertyInfo propInfo = obj.GetType().GetProperty(propertyName);
            if (propInfo != null)
            {
                propInfo.SetValue(obj, value);
            }
        }

        public static object GetDynamicProperty(this object o, string member)
        {
            if (o == null) throw new ArgumentNullException("o");
            if (member == null) throw new ArgumentNullException("member");
            Type scope = o.GetType();
            IDynamicMetaObjectProvider provider = o as IDynamicMetaObjectProvider;
            if (provider != null)
            {
                ParameterExpression param = Expression.Parameter(typeof(object));
                DynamicMetaObject mediaObject = provider.GetMetaObject(param);
                GetMemberBinder binder = (GetMemberBinder)Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, member, scope, new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(0, null) });
                DynamicMetaObject ret = mediaObject.BindGetMember(binder);
                BlockExpression final = Expression.Block(
                    Expression.Label(CallSiteBinder.UpdateLabel),
                    ret.Expression
                );
                LambdaExpression lambda = Expression.Lambda(final, param);
                Delegate del = lambda.Compile();
                return del.DynamicInvoke(o);
            }
            else
            {
                return o.GetType().GetProperty(member, BindingFlags.Public | BindingFlags.Instance).GetValue(o, null);
            }
        }
   
    }
}
