using Hangfire.Dashboard;

namespace Znode.Engine.Api
{
    public class PassThroughDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
         public bool Authorize(DashboardContext context) => true;
    }
}