using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Services.Protocols;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Engine.Shipping.FedEx.FedExEnum;
using RateAvailableServiceWebServiceClient.RateServiceWebReference;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Shipping.FedEx
{
    public class FedExAgent : ZnodeBusinessBase
    {
        #region Constants
        private const string FedExConstantValue = "1";
        #endregion

        private string _errorDescription;

        public string ClientProductId { get; set; }
        public string ClientProductVersion { get; set; }
        public string CspAccessKey { get; set; }
        public string CspPassword { get; set; }
        public string CurrencyCode { get; set; } = "USD";
        public string DimensionUnit { get; set; } = "IN";
        public string DropOffType { get; set; } = "REGULAR_PICKUP";
        public string ErrorCode { get; set; } = "0";
        public string FedExAccessKey { get; set; }
        public string FedExAccountNumber { get; set; }
        public string FedExGatewayUrl { get; set; } = ZnodeApiSettings.FedExGatewayURL;
        public string FedExMeterNumber { get; set; }
        public string FedExSecurityCode { get; set; }
        public string FedExServiceType { get; set; }
        public byte[] LabelImage { get; set; }
        public string OriginLocationId { get; set; }
        public string PackageHeight { get; set; } = FedExConstantValue;
        public string PackageLength { get; set; } = FedExConstantValue;
        public string PackageTypeCode { get; set; } = "YOUR_PACKAGING";
        public decimal PackageWeight { get; set; } = 0;
        public string PackageWidth { get; set; } = FedExConstantValue;
        public decimal ShipmentCharge { get; set; }
        public string ShipperAddress1 { get; set; }
        public string ShipperAddress2 { get; set; }
        public bool ShipperAddressIsResidential { get; set; } = false;
        public string ShipperCity { get; set; }
        public string ShipperCompany { get; set; }
        public string ShipperCountryCode { get; set; }
        public string ShipperPhone { get; set; }
        public string ShipperStateCode { get; set; }
        public string ShipperZipCode { get; set; }
        public string ShipToAddress1 { get; set; }
        public string ShipToAddress2 { get; set; }
        public bool ShipToAddressIsResidential { get; set; } = false;
        public string ShipToCity { get; set; }
        public string ShipToCompany { get; set; }
        public string ShipToCountryCode { get; set; }
        public string ShipToFirstName { get; set; }
        public string ShipToLastName { get; set; }
        public string ShipToPhone { get; set; }
        public string ShipToStateCode { get; set; }
        public string ShipToZipCode { get; set; }
        public string TrackingNumber { get; set; }
        public decimal TotalCustomsValue { get; set; } = 0;
        public decimal TotalInsuredValue { get; set; } = 0;
        public bool UseDiscountRate { get; set; } = false;
        public string VendorProductPlatform { get; set; }
        public string WeightUnit { get; set; } = "LB";
        public string FedExLTLAccountNumber { get; set; }
        public decimal PackageWeightLimit { get; set; }
        public string PackageGroupCount { get; set; }
        public string PackageCount { get; set; }
        public string TotalHandlingUnits { get; set; }
        public string ItemPackageTypeCode { get; set; }
        public Dictionary<string, string> RateTimeInTransit { get; set; } = new Dictionary<string, string>();
        public string ErrorDescription
        {
            get
            {
                // FedEx returns "Service type is missing or invalid". Replace this with a user friendly message.
                if (ErrorCode.Equals("540"))
                    _errorDescription = "FedEx does not support the selected shipping option to this zip code. Please select another shipping option.";

                return _errorDescription;
            }
            set
            {
                _errorDescription = value;
            }
        }

        public bool SignatureRequired
        {
            get;
            set;
        }

        public ShippingRateModel GetShippingRate(RateRequest fedexRequest = null, decimal maxWeightLimit = 0.00M, bool isTimeInTransit = false, ZnodeGenericCollection<ZnodeShoppingCartItem> shipSeparatelyItems = null)
        {
            decimal shippingRate = 0;
            string shippingETA = string.Empty;
            RateService fedExRateService = null;
            FedExAgent fedexAgent = this;
            try
            {
                // Instantiate the FedEx web service
                fedExRateService = new RateService { Url = FedExGatewayUrl };

                // As per service selection check if it is freight service or not.
                bool freightType = IsFreightServiceType(FedExServiceType);

                // Check request for freight and Create the rate request and get the reply from FedEx
                RateRequest request = freightType ? CreateFreightRateRequest(fedexRequest, isTimeInTransit, fedexAgent?.FedExServiceType) : CreateRateRequest(fedexRequest, maxWeightLimit, isTimeInTransit, fedexAgent?.FedExServiceType, shipSeparatelyItems);

                ZnodeLogging.LogMessage("FedEx Execution Started:", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
                RateReply reply = fedExRateService.getRates(request);
                ZnodeLogging.LogMessage("FedEx Execution Ended:", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("FedEx Reply:", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Verbose, new { reply });
                // Check request for freight

                // For most FedEx calls we can consider Note, Warning, and above success
                if (IsRateResponseValid(reply.HighestSeverity))
                {
                    ErrorDescription = reply.Notifications[0].Message;
                    ErrorCode = "0";

                    foreach (RateReplyDetail rateDetail in reply.RateReplyDetails)
                    {
                        shippingETA = rateDetail.CommitDetails[0].CommitTimestamp.ToString("MM-dd-yyyy");

                        foreach (RatedShipmentDetail shipmentDetail in rateDetail.RatedShipmentDetails)
                        {
                            string dateandrate = string.Empty;
                            decimal netCharge;
                            if (UseDiscountRate)
                            {
                                if (IsFreightServiceType(FedExServiceType))
                                {

                                    // Freight rate
                                    if (shipmentDetail.ShipmentRateDetail.RateType.Equals(ReturnedRateType.PAYOR_ACCOUNT_PACKAGE)
                                        || shipmentDetail.ShipmentRateDetail.RateType.Equals(ReturnedRateType.PAYOR_ACCOUNT_SHIPMENT))
                                    {
                                        netCharge = shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount;

                                        if (!isTimeInTransit)
                                        {
                                            shippingRate += netCharge;
                                            break;
                                        }
                                        else
                                        {
                                            dateandrate = shippingETA + "," + Convert.ToString(netCharge);
                                            RateTimeInTransit.Add(Convert.ToString(rateDetail.ServiceType), dateandrate);
                                        }
                                    }

                                }
                                else
                                {
                                    //If we want to apply the discounted rates then we have to use "PAYOR_ACCOUNT_PACKAGE" rate type which contains only discounted rates.
                                    //The rates provided by "PAYOR_ACCOUNT_PACKAGE" are already have deductions and can be applied directly.  
                                    //This will only work with the production account as FedEx does not provide the discounted rates for Test Accounts.
                                    if (shipmentDetail.ShipmentRateDetail.RateType.Equals(ReturnedRateType.PAYOR_ACCOUNT_PACKAGE)
                                         || shipmentDetail.ShipmentRateDetail.RateType.Equals(ReturnedRateType.PAYOR_ACCOUNT_SHIPMENT))
                                    {
                                        netCharge = shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount;

                                        if (!isTimeInTransit)
                                        {
                                            shippingRate += netCharge;
                                            break;
                                        }
                                        else
                                        {
                                            dateandrate = shippingETA + "," + Convert.ToString(netCharge);
                                            RateTimeInTransit.Add(Convert.ToString(rateDetail.ServiceType), dateandrate);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (IsFreightServiceType(FedExServiceType))
                                {
                                    // Freight rate
                                    if (shipmentDetail.ShipmentRateDetail.RateType.Equals(ReturnedRateType.PAYOR_ACCOUNT_PACKAGE)
                                        || shipmentDetail.ShipmentRateDetail.RateType.Equals(ReturnedRateType.PAYOR_ACCOUNT_SHIPMENT))
                                    {
                                        netCharge = shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount;
                                        if (!isTimeInTransit)
                                        {
                                            shippingRate += netCharge;
                                            break;
                                        }
                                        else
                                        {
                                            dateandrate = shippingETA + "," + Convert.ToString(netCharge);
                                            RateTimeInTransit.Add(Convert.ToString(rateDetail.ServiceType), dateandrate);
                                        }
                                    }
                                }
                                else
                                {
                                    //To apply normal rates the rate type should be "PAYOR_LIST_PACKAGE".
                                    //The condition is written only because every call will calculate both the rates discounted and non-discounted.  We need to choose the 
                                    //non-discounted rates which provided by the rate type "PAYOR_LIST_PACKAGE"
                                    if (shipmentDetail.ShipmentRateDetail.RateType.Equals(ReturnedRateType.PAYOR_LIST_PACKAGE)
                                        || shipmentDetail.ShipmentRateDetail.RateType.Equals(ReturnedRateType.PAYOR_LIST_SHIPMENT))
                                    {
                                        netCharge = shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount;
                                        if (!isTimeInTransit)
                                        {
                                            shippingRate += netCharge;
                                            break;
                                        }
                                        else
                                        {
                                            dateandrate = shippingETA + "," + Convert.ToString(netCharge);
                                            RateTimeInTransit.Add(Convert.ToString(rateDetail.ServiceType), dateandrate);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    ErrorCode = reply.Notifications[0].Code;
                    ErrorDescription = reply.Notifications[0].Message;
                }
            }
            catch (SoapException ex)
            {
                ZnodeLogging.LogMessage(ex, "Shipping", TraceLevel.Error);
                ErrorCode = "-1";
                ErrorDescription = ex.Detail.InnerText;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, "Shipping", TraceLevel.Error);
                ErrorCode = "-1";
                ErrorDescription = ex.Message;
            }

            return new ShippingRateModel { ShippingRate = shippingRate, ApproximateArrival = shippingETA };
        }

        public List<ShippingModel> GetFedExEstimateRate(RateRequest fedexRequest = null, decimal maxWeightLimit = 0.00M, ZnodeGenericCollection<ZnodeShoppingCartItem> shipSeparatelyItems = null)
        {
            GetShippingRate(fedexRequest, maxWeightLimit, true, shipSeparatelyItems);
            List<ShippingModel> list = new List<ShippingModel>();

            foreach (var item in RateTimeInTransit ?? new Dictionary<string, string>())
            {
                string[] value = item.Value?.Split(',');
                if(!Equals(value, null))
                    list.Add(new ShippingModel { ShippingCode = item.Key, EstimateDate = value[0], ShippingRate = Convert.ToDecimal(value[1]) });
            }
            return list ?? new List<ShippingModel>();
        }

        public bool IsFreightServiceType(string FedExServiceType)
        {
            List<string> serviceType = new List<string>() { Convert.ToString(ServiceType.FEDEX_1_DAY_FREIGHT), Convert.ToString(ServiceType.FEDEX_2_DAY_FREIGHT), Convert.ToString(ServiceType.FEDEX_3_DAY_FREIGHT), Convert.ToString(ServiceType.FEDEX_3_DAY_FREIGHT), Convert.ToString(ServiceType.FEDEX_FIRST_FREIGHT), Convert.ToString(ServiceType.FEDEX_FREIGHT_ECONOMY), Convert.ToString(ServiceType.FEDEX_FREIGHT_PRIORITY), Convert.ToString(ServiceType.FEDEX_NEXT_DAY_FREIGHT), Convert.ToString(ServiceType.INTERNATIONAL_ECONOMY_FREIGHT), Convert.ToString(ServiceType.INTERNATIONAL_PRIORITY_FREIGHT) };
            return serviceType.Contains(FedExServiceType);
        }

        private RateRequest CreateRateRequest(RateRequest request = null, decimal maxWeightLimit = 0.0M, bool isTimeInTransit = false, string serviceType = null, ZnodeGenericCollection<ZnodeShoppingCartItem> shipSeparatelyItems = null)
        {
            // Build the RateRequest
            if (Equals(request, null))
            {
                request = new RateRequest();
            }

            request.WebAuthenticationDetail = new WebAuthenticationDetail();
            request.WebAuthenticationDetail.UserCredential = new WebAuthenticationCredential();
            request.WebAuthenticationDetail.UserCredential.Key = FedExAccessKey;
            request.WebAuthenticationDetail.UserCredential.Password = FedExSecurityCode;

            request.ClientDetail = new ClientDetail();
            request.ClientDetail.AccountNumber = FedExAccountNumber;
            request.ClientDetail.MeterNumber = FedExMeterNumber;

            request.TransactionDetail = new TransactionDetail();

            // This is a reference field for the customer, any value can be used and will be provided in the response
            request.TransactionDetail.CustomerTransactionId = "Rate Request";

            // WSDL version information, value is automatically set from WSDL
            request.Version = new VersionId();
            request.ReturnTransitAndCommit = true;
            request.ReturnTransitAndCommitSpecified = true;
            request.CarrierCodes = new CarrierCodeType[1];
            request.CarrierCodes[0] = CarrierCodeType.FDXE;

            SetShipmentDetails(request, isTimeInTransit, serviceType);
            SetOrigin(request);
            SetDestination(request);
            SetPayment(request);
            SetIndividualPackageLineItems(request, maxWeightLimit, shipSeparatelyItems);

            return request;
        }

        /// <summary>
        /// Set the individual package line item details.
        /// </summary>
        /// <param name="request">RateRequest object to set the package line item details.</param>
        private void SetIndividualPackageLineItems(RateRequest request, decimal maxWeightLimit, ZnodeGenericCollection<ZnodeShoppingCartItem> shipSeparatelyItems = null)
        {
            // Passing individual pieces rate request            

            decimal packageWeightLimit = 0;
            decimal.TryParse(maxWeightLimit.ToString(), out packageWeightLimit);

            Dictionary<int, decimal> packageWeightCollection = packageWeightLimit > 0 ? GetPackageWithWeight(this.PackageWeight, packageWeightLimit) : new Dictionary<int, decimal>();

            request.RequestedShipment.PackageCount = packageWeightCollection.Count.ToString();

            if (HelperUtility.IsNotNull(shipSeparatelyItems) && shipSeparatelyItems.Count >= 1)
            {
                request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[shipSeparatelyItems.Count];
            }
            else
            {
                request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[packageWeightCollection.Count];
            }
            int count = 0;

            if (HelperUtility.IsNotNull(shipSeparatelyItems) && shipSeparatelyItems.Count >= 1)
            {
                foreach (ZnodeShoppingCartItem ShipSeparatelyItem in shipSeparatelyItems)
                {
                    request.RequestedShipment.RequestedPackageLineItems[count] = new RequestedPackageLineItem();
                    request.RequestedShipment.RequestedPackageLineItems[count].GroupPackageCount = "1";

                    //Set Packaging Type
                    request.RequestedShipment.RequestedPackageLineItems[count].PhysicalPackaging = PhysicalPackagingType.BOX;
                    request.RequestedShipment.RequestedPackageLineItems[count].PhysicalPackagingSpecified = true;

                    // Set the package sequence number
                    request.RequestedShipment.RequestedPackageLineItems[count].SequenceNumber = (count + 1).ToString();

                    // Set the package weight
                    request.RequestedShipment.RequestedPackageLineItems[count].Weight = new Weight();
                    request.RequestedShipment.RequestedPackageLineItems[count].Weight.Value = ShipSeparatelyItem.Product.Weight;
                    request.RequestedShipment.RequestedPackageLineItems[count].Weight.ValueSpecified = true;
                    request.RequestedShipment.RequestedPackageLineItems[count].Weight.Units = (WeightUnits)Enum.Parse(typeof(WeightUnits), this.WeightUnit);
                    request.RequestedShipment.RequestedPackageLineItems[count].Weight.UnitsSpecified = true;

                    if (SignatureRequired)
                    {
                        request.RequestedShipment.RequestedPackageLineItems[count].SpecialServicesRequested = new PackageSpecialServicesRequested();
                        request.RequestedShipment.RequestedPackageLineItems[count].SpecialServicesRequested.SignatureOptionDetail = new SignatureOptionDetail();
                        request.RequestedShipment.RequestedPackageLineItems[count].SpecialServicesRequested.SignatureOptionDetail.OptionType = SignatureOptionType.DIRECT;
                    }

                    if (this.PackageTypeCode == PackagingType.YOUR_PACKAGING.ToString())
                    {
                        // Set the package dimensions
                        request.RequestedShipment.RequestedPackageLineItems[count].Dimensions = new Dimensions();
                        request.RequestedShipment.RequestedPackageLineItems[count].Dimensions.Length = ShipSeparatelyItem.Product.Length.ToString();
                        request.RequestedShipment.RequestedPackageLineItems[count].Dimensions.Width = ShipSeparatelyItem.Product.Width.ToString();
                        request.RequestedShipment.RequestedPackageLineItems[count].Dimensions.Height = ShipSeparatelyItem.Product.Height.ToString();
                        request.RequestedShipment.RequestedPackageLineItems[count].Dimensions.Units = (LinearUnits)Enum.Parse(typeof(LinearUnits), this.DimensionUnit);
                        request.RequestedShipment.RequestedPackageLineItems[count].Dimensions.UnitsSpecified = true;
                    }
                    count++;
                }
            }
            //custom code end.
            else if (packageWeightCollection != null && packageWeightCollection.Count > 0)
            {
                foreach (KeyValuePair<int, decimal> packageWight in packageWeightCollection)
                {
                    request.RequestedShipment.RequestedPackageLineItems[count] = new RequestedPackageLineItem();
                    request.RequestedShipment.RequestedPackageLineItems[count].GroupPackageCount = "1";

                    //Set Packaging Type
                    request.RequestedShipment.RequestedPackageLineItems[count].PhysicalPackaging = PhysicalPackagingType.BOX;
                    request.RequestedShipment.RequestedPackageLineItems[count].PhysicalPackagingSpecified = true;

                    // Set the package sequence number
                    request.RequestedShipment.RequestedPackageLineItems[count].SequenceNumber = (count + 1).ToString();

                    // Set the package weight
                    request.RequestedShipment.RequestedPackageLineItems[count].Weight = new Weight();
                    request.RequestedShipment.RequestedPackageLineItems[count].Weight.Value = packageWight.Value;
                    request.RequestedShipment.RequestedPackageLineItems[count].Weight.ValueSpecified = true;
                    request.RequestedShipment.RequestedPackageLineItems[count].Weight.Units = (WeightUnits)Enum.Parse(typeof(WeightUnits), this.WeightUnit);
                    request.RequestedShipment.RequestedPackageLineItems[count].Weight.UnitsSpecified = true;

                    if (SignatureRequired)
                    {
                        request.RequestedShipment.RequestedPackageLineItems[count].SpecialServicesRequested = new PackageSpecialServicesRequested();
                        request.RequestedShipment.RequestedPackageLineItems[count].SpecialServicesRequested.SignatureOptionDetail = new SignatureOptionDetail();
                        request.RequestedShipment.RequestedPackageLineItems[count].SpecialServicesRequested.SignatureOptionDetail.OptionType = SignatureOptionType.DIRECT;
                    }

                    if (this.PackageTypeCode == PackagingType.YOUR_PACKAGING.ToString())
                    {
                        // Set the package dimensions
                        request.RequestedShipment.RequestedPackageLineItems[count].Dimensions = new Dimensions();
                        request.RequestedShipment.RequestedPackageLineItems[count].Dimensions.Length = this.PackageLength;
                        request.RequestedShipment.RequestedPackageLineItems[count].Dimensions.Width = this.PackageWidth;
                        request.RequestedShipment.RequestedPackageLineItems[count].Dimensions.Height = this.PackageHeight;
                        request.RequestedShipment.RequestedPackageLineItems[count].Dimensions.Units = (LinearUnits)Enum.Parse(typeof(LinearUnits), this.DimensionUnit);
                        request.RequestedShipment.RequestedPackageLineItems[count].Dimensions.UnitsSpecified = true;
                    }
                    count++;

                }
            }
            else
            {
                request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[1];
                request.RequestedShipment.RequestedPackageLineItems[0] = new RequestedPackageLineItem();

                // Set the package sequence number
                request.RequestedShipment.RequestedPackageLineItems[0].SequenceNumber = "1";

                // Set the package weight
                request.RequestedShipment.RequestedPackageLineItems[0].Weight = new Weight();
                request.RequestedShipment.RequestedPackageLineItems[0].Weight.Value = this.PackageWeight;
                request.RequestedShipment.RequestedPackageLineItems[0].Weight.Units = (WeightUnits)Enum.Parse(typeof(WeightUnits), this.WeightUnit);

                if (this.PackageTypeCode == PackagingType.YOUR_PACKAGING.ToString())
                {
                    // Set the package dimensions
                    request.RequestedShipment.RequestedPackageLineItems[0].Dimensions = new Dimensions();
                    request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Length = this.PackageLength;
                    request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Width = this.PackageWidth;
                    request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Height = this.PackageHeight;
                    request.RequestedShipment.RequestedPackageLineItems[0].Dimensions.Units = (LinearUnits)Enum.Parse(typeof(LinearUnits), this.DimensionUnit);
                }
            }
        }

        private Dictionary<int, decimal> GetPackageWithWeight(decimal totalWeight, decimal packageWeightLimit)
        {
            int totalPackage = 0; int totalWeightOfPackage; decimal packageWeight;
            Dictionary<int, decimal> packageCollection = new Dictionary<int, decimal>();
            if ((totalWeight % packageWeightLimit) > 0)
            {
                totalPackage = (int)(totalWeight / packageWeightLimit) + 1;
            }
            else
            {
                totalPackage = (int)(totalWeight / packageWeightLimit);
                if (totalPackage < 1 && totalWeight < 1)
                {
                    totalPackage = 1;
                }
            }
            totalWeightOfPackage = Convert.ToInt32(totalWeight);
            packageWeight = packageWeightLimit;
            for (int index = 0; index < totalPackage; index++)
            {
                if ((index + 1).Equals(totalPackage))
                {
                    decimal createdPackageWeight = packageWeightLimit * index;
                    packageWeight = totalWeightOfPackage - createdPackageWeight;
                    if (packageWeight < 1)
                    {
                        packageWeight = 1;
                    }
                    packageCollection.Add(index, packageWeight);
                }
                else
                {
                    packageCollection.Add(index, packageWeight);
                }

            }
            return packageCollection;
        }
        private void SetShipmentDetails(RateRequest request = null, bool isTimeInTransit = false, string serviceType = null)
        {
            if (HelperUtility.IsNull(request.RequestedShipment))
                request.RequestedShipment = new RequestedShipment();

            // Drop off types are BUSINESS_SERVICE_CENTER, DROP_BOX, REGULAR_PICKUP, REQUEST_COURIER, STATION
            request.RequestedShipment.DropoffType = (DropoffType)Enum.Parse(typeof(DropoffType), DropOffType);

            // Service types are STANDARD_OVERNIGHT, PRIORITY_OVERNIGHT, FEDEX_GROUND, etc.
            if (!isTimeInTransit && !string.IsNullOrEmpty(serviceType))
                request.RequestedShipment.ServiceType = serviceType;

            // Packaging type FEDEX_BOK, FEDEX_PAK, FEDEX_TUBE, YOUR_PACKAGING, etc.
            request.RequestedShipment.PackagingType = PackageTypeCode;

            // If TotalInsured value is greater than 0, then set related properties
            if (TotalInsuredValue > 0)
            {
                request.RequestedShipment.TotalInsuredValue = new Money();
                request.RequestedShipment.TotalInsuredValue.Amount = TotalInsuredValue;
                request.RequestedShipment.TotalInsuredValue.Currency = CurrencyCode;
            }

            IZnodeShippingHelper helper = GetService<IZnodeShippingHelper>();
            // Shipping date and time
            request.RequestedShipment.ShipTimestamp = helper.GetPickUpDate();
            request.RequestedShipment.ShipTimestampSpecified = true;

            request.RequestedShipment.RateRequestTypes = new RateRequestType[1] { RateRequestType.LIST };
        }

        private void SetOrigin(RateRequest request)
        {
            request.RequestedShipment.Shipper = new Party();
            request.RequestedShipment.Shipper.Address = new Address();

            // Residential or Standard
            request.RequestedShipment.Shipper.Address.Residential = ShipperAddressIsResidential;
            request.RequestedShipment.Shipper.Address.StreetLines = new string[2] { ShipperAddress1, ShipperAddress2 };
            request.RequestedShipment.Shipper.Address.City = ShipperCity;
            ShipperStateCode = !string.IsNullOrEmpty(ShipperStateCode) && ShipperStateCode.Length >= 3 ? ShipperStateCode.Substring(0, 2) : ShipperStateCode;
            request.RequestedShipment.Shipper.Address.StateOrProvinceCode = ShipperStateCode;
            request.RequestedShipment.Shipper.Address.PostalCode = ShipperZipCode;
            request.RequestedShipment.Shipper.Address.CountryCode = ShipperCountryCode;
        }

        private void SetDestination(RateRequest request)
        {
            request.RequestedShipment.Recipient = new Party();
            request.RequestedShipment.Recipient.Address = new Address();
            request.RequestedShipment.Recipient.Address.StreetLines = new string[2] { ShipToAddress1, ShipToAddress2 };
            request.RequestedShipment.Recipient.Address.City = ShipToCity;
            ShipToStateCode = !string.IsNullOrEmpty(ShipToStateCode) && ShipToStateCode.Length >= 3 ? ShipToStateCode.Substring(0, 2) : ShipToStateCode;
            request.RequestedShipment.Recipient.Address.StateOrProvinceCode = ShipToStateCode;
            request.RequestedShipment.Recipient.Address.PostalCode = ShipToZipCode;
            request.RequestedShipment.Recipient.Address.CountryCode = ShipToCountryCode;
            request.RequestedShipment.Recipient.Address.Residential = ShipToAddressIsResidential;
        }

        private void SetPayment(RateRequest request)
        {
            request.RequestedShipment.ShippingChargesPayment = new Payment();

            // Payment options are RECIPIENT, SENDER, THIRD_PARTY
            request.RequestedShipment.ShippingChargesPayment.PaymentType = PaymentType.SENDER;
            request.RequestedShipment.ShippingChargesPayment.PaymentTypeSpecified = true;
            request.RequestedShipment.ShippingChargesPayment.Payor = new Payor();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty = new Party();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.AccountNumber = FedExAccountNumber;
        }

        // SUCCESS, NOTE, or WARNING equals a valid response
        private bool IsRateResponseValid(NotificationSeverityType highestSeverity) => (Equals(highestSeverity, NotificationSeverityType.SUCCESS) || Equals(highestSeverity, NotificationSeverityType.NOTE) || Equals(highestSeverity, NotificationSeverityType.WARNING));

        #region Freight Shipping Rate
        private RateRequest CreateFreightRateRequest(RateRequest request = null, bool isTimeInTransit = false, string serviceType = null)
        {
            if (HelperUtility.IsNull(request))
                request = new RateRequest();

            request.WebAuthenticationDetail = new WebAuthenticationDetail();
            request.WebAuthenticationDetail.UserCredential = new WebAuthenticationCredential();

            request.WebAuthenticationDetail.UserCredential.Key = FedExAccessKey;
            request.WebAuthenticationDetail.UserCredential.Password = FedExSecurityCode;

            request.ClientDetail = new ClientDetail();
            request.ClientDetail.AccountNumber = FedExAccountNumber;
            request.ClientDetail.MeterNumber = FedExMeterNumber;

            request.TransactionDetail = new TransactionDetail();
            request.TransactionDetail.CustomerTransactionId = "Freight Rate Request";

            request.Version = new VersionId();
            request.ReturnTransitAndCommit = true;
            request.ReturnTransitAndCommitSpecified = true;
            request.CarrierCodes = new CarrierCodeType[1];
            request.CarrierCodes[0] = CarrierCodeType.FXFR;


            SetFreightShipmentDetails(request, isTimeInTransit, serviceType);

            return request;
        }

        private void SetFreightShipmentDetails(RateRequest request, bool isTimeInTransit = false, string serviceType = null)
        {
            if (HelperUtility.IsNull(request.RequestedShipment))
                request.RequestedShipment = new RequestedShipment();

            IZnodeShippingHelper helper = GetService<IZnodeShippingHelper>();
            request.RequestedShipment.ShipTimestamp = helper.GetPickUpDate();
            request.RequestedShipment.ShipTimestampSpecified = true;
            request.RequestedShipment.DropoffType = (DropoffType)Enum.Parse(typeof(DropoffType), DropOffType);// DropoffType.REGULAR_PICKUP; //Drop off types are BUSINESS_SERVICE_CENTER, DROP_BOX, REGULAR_PICKUP, REQUEST_COURIER, STATION
            // If ServiceType is omitted, all applicable ServiceTypes are returned.
            if (!isTimeInTransit && !string.IsNullOrEmpty(serviceType))
                request.RequestedShipment.ServiceType = serviceType;// ServiceType.FEDEX_FREIGHT_PRIORITY; // Service types are STANDARD_OVERNIGHT, PRIORITY_OVERNIGHT, FEDEX_GROUND ...

            request.RequestedShipment.PackagingType = PackageTypeCode;

            if (string.IsNullOrEmpty(request.RequestedShipment.PackageCount))
                request.RequestedShipment.PackageCount = "1";

            // Set origin address
            SetOrigin(request);
            // Set destination address
            SetDestination(request);
            // Set freight address 
            SetFreightPayment(request);
            // Set freight shipment detail
            SetFreightShipmentDetail(request);
        }

        /// <summary>
        /// Set freight payment.
        /// </summary>
        /// <param name="request"></param>
        private void SetFreightPayment(RateRequest request)
        {
            request.RequestedShipment.ShippingChargesPayment = new Payment();
            request.RequestedShipment.ShippingChargesPayment.PaymentType = PaymentType.SENDER;
            request.RequestedShipment.ShippingChargesPayment.PaymentTypeSpecified = true;
            request.RequestedShipment.ShippingChargesPayment.Payor = new Payor();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty = new Party();
            request.RequestedShipment.ShippingChargesPayment.Payor.ResponsibleParty.AccountNumber = FedExLTLAccountNumber;
        }

        /// <summary>
        /// Set freight shipment detail.
        /// </summary>
        /// <param name="request"></param>
        private void SetFreightShipmentDetail(RateRequest request)
        {
            if (HelperUtility.IsNull(request.RequestedShipment.FreightShipmentDetail))
                request.RequestedShipment.FreightShipmentDetail = new FreightShipmentDetail();

            request.RequestedShipment.FreightShipmentDetail.FedExFreightAccountNumber = FedExLTLAccountNumber;
            SetFreightBillingContactAddress(request);
            request.RequestedShipment.FreightShipmentDetail.Role = FreightShipmentRoleType.SHIPPER;
            request.RequestedShipment.FreightShipmentDetail.RoleSpecified = true;
            SetFreightShipmentLineItems(request);
        }

        /// <summary>
        /// Set freight billing contact address.
        /// </summary>
        /// <param name="request"></param>
        private void SetFreightBillingContactAddress(RateRequest request)
        {
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress = new ContactAndAddress();
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Contact = new Contact();
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address = new Address();
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address.StreetLines = new string[2] { ShipperAddress1, ShipperAddress2 };

            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address.City = ShipperCity;
            ShipperStateCode = !string.IsNullOrEmpty(ShipperStateCode) && ShipperStateCode.Length >= 3 ? ShipperStateCode.Substring(0, 2) : ShipperStateCode;
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address.StateOrProvinceCode = ShipperStateCode;
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address.PostalCode = ShipperZipCode;
            request.RequestedShipment.FreightShipmentDetail.FedExFreightBillingContactAndAddress.Address.CountryCode = ShipperCountryCode;
        }

        /// <summary>
        /// Set freight shipment line items.
        /// </summary>
        /// <param name="request"></param>
        private void SetFreightShipmentLineItems(RateRequest request)
        {
            if (HelperUtility.IsNull(request.RequestedShipment.FreightShipmentDetail.LineItems))
            {
                request.RequestedShipment.FreightShipmentDetail.LineItems = new FreightShipmentLineItem[1];
                request.RequestedShipment.FreightShipmentDetail.LineItems[0] = new FreightShipmentLineItem();
                request.RequestedShipment.FreightShipmentDetail.LineItems[0].FreightClass = FreightClassType.CLASS_100;
                request.RequestedShipment.FreightShipmentDetail.LineItems[0].FreightClassSpecified = true;

                //Set Packaging Type
                request.RequestedShipment.FreightShipmentDetail.LineItems[0].Packaging = string.IsNullOrEmpty(ItemPackageTypeCode) ? PhysicalPackagingType.BOX : (PhysicalPackagingType)Enum.Parse(typeof(PhysicalPackagingType), ItemPackageTypeCode, true);
                request.RequestedShipment.FreightShipmentDetail.LineItems[0].PackagingSpecified = true;
                request.RequestedShipment.FreightShipmentDetail.LineItems[0].Description = "Freight line item description";

                request.RequestedShipment.FreightShipmentDetail.LineItems[0].Weight = new Weight();
                request.RequestedShipment.FreightShipmentDetail.LineItems[0].Weight.Value = PackageWeight;
                request.RequestedShipment.FreightShipmentDetail.LineItems[0].Weight.ValueSpecified = true;
                request.RequestedShipment.FreightShipmentDetail.LineItems[0].Weight.Units = (WeightUnits)Enum.Parse(typeof(WeightUnits), WeightUnit);
                request.RequestedShipment.FreightShipmentDetail.LineItems[0].Weight.UnitsSpecified = true;


                request.RequestedShipment.FreightShipmentDetail.LineItems[0].Dimensions = new Dimensions();
                request.RequestedShipment.FreightShipmentDetail.LineItems[0].Dimensions.Length = PackageLength;
                request.RequestedShipment.FreightShipmentDetail.LineItems[0].Dimensions.Width = PackageWidth;
                request.RequestedShipment.FreightShipmentDetail.LineItems[0].Dimensions.Height = PackageHeight;
                request.RequestedShipment.FreightShipmentDetail.LineItems[0].Dimensions.Units = (LinearUnits)Enum.Parse(typeof(LinearUnits), DimensionUnit);
                request.RequestedShipment.FreightShipmentDetail.LineItems[0].Dimensions.UnitsSpecified = true;

                request.RequestedShipment.FreightShipmentDetail.TotalHandlingUnits = TotalHandlingUnits;
                request.RequestedShipment.PackageCount = PackageCount;

            }
        }
        #endregion
    }
}
