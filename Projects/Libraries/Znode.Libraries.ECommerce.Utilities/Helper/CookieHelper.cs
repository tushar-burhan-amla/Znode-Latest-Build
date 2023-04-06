using System;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;


namespace Znode.Libraries.ECommerce.Utilities
{
    public static class CookieHelper
    {

        /// <summary>
        /// To create HtppCookies default global setting
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static HttpCookie CreateHttpCookies(string name, string value = "")
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            HttpCookie httpCookie = new HttpCookie(name, value);

            return httpCookie;
        }

        /// <summary>
        /// Method to get the http cookies
        /// </summary>
        /// <param name="name"></param>
        /// <returns>HttpCookie</returns>
        public static HttpCookie GetCookie(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            //Read cookie from Response. Cookie will be available in Response if server round trip has not happend after adding cookie            
            if (HttpContext.Current.Response.Cookies.AllKeys.Contains(name))
            {
                return HttpContext.Current.Response.Cookies[name];
            }

            // Read cookie from Request. Cookie will be available in Request after server round trip.
            if (HttpContext.Current.Request.Cookies.AllKeys.Contains(name))
            {
                return HttpContext.Current.Request.Cookies[name];
            }
            return null;
        }

        /// <summary>
        /// To set or update HttpCookies
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="cookieExpireInMinutes">Cookie expiration time. If time is not provide then cookie will set as default time. You may use ZnodeConstants for parameter</param>
        /// <param name="isCookieHttpOnly">Set HttpOnly property of cookie. If parameter is null then it will set default setting</param>
        /// <param name="isCookieSecure">Set Secure property of cookie. If parameter is null then it will set default setting</param>
        /// <returns></returns>
        public static void SetCookie(string name, string value, double cookieExpireInMinutes = 0, bool? isCookieHttpOnly = null, bool? isCookieSecure = null)
        {
            HttpCookie cookie = GetCookie(name) ?? CreateHttpCookies(name);
            cookie.Value = value;
            SetCookie(cookie, cookieExpireInMinutes, isCookieHttpOnly, isCookieSecure);
        }

        /// <summary>
        /// To set or update HttpCookies with httponly flag as default true.
        /// This method should be use when httponly cookies needs to be created so that those  
        /// Cookies can be accessible from server application only and not from the client application.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="cookieExpireInMinutes">Cookie expiration time. If time is not provide then cookie will set as default time. You may use ZnodeConstants for parameter</param>
        /// <param name="isCookieSecure">Set Secure property of cookie. If parameter is null then it will set default setting</param>
        /// <returns></returns>
        public static void SetHttpOnlyCookie(string name, string value, double cookieExpireInMinutes = 0, bool? isCookieSecure = null)
        {
            HttpCookie cookie = GetCookie(name) ?? CreateHttpCookies(name);
            cookie.Value = value;
            cookie.HttpOnly = true;
            SetCookie(cookie, cookieExpireInMinutes, true, isCookieSecure);
        }

        /// <summary>
        /// To set or update HttpCookies
        /// </summary>
        /// <param name="cookie">Object of cookie</param>
        /// <param name="cookieExpireInMinutes">Cookie expiration time. If time is not provide then cookie will set as default time</param>
        /// <param name="IsCookieHttpOnly">Set HttpOnly property of cookie. If parameter is null then it will set default setting</param>
        /// <param name="IsCookieSecure">Set Secure property of cookie. If parameter is null then it will set default setting</param>
        /// <returns></returns>
        private static void SetCookie(HttpCookie cookie, double cookieExpireInMinutes = 0, bool? isCookieHttpOnly = null, bool? isCookieSecure = null)
        {
            try
            {
                if (cookieExpireInMinutes != 0)
                    cookie.Expires = DateTime.Now.AddMinutes(cookieExpireInMinutes);

                if (HelperUtility.IsNull(isCookieHttpOnly))
                    cookie.HttpOnly = ZnodeApiSettings.IsCookieHttpOnly;

                if (HelperUtility.IsNull(isCookieSecure))
                    cookie.Secure = ZnodeApiSettings.IsCookieSecure;


                if (!HttpContext.Current.Response.Cookies.AllKeys.Contains(cookie.Name))
                {
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
                else
                {
                    HttpContext.Current.Response.Cookies.Set(cookie);
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, "CookieHelper", TraceLevel.Error);
            }
        }

        /// <summary>
        /// To Get value of cookie
        /// </summary>
        /// <param name="name">Name whoose value to get</param>
        /// <returns>T</returns>
        public static T GetCookieValue<T>(string name)
                => (T)Convert.ChangeType(GetCookie(name)?.Value, typeof(T));

        /// <summary>
        /// To Get values of cookie
        /// </summary>
        /// <param name="name">Name whoose value to get</param>
        /// <returns>NameValueCollection</returns>
        public static NameValueCollection GetCookieValues(string name)
                => GetCookie(name)?.Values;

        /// <summary>
        /// To Check if cookie exists 
        /// </summary>
        /// <param name="name">Name whoose value to get</param>
        /// <returns>bool</returns>
        public static bool IsCookieExists(string name)
                => HelperUtility.IsNull(GetCookie(name)) ? false : true;


        /// <summary>
        /// To remove http cookie
        /// </summary>
        /// <param name="name">Name of cookie which need to remove</param>
        public static void RemoveCookie(string name)
        {
            if (HelperUtility.IsNotNull(GetCookie(name)))
            {
                HttpContext.Current.Response.Cookies[name].Expires = DateTime.Now.AddDays(-1);
            }
        }

        /// <summary>
        /// To remove http cookie
        /// </summary>
        /// <param name="cookie">Cookie object which need to remove</param>
        public static void RemoveCookie(HttpCookie cookie)
        {
            if (GetCookie(cookie.Name) == null) return;

            RemoveCookie(cookie);
        }
    }
}
