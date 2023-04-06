namespace Znode.Engine.Admin.Agents
{
    public interface IHelperAgent
    {
        /// <summary>
        /// Check Code Field is already exists in DB or not
        /// </summary>
        /// <param name="code">code field</param>
        /// <param name="service">Service name</param>
        /// <param name="methodName">method name</param>
        /// <returns></returns>
        bool IsCodeExists(string code, string service, string methodName);
    }
}
