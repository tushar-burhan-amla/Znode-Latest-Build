using Znode.Engine.Api.Models;

namespace Znode.Libraries.Hangfire
{
    public interface ISchedulerProviders
    {
        //Invokes the methods under the schedulers
        void InvokeMethod(ERPTaskSchedulerModel model);
    }
}
