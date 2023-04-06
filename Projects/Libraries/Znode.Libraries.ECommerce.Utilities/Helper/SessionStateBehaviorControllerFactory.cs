using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.ECommerce.Utilities
{
    public class SessionStateBehaviorControllerFactory : DefaultControllerFactory
    {
        protected override SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return SessionStateBehavior.Default;
            }

            try
            {
                string actionName = requestContext.RouteData.Values["action"].ToString();

                MethodInfo actionMethodInfo = null;
                MethodInfo[] matchedMethods = controllerType.GetMethods(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public).Where(m => m.Name.ToLower() == actionName.ToLower()).ToArray<MethodInfo>();

                if (matchedMethods.Length > 1)
                {
                    Type typeOfRequest = requestContext.HttpContext.Request.RequestType.ToLower() == "get" ? typeof(HttpGetAttribute) : typeof(HttpPostAttribute);

                    var cntMethods = controllerType.GetMethods()
                         .Where(m =>
                            m.Name == actionName &&
                            ((typeOfRequest == typeof(HttpPostAttribute) &&
                                  m.CustomAttributes.Where(a => a.AttributeType == typeOfRequest).Count() > 0
                               )
                               ||
                               (typeOfRequest == typeof(HttpGetAttribute) &&
                                  m.CustomAttributes.Where(a => a.AttributeType == typeof(HttpPostAttribute)).Count() == 0
                               )
                            )
                        );
                    actionMethodInfo = actionMethodInfo = cntMethods != null && cntMethods.Count() == 1 ? cntMethods.ElementAt(0) : null;
                }
                else
                    actionMethodInfo = controllerType.GetMethod(actionName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (actionMethodInfo != null)
                {
                    ActionSessionStateAttribute actionSessionStateAttr = actionMethodInfo.GetCustomAttributes(typeof(ActionSessionStateAttribute), false)
                                        .OfType<ActionSessionStateAttribute>()
                                        .FirstOrDefault();

                    if (HelperUtility.IsNotNull(actionSessionStateAttr))
                    {
                        return actionSessionStateAttr.Behavior;
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, "SessionState", TraceLevel.Warning);
            }

            return base.GetControllerSessionBehavior(requestContext, controllerType);
        }
    }
}
