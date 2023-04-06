namespace Znode.Engine.Api.Client
{
    public interface IHelperClient : IBaseClient
    {
        /// <summary>
        /// It will check code field exist or not
        /// </summary>
        /// <param name="codeField">CodeField</param>
        /// <param name="service">Service Name to get reference</param>
        /// <param name="methodName">method name</param>
        /// <returns></returns>
        bool CheckCodeExist(string codeField, string service, string methodName);
    }
}
