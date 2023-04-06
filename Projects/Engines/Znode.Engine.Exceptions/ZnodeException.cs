using System;
using System.Collections.Generic;
using System.Net;

namespace Znode.Engine.Exceptions
{
    public class ZnodeException : Exception
    {
        public int? ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public Dictionary<string, string> ErrorDetailList { get; private set; }

        /// <summary>
        /// Creates a new ZnodeException.
        /// </summary>
        public ZnodeException()
            : base("Znode Exception")
		{
		}

		/// <summary>
		/// Creates a new ZnodeException.
		/// </summary>
		/// <param name="errorCode">The error code.</param>
		/// <param name="errorMessage">The error message.</param>
        public ZnodeException(int? errorCode, string errorMessage)
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
        /// <param name="errorDetailList">The error details.</param>
        public ZnodeException(int? errorCode, string errorMessage, Dictionary<string, string> errorDetailList)
            : base(errorMessage ?? "ZnodeException with errorCode" + errorCode.GetValueOrDefault().ToString())
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            ErrorDetailList = errorDetailList;
        }

        /// <summary>
        /// Creates a new ZnodeException.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        public ZnodeException(int? errorCode, string errorMessage, HttpStatusCode statusCode)
            : base(errorMessage ?? "ZnodeException with status code " + statusCode.ToString())
		{
			ErrorCode = errorCode;
			ErrorMessage = errorMessage;
			StatusCode = statusCode;
		}

        /// <summary>
        /// Creates a new ZnodeException.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="errorDetailList">The error details.</param>
        public ZnodeException(int? errorCode, string errorMessage, HttpStatusCode statusCode,Dictionary<string,string> errorDetailList)
            : base(errorMessage ?? "ZnodeException with status code " + statusCode.ToString())
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
            ErrorDetailList = errorDetailList;
        }
    }
}
