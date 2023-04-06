using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Threading.Tasks;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore
{
    public class ApplicationRefreshTokenProvider : IAuthenticationTokenProvider
    {
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            Create(context);
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            Receive(context);
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            object inputs;
            context.OwinContext.Environment.TryGetValue("Microsoft.Owin.Form#collection", out inputs);
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddDays(ZnodeWebstoreSettings.RefreshTokenExpireTimeSpan);
            context.SetToken(context.SerializeTicket());
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);

            if (context.Ticket == null)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                context.Response.ReasonPhrase = "invalid token";
                return;
            }

            if (context.Ticket.Properties.ExpiresUtc <= DateTime.UtcNow)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                context.Response.ReasonPhrase = "unauthorized";
                return;
            }
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddDays(ZnodeWebstoreSettings.RefreshTokenExpireTimeSpan);
            context.SetTicket(context.Ticket);
        }
    }
}
