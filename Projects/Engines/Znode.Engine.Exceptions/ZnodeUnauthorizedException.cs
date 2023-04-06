using System.Net;

namespace Znode.Engine.Exceptions
{
    public class ZnodeUnauthorizedException : ZnodeException 
    {
		/// <summary>
		/// Creates a new ZnodeUnauthorizedException.
		/// </summary>
		public ZnodeUnauthorizedException()
		{
		}

		/// <summary>
		/// Creates a new ZnodeUnauthorizedException.
		/// </summary>
		/// <param name="errorCode">The error code.</param>
		/// <param name="errorMessage">The error message.</param>
		public ZnodeUnauthorizedException(int? errorCode, string errorMessage) : base(errorCode, errorMessage)
		{
		}

		/// <summary>
		/// Creates a new ZnodeUnauthorizedException.
		/// </summary>
		/// <param name="errorCode">The error code.</param>
		/// <param name="errorMessage">The error message.</param>
		/// <param name="statusCode">The HTTP status code.</param>
		public ZnodeUnauthorizedException(int? errorCode, string errorMessage, HttpStatusCode statusCode) : base(errorCode, errorMessage, statusCode)
		{
		}
    }
}
