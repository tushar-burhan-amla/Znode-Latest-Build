namespace Znode.Engine.Api.Models.Responses
{
    public class AccountQuoteResponse : BaseResponse
    {
        public AccountQuoteModel AccountQuote { get; set; }
        public QuoteStatusModel QuoteStatus { get; set; }
        public AccountTemplateModel AccountTemplate { get; set; }
    }
}
