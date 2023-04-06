using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Znode.Engine.Api.Client;
using Znode.Engine.ERPConnector.Model;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.ERPConnector
{
    // This is the base class for all ERP Configurator Classes.
    public abstract class BaseERP : BaseClient
    {
        static string erpConnectionfilePath = HttpContext.Current.Server.MapPath(ZnodeConstant.ERPConnectionfilePath);
        //Method for get Responce as per access type.
        public virtual T GetResponse<T>(ERPParameterModel erpParameterModel)
        {
            T result = default(T);
            switch (erpParameterModel.AccessType)
            {
                case ZnodeConstant.API:
                    //Method for get API Result
                    result = GetAPIResult<T>(erpParameterModel);
                    break;
                case ZnodeConstant.XML:
                    //Method for get XML Result
                    result = GetXMLResult<T>(erpParameterModel);
                    break;
                case ZnodeConstant.CSV:
                    //Method for get CSV Result
                    result = GetCSVResult<T>(erpParameterModel);
                    break;
            }
            return (T)Convert.ChangeType(result, typeof(T));

        }

        //Method for get API Result
        protected virtual T GetAPIResult<T>(ERPParameterModel erpParameterModel)
        {
            //create request
            var request = (HttpWebRequest)WebRequest.Create(string.Concat(GetLinkURL(), "/", erpParameterModel.ControllerName, "/", erpParameterModel.MethodName));

            // Add Headers to request
            request = GetHeadersForRequest(request, ZnodeConstant.Get);

            var result = ReadResponseInTextReader<T>(GetProcessesResponse(request));
            return result;
        }

        //Method for map Xml to object 
        protected virtual T GetXMLResult<T>(ERPParameterModel erpParameterModel) => ConvertXMLToDataTable<T>(erpParameterModel.FilePath);

        //Method for map CSV to object 
        protected virtual T GetCSVResult<T>(ERPParameterModel erpParameterModel) => ConvertCsvFileToDataTable<T>(erpParameterModel.FilePath, erpParameterModel.IsHeadersAvailable, erpParameterModel.Headers);

        //Method for post Responce.
        protected virtual T PostResourceToERPEndpoint<T>(ERPParameterModel erpParameterModel, ApiStatus status)
        {
            var dataBytes = Encoding.UTF8.GetBytes(erpParameterModel.Data);

            //create request
            var request = (HttpWebRequest)WebRequest.Create(string.Concat(GetLinkURL(), "/", erpParameterModel.ControllerName, "/", erpParameterModel.MethodName));
            request.ContentType = "application/json";
            request.ContentLength = dataBytes.Length;

            // Add Headers to request
            request = GetHeadersForRequest(request, ZnodeConstant.Post);

            using (var reqStream = request.GetRequestStream())
                reqStream.Write(dataBytes, 0, dataBytes.Length);

            var result = GetAPIResultFromResponse<T>(request, status);
            return result;
        }

        protected virtual T GetAPIResultFromResponse<T>(HttpWebRequest request, ApiStatus status)
        {
            T result;
            using (var responce = (HttpWebResponse)request.GetResponse())
            {
                // This deserialization gives back the populated resource
                result = DeserializeResponseStream<T>(responce);
                UpdateApiStatus(result, responce, status);
            }
            return result;
        }

        protected virtual T DeserializeResponseStream<T>(WebResponse response)
        {
            if (response != null)
            {
                using (var body = response.GetResponseStream())
                {
                    if (body != null)
                    {
                        using (var stream = new StreamReader(body))
                        {
                            using (var jsonReader = new JsonTextReader(stream))
                            {
                                var jsonSerializer = new JsonSerializer();
                                return jsonSerializer.Deserialize<T>(jsonReader);
                            }
                        }
                    }
                }
            }
            return default(T);
        }

        protected virtual void UpdateApiStatus<T>(T result, HttpWebResponse response, ApiStatus status)
        {
            if (status == null)
                status = new ApiStatus();

            if (result != null)
            {
                status.HasError = false;
                status.ErrorCode = null;
                status.ErrorMessage = null;
            }

            if (response != null) status.StatusCode = response.StatusCode;

        }

        // Method for Processes Response
        protected static string GetProcessesResponse(HttpWebRequest request)
        {
            string jsonString = string.Empty;
            using (HttpWebResponse responce = (HttpWebResponse)request.GetResponse())
            {
                Stream datastream = responce.GetResponseStream();
                StreamReader reader = new StreamReader(datastream);
                jsonString = reader.ReadToEnd();
                reader.Close();
                datastream.Close();
            }
            return jsonString;
        }

        // Method for Headers For Request
        protected static HttpWebRequest GetHeadersForRequest(HttpWebRequest request, string Method)
        {
            var collection = GetHeaders();

            for (int i = 0; i < collection.Count; i++)
                request.Headers.Add(collection.GetKey(i), Convert.ToString(collection.GetValues(collection.GetKey(i))));

            request.KeepAlive = false;
            request.Method = Method;
            return request;
        }

        // Method for Get Name Value Collection Section
        protected static NameValueCollection GetHeaders()
        {
            NameValueCollection nameValueColl = new NameValueCollection();
            string Json = System.IO.File.ReadAllText(erpConnectionfilePath);
            JObject obj = JObject.Parse(Json);
            foreach (var item in obj[ZnodeConstant.ERPConnectorControlList])
                if (item[ZnodeConstant.IsHeader].ToString() == ZnodeConstant.True)
                    nameValueColl.Add(item[ZnodeConstant.Name].ToString(), item[ZnodeConstant.Value].ToString());

            return nameValueColl;
        }
        // Get API client object with current domain name and key.
        protected virtual T GetClient<T>() where T : class
        {
            var obj = Activator.CreateInstance<T>();
            if (obj is BaseClient)
            {
                int userId = GetLoginUserId();
                if (userId > 0)
                {
                    (obj as BaseClient).UserId = userId;
                    (obj as BaseClient).RefreshCache = true;
                }
            }
            return obj;
        }
        // Method for get Link URL 
        protected static string GetLinkURL()
        {
            string Json = System.IO.File.ReadAllText(erpConnectionfilePath);
            JObject obj = JObject.Parse(Json);
            string erpLinkUrlValue = string.Empty;
            foreach (var item in obj[ZnodeConstant.ERPConnectorControlList])
                if (item[ZnodeConstant.Name].ToString() == ZnodeConstant.ERPLinkUrl)
                    erpLinkUrlValue = item[ZnodeConstant.Value].ToString();

            return erpLinkUrlValue;
        }

        //Get User ID of logged in user.
        protected virtual int GetLoginUserId()
        {
            const string headerUserId = ZnodeConstant.Znode_UserId;
            int userId = 0;
            var headers = HttpContext.Current.Request.Headers;
            int.TryParse(headers[headerUserId], out userId);
            return userId;
        }

        // read Response in TextReader
        protected virtual T ReadResponseInTextReader<T>(string response)
        {
            using (TextReader sr = new StringReader(response))
            {
                using (var jsonReader = new JsonTextReader(sr))
                {
                    var jsonSerializer = new JsonSerializer();
                    return jsonSerializer.Deserialize<T>(jsonReader);
                }
            }
        }

        //Convert csv file into a datatable object
        protected static T ConvertCsvFileToDataTable<T>(string csvFilePath, bool isHeadersAvailable, string headers)
        {
            DataTable dataTable = new DataTable();
            string Fulltext;
            using (StreamReader sr = new StreamReader(csvFilePath))
            {
                while (!sr.EndOfStream)
                {
                    Fulltext = sr.ReadToEnd().ToString(); //read full file text  
                    string[] rows = Fulltext.Split('\n'); //split full file text into rows  
                    if (isHeadersAvailable)
                        ReadRowsWithHeader(dataTable, rows);
                    else
                        ReadRowsWithoutHeader(dataTable, rows, headers);
                }
            }
            return (T)Convert.ChangeType(dataTable, typeof(T));
        }



        //Convert xml file into a datatable object
        protected static T ConvertXMLToDataTable<T>(string filePath)
        {
            XElement x = XElement.Load(filePath);//get your file
            DataTable dataTable = new DataTable();

            XElement setup = (from p in x.Descendants() select p).First();
            // build your DataTable
            foreach (XElement xe in setup.Descendants())
                dataTable.Columns.Add(new DataColumn(xe.Name.ToString(), typeof(string))); // add columns to your dt

            var all = from p in x.Descendants(setup.Name.ToString()) select p;
            foreach (XElement xe in all)
            {
                DataRow dr = dataTable.NewRow();
                foreach (XElement xe2 in xe.Descendants())
                    dr[xe2.Name.ToString()] = xe2.Value; //add in the values
                dataTable.Rows.Add(dr);
            }

            return (T)Convert.ChangeType(dataTable, typeof(T));
        }

        //This method get name of the current method 
        protected static string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            return sf.GetMethod().Name;
        }

        //Method for Read Rows With Header.
        private static void ReadRowsWithHeader(DataTable dataTable, string[] rows)
        {
            for (int i = 0; i < rows.Count(); i++)
            {
                string[] rowValues = rows[i].Split(','); //split each row with comma to get individual values  
                {
                    if (i == 0)
                        for (int j = 0; j < rowValues.Count(); j++)
                            dataTable.Columns.Add(rowValues[j]); //add headers  
                    else
                    {
                        DataRow dr = dataTable.NewRow();
                        for (int k = 0; k < rowValues.Count(); k++)
                            dr[k] = rowValues[k].ToString();
                        dataTable.Rows.Add(dr); //add other rows  
                    }
                }
            }
        }

        //Method for Read Rows Without Header.
        private static void ReadRowsWithoutHeader(DataTable dataTable, string[] rows, string headers)
        {

            string[] headerArray = headers.Split(',');

            for (int j = 0; j < headerArray.Count(); j++)
                dataTable.Columns.Add(headerArray[j]); //add headers  

            for (int i = 0; i < rows.Count(); i++)
            {
                string[] rowValues = rows[i].Split(','); //split each row with comma to get individual values  
                {
                    DataRow dr = dataTable.NewRow();
                    for (int k = 0; k < rowValues.Count(); k++)
                        dr[k] = rowValues[k].ToString();

                    dataTable.Rows.Add(dr); //add other rows  
                }
            }
        }
        //AR History from ERP to ZNODE
        public virtual bool ARHistory() => true;
        //AR Payment Details from ERP to ZNODE
        public virtual bool ARPaymentDetails() => true;
        //AR Balance from ERP to ZNODE
        public virtual bool ARBalance() => true;
        //Refresh Product Content from ERP to Znode
        public virtual bool ProductRefresh() => true;
        //Refresh Category from ERP to Znode
        public virtual bool CategoryRefresh() => true;
        //Refresh Product Category Link from ERP to Znode                            
        public virtual bool ProductCategoryLinkRefresh() => true;
        //Refresh  Contact List from ERP to Znode          
        public virtual bool ContactListRefresh() => true;
        //Get Contact List on real-time from ERP to Znode   
        public virtual bool GetContactList() => true;
        //Refresh Contact Details from ERP to Znode           
        public virtual bool ContactDetailsRefresh() => true;
        //Get Contact Details on real-time from ERP to Znode  
        public virtual bool GetContactDetails() => true;
        //Get Login from ZNODE to ERP
        public virtual bool Login() => true;
        //Create Contact from ZNODE to ERP
        public virtual bool CreateContact() => true;
        //Create Update from ZNODE to ERP
        public virtual bool UpdateContact() => true;

        public virtual bool PaymentAuthorization() => true;
        public virtual bool SaveCreditCard() => true;
        //Refresh customer details from ERP to Znode
        public virtual bool CustomerDetailsRefresh() => true;
        //Get Customer Details on real-time from   ERP to Znode  
        public virtual bool GetCustomerDetails() => true;
        //Refresh ShipToList from ERP to Znode
        public virtual bool ShipToListRefresh() => true;
        //Get ShipToList on real-time from   ERP to Znode  
        public virtual bool GetShipToList() => true;
        //Locate My Account Or Match Customer from Znode to ERP
        public virtual bool LocateMyAccountOrMatchCustomer() => true;
        //Create Customer from Znode to ERP
        public virtual bool CreateCustomer() => true;
        //Update Customer from Znode to ERP
        public virtual bool UpdateCustomer() => true;
        //Create ShipTo from Znode to ERP
        public virtual bool CreateShipTo() => true;
        //Update ShipTo from Znode to ERP
        public virtual bool UpdateShipTo() => true;
        //Refresh inventory for products by warehouse/DC from ERP to Znode on a scheduled basis.
        public virtual bool InventoryRefresh() => true;
        //Get inventory on real-time from ERP to Znode              
        public virtual bool GetInventoryRealtime() => true;
        //Get Invoice History from ERP to Znode       
        public virtual bool InvoiceHistory() => true;
        //Get Invoice Details Status from ERP to Znode       
        public virtual bool InvoiceDetailsStatus() => true;
        public virtual bool RequestACatalog() => true;
        //Submit A Prospect from ERP to Znode       
        public virtual bool SubmitAProspect() => true;
        //OrderSimulate from Znode  to ERP      
        public virtual bool OrderSimulate() => true;
        //Create Order from Znode  to ERP   
        public virtual bool OrderCreate() => true;
        //Get Order History from ERP to Znode       
        public virtual bool OrderHistory() => true;
        //Get Order Details Status from ERP to Znode       
        public virtual bool OrderDetailsStatus() => true;
        //Pay Online from Znode to ERP
        public virtual bool PayOnline() => true;
        //Refresh standard price list from ERP to Znode on a scheduled basis.
        public virtual bool PricingStandardPriceListRefresh() => true;
        //Refresh customer / contract price list from ERP to Znode on a scheduled basis.
        public virtual bool PricingCustomerPriceListRefresh() => true;
        //Get list or customer specific price on real-time from ERP to Znode      
        public virtual bool GetPricing() => true;
        //Create Quote from Znode to ERP   
        public virtual bool QuoteCreate() => true;
        //Get Quote History from ERP to Znode 
        public virtual bool QuoteHistory() => true;
        //Get Quote Details Status from ERP to Znode 
        public virtual bool QuoteDetailsStatus() => true;
        public virtual bool ShippingOptions() => true;
        public virtual bool ShippingNotification() => true;
        //Tax calculation from ERP to Znode 
        public virtual bool TaxCalculation() => true;
    }
}
