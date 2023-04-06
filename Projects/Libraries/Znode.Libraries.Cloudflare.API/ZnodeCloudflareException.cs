using System;
using System.Net;

namespace Znode.Libraries.Cloudflare.API
{
    public class ZnodeCloudflareException : Exception
    {
        public int? ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Creates a new ZnodeException.
        /// </summary>
        public ZnodeCloudflareException()
            : base("Znode Exception")
        {
        }

        /// <summary>
        /// Creates a new ZnodeException.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        public ZnodeCloudflareException(int? errorCode, string errorMessage)
            : base(errorMessage ?? "ZnodeException with errorCode" + errorCode.GetValueOrDefault().ToString())
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Creates a new ZnodeException.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        public ZnodeCloudflareException(int? errorCode, string errorMessage, HttpStatusCode statusCode)
            : base(errorMessage ?? "ZnodeException with status code " + statusCode.ToString())
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
        }
    }
}
