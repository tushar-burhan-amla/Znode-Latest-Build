using System;
using System.IO;
using System.Net;

namespace Znode.Engine.ABSConnector
{
    public class ABSConnector
    {
        private readonly ABSRequestHelper _xmlHelper;
        private readonly ABSResponseHelper _responseHelper;
        public ABSConnector()
        {
            _xmlHelper = new ABSRequestHelper();
            _responseHelper = new ABSResponseHelper();
        }

        public TResponse GetResponse<TResponse, TRequest>(string destinationUrl, TRequest requestModel, string requestType)
        {
            var typeOfResponse = typeof(TResponse);
            var obj = Activator.CreateInstance<TResponse>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
                byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(_xmlHelper.GetRequestXML(requestModel, requestType));
                request.ContentType = "text/xml; encoding='utf-8'";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                if (!Equals(typeOfResponse.Name, "ABSEmptyResponseModel"))
                {

                    HttpWebResponse response;
                    response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream responseStream = response.GetResponseStream();
                        string responseStr = new StreamReader(responseStream).ReadToEnd();
                        response.Close();
                        return (_responseHelper.GetResponseModel(obj, requestType, responseStr));

                    }
                }
                requestStream.Close();

            }
            catch (Exception)
            {
                throw;
            }
            return obj;
        }
    }
}
