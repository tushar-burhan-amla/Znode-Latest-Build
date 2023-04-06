
namespace Znode.Engine.Services
{
    public interface IPaymentTokenService
    {
        /// <summary>
        /// Delete expired token from payment api.
        /// </summary>
        /// <param></param>
        /// <returns>Return bool</returns>
        bool DeletePaymentToken();
    }
}
