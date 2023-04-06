using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;

using Znode.Engine.Api.Models;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Shipping.Usps
{
    public class UspsAgent : ZnodeBusinessBase
    {

        #region Member Variables
        private const string AddressLookupQueryString = "API=Verify&XML=";
        private const string RateLookupQueryString = "API=RateV4&XML=";
        private const string RateIntlLookupQueryString = "API=IntlRateV2&XML=";
        private const string CityStateLookupQueryString = "API=CityStateLookup&XML=";
        private const string ZipCodeLookupQueryString = "API=ZipCodeLookup&XML=";
        private const string USCountryCode = "US";
        private const string FirstClassServiceType = "first class";

        public string ErrorCode { get; set; } = "0";
        public string ErrorDescription { get; set; }
        public string OriginZipCode { get; set; }
        public string PostageDeliveryUnitZip5 { get; set; }
        public string ServiceType { get; set; } = "Priority";

        public AddressModel ShippingAddress { get; set; }
        public decimal ShippingRate { get; set; }
        public string Sku { get; set; }
        public string UspsShippingApiUrl { get; set; }
        public string UspsWebToolsUserId { get; set; }
        public decimal? PackageWeightLimit { get; set; }
        public string WeightInOunces { get; set; } = "0";
        public string WeightInPounds { get; set; } = "0";
        public string Container { get; set; }
        public string Size { get; set; }
        public string Country { get; set; }

        public decimal PackageHeight { get; set; }
        public decimal PackageLength { get; set; }
        public decimal PackageWidth { get; set; }
        public int PublishStateId { get; set; }

        #endregion

        #region Public Method
        public ShippingRateModel CalculateShippingRate()
        {
            ShippingRateModel model = new ShippingRateModel();

            // Build the XML and URL for the API call
            string xml = ServiceType.ToLower().Contains("international") ? BuildRateRequestInternationalXml() : BuildRateRequestXml();
            string queryString = ServiceType.ToLower().Contains("international") ? RateIntlLookupQueryString : RateLookupQueryString;
            string url = BuildRequestUrl(UspsShippingApiUrl, queryString, HttpUtility.UrlEncode(xml));

            try
            {
                DataSet ds = GetResponse(url);

                model = ServiceType.ToLower().Contains("international") ? ProcessInternationalRateResult(ds) : ProcessRateResult(ds);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, "Shipping", TraceLevel.Error);
                // Set error code and description
                ErrorCode = "-1";
                ErrorDescription = ex.Message;
            }
            return model;
        }

        public BooleanModel IsAddressValid(int portalId) => IsAddressValid(ShippingAddress, portalId);

        public BooleanModel IsAddressValid(AddressModel address, int portalId)
        {
            if (!Equals(address?.CountryName, USCountryCode))
                return new BooleanModel { IsSuccess = true, ErrorMessage = string.Empty };

            ErrorCode = "-1";

            GetStateCode(address);

            if (!ValidateAddressProperty(address))
                return new BooleanModel { IsSuccess = false, ErrorMessage = ErrorDescription };

            SetShippingDetails(portalId, address.PublishStateId);
            BooleanModel boolModel = new BooleanModel();
            try
            {
                    boolModel = ValidateAddressLookupRequest(address);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    ZnodeLogging.LogMessage($"Timeout Error occurred for address : " +
                                            $"{address?.Address1}, " +
                                            $"{address?.Address2}, " +
                                            $"{address?.Address3}, " +
                                            $"{address?.CityName}, " +
                                            $"{address?.PostalCode}, " +
                                            $"{address?.StateCode}, " +
                                            $"{address?.CountryCode}", "Shipping", TraceLevel.Error);
                    ZnodeLogging.LogMessage(ex, "Shipping", TraceLevel.Error);
                    boolModel.HasError = false;
                    boolModel.IsSuccess = true;
                }
                else
                {
                    ZnodeLogging.LogMessage(ex, "Shipping", TraceLevel.Error);
                    boolModel.IsSuccess = false;
                    boolModel.ErrorMessage = ex.Message;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, "Shipping", TraceLevel.Error);
                boolModel.IsSuccess = false;

                boolModel.ErrorMessage = Admin_Resources.ErrorInvalidAddress;
            }
            return boolModel;
        }

        //Get valid address from usps and then compares it with the entered address.
        private BooleanModel ValidateAddressLookupRequest(AddressModel address)
        {
            // Build the XML and URL for the API call
            string xml = BuildAddressRequestXml(address);
            string url = BuildRequestUrl(UspsShippingApiUrl, AddressLookupQueryString, HttpUtility.UrlEncode(xml));

            //Returns data set response.
            DataSet dataSet = GetResponse(url);

            return new BooleanModel
            {
                IsSuccess = ProcessZipCodeAddressResult(address, dataSet),
                ErrorMessage = Equals(ErrorDescription, "Address Not Found.  ")
                ? "Please verify your street address is correct before saving." : ErrorDescription
            };
        }

        //Returns data set response.
        protected virtual DataSet GetResponse(string url)
        {
            DataSet dataSet = new DataSet();
            WebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = ZnodeAdminSettings.USPSWebRequestTimeOutMs;
            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    dataSet.ReadXml(stream);
                }
            }
            return dataSet;
        }

        //Validate city state and zip code matches or not.
        private BooleanModel ValidateCityStateLookupRequest(AddressModel address)
        {
            string cityStateLookupXml = BuildCityStateLookupRequestXml(address);
            string cityStateLookupUrl = BuildRequestUrl(UspsShippingApiUrl, CityStateLookupQueryString, HttpUtility.UrlEncode(cityStateLookupXml));

            DataSet dataSet = GetResponse(cityStateLookupUrl);

            return new BooleanModel { IsSuccess = ProcessCityStateAddressResult(address, dataSet), ErrorMessage = ErrorDescription };
        }

        //Verify address result matches with the entered address or not.
        private bool ProcessCityStateAddressResult(AddressModel address, DataSet responseDataSet)
        {
            bool isSuccess = true;

            // Reset the error code and description
            ErrorCode = "0";
            ErrorDescription = string.Empty;

            // Determine if error response was returned
            if (Equals(responseDataSet.Tables["Error"], null))
            {
                if (address?.StateCode?.Trim().ToLower() != responseDataSet?.Tables["ZipCode"]?.Rows[0]["State"]?.ToString().Trim().ToLower())
                {
                    ErrorDescription = "State & Zip Code does not match.";
                    return false;
                }
                if (address?.CityName?.Trim().ToLower() != responseDataSet?.Tables["ZipCode"]?.Rows[0]["City"]?.ToString().Trim().ToLower())
                {
                    ErrorDescription = "City & Zip Code does not match.";
                    return false;
                }
            }
            else
            {
                isSuccess = false;
                SetErrorResponse(responseDataSet);
            }

            return isSuccess;
        }

        //Set error code and error description response.
        private void SetErrorResponse(DataSet responseDataSet)
        {
            ErrorCode = responseDataSet?.Tables["Error"]?.Rows[0]["Number"]?.ToString();
            ErrorDescription = responseDataSet?.Tables["Error"]?.Rows[0]["Description"]?.ToString();
        }

        //Verify zip code address result matches with the entered zip code or not.
        private bool ProcessZipCodeAddressResult(AddressModel address, DataSet responseDataSet)
        {
            bool isSuccess = true;

            // Reset the error code and description
            ErrorCode = "0";
            ErrorDescription = string.Empty;

            int postalCodeLength = address.PostalCode.Length;
            string postalCode = postalCodeLength >= 5 ? address?.PostalCode?.Substring(0, 5) : address.PostalCode;

            DataTable addressTable = responseDataSet.Tables["Address"];

            if (addressTable.Columns.Contains("Address1")
                            || addressTable.Columns.Contains("Address2") || addressTable.Columns.Contains("City") || addressTable.Columns.Contains("State")
                            || addressTable.Columns.Contains("Zip5"))
            {
                if (postalCode?.Trim().ToLower() != responseDataSet?.Tables["Address"]?.Rows[0]["Zip5"]?.ToString().Trim().ToLower() && address?.PostalCode?.Substring(5)?.Trim().ToLower() != string.Concat("-", (!string.IsNullOrEmpty(Convert.ToString(responseDataSet?.Tables["Address"]?.Rows[0]["Zip4"])) ? responseDataSet?.Tables["Address"]?.Rows[0]["Zip4"]?.ToString().Trim().ToLower() : string.Empty)))
                {
                    ErrorDescription = "Please verify zip code.";
                    return false;
                }
                if (address?.StateCode?.Trim().ToLower() != responseDataSet?.Tables["Address"]?.Rows[0]["State"]?.ToString().Trim().ToLower())
                {
                    ErrorDescription = "Please verify state.";
                    return false;
                }
                if (address?.CityName?.Trim().ToLower() != responseDataSet?.Tables["Address"]?.Rows[0]["City"]?.ToString().Trim().ToLower())
                {
                    ErrorDescription = "Please verify city.";
                    return false;
                }
            }
            else
            {
                isSuccess = false;
                SetErrorResponse(responseDataSet);
            }
            return isSuccess;
        }

        public AddressListModel RecommendedAddress(AddressModel address, int portalId)
        {
            if (!Equals(address.CountryName, USCountryCode))
                return new AddressListModel();

            GetStateCode(address);

            if (!ValidateAddressProperty(address))
                new AddressListModel();

            SetShippingDetails(portalId, address.PublishStateId);
            string zipCodeXML = BuildZipCodeLookupRequestXml(address);
            string zipCodeLookupURL = BuildRequestUrl(UspsShippingApiUrl, ZipCodeLookupQueryString, HttpUtility.UrlEncode(zipCodeXML));

            AddressListModel addressList = new AddressListModel();
            try
            {
                addressList = GetAddressList(zipCodeLookupURL, addressList, address);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, "Shipping", TraceLevel.Error);
                ErrorCode = "-1";
                ErrorDescription = ex.Message;
            }

            return addressList ?? new AddressListModel();
        }

        protected virtual AddressListModel GetAddressList(string url, AddressListModel addressList, AddressModel model)
        {
            WebRequest zipCodeRequest = (HttpWebRequest)WebRequest.Create(url);
            zipCodeRequest.Timeout = ZnodeAdminSettings.USPSWebRequestTimeOutMs;
            using (WebResponse response = zipCodeRequest.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    DataSet ds = new DataSet();
                    ds.ReadXml(stream);
                    ProcessAddressResult(ds);
                    DataTable addressTable = ds?.Tables["Address"];
                    if (addressTable?.Rows.Count > 0)
                    {
                        addressList.AddressList = new List<AddressModel>();
                        if (addressTable.Columns.Contains("Address1")
                            || addressTable.Columns.Contains("Address2") || addressTable.Columns.Contains("City") || addressTable.Columns.Contains("State")
                            || addressTable.Columns.Contains("Zip5"))
                        {
                            foreach (DataRow row in addressTable.Rows)
                            {
                                bool isUspsAddress1Contains = ValidateAddress1Field(row);
                                AddressModel addressModel = new AddressModel
                                {
                                    //At Usps level Address1 field is optional and Address2 field is mandatory. While in Znode Address1 is mandatory and Address2 field is optional.
                                    //Whenever Usps response Address1 is null and Address2 field is provided then in Znode the Address2 field is set in Address1 field. 
                                    Address1 = BindAddressFieldsFromUsps(row, isUspsAddress1Contains, false),
                                    Address2 = BindAddressFieldsFromUsps(row, isUspsAddress1Contains, true),
                                    CityName = row.Table.Columns.Contains("City") ? Convert.ToString(row["City"])?.Replace("\r\n", string.Empty).Trim() : string.Empty,
                                    StateName = row.Table.Columns.Contains("State") ? Convert.ToString(row["State"])?.Replace("\r\n", string.Empty).Trim() : string.Empty,
                                    PostalCode = string.Concat((row.Table.Columns.Contains("Zip5") ? Convert.ToString(row["Zip5"])?.Replace("\r\n", string.Empty).Trim() : string.Empty), (!string.IsNullOrEmpty(Convert.ToString(row["Zip4"])) ? "-" : string.Empty), (row.Table.Columns.Contains("Zip4") ? Convert.ToString(row["Zip4"])?.Replace("\r\n", string.Empty).Trim() : string.Empty)),
                                    CountryName = model.CountryName,
                                    FirstName = model.FirstName,
                                    LastName = model.LastName,
                                    PhoneNumber = model.PhoneNumber,
                                    EmailAddress = model.EmailAddress,
                                    AspNetUserId = model.AspNetUserId,
                                    UserId = model.UserId,
                                    PortalId = model.PortalId
                                };
                                addressList.AddressList.Add(addressModel);
                            }
                        }
                    }
                }
            }

            return addressList ?? new AddressListModel();
        }

        public bool ValidateAddressProperty(AddressModel address)
        {
            if (address?.Address1?.Length > 100)
            {
                ErrorDescription = "Address1 is limited to a maximum of 100 characters.";
                return false;
            }

            if (address?.Address2?.Length > 100)
            {
                ErrorDescription = "Address2 is limited to a maximum of 100 characters.";
                return false;
            }

            if (address?.CityName?.Length > 100)
            {
                ErrorDescription = "City is limited to a maximum of 100 characters.";
                return false;
            }

            if (address?.StateCode?.Length > 3)
            {
                ErrorDescription = "State code is limited to a maximum of 3 characters.";
                return false;
            }

            if (address?.PostalCode?.Length > 10)
            {
                ErrorDescription = "Postal code is limited to a maximum of 10 characters.";
                return false;
            }

            return true;
        }

        // This function split the PackageWeight in elements according to specified packageWeight limit.        
        public List<decimal> SplitPackageWeight(decimal packageWeight, decimal packageWeightLimitLbs)
        {
            try
            {
                decimal actualWeight = packageWeight;
                int count = 0;
                List<decimal> weightList = new List<decimal>();

                while (actualWeight > packageWeightLimitLbs)
                {
                    decimal extraWeight = 0;
                    extraWeight = actualWeight - packageWeightLimitLbs;
                    actualWeight = actualWeight - extraWeight;
                    weightList.Add(actualWeight);
                    actualWeight = extraWeight;
                    count++;
                }
                if (actualWeight > 0)
                    weightList.Add(actualWeight);

                return weightList;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, "Shipping", TraceLevel.Error);
                throw;
            }
        }

        #endregion

        #region Private Method
        //to set shipping details by portalId
        private void SetShippingDetails(int portalId, int publishStateId = 0)
        {
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
            PortalShippingModel portalShippingModel = shippingHelper.GetPortalShipping(portalId, publishStateId);
            if (HelperUtility.IsNotNull(portalShippingModel))
            {
                UspsShippingApiUrl = portalShippingModel.USPSShippingAPIURL;
                UspsWebToolsUserId = portalShippingModel.USPSWebToolsUserID;
            }
        }

        //to get state code and assign to address model 
        private void GetStateCode(AddressModel address)
        {
            IZnodeShippingHelper helper = GetService<IZnodeShippingHelper>();
            string stateCode = !string.IsNullOrEmpty(address.StateCode) ? address.StateCode : helper.GetStateCode(address?.StateName);
            address.StateCode = stateCode;
        }

        private string BuildRequestUrl(string apiUrl, string apiQueryString, string xmlValue) => $"{apiUrl}?{apiQueryString}{xmlValue}";

        private string BuildRateRequestXml()
        {
            string xml = String.Empty;
            string weightForFirstClassService;
            //For the first class service type weight in pound should be zero 
            if (ServiceType?.ToLower() == FirstClassServiceType)
            {
                weightForFirstClassService = "0";
            }
            else
            {
                weightForFirstClassService = WeightInPounds;
            }

            XmlDocument document = new XmlDocument();
            XmlElement requestNode = document.CreateElement("RateV4Request");
            requestNode.SetAttribute("USERID", UspsWebToolsUserId);

            XmlElement packageNode = document.CreateElement("Package");
            packageNode.SetAttribute("ID", Sku.Replace(" ", "_"));

            XmlElement serviceNode = document.CreateElement("Service");
            serviceNode.InnerText = ServiceType;

            XmlElement subServiceNode = document.CreateElement("FirstClassMailType");
            subServiceNode.InnerText = "PARCEL";

            XmlElement zipOriginationNode = document.CreateElement("ZipOrigination");
            zipOriginationNode.InnerText = OriginZipCode;

            XmlElement zipDestinationNode = document.CreateElement("ZipDestination");
            zipDestinationNode.InnerText = PostageDeliveryUnitZip5;

            XmlElement poundsNode = document.CreateElement("Pounds");
            poundsNode.InnerText = (Decimal.Parse(weightForFirstClassService)).ToString();

            XmlElement ouncesNode = document.CreateElement("Ounces");
            ouncesNode.InnerText = (ConvertPoundsToOunces(Decimal.Parse(WeightInPounds), Decimal.Parse(WeightInOunces))).ToString();

            XmlElement sizeNode = document.CreateElement("Size");
            sizeNode.InnerText = Size;

            XmlElement containerNode = document.CreateElement("Container");
            containerNode.InnerText = Container;

            // Append nodes
            packageNode.AppendChild(serviceNode);
            if (ServiceType?.ToLower() == FirstClassServiceType)
                packageNode.AppendChild(subServiceNode);
            packageNode.AppendChild(zipOriginationNode);
            packageNode.AppendChild(zipDestinationNode);
            packageNode.AppendChild(poundsNode);
            packageNode.AppendChild(ouncesNode);
            packageNode.AppendChild(containerNode);
            packageNode.AppendChild(sizeNode);
            requestNode.AppendChild(packageNode);


            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter xw = new XmlTextWriter(sw))
                {
                    requestNode.WriteTo(xw);
                    xml = sw.ToString();
                }
            }

            return xml;
        }

        private string BuildRateRequestInternationalXml()
        {
            string xml = String.Empty;
            XmlDocument document = new XmlDocument();
            XmlElement requestNode = document.CreateElement("IntlRateV2Request");
            requestNode.SetAttribute("USERID", UspsWebToolsUserId);

            XmlElement Revision = document.CreateElement("Revision");
            Revision.InnerText = "2";

            XmlElement packageNode = document.CreateElement("Package");
            packageNode.SetAttribute("ID", Sku.Replace(" ", "_"));

            XmlElement poundsNode = document.CreateElement("Pounds");
            poundsNode.InnerText = (Decimal.Parse(WeightInPounds)).ToString();

            XmlElement ouncesNode = document.CreateElement("Ounces");
            ouncesNode.InnerText = (Decimal.Parse(WeightInOunces)).ToString();

            XmlElement machinableNode = document.CreateElement("Machinable");
            machinableNode.InnerText = "True";

            XmlElement mailTypeNode = document.CreateElement("MailType");
            mailTypeNode.InnerText = "package";

            XmlElement ValueOfContents = document.CreateElement("ValueOfContents");
            ValueOfContents.InnerText = "2499";

            XmlElement countryNode = document.CreateElement("Country");
            countryNode.InnerText = Country;

            XmlElement containerNode = document.CreateElement("Container");
            containerNode.InnerText = Container;

            XmlElement sizeNode = document.CreateElement("Size");
            sizeNode.InnerText = Size;

            XmlElement widthNode = document.CreateElement("Width");
            widthNode.InnerText = Convert.ToString(PackageWidth);

            XmlElement lengthNode = document.CreateElement("Length");
            lengthNode.InnerText = Convert.ToString(PackageLength);

            XmlElement heightNode = document.CreateElement("Height");
            heightNode.InnerText = Convert.ToString(PackageHeight);

            XmlElement girthNode = document.CreateElement("Girth");
            girthNode.InnerText = "0";

            XmlElement zipOriginationNode = document.CreateElement("OriginZip");
            zipOriginationNode.InnerText = OriginZipCode;

            XmlElement commercialFlagNode = document.CreateElement("CommercialFlag");
            commercialFlagNode.InnerText = "Y";

            XmlElement extraServices = document.CreateElement("ExtraServices");

            XmlElement extraService = document.CreateElement("ExtraService");
            extraService.InnerText = "1";

            // Append nodes
            requestNode.AppendChild(Revision);
            packageNode.AppendChild(poundsNode);
            packageNode.AppendChild(ouncesNode);
            packageNode.AppendChild(machinableNode);
            packageNode.AppendChild(mailTypeNode);
            packageNode.AppendChild(ValueOfContents);
            packageNode.AppendChild(countryNode);
            packageNode.AppendChild(containerNode);
            packageNode.AppendChild(sizeNode);
            packageNode.AppendChild(widthNode);
            packageNode.AppendChild(lengthNode);
            packageNode.AppendChild(heightNode);
            packageNode.AppendChild(girthNode);
            packageNode.AppendChild(zipOriginationNode);
            packageNode.AppendChild(commercialFlagNode);
            packageNode.AppendChild(extraServices);
            extraServices.AppendChild(extraService);
            requestNode.AppendChild(packageNode);

            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter xw = new XmlTextWriter(sw))
                {
                    requestNode.WriteTo(xw);
                    xml = sw.ToString();
                }
            }
            return xml;
        }

        private string BuildAddressRequestXml(AddressModel address)
        {
            var xml = String.Empty;
            XmlDocument document = new XmlDocument();
            XmlElement requestNode = document.CreateElement("AddressValidateRequest");
            requestNode.SetAttribute("USERID", UspsWebToolsUserId);

            XmlElement addressNode = document.CreateElement("Address");
            addressNode.SetAttribute("ID", address.AddressId.ToString());

            // Swap the address line
            XmlElement addressLine1Node = document.CreateElement("Address1");
            addressLine1Node.InnerText = address.Address1;

            XmlElement addressLine2Node = document.CreateElement("Address2");
            addressLine2Node.InnerText = address.Address2;

            XmlElement cityNode = document.CreateElement("City");
            cityNode.InnerText = address.CityName;

            XmlElement stateNode = document.CreateElement("State");
            stateNode.InnerText = address.StateCode;

            XmlElement zip5Node = document.CreateElement("Zip5");
            zip5Node.InnerText = address.PostalCode;

            XmlElement zip4Node = document.CreateElement("Zip4");

            zip4Node.InnerText = address.PostalCode.Length > 4 ? address.PostalCode.Substring(0, 4) : address.PostalCode;

            // Append nodes 
            // Adding firm name node gives different zip than recommended zip
            addressNode.AppendChild(addressLine1Node);
            addressNode.AppendChild(addressLine2Node);
            addressNode.AppendChild(cityNode);
            addressNode.AppendChild(stateNode);
            addressNode.AppendChild(zip5Node);
            addressNode.AppendChild(zip4Node);
            requestNode.AppendChild(addressNode);

            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter xw = new XmlTextWriter(sw))
                {
                    requestNode.WriteTo(xw);
                    xml = sw.ToString();
                }
            }

            return xml;
        }

        private string BuildCityStateLookupRequestXml(AddressModel address)
        {
            var xml = String.Empty;
            XmlDocument document = new XmlDocument();
            XmlElement requestNode = document.CreateElement("CityStateLookupRequest");
            requestNode.SetAttribute("USERID", UspsWebToolsUserId);

            XmlElement zipCode = document.CreateElement("ZipCode");
            zipCode.SetAttribute("ID", "0");

            int postalCodeLength = address.PostalCode.Length;
            string postalCode = postalCodeLength >= 5 ? address.PostalCode.Substring(0, 5) : address.PostalCode;

            XmlElement zip5Node = document.CreateElement("Zip5");
            zip5Node.InnerText = postalCode;

            XmlElement cityNode = document.CreateElement("City");
            cityNode.InnerText = string.Empty;

            XmlElement stateNode = document.CreateElement("State");
            stateNode.InnerText = string.Empty;

            // Append nodes
            zipCode.AppendChild(zip5Node);
            zipCode.AppendChild(cityNode);
            zipCode.AppendChild(stateNode);
            requestNode.AppendChild(zipCode);

            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter xw = new XmlTextWriter(sw))
                {
                    requestNode.WriteTo(xw);
                    xml = sw.ToString();
                }
            }

            return xml;
        }

        private string BuildZipCodeLookupRequestXml(AddressModel address)
        {
            var xml = String.Empty;
            XmlDocument document = new XmlDocument();
            XmlElement requestNode = document.CreateElement("ZipCodeLookupRequest");
            requestNode.SetAttribute("USERID", UspsWebToolsUserId);

            XmlElement addressNode = document.CreateElement("Address");
            addressNode.SetAttribute("ID", address.AddressId.ToString());

            // Swap the address line
            XmlElement addressLine1Node = document.CreateElement("Address1");
            addressLine1Node.InnerText = address.Address1;

            XmlElement addressLine2Node = document.CreateElement("Address2");
            addressLine2Node.InnerText = address.Address2;

            XmlElement cityNode = document.CreateElement("City");
            cityNode.InnerText = address.CityName;

            XmlElement stateNode = document.CreateElement("State");
            stateNode.InnerText = address.StateCode;

            XmlElement zipCode = document.CreateElement("ZipCode");
            zipCode.InnerText = address.PostalCode;

            XmlElement zip5Node = document.CreateElement("Zip5");
            zip5Node.InnerText = address.PostalCode;

            XmlElement zip4Node = document.CreateElement("Zip4");

            zip4Node.InnerText = address.PostalCode.Length > 4 ? address.PostalCode.Substring(0, 4) : address.PostalCode;

            // Append nodes
            addressNode.AppendChild(addressLine1Node);
            addressNode.AppendChild(addressLine2Node);
            addressNode.AppendChild(cityNode);
            addressNode.AppendChild(stateNode);
            addressNode.AppendChild(zip5Node);
            addressNode.AppendChild(zip4Node);
            requestNode.AppendChild(addressNode);
            requestNode.AppendChild(zipCode);

            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter xw = new XmlTextWriter(sw))
                {
                    requestNode.WriteTo(xw);
                    xml = sw.ToString();
                }
            }

            return xml;
        }

        private ShippingRateModel ProcessRateResult(DataSet responseDataSet)
        {
            ShippingRateModel model = new ShippingRateModel();

            // Reset the error code and description
            ErrorCode = "0";
            ErrorDescription = String.Empty;

            // Determine if error response was returned
            if (Equals(responseDataSet.Tables["Error"], null) && !Equals(responseDataSet.Tables["Package"], null))
            {
                // Process response
                DataTable tempTable = responseDataSet.Tables["Package"];
                DataRow tempPackage = tempTable.Rows[0];

                if (!Equals(tempPackage, null))
                {
                    DataRow[] rows = tempPackage.GetChildRows(responseDataSet.Relations["Package_Postage"]);
                    DataRow postageRow = rows[0];
                    ShippingRate = Decimal.Parse(postageRow["Rate"].ToString());
                }
            }
            else
                SetErrorResponse(responseDataSet);

            model.ShippingRate = ShippingRate;
            return model;
        }

        private ShippingRateModel ProcessInternationalRateResult(DataSet responseDataSet)
        {
            ShippingRateModel model = new ShippingRateModel();

            // Reset the error code and description
            ErrorCode = "0";
            ErrorDescription = String.Empty;

            // Determine if error response was returned
            if (Equals(responseDataSet.Tables["Error"], null) && !Equals(responseDataSet.Tables["Package"], null))
            {
                // Process response
                DataTable serviceTable = responseDataSet.Tables["Service"];

                foreach (DataRow tempPackage in serviceTable.Rows)
                {
                    if (!Equals(tempPackage, null) && Convert.ToString(tempPackage["SvcDescription"]).ToLower().Contains(ServiceType.ToLower()))
                    {
                        ShippingRate = Convert.ToDecimal(tempPackage["CommercialPostage"]);
                        model.ShippingRate = ShippingRate;
                        return model;
                    }
                }
            }
            else
                SetErrorResponse(responseDataSet);

            return model;
        }

        private bool ProcessAddressResult(DataSet responseDataSet)
        {
            bool isSuccess = true;

            // Reset the error code and description
            ErrorCode = "0";
            ErrorDescription = string.Empty;

            // Determine if error response was returned
            if (Equals(responseDataSet.Tables["Error"], null))
            {
                DataTable tempTable = responseDataSet.Tables["Address"];
            }
            else
            {
                isSuccess = false;
                SetErrorResponse(responseDataSet);
            }

            return isSuccess;
        }

        //For first class service it can not take weight in pound so we are converting it into ounces.
        private decimal ConvertPoundsToOunces(decimal pound, decimal ounce)
            => (ServiceType?.ToLower() == FirstClassServiceType && pound > 0) ? (pound * 16) : ounce;

        //If the Address1 field of the Usps is null or empty,
        //then binding the Address2 field in Address1 field.
        private string BindAddressFieldsFromUsps(DataRow row, bool isUspsAddress1Exist, bool isAddress2Required)
        {
           if (isUspsAddress1Exist && !isAddress2Required)
           {
                return Convert.ToString(row["Address1"])?.Replace("\r\n", string.Empty).Trim();
           }

            //For requiring Usps Address2 field in Znode Address1 field.
            //Will toggle only in case when Address1 is null.  
            if (!isUspsAddress1Exist)
            {
                isAddress2Required = isAddress2Required? false : true;
                isUspsAddress1Exist = isUspsAddress1Exist ? false : true;
            }


           return ((isUspsAddress1Exist && isAddress2Required) && (row.Table.Columns.Contains("Address2")))  ?
                            Convert.ToString(row["Address2"])?.Replace("\r\n", string.Empty).Trim() :
                            string.Empty;
        }

        private bool ValidateAddress1Field(DataRow row)
        {
            if (row.Table.Columns.Contains("Address1") && HelperUtility.IsNotNull(row["Address1"]))
                return true;

            return false;
        }
       #endregion
    }
}
