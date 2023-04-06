using Znode.Multifront.PaymentFramework.Bussiness;
using Znode.Multifront.PaymentApplication.Helpers;

namespace Znode.Multifront.PaymentApplication.Data
{
    public abstract class BaseService : ZnodePaymentBusinessBase
    {
        protected string Encrypt(string data) => EncryptionHelper.Encrypt(data);

        protected string Decrypt(string data) => EncryptionHelper.Decrypt(data);

        protected string EncryptPaymentToken(string data) => EncryptionHelper.EncryptToken(data);

        protected string DecryptPaymentToken(string data) => EncryptionHelper.DecryptToken(data);
    }
}
