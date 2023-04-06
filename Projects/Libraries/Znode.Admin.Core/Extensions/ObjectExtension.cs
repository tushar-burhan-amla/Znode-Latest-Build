using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Znode.Engine.Admin.Extensions
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
        public static object GetProperty(this object currentObj, string propertyName) => currentObj.GetType().GetProperty(propertyName)?.GetValue(currentObj, null);

        /// <summary>
        /// set the public property with the specified name.
        /// </summary>
        /// <param name="currentObj">To invoke this extension method.</param>
        /// <param name="propertyName">The string containing the name of the public property to set.</param>
        /// <param name="value">Sets the proper with this value.</param>
        public static void SetPropertyValue(this object currentObj, string propertyName, object value) => currentObj.GetType().GetProperty(propertyName)?.SetValue(currentObj, value);

        /// <summary>
        /// Gets the dynamic property.
        /// </summary>
        /// <param name="currentObject">To invoke this extension method.</param>
        /// <param name="member">The string containing the name of the public property to set.</param>
        /// <returns></returns>
        public static object GetDynamicProperty(this object currentObject, string member)
        {
            try
            {
                // If argument is null then throw an exception
                if (Equals(currentObject, null)) throw new ArgumentNullException("currentObject");
                if (Equals(member, null)) throw new ArgumentNullException("member");

                //get the type of object
                Type scope = currentObject.GetType();
                IDynamicMetaObjectProvider provider = currentObject as IDynamicMetaObjectProvider;
                if (!Equals(provider, null))
                {
                    //gets the parameter expression
                    ParameterExpression param = Expression.Parameter(typeof(object));
                    DynamicMetaObject metaObj = provider.GetMetaObject(param);
                    GetMemberBinder binder = (GetMemberBinder)Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, member, scope, new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(0, null) });
                    //binds the metaObj with the binder
                    DynamicMetaObject dynamicMember = metaObj.BindGetMember(binder);
                    BlockExpression result = Expression.Block(
                        Expression.Label(CallSiteBinder.UpdateLabel),
                        dynamicMember.Expression
                    );
                    LambdaExpression lambda = Expression.Lambda(result, param);
                    //complie the expression
                    Delegate del = lambda.Compile();
                    return del.DynamicInvoke(currentObject);
                }
                else
                    return currentObject.GetType().GetProperty(member, BindingFlags.Public | BindingFlags.Instance).GetValue(currentObject, null);
            }
            catch (Exception)
            {
                return null;
            }
        }

        //get the dynamic property
        public static object GetDynamicProperty(this object currentObject, string member, dynamic row = null)
        {
            try
            {
                return row[member];
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string TryGetElementValue(this XElement parentEl, string elementName, string defaultValue = null, string type = null)
        {
            var foundEl = parentEl.Element(elementName);
            string result = string.Empty;
            if (!Equals(foundEl, null))
            {
                result = foundEl.Value;
                if (string.IsNullOrEmpty(result.Trim()))
                {
                    if (Equals(type, "Char"))
                    {
                        return "n";
                    }
                    else if (Equals(type, "Int"))
                    {
                        return "0";
                    }
                }
                return result;
            }

            if (Equals(type, "Char"))
            {
                return "n";
            }
            else if (Equals(type, "Int"))
            {
                return "0";
            }
            return defaultValue;
        }
    }
}