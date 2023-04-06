namespace Znode.Engine.Api.Models.Responses
{
    public class AccountResponse : BaseResponse
    {
        public AccountModel Account { get; set; }

        public AddressModel AccountAddress { get; set; }
    }
}
