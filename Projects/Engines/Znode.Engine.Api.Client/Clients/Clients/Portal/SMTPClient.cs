using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class SMTPClient : BaseClient, ISMTPClient
    {
        public virtual SMTPModel GetSmtp(int portalId)
        {
            string endpoint = SMTPEndpoint.Get(portalId);
            ApiStatus status = new ApiStatus();
            SMTPResponse response = GetResourceFromEndpoint<SMTPResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Smtp;
        }

        public virtual SMTPModel UpdateSmtp(SMTPModel smtpModel)
        {
            string endpoint = SMTPEndpoint.Update();
            ApiStatus status = new ApiStatus();
            SMTPResponse response = PutResourceToEndpoint<SMTPResponse>(endpoint, JsonConvert.SerializeObject(smtpModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.Smtp;
        }


        public virtual EmailModel SendEmail(EmailModel emailModel)
        {
            string endpoint = SMTPEndpoint.SendEmail();
            ApiStatus status = new ApiStatus();
            EmailResponse response = PostResourceToEndpoint<EmailResponse>(endpoint, JsonConvert.SerializeObject(emailModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.EmailModel;
        }
    }
}
