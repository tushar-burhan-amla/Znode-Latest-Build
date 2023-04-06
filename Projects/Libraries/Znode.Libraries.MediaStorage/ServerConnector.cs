using System;
using System.IO;
using System.Reflection;

namespace Znode.Libraries.MediaStorage
{
    public class ServerConnector
    {
        public FileUploadPolicyModel UploadPolicyModel { get; private set; }

        public ServerConnector(FileUploadPolicyModel policymodel)
        {
            UploadPolicyModel = policymodel;
        }

        /// <summary>
        /// This method help to create runtime object of class using Reflection and invoke constructor of that class.
        /// Internally this method invoke the specified action.(Upload,Delete, etc.)
        /// </summary>
        /// <param name="serverAgentClassName">Agent ClassName</param>
        /// <param name="actionName">Name of Action (Upload,Delete)</param>
        /// <param name="stream">File stream</param>
        /// <param name="fileName">file name</param>
        /// <param name="folderName">folder name in which media is uploaded</param>
        /// <returns></returns>
        public object CallConnector(string serverAgentClassName, string actionName, MemoryStream stream, string fileName, string folderName)
        {
            //Retrive type of AgentClass by name 
            var _connector = Type.GetType("Znode.Libraries.MediaStorage." + serverAgentClassName + ", Znode.Libraries.MediaStorage");

            //get constructor of class
            ConstructorInfo ctor = _connector.GetConstructor(new[] { typeof(FileUploadPolicyModel) });

            //Call constructor of class
            object instance = ctor.Invoke(new object[] { UploadPolicyModel });

            //Get methods info of specified action name
            MethodInfo _fun = Equals(stream, null) ? _connector.GetMethod(actionName, new[] { typeof(string), typeof(string) }) : _connector.GetMethod(actionName, new[] { typeof(MemoryStream), typeof(string), typeof(string) });

            //Invoke methods to action
            return Equals(stream, null) ? _fun.Invoke(instance, new object[] { fileName, folderName }) : _fun.Invoke(instance, new object[] { stream, fileName, folderName });
        }

        /// <summary>
        /// This method help to create runtime object of class using Reflection and invoke constructor of that class.
        /// Internally this method invoke the specified action.(Upload,Delete, etc.)
        /// </summary>
        /// <param name="serverAgentClassName">Agent ClassName</param>
        /// <param name="actionName">Name of Action (Upload,Delete)</param>
        /// <param name="fileName">file name</param>
        /// <param name="folderName">folder name in which media is uploaded</param>
        /// <returns></returns>
        public object CallConnector(string serverAgentClassName, string actionName, string fileName, string folderName)
        {
            return CallConnector(serverAgentClassName, actionName, null, fileName, folderName);
        }
    }
}
