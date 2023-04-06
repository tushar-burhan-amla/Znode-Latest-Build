using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using Znode.Api.Core.Helper;

namespace Znode.Engine.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.Filters.Add(new APIAuthorizeAttribute());
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new DefaultContractResolver { IgnoreSerializableAttribute = true };

            RegisterV1Routes(config);
            RegisterV2Routes(config);
        }

        static void RegisterV1Routes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute("generatetoken", "token/generatetoken", new { controller = "Token", action = "GenerateToken" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("demo-cart", "cart/get", new { controller = "cart", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //Account
            config.Routes.MapHttpRoute("accounts-list", "accounts/list", new { controller = "account", action = "getaccountlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("fileupload-upload", "apiupload/upload", new { controller = "fileupload", action = "postasync" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("fileupload-uploadpodocument", "apiupload/uploadpodocument", new { controller = "fileupload", action = "uploadpodocument" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("fileupload-remove", "apiupload/remove", new { controller = "fileupload", action = "remove" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("fileupload-removepodocument", "apiupload/removepodocument", new { controller = "fileupload", action = "removepodocument" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("accounts-getaccountbycode", "account/getaccountbycode/{accountcode}/", new { controller = "account", action = "getaccountbycode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("accounts-deleteaccounts", "account/deleteaccountbycode", new { controller = "account", action = "deleteaccountbycode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("account-getparentaccountlist", "account/getparentaccountlist", new { controller = "account", action = "getparentaccountlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Progress Notification
            config.Routes.MapHttpRoute("progressnotification-list", "apiprogressnotification/List", new { controller = "progressnotification", action = "getProgressnotifications" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Document Upload
            config.Routes.MapHttpRoute("fileupload-uploadformdocument", "apiupload/uploadformdocument", new { controller = "fileupload", action = "uploadformdocument" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Account Address
            config.Routes.MapHttpRoute("accounts-addresslist", "account/addresslist", new { controller = "account", action = "addresslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("account-createaccountaddress", "account/createaccountaddress", new { controller = "account", action = "createaccountaddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("account-getaccountaddress", "account/getaccountaddress", new { controller = "account", action = "getaccountaddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("account-updateaccountaddress", "account/updateaccountaddress", new { controller = "account", action = "updateaccountaddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("account-deleteaccountaddress", "account/deleteaccountaddress", new { controller = "account", action = "deleteaccountaddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Account Department
            config.Routes.MapHttpRoute("account-createaccountdepartment", "account/createaccountdepartment", new { controller = "account", action = "createaccountdepartment" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("account-getaccountdepartments", "account/getaccountdepartments", new { controller = "account", action = "getaccountdepartments" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("account-getaccountdepartment", "account/getaccountdepartment/{departmentId}", new { controller = "account", action = "getaccountdepartment" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), departmentId = @"^\d+$" });
            config.Routes.MapHttpRoute("account-updateaccountdepartment", "account/updateaccountdepartment", new { controller = "account", action = "updateaccountdepartment" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("account-deleteaccountdepartment", "account/deleteaccountdepartment", new { controller = "account", action = "deleteaccountdepartment" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Account Notes
            config.Routes.MapHttpRoute("account-createaccountnote", "account/createaccountnote", new { controller = "account", action = "createaccountnote" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("account-getaccountnotes", "account/getaccountnotes", new { controller = "account", action = "getaccountnotes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("account-getaccountnote", "account/getaccountnote/{noteId}", new { controller = "account", action = "getaccountnote" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), noteId = @"^\d+$" });
            config.Routes.MapHttpRoute("account-updateaccountnote", "account/updateaccountnote", new { controller = "account", action = "updateaccountnote" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("account-deleteaccountnote", "account/deleteaccountnote", new { controller = "account", action = "deleteaccountnote" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Account Price
            config.Routes.MapHttpRoute("account-associatepricelist", "account/associatepricelist", new { controller = "account", action = "associatepricelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("account-unassociatepricelist", "account/unassociatepricelist", new { controller = "account", action = "unassociatepricelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("account-getassociatedpricelistprecedence", "account/getassociatedpricelistprecedence", new { controller = "account", action = "getassociatedpricelistprecedence" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("account-updateassociatedpricelistprecedence", "account/updateassociatedpricelistprecedence", new { controller = "account", action = "updateassociatedpricelistprecedence" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //Access permission
            config.Routes.MapHttpRoute("accesspermission-accountpermissionlist", "accesspermission/accountpermissionlist", new { controller = "accesspermission", action = "accountpermissionlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("accesspermission-createaccountpermission", "accesspermission/createaccountpermission", new { controller = "accesspermission", action = "createaccountpermission" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("accesspermission-deleteaccountpermission", "accesspermission/deleteaccountpermission", new { controller = "accesspermission", action = "deleteaccountpermission" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("accesspermission-getaccountpermission", "accesspermission/getaccountpermission", new { controller = "accesspermission", action = "getaccountpermission" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("accesspermission-updateaccountpermission", "accesspermission/updateaccountpermission", new { controller = "accesspermission", action = "updateaccountpermission" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("accesspermission-accesspermissionlist", "accesspermission/accesspermissionlist", new { controller = "accesspermission", action = "accesspermissionlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Account Order
            config.Routes.MapHttpRoute("accounts-getaccountuserorderlist", "account/getaccountuserorderlist/{accountId}", new { controller = "account", action = "getaccountuserorderlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), accountId = @"^\d+$" });

            //Account Profile 
            config.Routes.MapHttpRoute("account-getassociatedunassociatedprofile", "account/getassociatedunassociatedprofile", new { controller = "account", action = "getassociatedunassociatedprofile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("account-associateprofile", "account/associateprofile", new { controller = "account", action = "associateprofile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("account-unassociateprofile", "account/unassociateprofile", new { controller = "account", action = "unassociateprofile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Account Quote
            config.Routes.MapHttpRoute("accountquote-list", "accountquote/list", new { controller = "accountquote", action = "getaccountquotelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("accountquote-create", "accountquote/create", new { controller = "accountquote", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("accountquote-get", "accountquote/get", new { controller = "accountquote", action = "getaccountquote" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("accountquote-updatequotestatus", "accountquote/updatequotestatus", new { controller = "accountquote", action = "updatequotestatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("accountquote-updatequotelineitemquantity", "accountquote/updatequotelineitemquantity", new { controller = "accountquote", action = "updatequotelineitemquantity" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("accountquote-updatequotelineitemquantities", "accountquote/updatequotelineitemquantities", new { controller = "accountquote", action = "updatequotelineitemquantities" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("accountquote-deletequotelineitem", "accountquote/deletequotelineitem/{omsQuoteLineItemId}/{omsParentQuoteLineItemId}/{omsQuoteId}", new { controller = "accountquote", action = "deletequotelineitem" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });
            config.Routes.MapHttpRoute("accountquote-create-template", "accountquote/createtemplate", new { controller = "accountquote", action = "createtemplate" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("accountquote-templatelist", "accountquote/gettemplatelist", new { controller = "accountquote", action = "gettemplatelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("accountquote-deletetemplate", "accountquote/deletetemplate", new { controller = "accountquote", action = "deletetemplate" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("accountquote-deletecartitem", "accountquote/deletecartitem", new { controller = "accountquote", action = "deletecartitem" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("accountquote-gettemplate", "accountquote/gettemplate/{omsTemplateId}", new { controller = "accountquote", action = "getaccounttemplate" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("accountquote-getuserapproverlist", "accountquote/getuserapproverlist", new { controller = "accountquote", action = "getuserapproverlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("accountquote-userapproverdetails", "accountquote/userapproverdetails/{userId}", new { controller = "accountquote", action = "userapproverdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("accountquote-islastapprover", "accountquote/islastapprover/{quoteId}", new { controller = "accountquote", action = "islastapprover" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("accountquote-getbillingaccountnumber", "accountquote/getbillingaccountnumber/{userid}", new { controller = "accountquote", action = "getbillingaccountnumber" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("accountquote-updatequoteshippingaddress", "accountquote/updatequoteshippingaddress", new { controller = "accountquote", action = "updatequoteshippingaddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("accountquote-userdashboardpendingorderdetailscount", "accountquote/userdashboardpendingorderdetailscount", new { controller = "accountquote", action = "getuserdashboardpendingorderdetailscount" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //Add-on Group routes
            config.Routes.MapHttpRoute("addongroup-get", "addongroup", new { controller = "addongroup", action = "getaddongroup" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("addongroup-create", "addongroup/create", new { controller = "addongroup", action = "createaddongroup" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("addongroup-update", "addongroup/update", new { controller = "addongroup", action = "updateaddongroup" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("addongroup-list", "addongroup/list", new { controller = "addongroup", action = "getaddongrouplist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("addongroup-delete", "addongroup/delete", new { controller = "addongroup", action = "deleteaddongroup" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("addongroup-associateproduct", "addongroup/associateaddongroupproduct", new { controller = "addongroup", action = "associateaddongroupproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("addongroup-associatedproductlist", "addongroup/getassociatedaddongroupproducts/{addonGroupId}", new { controller = "addongroup", action = "getassociatedaddongroupproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), addonGroupId = @"^\d+$" });
            config.Routes.MapHttpRoute("addongroup-unassociatedproductlist", "addongroup/getunassociatedaddongroupproducts/{addonGroupId}", new { controller = "addongroup", action = "getunassociatedaddongroupproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), addonGroupId = @"^\d+$" });
            config.Routes.MapHttpRoute("addongroup-deleteaddongroupproductassociation", "addongroup/deleteaddongroupproductassociation", new { controller = "addongroup", action = "deleteaddongroupproductassociation" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //address routes
            config.Routes.MapHttpRoute("adress-list", "address/list", new { controller = "address", action = "getaddresslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Application settings routes
            config.Routes.MapHttpRoute("applicationsetting-reader", "applicationsettings/{itemName}", new { controller = "applicationsettings", action = "getfilterconfigurationxml" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("applicationsetting-list", "applicationsettings", new { controller = "applicationsettings", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("applicationsetting-columnlist", "applicationsettings/getcolumnlist/{entityType}/{entityName}", new { controller = "applicationsettings", action = "getcolumnlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("applicationsetting-create", "applicationsettings", new { controller = "applicationsettings", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("applicationsetting-viewsave", "applicationsettings/createnewview", new { controller = "applicationsettings", action = "CreateNewView" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("applicationsetting-deleteview", "applicationsettings/deleteview", new { controller = "applicationsettings", action = "deleteview" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("applicationsetting-getviewbyid", "applicationsettings/getviewbyid/{id}", new { controller = "applicationsettings", action = "GetViewById" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"^\d+$" });
            config.Routes.MapHttpRoute("applicationsetting-reader1", "applicationsettings/{itemName}/{userId}", new { controller = "applicationsettings", action = "getfilterconfigurationxml" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("applicationsetting-removeselected", "applicationsettings/updateviewselectedstatus/{applicationSettingId}", new { controller = "applicationsettings", action = "updateviewselectedstatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Attribute Family Route
            config.Routes.MapHttpRoute("attributefamily-list", "attributefamily/list", new { controller = "attributefamily", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("attributefamily-create", "attributefamily/create", new { controller = "attributefamily", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("attributefamily-getassignedattributegroups", "attributefamily/getassignedattributegroups", new { controller = "attributefamily", action = "getassignedattributegroups" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("attributefamily-associateattributegroup", "attributefamily/associateattributegroup", new { controller = "attributefamily", action = "associateattributegroup" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("attributefamily-unassociateattributegroup", "attributefamily/unassociateattributegroup/{attributeFamilyId}/{attributeGroupId}", new { controller = "attributefamily", action = "unassociateattributegroup" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });
            config.Routes.MapHttpRoute("attributefamily-delete", "attributefamily/delete", new { controller = "attributefamily", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("attributefamily-get", "attributefamily/getattributefamily/{attributeFamilyId}", new { controller = "attributefamily", action = "getattributefamily" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("attributefamily-getfamilylocales", "attributefamily/getfamilylocale/{attributeFamilyId}", new { controller = "attributefamily", action = "getfamilylocale" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("attributefamily-savelocales", "attributefamily/savelocales", new { controller = "attributefamily", action = "savelocales" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("attributefamily-getunassignedattributegroups", "attributefamily/getunassignedattributegroups", new { controller = "attributefamily", action = "getunassignedattributegroups" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("attributefamily-getattributesbygroupids", "attributefamily/getattributesbygroupids", new { controller = "attributefamily", action = "getattributesbygroupids" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Attribute Group Routes
            config.Routes.MapHttpRoute("attributegroup-list", "attributegroups", new { controller = "attributegroup", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("attributegroup-create", "attributegroup", new { controller = "attributegroup", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("attributegroup-get", "attributegroup/{attributeGroupId}", new { controller = "attributegroup", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("attributegroup-update", "attributegroup/update", new { controller = "attributegroup", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("attributegroup-delete", "attributegroup/delete", new { controller = "attributegroup", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("attributegroup-associateattribute", "assignattributes", new { controller = "attributegroup", action = "assignattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("attributegroup-associatedattribute", "associatedattributes", new { controller = "attributegroup", action = "getassignedattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("attributegroup-updateattributegroupmapper", "updateattributegroupmapper", new { controller = "attributegroup", action = "updateattributegroupmapper" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("attributegroup-deleteassociatedattribute", "deleteassociatedattribute/{attributeGroupMapperId}", new { controller = "attributegroup", action = "deleteassociatedattribute" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });
            config.Routes.MapHttpRoute("attributegroup-attributegrouplocales", "attributegroup/attributegrouplocales/{attributeGroupId}", new { controller = "attributegroup", action = "getattributegrouplocale" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("attributegroup-getunassignedattributelist", "attributegroup/getunassignedattributelist/{attributeGroupId}", new { controller = "attributegroup", action = "getunassignedattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Attributes routes 
            config.Routes.MapHttpRoute("attribute-getdefaultvalues", "attributes/getdefaultvalues/{AttributeId}", new { controller = "attributes", action = "getdefaultvalues" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), AttributeId = @"^\d+$" });
            config.Routes.MapHttpRoute("attribute-deletedefaultvalues", "attributes/deletedefaultvalues/{defaultvalueId}", new { controller = "attributes", action = "deletedefaultvalues" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete), defaultvalueId = @"^\d+$" });
            config.Routes.MapHttpRoute("attribute-savedefaultvalues", "attributes/savedefaultvalues", new { controller = "attributes", action = "savedefaultvalues" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("attribute-inputvalidations", "attributes/inputvalidations/{typeId}/{attributeId}", new { controller = "attributes", action = "InputValidations" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), typeId = @"^\d+$" });
            config.Routes.MapHttpRoute("attribute-savelocales", "attributes/savelocales", new { controller = "attributes", action = "savelocales" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("attribute-create", "attributes/create", new { controller = "attributes", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("attribute-update", "attributes/update", new { controller = "attributes", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("attributes-list", "attributes/list", new { controller = "attributes", action = "getattributelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("attributes-get", "attributes/{attributeId}", new { controller = "attributes", action = "getattribute" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("attributes-delete", "attributes/delete", new { controller = "attributes", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("attributetype-list", "attributestype/list", new { controller = "attributes", action = "getattributetypelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("attributeslocale-get", "attributelocale/{attributeId}", new { controller = "attributes", action = "Getattributelocale" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("attribute-isattributecodeexist", "attributes/isattributecodeexist/{attributeCode}", new { controller = "attributes", action = "isattributecodeexist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("attribute-isattributevalueunique", "attributes/isattributevalueunique", new { controller = "attributes", action = "isattributevalueunique" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Account
            config.Routes.MapHttpRoute("accounts-get", "account/{accountId}", new { controller = "account", action = "getaccount" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), accountId = @"^\d+$" });

            //Account Routes
            config.Routes.MapHttpRoute("account-list", "account/list", new { controller = "account", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("account-create", "account/create", new { controller = "account", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("account-update", "account/update", new { controller = "account", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("account-delete", "account/delete", new { controller = "account", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("account-getbyname", "account/getaccountbyname/{accountName}/{portalId}", new { controller = "account", action = "getaccountbyname" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Banner - Slider Configuration
            config.Routes.MapHttpRoute("banner-list", "slider/banner/list", new { controller = "slider", action = "bannerlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("banner-create", "slider/createbanner", new { controller = "slider", action = "createbanner" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("banner-get", "slider/getbanner/{cmssliderbannerid}", new { controller = "slider", action = "getbanner" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("banner-update", "slider/updatebanner", new { controller = "slider", action = "updatebanner" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("banner-delete", "slider/deletebanner", new { controller = "slider", action = "deletebanner" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Brand
            config.Routes.MapHttpRoute("brand-list", "brand/list", new { controller = "brand", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("brand-create", "brand/create", new { controller = "brand", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("brand-get", "brand/getbrand/{brandId}/{localeId}", new { controller = "brand", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("brand-update", "brand/update", new { controller = "brand", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("brand-delete", "brand/delete", new { controller = "brand", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("brand-codelist", "brand/brandcodelist/{attributeCode}", new { controller = "brand", action = "getbrandcodelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("brand-associateproduct", "brand/associatebrandproduct", new { controller = "brand", action = "associateandunassociateproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("brand-activeinactivebrand", "brand/activeinactivebrand", new { controller = "brand", action = "activeinactivebrand" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("brand-getportallist", "brand/getbrandportallist", new { controller = "brand", action = "getbrandportallist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("brand-associateportal", "brand/associatebrandportal", new { controller = "brand", action = "associateandunassociateportal" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("brand-checkuniqueforbrandcode", "brand/checkuniquebrandcode/{brandCode}", new { controller = "brand", action = "checkuniquebrandcode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            // Portal Brand
            config.Routes.MapHttpRoute("portalbrand-associateportalbrands", "brand/associateandunassociateportalbrand", new { controller = "brand", action = "associateandunassociateportalbrand" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portalbrand-list", "brand/portalbrandlist", new { controller = "brand", action = "getportalbrandlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portalbrand-updateassociatedportalbranddetail", "brand/UpdateAssociatedPortalBrandDetail", new { controller = "brand", action = "updateassociatedportalbranddetail" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //CMS Theme Routes
            config.Routes.MapHttpRoute("theme-list", "theme/list", new { controller = "theme", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("theme-create", "theme", new { controller = "theme", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("theme-get", "theme/gettheme/{cmsThemeId}", new { controller = "theme", action = "gettheme" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("theme-update", "theme/updatetheme", new { controller = "theme", action = "updatetheme" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("theme-delete", "theme/delete", new { controller = "theme", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("theme-associatestore", "theme/associatestore", new { controller = "theme", action = "associatestore" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("theme-removeassociatedstores", "theme/removeassociatedstores", new { controller = "theme", action = "removeassociatedstores" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("theme-getarealist", "theme/getareas", new { controller = "theme", action = "getareas" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //CMS : Contents
            config.Routes.MapHttpRoute("managemessage-managemessagelist", "managemessage/managemessagelist", new { controller = "managemessage", action = "getmanagemessages" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("managemessage-getmanagemessage", "managemessage/getmanagemessage", new { controller = "managemessage", action = "getmanagemessage" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("managemessage-createmanagemessage", "managemessage/createmanagemessage", new { controller = "managemessage", action = "createmanagemessage" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("managemessage-updatemanagemessage", "managemessage/updatemanagemessage", new { controller = "managemessage", action = "updatemanagemessage" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("managemessage-deletemanagemessage", "managemessage/deletemanagemessage", new { controller = "managemessage", action = "deletemanagemessage" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("managemessage-publishmanagemessage", "managemessage/publishmanagemessage", new { controller = "managemessage", action = "publishmanagemessage" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("managemessage-publishmanagemessagewithpreview", "managemessage/publishmanagemessagewithpreview", new { controller = "managemessage", action = "publishmanagemessagewithpreview" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //CMS Widdgets
            config.Routes.MapHttpRoute("cmswidgets-list", "cmswidgets/list", new { controller = "cmswidgets", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("cmswidgets-getwidgetbycode", "cmswidgets/getwidgetbycode", new { controller = "cmswidgets", action = "getwidgetbycodes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });


            //CMS Content Containers
            config.Routes.MapHttpRoute("cmscontentcontainer-list", "contentcontainer/list", new { controller = "contentcontainer", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("cmscontentcontainers-create", "contentcontainer/create", new { controller = "contentcontainer", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cmscontentcontainers-update", "contentcontainer/update", new { controller = "contentcontainer", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("cmscontentcontainers-get", "contentcontainer/getcontentcontainer/{containerkey}", new { controller = "contentcontainer", action = "getcontentcontainer" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //renamed the widgetkey to containerKey as this is overall URL change required
            config.Routes.MapHttpRoute("cmscontentcontainers-getassociatedvariants", "contentcontainer/getassociatedvariants/{containerKey}", new { controller = "contentcontainer", action = "getassociatedvariants" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("cmscontentcontainers-associatedvariant", "contentcontainer/associatevariant", new { controller = "contentcontainer", action = "associatevariant" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cmscontentcontainers-deleteassociatedvariant", "contentcontainer/deleteassociatedvariant", new { controller = "contentcontainer", action = "deleteassociatedvariant" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cmscontentcontainers-deletecontentContainer", "contentcontainer/deletecontentcontainer", new { controller = "contentcontainer", action = "deletecontentcontainer" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            //renamed the widgetkey to containerKey as this is overall URL change required
            config.Routes.MapHttpRoute("cmscontentcontainers-iscontainerkeyexists", "contentcontainer/iscontainerkeyexists/{containerKey}", new { controller = "contentcontainer", action = "iscontainerkeyexists" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("cmscontentcontainers-iscontainertemplateexists", "contentcontainer/associatecontainertemplate/{variantId}/{containerTemplateId}", new { controller = "contentcontainer", action = "associatecontainertemplate" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("cmscontentcontainers-deletebykey", "contentcontainer/deletecontentcontainerbycontainerkey/{containerKey}", new { controller = "contentcontainer", action = "DeleteContentContainerByContainerKey" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cmscontentcontainers-publish", "contentcontainer/publishcontentcontainer/{containerKey}/{targetPublishState}", new { controller = "contentcontainer", action = "publishcontentcontainer" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cmscontentcontainers-getassociatedvariantlist", "contentcontainer/getassociatedvariantlist", new { controller = "contentcontainer", action = "getassociatedvariantlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //added isActive parameter to pass the IsActive status to API for saving in database.
            config.Routes.MapHttpRoute("cmscontentcontainers-savevariantdata", "contentcontainer/savevariantdata/{localeId}/{templateId}/{variantId}/{isActive}", new { controller = "contentcontainer", action = "savevariantdata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("cmscontentcontainers-getdata", "contentcontainer/getcontentcontainerdata", new { controller = "contentcontainer", action = "getcontentcontainerdata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("cmscontentcontainers-getvariantlocaledata", "contentcontainer/getvariantlocaledata/{variantId}", new { controller = "contentcontainer", action = "getvariantlocaledata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("cmscontentcontainers-activatedeactivatevariant", "contentcontainer/activatedeactivatevariant/{isActivate}", new { controller = "contentcontainer", action = "activatedeactivatevariant" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cmscontentcontainers-publishcontentcontainer", "contentcontainer/publishcontentcontainer/{containerKey}/{targetPublishState}", new { controller = "contentcontainer", action = "publishcontentcontainer" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cmscontentcontainers-publishcontentcontainervariant", "contentcontainer/publishcontainervariant/{containerKey}/{containerprofilevariantid}/{targetPublishState}", new { controller = "contentcontainer", action = "publishcontainervariant" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //CMS Container Templates
            config.Routes.MapHttpRoute("cmscontainertemplates-list", "containertemplate/list", new { controller = "containertemplate", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("cmscontainertemplates-create", "containertemplate/create", new { controller = "containertemplate", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cmscontainertemplates-get", "containertemplate/get/{templateCode}", new { controller = "containertemplate", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("cmscontainertemplates-update", "containertemplate/update", new { controller = "containertemplate", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("cmscontainertemplates-delete", "containertemplate/delete", new { controller = "containertemplate", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cmscontainertemplates-iscontainertemplateexists", "containertemplate/iscontainertemplateexist/{templateCode}", new { controller = "containertemplate", action = "iscontainertemplateexist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("cmscontainertemplates-deletebycode", "containertemplate/deletetemplatebyCode/{templateCode}", new { controller = "containertemplate", action = "DeleteTemplateByCode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });


            //Recommendation engine
            config.Routes.MapHttpRoute("recommendations-getrecommendationsetting", "recommendation/getrecommendationsetting/{portalId}/{touchPointName}", new { controller = "recommendation", action = "getrecommendationsetting" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalId = @"^\d+$" });
            config.Routes.MapHttpRoute("recommendations-saverecommendationsetting", "recommendation/saverecommendationsetting", new { controller = "recommendation", action = "saverecommendationsetting" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("recommendations-getrecommendation", "recommendation/getrecommendation", new { controller = "Recommendation", action = "getrecommendation" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("recommendations-generaterecommendationdata", "recommendation/generaterecommendationdata", new { controller = "Recommendation", action = "generaterecommendationdata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //CSS Routes
            config.Routes.MapHttpRoute("css-list", "css/list", new { controller = "css", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("css-get", "css/{cssid}", new { controller = "css", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), cssid = @"^\d+$" });
            config.Routes.MapHttpRoute("css-create", "css", new { controller = "css", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("css-update", "css/update", new { controller = "css", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("css-delete", "css/delete", new { controller = "css", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("css-getcsslistbythemeid", "css/getcsslistbythemeid/{cmsThemeId}", new { controller = "css", action = "getcsslistbythemeid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Country
            config.Routes.MapHttpRoute("country-list", "country/list", new { controller = "country", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("country-get", "country", new { controller = "country", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("country-update", "country/update", new { controller = "country", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //CMS-Customer Review Routes
            config.Routes.MapHttpRoute("customerreview-list", "customerreviewlist/{localeId}", new { controller = "customerreview", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("customerreview-get", "customerreview/getcustomerreview/{customerReviewId}/{localeId}", new { controller = "customerreview", action = "getcustomerreview" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("customerreview-update", "customerreview/updatecustomerreview", new { controller = "customerreview", action = "updatecustomerreview" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("customerreview-delete", "customerreview/deletecustomerreview", new { controller = "customerreview", action = "deletecustomerreview" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("customerreview-bulkstatuschange", "customerreview/bulkstatuschange/{statusId}", new { controller = "customerreview", action = "bulkstatuschange" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("customerreview-create", "customerreview/create", new { controller = "customerreview", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Customer
            config.Routes.MapHttpRoute("customer-getunassociatedprofilelist", "customer/getunassociatedprofilelist", new { controller = "customer", action = "getunassociatedprofilelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("customer-getassociatedprofilelist", "customer/getassociatedprofilelist", new { controller = "customer", action = "getassociatedprofilelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("customer-unassociateprofiles", "customer/unassociateprofiles/{userId}", new { controller = "customer", action = "unassociateprofiles" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("customer-associateprofiles", "customer/associateprofiles", new { controller = "customer", action = "associateprofiles" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("customer-setdefaultprofile", "customer/setdefaultprofile", new { controller = "customer", action = "setdefaultprofile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("customer-getreferralcommissiontypelist", "customer/getreferralcommissiontypelist", new { controller = "customer", action = "getreferralcommissiontypelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("customer-getcustomeraffiliate", "customer/getcustomeraffiliate/{userId}", new { controller = "customer", action = "getcustomeraffiliate" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("customer-updatecustomeraffiliate", "customer/updatecustomeraffiliate", new { controller = "customer", action = "updatecustomeraffiliate" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("customer-getreferralcommissionlist", "customer/getreferralcommissionlist", new { controller = "customer", action = "getreferralcommissionlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("customer-getcustomerportalprofilelist", "customer/getcustomerportalprofilelist", new { controller = "customer", action = "getcustomerportalprofilelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Customer Address
            config.Routes.MapHttpRoute("customer-addresslist", "customer/addresslist", new { controller = "customer", action = "addresslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("customer-createcustomeraddress", "customer/createcustomeraddress", new { controller = "customer", action = "createcustomeraddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("customer-getcustomeraddress", "customer/getcustomeraddress", new { controller = "customer", action = "getcustomeraddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("customer-updatecustomeraddress", "customer/updatecustomeraddress", new { controller = "customer", action = "updatecustomeraddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("customer-deletecustomeraddress", "customer/deletecustomeraddress", new { controller = "customer", action = "deletecustomeraddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("customer-getsearchlocation", "customer/getsearchlocation/{portalId}/{searchTerm}", new { controller = "customer", action = "getsearchlocation" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Customer Price
            config.Routes.MapHttpRoute("customer-associatepricelist", "customer/associatepricelist", new { controller = "customer", action = "associatepricelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("customer-unassociatepricelist", "customer/unassociatepricelist", new { controller = "customer", action = "unassociatepricelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("customer-getassociatedpricelistprecedence", "customer/getassociatedpricelistprecedence", new { controller = "customer", action = "getassociatedpricelistprecedence" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("customer-updateassociatedpricelistprecedence", "customer/updateassociatedpricelistprecedence", new { controller = "customer", action = "updateassociatedpricelistprecedence" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //Dashboard routes
            config.Routes.MapHttpRoute("dashboard-gettopbrandslist", "dashboard/getdashboardtopbrandslist", new { controller = "dashboard", action = "getdashboardtopbrandslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("dashboard-gettopcategorieslist", "dashboard/getdashboardtopcategorieslist", new { controller = "dashboard", action = "getdashboardtopcategorieslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("dashboard-gettopproductslist", "dashboard/getdashboardtopproductlist", new { controller = "dashboard", action = "getdashboardtopproductslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("dashboard-gettopsearcheslist", "dashboard/getdashboardtopsearcheslist", new { controller = "dashboard", action = "getdashboardtopsearcheslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("dashboard-getdashboardsalesdetails", "dashboard/getdashboardsalesdetails", new { controller = "dashboard", action = "getdashboardsalesdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("dashboard-getdashboardsalescountdetails", "dashboard/getdashboardsalescountdetails", new { controller = "dashboard", action = "getdashboardsalescountdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("dashboard-getlowinventoryproductcount", "dashboard/getdashboardlowinventoryproductcount", new { controller = "dashboard", action = "getdashboardlowinventoryproductcount" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute("dashboard-getdashboardquotes", "dashboard/getdashboardquotes", new { controller = "dashboard", action = "getdashboardquotes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("dashboard-getdashboardorders", "dashboard/getdashboardorders", new { controller = "dashboard", action = "getdashboardorders" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("dashboard-getdashboardreturns", "dashboard/getdashboardreturns", new { controller = "dashboard", action = "getdashboardreturns" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("dashboard-getdashboardtopaccounts", "dashboard/getdashboardtopaccounts", new { controller = "dashboard", action = "getdashboardtopaccounts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("dashboard-getdashboarddetails", "dashboard/getdashboarddetails", new { controller = "dashboard", action = "getdashboarddetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("dashboard-getdashboardsaledetails", "dashboard/getdashboardsaledetails", new { controller = "dashboard", action = "getdashboardsaledetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("dashboard-getaccountandstorelist", "dashboard/getaccountandstorelist", new { controller = "dashboard", action = "getaccountandstorelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            // Domain routes
            config.Routes.MapHttpRoute("domain-get", "domain/{domainId}", new { controller = "domain", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), domainId = @"^\d+$" });
            config.Routes.MapHttpRoute("domain-list", "domain", new { controller = "domain", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("domain-create", "domain", new { controller = "domain", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("domain-update", "domain", new { controller = "domain", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("domain-delete", "domain/delete", new { controller = "domain", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("domain-enabledisabledomain", "domain/enabledisabledomain", new { controller = "domain", action = "enabledisabledomain" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Ecommerce Catalog Routes
            config.Routes.MapHttpRoute("ecommercecatalog-getpublishcatalog", "ecommercecatalog/getpublishcataloglist", new { controller = "ecommercecatalog", action = "getpublishcataloglist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("ecommercecatalog-getportalcatalog", "ecommercecatalog/getportalcatalog/{portalcatalogid}", new { controller = "ecommercecatalog", action = "getportalcatalog" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("ecommercecatalog-getassociatedportalcatalogbyportalid", "ecommercecatalog/getassociatedportalcatalogbyportalid/{portalId}", new { controller = "ecommercecatalog", action = "getassociatedportalcatalogbyportalid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("ecommercecatalog-getassociatedportalcatalog", "ecommercecatalog/getassociatedportalcatalog", new { controller = "ecommercecatalog", action = "getassociatedportalcatalog" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("ecommercecatalog-updateportalcatalog", "ecommercecatalog/updateportalcatalog", new { controller = "ecommercecatalog", action = "updateportalcatalog" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("ecommercecatalog-getcategorytree", "ecommercecatalog/getcatalogtree/{catalogId}/{categoryId}", new { controller = "ecommercecatalog", action = "getcatalogtree" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("ecommercecatalog-getpublishcatalogdetails", "ecommercecatalog/getpublishcatalogdetails/{publishCatalogId}", new { controller = "ecommercecatalog", action = "getpublishcatalogdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("ecommercecatalog-getpublishcategorydetails", "ecommercecatalog/getpublishcategorydetails/{publishCategoryId}", new { controller = "ecommercecatalog", action = "getpublishcategorydetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("ecommercecatalog-getpublishproductdetails", "ecommercecatalog/getpublishproductdetails/{publishProductId}/{portalId}", new { controller = "ecommercecatalog", action = "getpublishproductdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("ecommercecatalog-getcatalogbycatalogcode", "catalog/getcatalogbycatalogcode/{catalogcode}/", new { controller = "Catalog", action = "getcatalogbycatalogcode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("ecommercecatalog-deletecatalogs", "catalog/deletecatalogbycode", new { controller = "catalog", action = "deletecatalogbycode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //GlobalSettings
            config.Routes.MapHttpRoute("generalsetting-list", "generalsetting/list", new { controller = "generalsetting", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("generalsetting-update", "generalsetting/update", new { controller = "generalsetting", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("generalsetting-cachedata", "generalsetting/getcachedata", new { controller = "generalsetting", action = "getcachedata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("generalsetting-createupdatecache", "generalsetting/createupdatecache", new { controller = "generalsetting", action = "createupdatecache" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("generalsetting-refreshcache", "generalsetting/refreshcache", new { controller = "generalsetting", action = "refreshcache" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("generalsetting-getconfigurationsettings", "generalsetting/getconfigurationsettings", new { controller = "generalsetting", action = "getconfigurationsettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("generalsetting-updateconfigurationsettings", "generalsetting/updateconfigurationsettings", new { controller = "generalsetting", action = "updateconfigurationsettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //PublishStateMapping
            config.Routes.MapHttpRoute("publishstatemapping-list", "publishstatemapping/list", new { controller = "publishstate", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("publishstatemapping-enabledisable", "publishstatemapping/enabledisable/{isenabled}", new { controller = "publishstate", action = "enabledisablemapping" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //GlobalSettings Route
            config.Routes.MapHttpRoute("defaultglobalconfig-list", "defaultglobalconfig", new { controller = "defaultglobalconfig", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("defaultLoggingconfig-list", "defaultLoggingconfig", new { controller = "defaultglobalconfig", action = "GetDefaultLoggingConfig" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //GiftCard Routes
            config.Routes.MapHttpRoute("giftcard-create", "giftcard/create", new { controller = "giftcard", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("giftcard-update", "giftcard/update", new { controller = "giftcard", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("giftcard-list", "giftcard/list", new { controller = "giftcard", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("giftcard-get", "giftcard/{giftcardId}", new { controller = "giftcard", action = "getgiftCard" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), giftCardId = @"^\d+$" });
            config.Routes.MapHttpRoute("giftcard-delete", "giftcard/delete", new { controller = "giftcard", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("giftcard-getrandomnumber", "giftcard/getrandomgiftcardnumber", new { controller = "giftcard", action = "getrandomgiftcardnumber" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("giftcard-getgiftcardhistorylist", "giftcard/getgiftcardhistorylist", new { controller = "giftcard", action = "getgiftcardhistorylist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("giftcard-activatedeactivatevouchers", "giftcard/activatedeactivatevouchers/{isActive}", new { controller = "giftcard", action = "activatedeactivatevouchers" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("giftcard-sendvoucherexpirationreminderemail", "giftcard/sendvoucherexpirationreminderemail", new { controller = "giftcard", action = "sendvoucherexpirationreminderemail" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("giftcard-getvoucher", "giftcard/getvoucher/{voucherCode}", new { controller = "giftcard", action = "GetVoucher" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get)});
            config.Routes.MapHttpRoute("giftcard-deletevoucher", "giftcard/deletevoucher", new { controller = "giftcard", action = "deletevoucher" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("giftcard-activatedeactivatevouchersbyvouchercode", "giftcard/activatedeactivatevouchersbyvouchercode/{isActive}", new { controller = "giftcard", action = "activatedeactivatevouchersbyvouchercode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Export route
            config.Routes.MapHttpRoute("export-getexportfilepath", "export/getexportfilepath/{tableName}", new { controller = "export", action = "getexportfilepath" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("export-list", "export/getexportlogs", new { controller = "export", action = "getexportlogs" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("Export-deleteexpiredexportfiles", "export/deleteexpiredexportfiles", new { controller = "export", action = "deleteexpiredexportfiles" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });
            config.Routes.MapHttpRoute("export-delete", "export/delete", new { controller = "export", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Import routes
            config.Routes.MapHttpRoute("import-data", "import/importdata", new { controller = "import", action = "importdata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("import-importlist", "import/getimporttypelist", new { controller = "import", action = "getimporttypelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("import-getalltemplates", "import/getalltemplates/{importheadid}/{familyid}/{promotionTypeId}", new { controller = "import", action = "getalltemplates" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("import-gettemplatedata", "import/gettemplatedata/{templateid}/{importheadid}/{familyid}/{promotionTypeId}", new { controller = "import", action = "gettemplatedata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("import-downloadtemplate", "import/downloadtemplate/{importheadid}/{familyid}/{promotionTypeId}", new { controller = "import", action = "downloadtemplate" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), importHeadId = @"^\d+$" });
            config.Routes.MapHttpRoute("import-list", "import/getimportlogs", new { controller = "import", action = "getimportlogs" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("import-detailslist", "import/getimportlogdetails/{importprocesslogid}", new { controller = "import", action = "getimportlogdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("import-getstatus", "import/getlogstatus/{importprocesslogid}", new { controller = "import", action = "getlogstatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("import-delete", "import/delete", new { controller = "import", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("import-getallfamilies", "import/getfamilies/{iscategory}", new { controller = "import", action = "getfamilies" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("import-updatemappings", "import/updatemappings", new { controller = "import", action = "updatemappings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("import-checkimportprocess", "import/checkimportprocess", new { controller = "import", action = "checkimportstatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("import-getdefaulttemplate", "import/getdefaulttemplate/{templatename}", new { controller = "import", action = "getdefaulttemplate" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("import-customimporttemplatelist", "import/getcustomimporttemplatelist", new { controller = "import", action = "getcustomimporttemplatelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("import-deletetemplate", "import/deleteimporttemplate", new { controller = "import", action = "deleteimporttemplate" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("import-importprocessList", "import/getimportlogdetailslist/{fileType}/{importProcessLogId}", new { controller = "import", action = "getimportlogdetailslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("import-exportProcess", "import/checkexportprocess", new { controller = "import", action = "checkexportprocess" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //State
            config.Routes.MapHttpRoute("state-list", "state/list", new { controller = "state", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //City
            config.Routes.MapHttpRoute("city-list", "city/list", new { controller = "city", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Currency
            config.Routes.MapHttpRoute("currency-list", "currency/list", new { controller = "currency", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("currency-get", "currency", new { controller = "currency", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("currency-update", "currency/update", new { controller = "currency", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("currency-getculturelist", "currency/getculturelist", new { controller = "currency", action = "getculturelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("currency-getculturecode", "currency/getculturecode", new { controller = "currency", action = "getculturecode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("currency-getcurrencyculturelist", "currency/getcurrencyculturelist", new { controller = "currency", action = "getcurrencyculturelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Weight Unit
            config.Routes.MapHttpRoute("weightunit-list", "weightunit/list", new { controller = "weightunit", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("weightunit-update", "weightunit/update", new { controller = "weightunit", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //Locale
            config.Routes.MapHttpRoute("locale-list", "locale/list", new { controller = "locale", action = "getlocalelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("locale-get", "locale", new { controller = "locale", action = "getlocale" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("locale-update", "locale/update", new { controller = "locale", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //Media configuration routes
            config.Routes.MapHttpRoute("media-server-list", "getmediaserver/list", new { controller = "mediaconfiguration", action = "getmediaserverlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("media-configuration-update", "mediaconfiguration/update", new { controller = "mediaconfiguration", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("media-configuration-get", "mediaconfiguration", new { controller = "mediaconfiguration", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("media-configuration-create", "mediaconfiguration/create", new { controller = "mediaconfiguration", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("media-default-configuration", "defaultmediaconfiguration", new { controller = "mediaconfiguration", action = "GetDefaultMediaConfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("media-Sync-Functionality", "mediaconfiguration/SyncMedia/{folderName}", new { controller = "mediaconfiguration", action = "SyncMedia" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("media-generateimages", "mediaconfiguration/generateimages", new { controller = "mediaconfiguration", action = "generateimages" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("media-getglobalmediadisplaysetting", "mediaconfiguration/getglobalmediadisplaysetting", new { controller = "mediaconfiguration", action = "getglobalmediadisplaysetting" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("media-getmediacount", "mediaconfiguration/getmediacount", new { controller = "mediaconfiguration", action = "getmediacount" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("media-getmedialist", "mediaconfiguration/getmedialistforgenerateimages", new { controller = "mediaconfiguration", action = "getmedialistforgenerateimages" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Media Manager Config
            config.Routes.MapHttpRoute("media-gettree", "media/gettree", new { controller = "media", action = "gettree" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("media-getmedias", "media/list", new { controller = "media", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("media-savemedia", "media/create", new { controller = "media", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("media-addfolder", "media/addfolder", new { controller = "media", action = "addfolder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("media-renamefolder", "media/renamefolder", new { controller = "media", action = "renamefolder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("media-getmedia", "media/getmedia/{mediaId}", new { controller = "media", action = "getmediabyid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("media-movefolder", "media/movefolder", new { controller = "media", action = "movefolder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("media-movemediatofolder", "media/move", new { controller = "media", action = "movemedia" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("media-getmediaattributevalues", "media/getattributevalues/{mediaId}", new { controller = "media", action = "getattributevaluesbyid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("media-deletemedia", "media/delete", new { controller = "media", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("media-updatemediaattributevalue", "media/attributevalue/update", new { controller = "media", action = "updateattributevalue" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("media-getattributefamilyidbyname", "media/getattributefamilyidbyname/{extension}", new { controller = "media", action = "getattributefamilyidbyname" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("media-mediaattributevalue", "media/attributevalues/create", new { controller = "media", action = "createattributevalue" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("media-update", "media/update", new { controller = "media", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("media-share", "media/share", new { controller = "media", action = "sharefolder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("media-getallowedextensions", "media/getallowedextensions", new { controller = "media", action = "getallowedextensions" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("media-generateimageonedit", "media/generateimageonedit", new { controller = "media", action = "generateimageonedit" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("media-getmediadetailsbyid", "media/getmediadetailsbyid/{mediaId}", new { controller = "media", action = "getmediadetailsbyid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("media-getmediadetailsbyguid", "media/getmediadetailsbyguid/{mediaGuid}", new { controller = "media", action = "getmediadetailsbyguid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Menu routes
            config.Routes.MapHttpRoute("menu-list", "menu/list", new { controller = "menu", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("menu-create", "menu", new { controller = "menu", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("menubyparentid-list", "menubyparent/{preSelectedMenuIds}", new { controller = "menu", action = "getmenusbyparentmenuid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("menu-delete", "deletemenu", new { controller = "menu", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("menu-update", "menu/{menuId}", new { controller = "menu", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("menu-get", "menus/{menuId}", new { controller = "menu", action = "getmenu" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("menu-getunselectedmenus", "getunselectedmenus", new { controller = "menu", action = "getunselectedmenus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("menu-getmenuactionspermissionlist", "menu/getmenuactionspermissionlist/{menuId}", new { controller = "menu", action = "getmenuactionspermissionlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("menu-updateactionpermissions", "menu/updateactionpermissions", new { controller = "menu", action = "updateactionpermissions" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Navigation routes
            config.Routes.MapHttpRoute("navigation-get", "navigation/getnavigationdetails", new { controller = "navigation", action = "getnavigationdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Order Routes
            config.Routes.MapHttpRoute("order-getorderreceiptbyorderid", "orders/getorderreceiptbyorderid/{orderId}/", new { controller = "order", action = "getorderreceiptbyorderid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-list", "orders/list", new { controller = "order", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-failedordertransactionlist", "orders/failedordertransactionlist", new { controller = "order", action = "failedordertransactionlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-getgrouporderlist", "orders/getgrouporderlist", new { controller = "order", action = "getgrouporderlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-create", "orders/create", new { controller = "order", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("order-update", "orders/update", new { controller = "order", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("order-getorderdetailsforpayment", "orders/getorderdetailsforpayment", new { controller = "order", action = "getorderdetailsforpayment" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("order-getpaymentstatelist", "orders/getpaymentstatelist", new { controller = "order", action = "GetPaymentStateList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-get", "orders/{orderId}", new { controller = "order", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-getbyordernumber", "orders/getbyordernumber/{orderNumber}", new { controller = "order", action = "getbyordernumber" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-updateorderpaymentstatus", "orders/updateorderpaymentstatus/{orderId}/{paymentStatus}", new { controller = "order", action = "updateorderpaymentstatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-updatetrackingnumber", "orders/updatetrackingnumber/{orderId}/{trackingNumber}", new { controller = "order", action = "updatetrackingnumber" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-updatetrackingnumberPut", "orders/updatetrackingnumber/{orderId}/{trackingNumber}", new { controller = "order", action = "updatetrackingnumber" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("order-updatetrackingbyordernumber", "orders/updatetrackingbyordernumber/{orderNumber}/{trackingNumber}", new { controller = "order", action = "updatetrackingbyordernumber" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("order-createnewcustomer", "orders/createnewcustomer", new { controller = "order", action = "createnewcustomer" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("order-getorderdetailsforinvoice", "orders/getorderdetailsforinvoice", new { controller = "order", action = "getorderdetailsforinvoice" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("order-addrefundpaymentdetails", "orders/addrefundpaymentdetails", new { controller = "order", action = "AddRefundPaymentDetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("order-getorderlineitemswithrefund", "orders/getorderlineitemswithrefund/{orderDetailsId}", new { controller = "order", action = "getorderlineitemswithrefund" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-resendorderconfirmationemail", "orders/resendorderconfirmationemail/{orderId}", new { controller = "order", action = "ResendOrderConfirmationEmail" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-resendorderconfirmationemailForCartLineItem", "orders/ResendOrderEmailForCartLineItem/{orderId}/{cartItemId}", new { controller = "order", action = "ResendOrderEmailForCartLineItem" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-updateorderstatus", "orders/updateorderstatus", new { controller = "order", action = "updateorderstatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("order-getorderbyorderlineitemid", "orders/getorderbyorderlineitemid/{orderLineItemId}", new { controller = "order", action = "getorderbyorderlineitemid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-addordernote", "orders/addordernote", new { controller = "order", action = "addordernote" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("order-getordernoteslist", "orders/getordernoteslist/{omsOrderId}/{omsQuoteId}", new { controller = "order", action = "OrderNoteList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-checkinventoryandminmaxquantity", "orders/checkinventoryandminmaxquantity", new { controller = "order", action = "checkinventoryandminmaxquantity" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("order-createorderhistory", "orders/createorderhistory", new { controller = "order", action = "CreateOrderHistory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("order-getorderstate", "orders/getorderstatevaluebyid/{omsOrderStateId}", new { controller = "order", action = "GetOrderStateValueById" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("SendPOEmail", "order/sendpoemail", new { controller = "order", action = "sendpoemail" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("order-sendreturnedorderemail", "orders/sendreturnedorderemail/{orderId}", new { controller = "order", action = "sendreturnedorderemail" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-updateorderlineitems", "order/updateorderlineitems", new { controller = "order", action = "updateorderlineitems" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("order-updateordertransactionid", "orders/updateordertransactionid/{orderId}/{transactionId}", new { controller = "order", action = "updateordertransactionid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-updateorderdetailsbyordernumber", "orders/updateorderdetailsbyordernumber", new { controller = "order", action = "updateorderdetailsbyordernumber" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("orders-converttoorder", "orders/converttoorder", new { controller = "order", action = "converttoorder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("order-updateAddress", "orders/UpdateOrderAddress", new { controller = "order", action = "UpdateOrderAddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            //OrderState Routes
            config.Routes.MapHttpRoute("orderstate-list", "orderstate/list", new { controller = "orderstate", action = "orderstatelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-reordersinglelineitemorder", "orders/reordersinglelineitemorder/{omsOrderLineItemsId}/{portalId}/{userId}", new { controller = "order", action = "ReorderSinglelineItemOrder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-reordercompleteorder", "orders/reordercompleteorder/{orderId}/{portalId}/{userId}", new { controller = "order", action = "reordercompleteorder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("order-getaddresslistwithshipment", "orders/getaddresslistwithshipment/{orderId}/{userId}", new { controller = "order", action = "getaddresslistwithshipment" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //Payment
            config.Routes.MapHttpRoute("Payment-create", "payment/create", new { controller = "payment", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("Payment-update", "payment/update", new { controller = "payment", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("Payment-list", "payment/list", new { controller = "payment", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("Payment-calltopaymentapi", "payment/calltopaymentapi/{paymentTypeCode}", new { controller = "payment", action = "calltopaymentapi" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("Payment-delete", "payment/delete", new { controller = "payment", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("Payment-isactivepaymentsettingpresent", "payment/isactivepaymentsettingpresent", new { controller = "payment", action = "isactivepaymentsettingpresent" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("Payment-isactivepaymentsettingpresentbypaymentcode", "payment/isactivepaymentsettingpresentbypaymentcode", new { controller = "payment", action = "isactivepaymentsettingpresentbypaymentcode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("Payment-associatepaymentsettings", "payment/associatepaymentsettings", new { controller = "payment", action = "associatepaymentsettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("Payment-associatepaymentsettingsforinvoice", "payment/associatepaymentsettingsforinvoice", new { controller = "payment", action = "associatepaymentsettingsforinvoice" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute("Payment-removeassociatedpaymentsettings", "payment/removeassociatedpaymentsettings", new { controller = "payment", action = "removeassociatedpaymentsettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("Payment-getcapturedpaymentdetails", "payment/paymentcaptured/{omsOrderId}", new { controller = "payment", action = "getcapturedpaymentdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("Payment-updateportalpaymentsettings", "payment/updateportalpaymentsettings", new { controller = "payment", action = "updateportalpaymentsettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("Payment-get", "payment/{paymentsettingId}", new { controller = "payment", action = "GetPaymentSetting" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("Payment-getbyportal", "payment/{paymentsettingId}/{portalId}", new { controller = "payment", action = "GetPaymentSettingByPortalId" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("Payment-updateprofilepaymentsettings", "payment/updateprofilepaymentsettings", new { controller = "payment", action = "updateprofilepaymentsettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("Payment-IsPaymentDisplayNameExists", "payment/ispaymentdisplaynameexists", new { controller = "payment", action = "IsPaymentDisplayNameExists" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("payment-GetPaymentSettingByUserDetails", "payment/getpaymentsettingbyuserdetails", new { controller = "payment", action = "getpaymentsettingbyuserdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            //Payment Token
            config.Routes.MapHttpRoute("payment-DeletePaymentAuthToken", "payment/deletepaymenttoken", new { controller = "payment", action = "deletepaymenttoken" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });
            //PIM

            //PIM Catalog routes
            config.Routes.MapHttpRoute("catalog-list", "catalogs", new { controller = "catalog", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("catalog-get", "catalog/{pimCatalogId}", new { controller = "catalog", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("catalog-create", "catalog", new { controller = "catalog", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("catalog-update", "catalog/update", new { controller = "catalog", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("catalog-delete", "catalog/delete", new { controller = "catalog", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("catalog-copy", "catalog/copycatalog", new { controller = "catalog", action = "copycatalog" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("catalog-getcategorytree", "catalog/getcategorytree", new { controller = "catalog", action = "getcategorytree" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("catalog-getAssociatedCatalogTree", "catalog/getAssociatedCatalogTree/{pimProductId}", new { controller = "catalog", action = "getAssociatedCatalogTree" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("catalog-associatecategory", "catalog/associatecategory", new { controller = "catalog", action = "associatecategory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("catalog-getassociatedcategorylist", "catalogs/getassociatedcategorylist", new { controller = "catalog", action = "getassociatedcategorylist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("catalog-unassociatecategory", "catalog/unassociatecategory", new { controller = "catalog", action = "unassociatecategory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("catalog-getcategoryassociatedproducts", "catalogs/getcategoryassociatedproducts", new { controller = "catalog", action = "getcategoryassociatedproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("catalog-publish", "catalog/publish/{pimCatalogId}/{revisionType}", new { controller = "catalog", action = "publish" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("catalog-publishwithproductstatus", "catalog/publish/{pimCatalogId}/{revisionType}/{isDraftProductsOnly}", new { controller = "catalog", action = "publish" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("catalog-publishcatalogcategoryproducts", "catalog/publishcatalogcategoryproducts/{pimCatalogId}/{pimCategoryHierarchyId}/{revisionType}", new { controller = "catalog", action = "publishcatalogcategoryproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("catalog-getcatalogpublishstatus", "catalogs/getcatalogpublishstatus", new { controller = "catalog", action = "getcatalogpublishstatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("catalog-deletepublishcatalog", "catalog/deletepublishcatalog/{publishCatalogId}", new { controller = "catalog", action = "deletepublishcatalog" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });
            config.Routes.MapHttpRoute("catalog-updateassociatecategorydetails", "catalog/updateassociatecategorydetails", new { controller = "catalog", action = "updateassociatecategorydetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("catalog-getassociatecategorydetails", "catalog/getassociatecategorydetails", new { controller = "catalog", action = "getassociatecategorydetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("catalog-movecategory", "catalog/movecategory", new { controller = "catalog", action = "movecategory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("catalog-unassociateproduct", "catalog/unassociateproduct", new { controller = "catalog", action = "unassociateproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("catalog-updatecatalogcategoryproduct", "catalog/updatecatalogcategoryproduct", new { controller = "catalog", action = "updatecatalogcategoryproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("catalog-updatecateoryproductdisplayorder", "catalog/updatecateoryproductdisplayorder/{page}", new { controller = "catalog", action = "updatecateoryproductdisplayorder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portal-getportallistbycatalogid", "portal/getportallistbycatalogId/{CatalogId}", new { controller = "portal", action = "getportallistbycatalogId" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), CatalogId = @"^\d+$" });
            config.Routes.MapHttpRoute("catalog-associateproductstocatalogcategory", "catalog/associateproductstocatalogcategory", new { controller = "catalog", action = "associateproductstocatalogcategory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("catalog-unassociateproductsfromcatalogcategory", "catalog/unassociateproductsfromcatalogcategory", new { controller = "catalog", action = "unassociateproductsfromcatalogcategory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("catalog-associatecategorytocatalog", "catalog/associatecategorytocatalog", new { controller = "catalog", action = "associatecategorytocatalog" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("catalog-unassociatecategoryfromcatalog", "catalog/unassociatecategoryfromcatalog", new { controller = "catalog", action = "unassociatecategoryfromcatalog" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //PIM Category routes
            config.Routes.MapHttpRoute("category-list", "category/list", new { controller = "category", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("category-get", "category/getcategory/{categoryId}/{familyId}/{localeId}", new { controller = "category", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("category-create", "category/create", new { controller = "category", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("category-update", "category/update", new { controller = "category", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("category-delete", "category/delete", new { controller = "category", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("category-deletecategoryproduct", "category/deletecategoryproduct", new { controller = "category", action = "deletecategoryproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("category-associatecategoryproduct", "category/associatecategoryproduct", new { controller = "category", action = "associatecategoryproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("category-associatecategoriesToProduct", "category/associatecategoriesToProduct", new { controller = "category", action = "associatecategoriesToProduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("category-getassociatedunassociatedcategoryproducts", "category/getassociatedunassociatedcategoryproducts/{categoryId}/{associatedProducts}", new { controller = "category", action = "getassociatedunassociatedcategoryproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("category-getassociatedcategoryproducts", "category/getassociatedcategoryproducts/{categoryId}/{associatedProducts}", new { controller = "category", action = "getassociatedcategoryproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("category-publish", "category/publish", new { controller = "category", action = "publish" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("category-getassociatedcategoriestoproduct", "category/getassociatedcategoriestoproducts/{productId}/{associatedProducts}", new { controller = "category", action = "getassociatedcategoriestoproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("category-deleteassociatedcategoriestoproduct", "category/deleteassociatedcategoriestoproduct", new { controller = "category", action = "deleteassociatedcategoriestoproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("category-updatecategoryproductdetail", "category/updatecategoryproductdetail", new { controller = "category", action = "updatecategoryproductdetail" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //PIM Attribute Family Routes
            config.Routes.MapHttpRoute("pimattributefamily-list", "pimattributefamily/list", new { controller = "pimattributefamily", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("pimattributefamily-create", "pimattributefamily/create", new { controller = "pimattributefamily", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("pimattributefamily-getassignedattributegroups", "pimattributefamily/getassignedattributegroups", new { controller = "pimattributefamily", action = "getassignedattributegroups" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("pimattributefamily-associateattributegroup", "pimattributefamily/associateattributegroup", new { controller = "pimattributefamily", action = "associateattributegroup" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("pimattributefamily-unassociateattributegroup", "pimattributefamily/unassociateattributegroup/{attributeFamilyId}/{attributeGroupId}/{isCategory}", new { controller = "pimattributefamily", action = "unassociateattributegroup" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });
            config.Routes.MapHttpRoute("pimattributefamily-delete", "pimattributefamily/delete", new { controller = "pimattributefamily", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("pimattributefamily-get", "pimattributefamily/getattributefamily/{attributeFamilyId}", new { controller = "pimattributefamily", action = "getattributefamily" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("pimattributefamily-getunassignedattributegroups", "pimattributefamily/getunassignedattributegroups", new { controller = "pimattributefamily", action = "getunassignedattributegroups" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("pimattributefamily-getfamilylocales", "pimattributefamily/getfamilylocale/{attributeFamilyId}", new { controller = "pimattributefamily", action = "getfamilylocale" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("pimattributefamily-savelocales", "pimattributefamily/savelocales", new { controller = "pimattributefamily", action = "savelocales" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("pimattributefamily-getattributesbygroupids", "pimattributefamily/getattributesbygroupids", new { controller = "pimattributefamily", action = "getattributesbygroupids" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("pimattributefamily-updateattributegroupdisplayorder", "pimattributefamily/updateattributegroupdisplayorder", new { controller = "pimattributefamily", action = "updateattributegroupdisplayorder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("pimattributefamily-getassignedattributes", "pimattributefamily/getassignedattributes", new { controller = "pimattributefamily", action = "getassignedattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("pimattributefamily-unassignattributes", "pimattributefamily/unassignattributes", new { controller = "pimattributefamily", action = "unassignattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("pimattributefamily-getunassignedattributes", "pimattributefamily/getunassignedattributes", new { controller = "pimattributefamily", action = "getunassignedattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("pimattributefamily-assignattributes", "pimattributefamily/assignattributes", new { controller = "pimattributefamily", action = "assignattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //PIM Attribute Group Routes
            config.Routes.MapHttpRoute("pimattributegroup-list", "pimattributegroup/list", new { controller = "pimattributegroup", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("pimattributegroup-get", "pimattributegroup/{id}", new { controller = "pimattributegroup", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"^\d+$" });
            config.Routes.MapHttpRoute("pimattributegroup-create", "pimattributegroup/create", new { controller = "pimattributegroup", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("pimattributegroup-update", "pimattributegroup/update", new { controller = "pimattributegroup", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("pimattributegroup-delete", "pimattributegroup/delete", new { controller = "pimattributegroup", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("pimattributegroup-assignedattributes", "pimattributegroup/assignedattributes", new { controller = "pimattributegroup", action = "assignedattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("pimattributegroup-unassignedattributes", "pimattributegroup/unassignedattributes", new { controller = "pimattributegroup", action = "unassignedattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("pimattributegroup-attributegrouplocales", "pimattributegroup/attributegrouplocales/{attributeGroupId}", new { controller = "pimattributegroup", action = "getattributegrouplocale" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("pimattributegroup-saveattributegrouplocales", "pimattributegroup/saveattributegrouplocales", new { controller = "pimattributegroup", action = "saveattributegrouplocales" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("pimattributegroup-associateattributes", "pimattributegroup/associateattributes", new { controller = "pimattributegroup", action = "associateattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("pimattributegroup-removeassociatedattributes", "pimattributegroup/removeassociatedattributes", new { controller = "pimattributegroup", action = "removeassociatedattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("pimattributegroup-updateattributedisplayorder", "pimattributegroup/updateattributedisplayorder", new { controller = "pimattributegroup", action = "updateattributedisplayorder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //PIM Product routes
            config.Routes.MapHttpRoute("product-list", "product/list", new { controller = "products", action = "getproductlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("product-listbyproductid", "products/{productIds}", new { controller = "products", action = "getproductlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("product-get", "products/getproduct", new { controller = "products", action = "getproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("product-create", "products", new { controller = "products", action = "createproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("product-delete", "products/delete", new { controller = "products", action = "deleteproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("product-activatedeactivateproducts", "products/activatedeactivateproducts", new { controller = "products", action = "activatedeactivateproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("product-updateproductattributevalue", "products/updateproductattributevalue", new { controller = "products", action = "updateproductattributevalue" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("product-getbrandproductlist", "product/getbrandproductlist", new { controller = "products", action = "getbrandproductlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("product-updateassignlinkproducts", "products/updateassignlinkproducts", new { controller = "products", action = "updateassignlinkproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            config.Routes.MapHttpRoute("product-getassociatedunassociatedcategoryproducts", "products/getassociatedunassociatedcategoryproducts/{categoryId}/{associatedproduts}", new { controller = "products", action = "GetAssociatedUnAssociatedCategoryProducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("product-getproductfamilydetails", "products/getproductfamilydetails", new { controller = "products", action = "getproductfamilydetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("product-assignedpersonalizedattributelist", "products/assignedpersonalizedattribute/list", new { controller = "products", action = "getassignedpersonalizedattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("product-unassignedpersonalizedattributelist", "products/unassignedpersonalizedattribute/list", new { controller = "products", action = "getunassignedpersonalizedattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("product-associatepersonalizedattributes", "products/assignpersonalizedattributes", new { controller = "products", action = "assignpersonalizedattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("product-unassociatepersonalizedattributes", "products/unassignpersonalizedattributes/{parentProductId}", new { controller = "products", action = "unassignpersonalizedattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), parentProductId = @"^\d+$" });
            config.Routes.MapHttpRoute("product-getconfigureattributes", "products/getconfigureattributes", new { controller = "products", action = "getconfigureattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("product-getassociatedproduct", "products/getassociatedproduct/{PimProductTypeAssociationId}", new { controller = "products", action = "getassociatedproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("product-updateassociatedproducts", "products/updateassociatedproduct", new { controller = "products", action = "updateassociatedproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("product-publish", "products/publish", new { controller = "products", action = "publish" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("product-getproductskulist", "products/getproductskusbyattributecode/{attributeValue}", new { controller = "products", action = "getproductskusbyattributecode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute("linkproductdetails-associatedlinkproductlist", "linkproductdetails/associated/{parentProductId}/{linkAttributeId}", new { controller = "products", action = "getassociatedlinkproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), parentProductId = @"^\d+$" });
            config.Routes.MapHttpRoute("linkproductdetails-unassociatedLinkproductlist", "linkproductdetails/unassociated/{parentProductId}/{linkAttributeId}", new { controller = "products", action = "getunassociatedlinkproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), parentProductId = @"^\d+$" });
            config.Routes.MapHttpRoute("linkproductdetails-delete", "linkproductdetails/delete", new { controller = "products", action = "unassignlinkproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("linkproductdetails-createlinkproductassociation", "linkproductdetails", new { controller = "products", action = "assignlinkproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute("customfield-create", "customfield/create", new { controller = "products", action = "addcustomfield" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("customfield-list", "customfield/list", new { controller = "products", action = "getcustomfieldlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("customfield-get", "customfield/get/{customfieldId}", new { controller = "products", action = "getcustomfield" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("customfield-update", "customfield/update", new { controller = "products", action = "updatecustomfield" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("customfield-delete", "customfield/delete", new { controller = "products", action = "deletecustomfield" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });


            config.Routes.MapHttpRoute("product-associateaddon", "product/associateaddon", new { controller = "products", action = "associateaddon" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("product-deleteassociatedaddons", "product/deleteassociatedaddons", new { controller = "products", action = "deleteassociatedaddons" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("product-getassociatedaddondetails", "product/getassociatedaddondetails{parentProductId}", new { controller = "products", action = "getassociatedaddondetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), parentProductId = @"^\d+$" });
            config.Routes.MapHttpRoute("product-createaddonproductdetail", "product/createaddonproductdetail", new { controller = "products", action = "createaddonproductdetail" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("product-deleteaddonproductdetails", "product/deleteaddonproductdetails", new { controller = "products", action = "deleteaddonproductdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("product-getunassociatedaddongroups", "product/getunassociatedaddongroups/{parentProductId}", new { controller = "products", action = "getunassociatedaddongroups" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), parentProductId = @"^\d+$" });
            config.Routes.MapHttpRoute("product-getunassociatedaddonproducts", "product/getunassociatedaddonproducts/{addonProductId}", new { controller = "products", action = "getunassociatedaddonproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), addonProductId = @"^\d+$" });
            config.Routes.MapHttpRoute("product-updateaddonproductassociation", "product/updateaddonproductassociation", new { controller = "products", action = "updateproductaddonassociation" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("product-updateaddondisplayorder", "product/updateaddondisplayorder", new { controller = "products", action = "updateaddondisplayorder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            config.Routes.MapHttpRoute("product-getActiveProducts", "product/getActiveProducts/{parentIds}/{catalogId}/{localeId}/{versionId}", new { controller = "products", action = "getActiveProducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //PIM Product History routes 
            config.Routes.MapHttpRoute("producthistory-list", "producthistory/list", new { controller = "producthistory", action = "getproducthistorylist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("producthistory-get", "producthistory/{id}", new { controller = "producthistory", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"^\d+$" });
            config.Routes.MapHttpRoute("producthistory-create", "producthistory", new { controller = "producthistory", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("producthistory-update", "producthistory/update", new { controller = "producthistory", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("producthistory-delete", "producthistory/delete/{id}", new { controller = "producthistory", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            //PIM Product Type routes 
            config.Routes.MapHttpRoute("producttypeassociation-associatedproducts", "associatedproductlist/{parentProductId}/{attributeId}", new { controller = "products", action = "getassociatedproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), parentProductId = @"^\d+$", attributeId = @"^\d+$" });
            config.Routes.MapHttpRoute("producttypeassociation-unassociatedproducts", "unassociatedproductlist/{parentProductId}", new { controller = "products", action = "getunassociatedproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("producttypeassociation-unassociatedConfigureproducts", "unassociatedconfigureproductlist/{parentProductId}/{attributeIds}", new { controller = "products", action = "getunassociatedconfigureproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("producttypeassociation-associateproduct", "associateproduct", new { controller = "products", action = "associateproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("producttypeassociation-unassociateproduct", "unassociateproduct", new { controller = "products", action = "unassociateproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("producttypeassociation-GetAssociatedUnAssociatedConfigureProducts", "GetAssociatedUnAssociatedConfigureProducts/{parentProductId}/{associatedProductIds}/{associatedAttributeIds}/{pimProductIdsIn}", new { controller = "products", action = "GetAssociatedUnAssociatedConfigureProducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("producttypeassociation-getproductstobeassociated", "products/getproductstobeassociated", new { controller = "products", action = "getproductstobeassociated" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Product feed routes
            config.Routes.MapHttpRoute("productfeed-create", "productfeed", new { controller = "productfeed", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("productfeed-list", "productfeed/list", new { controller = "productfeed", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("productfeed-update", "productfeed/update", new { controller = "productfeed", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("productfeed-get", "productfeed/getproductfeed/{productfeedId}", new { controller = "productfeed", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), ProductFeedId = @"^\d+$" });
            config.Routes.MapHttpRoute("productfeed-delete", "productfeed/delete", new { controller = "productfeed", action = "deleteproductfeed" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("productfeed-masterDetails", "productfeed/getproductfeedmasterdetails", new { controller = "productfeed", action = "getproductfeedmasterdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("productfeed-getproductfeed", "productfeed/getproductfeedbyportalid/{portalId}", new { controller = "productfeed", action = "getproductfeedbyportalid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("productfeed-fileNamecombinationalreadyexist", "productfeed/filenamecombinationalreadyexist/{localeId}/{fileName}", new { controller = "productfeed", action = "filenamecombinationalreadyexist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //PIM Attribute Routes
            config.Routes.MapHttpRoute("pimattribute-inputvalidations", "pimattribute/inputvalidations/{typeId}/{attributeId}", new { controller = "pimattribute", action = "InputValidations" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), typeId = @"^\d+$" });
            config.Routes.MapHttpRoute("pimattribute-types", "pimattribute/types/{isCategory}", new { controller = "pimattribute", action = "attributetypes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("pimattribute-list", "pimattribute/list", new { controller = "pimattribute", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("pimattribute-get", "pimattribute/{id}", new { controller = "pimattribute", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"^\d+$" });
            config.Routes.MapHttpRoute("pimattribute-create", "pimattribute/create", new { controller = "pimattribute", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("pimattribute-update", "pimattribute/update", new { controller = "pimattribute", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("pimattribute-delete", "pimattribute/delete", new { controller = "pimattribute", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("pimattribute-getfrontendproperties", "pimfrontproperties/{pimAttributeId}", new { controller = "pimattribute", action = "frontendproperties" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), pimAttributeId = @"^\d+$" });
            config.Routes.MapHttpRoute("pimattribute-savelocales", "pimattribute/savelocales", new { controller = "pimattribute", action = "savelocales" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("pimattribute-savedefaultvalues", "pimattribute/savedefaultvalues", new { controller = "pimattribute", action = "savedefaultvalues" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("pimattribute-deletedefaultvalues", "pimattribute/deletedefaultvalues/{defaultvalueId}", new { controller = "pimattribute", action = "deletedefaultvalues" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete), defaultvalueId = @"^\d+$" });
            config.Routes.MapHttpRoute("pimattribute-getattributelocale", "pimattribute/getattributelocale/{pimAttributeId}", new { controller = "pimattribute", action = "getattributelocale" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), pimAttributeId = @"^\d+$" });
            config.Routes.MapHttpRoute("pimattribute-getdefaultvalues", "pimattribute/getdefaultvalues/{pimAttributeId}", new { controller = "pimattribute", action = "getdefaultvalues" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), pimAttributeId = @"^\d+$" });
            config.Routes.MapHttpRoute("pimattribute-isattributecodeexist", "pimattribute/isattributecodeexist/{attributeCode}/{isCategory}", new { controller = "pimattribute", action = "isattributecodeexist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("pimattribute-isattributevalueunique", "pimattribute/isattributevalueunique", new { controller = "pimattribute", action = "isattributevalueunique" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("pimattribute-attributevalidations", "pimattribute/getattributevalidationbycodes", new { controller = "pimattribute", action = "getattributevalidationbycodes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //PIM Category History Routes
            config.Routes.MapHttpRoute("categoryhistory-list", "categoryhistory/list", new { controller = "categoryhistory", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("categoryhistory-get", "categoryhistory/{id}", new { controller = "categoryhistory", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"^\d+$" });
            config.Routes.MapHttpRoute("categoryhistory-create", "categoryhistory", new { controller = "categoryhistory", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("categoryhistory-update", "categoryhistory/update", new { controller = "categoryhistory", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("categoryhistory-delete", "categoryhistory/delete/{id}", new { controller = "categoryhistory", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            //PIM Catalog History Routes
            config.Routes.MapHttpRoute("cataloghistory-list", "cataloghistory/list", new { controller = "cataloghistory", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("cataloghistory-get", "cataloghistory/{id}", new { controller = "cataloghistory", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"^\d+$" });
            config.Routes.MapHttpRoute("cataloghistory-create", "cataloghistory", new { controller = "cataloghistory", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cataloghistory-update", "cataloghistory/update", new { controller = "cataloghistory", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("cataloghistory-delete", "cataloghistory/delete/{id}", new { controller = "cataloghistory", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            //Portal Routes
            config.Routes.MapHttpRoute("portal-list", "portal/list", new { controller = "portal", action = "getportallist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portal-get", "portal/getportal/{portalId}", new { controller = "portal", action = "getportal" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalId = @"^\d+$" });
            config.Routes.MapHttpRoute("portal-getPortalByStoreCode", "portal/getPortalByStoreCode/{storeCode}", new { controller = "portal", action = "getPortalByStoreCode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portal-create", "portal/create", new { controller = "portal", action = "createportal" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portal-update", "portal/update", new { controller = "portal", action = "updateportal" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("portal-delete", "portal/delete/{isDeleteByStoreCode}", new { controller = "portal", action = "deleteportal" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portal-copystore", "portal/copystore", new { controller = "portal", action = "copystore" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portal-getportalfeaturelist", "portal/getportalfeaturelist", new { controller = "portal", action = "getportalfeaturelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portal-clearapicache", "portal/clearapicache/{domainid}", new { controller = "portal", action = "clearapicache" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portal-associatedwarehouselist", "portal/associatedwarehouselist/{portalId}", new { controller = "portal", action = "getassociatedwarehouselist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portal-associatewarehouse", "portal/associatewarehouse", new { controller = "portal", action = "associatewarehousetostore" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portal-localelist", "portal/localelist", new { controller = "portal", action = "localelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portal-updatelocale", "portal/updatelocale", new { controller = "portal", action = "updatelocale" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("portal-getportalshippinginformation", "portal/getportalshippinginformation/{portalId}", new { controller = "portal", action = "getportalshippinginformation" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portal-updateshipping", "portal/updateportalshipping", new { controller = "portal", action = "updateportalshipping" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("portal-getdisplaysetting", "portal/getdisplaysetting/{portalId}", new { controller = "portal", action = "getdisplaysetting" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalId = @"^\d+$" });
            config.Routes.MapHttpRoute("portal-updatedisplaysetting", "portal/updatedisplaysetting", new { controller = "portal", action = "updatedisplaysetting" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("portal-generateimages", "portal/generateimages/{portalId}", new { controller = "portal", action = "generateimages" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalId = @"^\d+$" });
            config.Routes.MapHttpRoute("portal-associateandunassociatetaxclass", "portal/associateandunassociatetaxclass", new { controller = "portal", action = "associateandunassociatetaxclass" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portal-setportaldefaulttax", "portal/setportaldefaulttax", new { controller = "portal", action = "setportaldefaulttax" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portal-getportalpublishstatus", "portal/getportalpublishstatus", new { controller = "portal", action = "getportalpublishstatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portal-gettaxportalinformation", "portal/gettaxportalinformation/{portalId}", new { controller = "portal", action = "gettaxportalinformation" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portal-updatetax", "portal/updatetaxportal", new { controller = "portal", action = "updatetaxportal" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("helper-isCodeExists", "helper/isCodeExists/{service}/{methodName}", new { controller = "helper", action = "isCodeExists" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portal-isportalcodeexist", "portal/isportalcodeexist/{portalCode}", new { controller = "portal", action = "isportalcodeexist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portal-saveupdateportalapprovaldetails", "portal/saveupdateportalapprovaldetails", new { controller = "portal", action = "saveupdateportalapprovaldetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
#if DEBUG
            config.Routes.MapHttpRoute("dev-portal-list", "portal/getdevportallist", new { controller = "portal", action = "getdevportallist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

#endif
            //PortalProfile Routes
            config.Routes.MapHttpRoute("portalprofile-get", "portalprofile/{portalProfileId}", new { controller = "portalprofile", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalProfileId = @"^\d+$" });
            config.Routes.MapHttpRoute("portalprofile-list", "portalprofile/list", new { controller = "portalprofile", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portalprofile-create", "portalprofile", new { controller = "portalprofile", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portalprofile-update", "portalprofile/update", new { controller = "portalprofile", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("portalprofile-delete", "portalprofile/delete", new { controller = "portalprofile", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Portal Unit Routes
            config.Routes.MapHttpRoute("portalunit-update", "portalunit/Update", new { controller = "portalunit", action = "CreateUpdatePortalUnit" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("portalunit-get", "portalunit/Get/{portalId}", new { controller = "portalunit", action = "GetPortalUnit" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalId = @"^\d+$" });

            //Portal : Country Association
            config.Routes.MapHttpRoute("portalcountry-getunassociatedcountrylist", "portalcountry/getunassociatedcountrylist", new { controller = "portalcountry", action = "getunassociatedcountrylist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portalcountry-getassociatedcountrylist", "portalcountry/getassociatedcountrylist", new { controller = "portalcountry", action = "getassociatedcountrylist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portalcountry-unassociatecountries", "portalcountry/unassociatecountries", new { controller = "portalcountry", action = "unassociatecountries" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portalcountry-associatecountries", "portalcountry/associatecountries", new { controller = "portalcountry", action = "associatecountries" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Portal : Tag Manager
            config.Routes.MapHttpRoute("tagmanager-save", "tagmanager/save", new { controller = "tagmanager", action = "save" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("tagmanager-get", "tagmanager/get/{portalId}", new { controller = "tagmanager", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Portal : Tracking Pixel. 
            config.Routes.MapHttpRoute("portal-saveportaltrackingpixel", "portal/saveportaltrackingpixel", new { controller = "portal", action = "saveportaltrackingpixel" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portal-getportaltrackingpixel", "portal/getportaltrackingpixel/{portalId}", new { controller = "portal", action = "getportaltrackingpixel" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Portal : Robots.txt.
            config.Routes.MapHttpRoute("portal-getrobotstxt", "portal/getrobotstxt/{portalId}", new { controller = "portal", action = "getrobotstxt" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("robottxt-save", "portal/saverobotstxt", new { controller = "portal", action = "saverobotstxt" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Product Review State Routes
            config.Routes.MapHttpRoute("productreviewstates-list", "productreviewstates/list", new { controller = "productreviewstate", action = "productreviewstatelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Profile Routes
            config.Routes.MapHttpRoute("profile-create", "profile/create", new { controller = "profile", action = "createprofile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("profile-update", "profile/update", new { controller = "profile", action = "updateprofile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("profile-list", "profile/list", new { controller = "profile", action = "getprofilelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("profile-get", "profile/getprofile/{profileId}", new { controller = "profile", action = "getprofile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), profileId = @"^\d+$" });
            config.Routes.MapHttpRoute("profile-delete", "profile/delete", new { controller = "profile", action = "deleteprofile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Profile Based Catalog Routes
            config.Routes.MapHttpRoute("profile-cataloglist", "profile/profilecataloglist", new { controller = "profile", action = "getprofilecataloglist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("profile-deleteassociatedprofilecatalog", "profile/deleteassociatedprofilecatalog/{profileId}", new { controller = "profile", action = "deleteassociatedprofilecatalog" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), profileId = @"^\d+$" });
            config.Routes.MapHttpRoute("profile-associatecatalogtoprofile", "profile/associatecatalogtoprofile", new { controller = "profile", action = "associatecatalogtoprofile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Profile Shipping Routes
            config.Routes.MapHttpRoute("portalprofileshipping-getassociatedshippinlist", "portalprofileshipping/getassociatedshippinglist", new { controller = "shipping", action = "getassociatedshippinglist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portalprofileshipping-getunassociatedshippinglist", "portalprofileshipping/getunassociatedshippinglist", new { controller = "shipping", action = "getunassociatedshippinglist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portalprofileshipping-associateshipping", "portalprofileshipping/associateshipping", new { controller = "shipping", action = "associateshipping" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portalprofileshipping-unassociateassociatedshipping", "portalprofileshipping/unassociateassociatedshipping", new { controller = "shipping", action = "unassociateassociatedshipping" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portalprofileshipping-UpdateShippingToPortal", "portalprofileshipping/UpdateShippingToPortal", new { controller = "shipping", action = "UpdateShippingToPortal" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portalprofileshipping-updateprofilesshipping", "portalprofileshipping/updateprofileshipping", new { controller = "shipping", action = "updateprofileshipping" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //Promotion Routes
            config.Routes.MapHttpRoute("promotion-get", "promotion/getpromotion/{promotionId}", new { controller = "promotion", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), promotionId = @"^\d+$" });
            config.Routes.MapHttpRoute("promotion-list", "promotion/list", new { controller = "promotion", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("promotion-create", "promotion/create", new { controller = "promotion", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-update", "promotion/update", new { controller = "promotion", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-delete", "promotion/delete", new { controller = "promotion", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-publishedcategories", "promotion/publishedcategories", new { controller = "promotion", action = "getpublishedcategorylist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-publishedproducts", "promotion/publishedproducts", new { controller = "promotion", action = "getpublishedproductlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-coupon-get", "promotion/getcoupon", new { controller = "promotion", action = "getcoupon" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("promotion-getcouponlist", "promotion/getcouponlist", new { controller = "promotion", action = "getcouponlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("promotion-getpromotionattribute", "promotion/getpromotionattribute/{discountName}", new { controller = "promotion", action = "getpromotionattribute" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("promotion-associatecatalogtopromotion", "promotion/associatecatalogtopromotion", new { controller = "promotion", action = "associatecatalogtopromotion" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-associatecategorytopromotion", "promotion/associatecategorytopromotion", new { controller = "promotion", action = "associatecategorytopromotion" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-associateproducttopromotion", "promotion/associateproducttopromotion", new { controller = "promotion", action = "associateproducttopromotion" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-getassociatedunassociatedproductlist", "promotion/getassociatedunassociatedproductlist/{isassociatedproduct}", new { controller = "promotion", action = "getassociatedunassociatedproductlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-getassociatedunassociatedcategorylist", "promotion/getassociatedunassociatedcategorylist/{isassociatedcategory}", new { controller = "promotion", action = "getassociatedunassociatedcategorylist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-getassociatedunassociatedcataloglist", "promotion/getassociatedunassociatedcataloglist/{isassociatedcatalog}", new { controller = "promotion", action = "getassociatedunassociatedcataloglist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-unassociateproduct", "promotion/unassociateproduct/{promotionId}", new { controller = "promotion", action = "unassociateproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-unassociatecategory", "promotion/unassociatecategory/{promotionId}", new { controller = "promotion", action = "unassociatecategory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-unassociatecatalog", "promotion/unassociatecatalog/{promotionId}", new { controller = "promotion", action = "unassociatecatalog" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-getassociatedunassociatedbrandlist", "promotion/getassociatedunassociatedbrandlist/{isassociatedbrand}", new { controller = "promotion", action = "getassociatedunassociatedbrandlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-unassociatebrand", "promotion/unassociatebrand/{promotionId}", new { controller = "promotion", action = "unassociatebrand" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-associatebrandtopromotion", "promotion/associatebrandtopromotion", new { controller = "promotion", action = "associatebrandtopromotion" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-getassociatedunassociatedshippinglist", "promotion/getassociatedunassociatedshippinglist/{isassociatedshipping}", new { controller = "promotion", action = "getassociatedunassociatedshippinglist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-unassociateshipping", "promotion/unassociateshipping/{promotionId}", new { controller = "promotion", action = "unassociateshipping" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotion-associateshippingtopromotion", "promotion/associateshippingtopromotion", new { controller = "promotion", action = "associateshippingtopromotion" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Promotion Type Routes
            config.Routes.MapHttpRoute("promotiontype-get", "promotiontype/get/{promotiontypeid}", new { controller = "promotiontype", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), promotionTypeId = @"^\d+$" });
            config.Routes.MapHttpRoute("promotiontype-list", "promotiontype/list", new { controller = "promotiontype", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("promotiontype-create", "promotiontype/create", new { controller = "promotiontype", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotiontype-update", "promotiontype/update", new { controller = "promotiontype", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("promotiontype-delete", "promotiontype/delete", new { controller = "promotiontype", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("promotiontype-getallpromotiontypesnotindatabase", "promotiontype/getallpromotiontypesnotindatabase", new { controller = "promotiontype", action = "getallpromotiontypesnotindatabase" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("promotiontype-bulkenabledisablepromotiontypes", "promotiontype/bulkenabledisablepromotiontypes/{isenable}", new { controller = "promotiontype", action = "bulkenabledisablepromotiontypes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Publish Catalog Routes
            config.Routes.MapHttpRoute("publishcatalog-get-localeId", "publishcatalog/get/{publishCatalogId}/{localeId}", new { controller = "publishcatalog", action = "getpublishcatalog" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), publishCatalogId = @"^\d+$", localeId = @"^\d+$" });
            config.Routes.MapHttpRoute("publishcatalog-get", "publishcatalog/get/{publishCatalogId}", new { controller = "publishcatalog", action = "getpublishcatalog" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), publishCatalogId = @"^\d+$" });
            config.Routes.MapHttpRoute("publishcatalog-list", "publishcatalog/list", new { controller = "publishcatalog", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("publishcatalog-unassignedlist", "publishcatalog/unassignedlist/{assignedIds}", new { controller = "publishcatalog", action = "unassignedlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Publish Category Routes
            config.Routes.MapHttpRoute("publishcategory-get", "publishcategory/get/{publishCategoryId}", new { controller = "publishcategory", action = "getpublishcategory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), publishCategoryId = @"^\d+$" });
            config.Routes.MapHttpRoute("publishcategory-list", "publishcategory/list", new { controller = "publishcategory", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("publishcategory-unassignedlist", "publishcategory/unassignedlist/{assignedIds}", new { controller = "publishcategory", action = "unassignedlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Publish Product Routes
            config.Routes.MapHttpRoute("publishproduct-get", "publishproduct/get/{publishProductId}", new { controller = "publishproduct", action = "GetPublishProduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), publishProductId = @"^\d+$" });
            config.Routes.MapHttpRoute("publishproduct-getbrief", "publishproduct/getproductbrief/{publishProductId}", new { controller = "publishproduct", action = "GetPublishProductBrief" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), publishProductId = @"^\d+$" });
            config.Routes.MapHttpRoute("publishproduct-getextended", "publishproduct/getextendedproductdetails/{publishProductId}", new { controller = "publishproduct", action = "GetExtendedPublishProductDetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), publishProductId = @"^\d+$" });
            config.Routes.MapHttpRoute("publishproduct-list", "publishproduct/list", new { controller = "publishproduct", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("publishproduct-listbyparameter", "publishproduct/getproductlist", new { controller = "publishproduct", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("publishproduct-productpriceinventory", "publishproduct/getproductpriceandinventory", new { controller = "publishproduct", action = "getproductpriceandinventory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("publishproduct-pricewithinventory", "publishproduct/getpricewithinventory", new { controller = "publishproduct", action = "getpricewithinventory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("publishproduct-getproductbysku", "publishproduct/getproductbysku", new { controller = "publishproduct", action = "getpublishproductbysku" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("publishproduct-getconfigurableproduct", "publishproduct/getconfigurableproduct", new { controller = "publishproduct", action = "getconfigurableproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("publishproduct-getbundleproduct", "publishproduct/getbundleproducts", new { controller = "publishproduct", action = "getbundleproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("publishproduct-getproductattribute", "publishproduct/getproductattribute/{productId}", new { controller = "publishproduct", action = "getproductattribute" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), productId = @"^\d+$" });
            config.Routes.MapHttpRoute("publishproduct-getgroupproduct", "publishproduct/getgroupproducts", new { controller = "publishproduct", action = "getgroupproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("publishproduct-unassignedlist", "publishproduct/unassignedlist", new { controller = "publishproduct", action = "unassignedlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("publishproduct-getproductprice", "publishproduct/getproductprice", new { controller = "publishproduct", action = "getproductprice" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("publishproduct-GetPublishProductForSiteMap", "publishproduct/GetPublishProductForSiteMap", new { controller = "publishproduct", action = "GetPublishProductForSiteMap" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("publishproduct-getpublishproductcount", "publishproduct/getpublishproductcount", new { controller = "publishproduct", action = "getpublishproductcount" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("publishproduct-getpublishedproductslistdata", "publishproduct/GetPublishedProductsListData", new { controller = "publishproduct", action = "GetPublishedProductsListData" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("publishparentproduct-get", "publishproduct/getpublishparentproduct/{publishProductId}", new { controller = "publishproduct", action = "getpublishparentproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), publishProductId = @"^\d+$" });
            config.Routes.MapHttpRoute("publishparentproduct-getdetail", "publishproduct/getpublishparentproduct/{publishProductId}", new { controller = "publishproduct", action = "getpublishparentproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), publishProductId = @"^\d+$" });
            config.Routes.MapHttpRoute("publishproduct-GetMainProduct", "publishproduct/GetParentProduct/{parentProductId}", new { controller = "publishproduct", action = "GetParentProduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), parentProductId = @"^\d+$" });
            config.Routes.MapHttpRoute("publishproduct-getinventory", "publishproduct/getinventory/{publishProductId}", new { controller = "publishproduct", action = "getproductinventory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), publishProductId = @"^\d+$" });
            config.Routes.MapHttpRoute("publishproduct-getassociatedconfigurablevariants", "publishproduct/getassociatedconfigurablevariants/{productId}", new { controller = "publishproduct", action = "getassociatedconfigurablevariants" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), productId = @"^\d+$" });
            config.Routes.MapHttpRoute("publishproduct-submitstockrequest", "publishproduct/submitstockrequest", new { controller = "publishproduct", action = "submitstockrequest" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("publishproduct-sendstocknotification", "publishproduct/sendstocknotification", new { controller = "publishproduct", action = "sendstocknotification" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Publish Brand Routes
            config.Routes.MapHttpRoute("publishbrand-getpublishbrands", "publishbrand/getpublishbrandlist", new { controller = "publishbrand", action = "getpublishbrandlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("publishbrand-getpublishbrand", "publishbrand/getpublishbrand/{brandId}/{localeId}/{portalId}", new { controller = "publishbrand", action = "getpublishbrand" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //SKU Price
            config.Routes.MapHttpRoute("skuprice-create", "skuprice/create", new { controller = "price", action = "addskuprice" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("skuprice-list", "skuprice/list", new { controller = "price", action = "getskupricelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("skuprice-get", "skuprice/get/{priceId}", new { controller = "price", action = "getskuprice" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), priceId = @"^\d+$" });
            config.Routes.MapHttpRoute("skuprice-update", "skuprice/update", new { controller = "price", action = "updateskuprice" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("skuprice-delete", "skuprice/delete", new { controller = "price", action = "deleteskuprice" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("uom-list", "uom/list", new { controller = "price", action = "getuomlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("price-getpagedpricesku", "price/getpagedpricesku", new { controller = "price", action = "getpagedpricesku" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("pricebysku-get", "skuprice/getpricebysku", new { controller = "price", action = "getpricebysku" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("productpricebysku-get", "skuprice/getproductpricingdetailsbysku", new { controller = "price", action = "getproductpricingdetailsbysku" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Tier Price  
            config.Routes.MapHttpRoute("tierprice-create", "tierprice/create", new { controller = "price", action = "addtierprice" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("tierprice-list", "tierprice/list", new { controller = "price", action = "gettierpricelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("tierprice-get", "tierprice/get/{priceTierId}", new { controller = "price", action = "gettierprice" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), priceTierId = @"^\d+$" });
            config.Routes.MapHttpRoute("tierprice-update", "tierprice/update", new { controller = "price", action = "updatetierprice" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("tierprice-delete", "tierprice/delete/{priceTierId}", new { controller = "price", action = "deletetierprice" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Price
            config.Routes.MapHttpRoute("price-create", "price/create", new { controller = "price", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("price-get", "price/getprice/{priceListId}", new { controller = "price", action = "getprice" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("price-update", "price/update", new { controller = "price", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("price-list", "pricelist", new { controller = "price", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("price-delete", "price/delete", new { controller = "price", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("price-copy", "price/copy", new { controller = "price", action = "copy" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("price-getexportpricedata", "price/getexportpricedata/{priceListIds}", new { controller = "price", action = "getexportpricedata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Price: Associate Store
            config.Routes.MapHttpRoute("associatedstore-list", "associatedstore/list", new { controller = "price", action = "getassociatedstorelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("unassociatedstore-list", "price/unassociatedstore/list", new { controller = "price", action = "GetUnAssociatedStoreList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("price-associatestore", "price/associatestore", new { controller = "price", action = "associatestore" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("price-removeassociatedstores", "price/removeassociatedstores", new { controller = "price", action = "removeassociatedstores" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("price-getassociatedstoreprecedence", "price/getassociatedstoreprecedence/{priceListPortalId}", new { controller = "price", action = "getassociatedstoreprecedence" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("price-updateassociatedstoreprecedence", "price/updateassociatedstoreprecedence", new { controller = "price", action = "updateassociatedstoreprecedence" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //Price: Associate Profile
            config.Routes.MapHttpRoute("associatedprofile-list", "associatedprofile/list", new { controller = "price", action = "getassociatedprofilelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("unassociatedprofile-list", "price/unassociatedprofile/list", new { controller = "price", action = "getunassociatedprofilelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("price-associateprofile", "price/associateprofile", new { controller = "price", action = "associateprofile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("price-removeassociatedprofiles", "price/removeassociatedprofiles", new { controller = "price", action = "removeassociatedprofiles" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("price-getassociatedprofileprecedence", "price/getassociatedprofileprecedence/{priceListProfileId}", new { controller = "price", action = "getassociatedprofileprecedence" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("price-updateassociatedprofileprecedence", "price/updateassociatedprofileprecedence", new { controller = "price", action = "updateassociatedprofileprecedence" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //Price: Associate Customer
            config.Routes.MapHttpRoute("associatedcustomer-list", "price/associatedcustomer/list", new { controller = "price", action = "getassociatedcustomerlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("unassociatedcustomer-list", "price/unassociatedcustomer/list", new { controller = "price", action = "getunassociatedcustomerlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("price-associatecustomer", "price/associatecustomer", new { controller = "price", action = "associatecustomer" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("associatedcustomer-delete", "price/associatedcustomer/delete", new { controller = "price", action = "deleteassociatedcustomer" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("price-getassociatedcustomerprecedence", "price/getassociatedcustomerprecedence/{priceListUserId}", new { controller = "price", action = "getassociatedcustomerprecedence" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("price-updateassociatedcustomerprecedence", "price/updateassociatedcustomerprecedence", new { controller = "price", action = "updateassociatedcustomerprecedence" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //Price: Associate Account
            config.Routes.MapHttpRoute("associatedaccount-list", "price/associatedaccount/list", new { controller = "price", action = "getassociatedaccountlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("unassociatedaccount-list", "price/unassociatedaccount/list", new { controller = "price", action = "getunassociatedaccountlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("price-associateaccount", "price/associateaccount", new { controller = "price", action = "associateaccount" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("associatedaccount-delete", "price/associatedaccount/delete", new { controller = "price", action = "removeassociatedaccounts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("price-getassociatedaccountprecedence", "price/getassociatedaccountprecedence/{priceListUserId}", new { controller = "price", action = "getassociatedaccountprecedence" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("price-updateassociatedaccountprecedence", "price/updateassociatedaccountprecedence", new { controller = "price", action = "updateassociatedaccountprecedence" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //Price Management
            config.Routes.MapHttpRoute("unassociatedprice-list", "price/unassociatedprice/list", new { controller = "price", action = "GetUnAssociatedPriceList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("price-removeassociatedpricelisttostore", "price/removeassociatedpricelisttostore", new { controller = "price", action = "removeassociatedpricelisttostore" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("price-removeassociatedpricelisttoprofile", "price/removeassociatedpricelisttoprofile", new { controller = "price", action = "removeassociatedpricelisttoprofile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("price-getassociatedpricelistprecedence", "price/getassociatedpricelistprecedence", new { controller = "price", action = "getassociatedpricelistprecedence" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("price-updateassociatedpricelistprecedence", "price/updateassociatedpricelistprecedence", new { controller = "price", action = "updateassociatedpricelistprecedence" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //RMA Configuration 
            config.Routes.MapHttpRoute("rmaconfiguration-creatermaconfiguration", "rmaconfiguration/creatermaconfiguration", new { controller = "rmaconfiguration", action = "creatermaconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("rmaconfiguration-getrmaconfiguration", "rmaconfiguration/getrmaconfiguration", new { controller = "rmaconfiguration", action = "getrmaconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //RMA Configuration : Reason For Reason
            config.Routes.MapHttpRoute("rmaconfiguration-createreasonforreturn", "rmaconfiguration/createreasonforreturn", new { controller = "rmaconfiguration", action = "createreasonforreturn" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("rmaconfiguration-updatereasonforreturn", "rmaconfiguration/updatereasonforreturn", new { controller = "rmaconfiguration", action = "updatereasonforreturn" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("rmaconfiguration-getreasonforreturnlist", "rmaconfiguration/getreasonforreturnlist", new { controller = "rmaconfiguration", action = "getreasonforreturnlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("rmaconfiguration-getreasonforreturn", "rmaconfiguration/getreasonforreturn/{rmareasonforreturnid}", new { controller = "rmaconfiguration", action = "getreasonforreturn" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), rmaReasonForReturnId = @"^\d+$" });
            config.Routes.MapHttpRoute("rmaconfiguration-deletereasonforreturn", "rmaconfiguration/deletereasonforreturn", new { controller = "rmaconfiguration", action = "deletereasonforreturn" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //RMA Configuration : Request Status
            config.Routes.MapHttpRoute("rmaconfiguration-updaterequeststatus", "rmaconfiguration/updaterequeststatus", new { controller = "rmaconfiguration", action = "updaterequeststatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("rmaconfiguration-getrequeststatusList", "rmaconfiguration/getrequeststatuslist", new { controller = "rmaconfiguration", action = "getrequeststatuslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("rmaconfiguration-getrequeststatus", "rmaconfiguration/getrequeststatus/{rmarequeststatusid}", new { controller = "rmaconfiguration", action = "getrequeststatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), rmaRequestStatusId = @"^\d+$" });
            config.Routes.MapHttpRoute("rmaconfiguration-deleterequeststatus", "rmaconfiguration/deleterequeststatus", new { controller = "rmaconfiguration", action = "deleterequeststatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //RMA Manager:Request
            config.Routes.MapHttpRoute("rmarequest-getrmarequestlist", "rmarequest/getrmarequestlist", new { controller = "rmarequest", action = "getrmarequestlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("rmarequest-getorderrmaflag", "getorderrmaflag/{omsorderdetailsid}", new { controller = "rmarequest", action = "getorderrmaflag" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("rmarequest-updatermarequest", "rmarequest/updatermarequest/{rmarequestid}", new { controller = "rmarequest", action = "updatermarequest" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("rmarequest-getrmarequest", "rmarequest/getrmarequest/{rmaRequestId}", new { controller = "rmarequest", action = "getrmarequest" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("rmarequest-create", "creatermarequest", new { controller = "rmarequest", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("rmarequest-sendrmastatusmail", "rmarequest/sendrmastatusmail/{rmaRequestId}", new { controller = "rmarequest", action = "sendrmastatusmail" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("rmarequest-sendgiftcardmail", "rmarequest/sendgiftcardmail", new { controller = "rmarequest", action = "sendgiftcardmail" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("rmarequest-getrmagiftcarddetails", "rmarequest/getrmagiftcarddetails/{rmaRequestId}", new { controller = "rmarequest", action = "getrmagiftcarddetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //RMA Request items routes
            config.Routes.MapHttpRoute("rmarequestitem-list", "rmarequestitems", new { controller = "rmarequestitem", action = "getrmarequestitemlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("rmarequestitem-get", "rmarequestitems/getrmarequestitemsforgiftcard/{orderlineitems}", new { controller = "rmarequestitem", action = "getrmarequestitemsforgiftcard" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("rmarequestitem-create", "rmarequestitems/create", new { controller = "rmarequestitem", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });


            //Role routes
            config.Routes.MapHttpRoute("role-create", "role", new { controller = "role", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("permission-list", "permission/list", new { controller = "role", action = "getpermissionlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("role-list", "role/list", new { controller = "role", action = "getrolelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("role-delete", "deleterole", new { controller = "role", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("role-update", "role/{roleId}", new { controller = "role", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("role-get", "roles/{roleId}", new { controller = "role", action = "getrole" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("role-deleteaccess", "rolesremoveallaccess/{rolemenuId}", new { controller = "role", action = "removeallaccess" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });
            config.Routes.MapHttpRoute("rolemenu-getrolesmenuspermissionswithrolemenus", "getrolesmenuspermissionswithrolemenus", new { controller = "role", action = "getrolesmenuspermissionswithrolemenus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("rolePremission-getrolepermission", "rolespermission/getrolepermission", new { controller = "role", action = "GetPermissionListByUserName" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Search Attributes routes
            config.Routes.MapHttpRoute("searchattributes-getassociatedunassociatedcatalogattributes", "searchattributes/getassociatedunassociatedcatalogattributes", new { controller = "searchprofile", action = "getassociatedunassociatedcatalogattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("searchattributes-associateattributestoprofile", "searchattributes/associateattributestoprofile", new { controller = "searchprofile", action = "associateattributestoprofile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("searchattributes-unassociateattributesfromprofile", "searchattributes/unassociateattributesfromprofile", new { controller = "searchprofile", action = "unassociateattributesfromprofile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Search Profile routes
            config.Routes.MapHttpRoute("searchprofile-create", "searchprofile/create", new { controller = "searchprofile", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("profiledetails-list", "searchprofile/getdetails", new { controller = "searchprofile", action = "getdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("searchprofile-list", "searchprofile/list", new { controller = "searchprofile", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("searchprofile-get", "searchprofile/{searchProfileId}", new { controller = "searchprofile", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), searchProfileId = @"^\d+$" });
            config.Routes.MapHttpRoute("searchprofile-update", "searchprofile/update", new { controller = "searchprofile", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("searchprofile-delete", "searchprofile/delete", new { controller = "searchprofile", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("profileproduct-list", "search/getsearchprofileproduct", new { controller = "search", action = "getsearchprofileproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("profilefeatures-list", "searchprofile/featurelist/{queryId}", new { controller = "searchprofile", action = "getfeaturesbyqueryid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("searchprofile-catalogbasedattributes", "searchprofile/catalogbasedattributes", new { controller = "searchprofile", action = "catalogbasedattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("profileportal-list", "searchprofileportals/list", new { controller = "searchprofile", action = "searchprofileportallist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("associateportalsearch-list", "searchprofile/associateunassociateportaltosearchprofile", new { controller = "searchprofile", action = "associateunassociateportaltosearchprofile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("searchprofile-setdefaultsearchprofile", "searchprofile/setdefaultsearchprofile", new { controller = "searchprofile", action = "setdefaultsearchprofile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("profileportal-unassociatedportals", "searchprofile/unassociatedportallist", new { controller = "searchprofile", action = "unassociatedportallist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("getfieldvalueslist-get", "getfieldvalueslist/{publishCatalogId}/{searchProfileId}", new { controller = "searchprofile", action = "getfieldvalueslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), publishCatalogId = @"^\d+$", searchProfileId = @"^\d+$" });
            config.Routes.MapHttpRoute("searchprofile-publish", "searchprofile/publishsearchprofile/{searchProfileId}", new { controller = "searchprofile", action = "publishsearchprofile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), searchProfileId = @"^\d+$" });
            config.Routes.MapHttpRoute("searchprofile-getcataloglist", "searchprofile/getcataloglist", new { controller = "searchprofile", action = "getcataloglist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get)});

            //Search Profile Triggers routes
            config.Routes.MapHttpRoute("searchprofiletriggers-createsearchprofiletriggers", "searchprofiletriggers/createsearchprofiletriggers", new { controller = "searchprofile", action = "createsearchtriggers" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("searchprofiletriggers-updatesearchprofiletriggers", "searchprofiletriggers/updatesearchprofiletriggers", new { controller = "searchprofile", action = "updatesearchtriggers" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("searchprofiletriggers-getsearchprofiletriggerlist", "searchprofiletriggers/getsearchprofiletriggerlist", new { controller = "searchprofile", action = "getsearchtriggerlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("searchprofiletriggers-getsearchprofiletriggers", "searchprofiletriggers/getsearchprofiletriggers/{searchProfileTriggerId}", new { controller = "searchprofile", action = "getsearchtrigger" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), searchProfileTriggerId = @"^\d+$" });
            config.Routes.MapHttpRoute("searchprofiletriggers-deletesearchprofiletriggers", "searchprofiletriggers/deletesearchprofiletriggers", new { controller = "searchprofile", action = "deletesearchtriggers" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Search boost and bury rules
            config.Routes.MapHttpRoute("searchboostandburyrule-list", "searchboostandburyrule/list", new { controller = "searchboostandburyrule", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("searchboostandburyrule-create", "searchboostandburyrule/createboostandburyrule", new { controller = "searchboostandburyrule", action = "createboostandburyrule" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("searchboostandburyrule-get", "searchboostandburyrule/getboostandburyrule/{searchCatalogRuleId}", new { controller = "searchboostandburyrule", action = "getboostandburyrule" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("searchboostandburyrule-update", "searchboostandburyrule/updateboostandburyrule", new { controller = "searchboostandburyrule", action = "updateboostandburyrule" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("searchboostandburyrule-delete", "searchboostandburyrule/delete", new { controller = "searchboostandburyrule", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("searchboostandburyrule-pausedsearchrule", "searchboostandburyrule/pausedsearchrule/{isPause}", new { controller = "searchboostandburyrule", action = "pausedsearchrule" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("searchboostandburyrule-searchablefieldlist", "searchboostandburyrule/searchablefieldlist/{PublishCatalogId}", new { controller = "searchboostandburyrule", action = "getsearchablefieldlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("searchboostandburyrule-getautosuggestion", "searchboostandburyrule/getautosuggestion", new { controller = "searchboostandburyrule", action = "getautosuggestion" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //SEO Settings.
            config.Routes.MapHttpRoute("seo-list", "seodetails/list", new { controller = "SEO", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("seo-get", "seo/defaultportalsetting/{portalId}", new { controller = "seo", action = "getportalseosetting" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalId = @"^\d+$" });
            config.Routes.MapHttpRoute("seo-create", "seo/createdefaultportalsetting", new { controller = "seo", action = "createportaldefaultsetting" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("seo-update", "seo/updateportalseosetting", new { controller = "seo", action = "updateportalseosetting" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("seo-delete", "seo/DeleteSeoDetail/{seoTypeId}/{portalId}/{seoCode}", new { controller = "seo", action = "DeleteSeoDetail" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute("seo-getpublishedproducts", "seo/getpublishedproducts", new { controller = "seo", action = "getpublishedproductlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("seo-getpublishedcategories", "seo/getpublishedcategories", new { controller = "seo", action = "getpublishedcategorylist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("seo-getseodetails", "seo/getseodetails/{itemId}/{seoTypeId}/{localeId}/{portalId}", new { controller = "seo", action = "getseodetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), itemId = @"^\d+$", seoTypeId = @"^\d+$", localeId = @"^\d+$", portalId = @"^\d+$" });

            config.Routes.MapHttpRoute("seo-getpublishseodetails", "seo/getpublishseodetails/{itemId}/{seoType}/{localeId}/{portalId}/{seoCode}", new { controller = "seo", action = "getpublishseodetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), itemId = @"^\d+$", localeId = @"^\d+$", portalId = @"^\d+$" });
            config.Routes.MapHttpRoute("seo-createseodetails", "seo/createseodetails", new { controller = "seo", action = "createseodetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("seo-updateseodetails", "seo/updateseodetails", new { controller = "seo", action = "updateseodetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("seo-publish", "seo/publish/{seoCode}/{portalId}/{localeId}/{seoTypeId}", new { controller = "seo", action = "publish" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), portalId = @"^\d+$", localeId = @"^\d+$", seoTypeId = @"^\d+$" });
            config.Routes.MapHttpRoute("seo-getseodetailsbyseocode", "seo/getseodetailsbyseocode/{seoCode}/{seoTypeId}/{localeId}/{portalId}", new { controller = "seo", action = "getseodetailsbyseocode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("seo-getproductsforseo", "seo/getproductsforseo", new { controller = "seo", action = "getproductsforseo" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("seo-publishwithpreview", "seo/publishwithpreview/{seoCode}/{portalId}/{localeId}/{seoTypeId}/{targetpublishstate}/{takefromdraftfirst}", new { controller = "seo", action = "publishwithpreview" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post), portalId = @"^\d+$", localeId = @"^\d+$", seoTypeId = @"^\d+$" });
            config.Routes.MapHttpRoute("seo-getcategorylistforseo", "seo/getcategorylistforseo", new { controller = "SEO", action = "getcategorylistforseo" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("seo-getdefaultseodetails", "seo/getdefaultseodetails/{seoCode}/{seoTypeId}/{localeId}/{portalId}/{itemId}", new { controller = "seo", action = "getdefaultseodetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //Server Side Validations.
            config.Routes.MapHttpRoute("Validate-ServerSide", "valid", new { controller = "servervalidation", action = "validate" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Shipping Routes
            config.Routes.MapHttpRoute("shipping-create", "shipping/create", new { controller = "shipping", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("shipping-update", "shipping/update", new { controller = "shipping", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("shipping-list", "shipping/list", new { controller = "shipping", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("shipping-get", "shipping/getshipping/{shippingId}", new { controller = "shipping", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), shippingId = @"^\d+$" });
            config.Routes.MapHttpRoute("shipping-delete", "shipping/delete", new { controller = "shipping", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("shipping-isshippingaddressvalid", "shipping/isshippingaddressvalid", new { controller = "shipping", action = "isshippingaddressvalid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("shipping-recommendedaddress", "shipping/recommendedaddress", new { controller = "shipping", action = "recommendedaddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Shipping Service code
            config.Routes.MapHttpRoute("shippingservicecode-get", "shippingservicecode/getshippingservicecode/{shippingservicecodeId}", new { controller = "shipping", action = "getshippingservicecode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), shippingservicecodeId = @"^\d+$" });
            config.Routes.MapHttpRoute("shippingservicecode-list", "shippingservicecode/list", new { controller = "shipping", action = "getshippingservicecodelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Shipping SKU 
            config.Routes.MapHttpRoute("shippingsku-create", "shippingsku/create", new { controller = "shipping", action = "addshippingsku" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("shippingsku-list", "shippingsku/list", new { controller = "shipping", action = "getshippingskulist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("shippingsku-get", "shippingsku/get/{shippingSKUId}", new { controller = "shipping", action = "getshippingsku" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("shippingsku-update", "shippingsku/update", new { controller = "shipping", action = "updateshippingsku" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("shippingsku-delete", "shippingsku/delete", new { controller = "shipping", action = "deleteshippingsku" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Shipping Rule 
            config.Routes.MapHttpRoute("shippingrule-create", "shippingrule/create", new { controller = "shipping", action = "addshippingrule" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("shippingrule-list", "shippingrule/list", new { controller = "shipping", action = "getshippingrulelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("shippingrule-get", "shippingrule/get/{shippingRuleId}", new { controller = "shipping", action = "getshippingrule" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("shippingrule-update", "shippingrule/update", new { controller = "shipping", action = "updateshippingrule" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("shippingrule-delete", "shippingrule/delete", new { controller = "shipping", action = "deleteshippingrule" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Shipping Rule Type
            config.Routes.MapHttpRoute("shippingruletype-list", "shippingruletype/list", new { controller = "shipping", action = "getshippingruletypelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Shipping Type Routes
            config.Routes.MapHttpRoute("shippingtype-get", "shippingtype/get/{shippingTypeId}", new { controller = "shippingtype", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), shippingTypeId = @"^\d+$" });
            config.Routes.MapHttpRoute("shippingtype-list", "shippingtype/list", new { controller = "shippingtype", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("shippingtype-create", "shippingtype/create", new { controller = "shippingtype", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("shippingtype-update", "shippingtype/update", new { controller = "shippingtype", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("shippingtype-delete", "shippingtype/delete", new { controller = "shippingtype", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("shippingtype-getallshippingtypesnotindatabase", "shippingtype/getallshippingtypesnotindatabase", new { controller = "shippingtype", action = "getallshippingtypesnotindatabase" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("shippingtype-bulkenabledisableshippingtypes", "shippingtype/bulkenabledisableshippingtypes/{isEnable}", new { controller = "shippingtype", action = "bulkenabledisableshippingtypes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            // Shopping cart routes
            config.Routes.MapHttpRoute("shoppingcart-getshoppingcart", "shoppingcarts/getshoppingcart", new { controller = "shoppingcart", action = "getshoppingcart" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute("shoppingcart-getcartcount", "shoppingcarts/getcartcount", new { controller = "shoppingcart", action = "getcartcount" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });


            config.Routes.MapHttpRoute("shoppingcart-create", "shoppingcarts", new { controller = "shoppingcart", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("shoppingcart-addtocartproduct", "addtocartproduct", new { controller = "shoppingcart", action = "addtocartproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("shoppingcart-calculate", "shoppingcarts/calculate", new { controller = "shoppingcart", action = "calculate" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("shoppingcart-removeallcartitem", "shoppingcarts/removeallcartitem", new { controller = "shoppingcart", action = "removeallcartitem" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("shoppingcart-getshippingestimates", "shoppingcarts/getshippingestimates/{zipcode}", new { controller = "shoppingcart", action = "getshippingestimates" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("shoppingcart-getomslineitemdetailslist", "shoppingcarts/getomslineitemdetails/{omsorderid}", new { controller = "shoppingcart", action = "getomslineitemdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("shoppingcarts-mergeguestuserscart", "shoppingcarts/mergeguestuserscart", new { controller = "shoppingcart", action = "mergeguestuserscart" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //SMTP Routes
            config.Routes.MapHttpRoute("smtp-update", "smtp/Update", new { controller = "smtp", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("smtp-sendemail", "smtp/sendemail", new { controller = "smtp", action = "sendemail" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute("smtp-get", "smtp/Get/{portalId}", new { controller = "smtp", action = "getsmtp" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalId = @"^\d+$" });
            //SMS Routes
            config.Routes.MapHttpRoute("sms-get", "sms/Get/{portalId}/{isSMSSettingEnabled}", new { controller = "sms", action = "getsmsdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalId = @"^\d+$" });
            config.Routes.MapHttpRoute("sms-getsmsproviderlist", "sms/GetSmsProviderList/", new { controller = "sms", action = "getsmsproviderlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("sms-insertupdatesmssetting", "sms/InsertUpdateSMSSetting", new { controller = "sms", action = "insertupdatesmssetting" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //Tax Class Routes
            config.Routes.MapHttpRoute("taxclass-get", "taxclass/get/{taxClassId}", new { controller = "taxclass", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), taxClassId = @"^\d+$" });
            config.Routes.MapHttpRoute("taxclass-list", "taxclass/list", new { controller = "taxclass", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("taxclass-create", "taxclass/create", new { controller = "taxclass", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("taxclass-update", "taxclass/update", new { controller = "taxclass", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("taxclass-delete", "taxclass/delete", new { controller = "taxclass", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("taxclasssku-testavalaraconnection", "taxclass/testavalaraconnection", new { controller = "taxclass", action = "testavalaraconnection" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Tax Class SKU 
            config.Routes.MapHttpRoute("taxclasssku-create", "taxclass/sku/create", new { controller = "taxclass", action = "addtaxclasssku" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("taxclasssku-list", "taxclass/sku/list", new { controller = "taxclass", action = "gettaxclassskulist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("taxclasssku-get", "taxclass/sku/get/{taxClassSKUId}", new { controller = "taxclass", action = "gettaxclasssku" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("taxclasssku-update", "taxclass/sku/update", new { controller = "taxclass", action = "updatetaxclasssku" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("taxclasssku-delete", "taxclass/sku/delete", new { controller = "taxclass", action = "deletetaxclasssku" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("taxclasssku-getunassociatedproductlist", "taxclass/sku/getunassociatedproductlist", new { controller = "taxclass", action = "getunassociatedproductlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Tax Rule 
            config.Routes.MapHttpRoute("taxrule-create", "taxrule/create", new { controller = "taxclass", action = "addtaxrule" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("taxrule-list", "taxrule/list", new { controller = "taxclass", action = "gettaxrulelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("taxrule-get", "taxrule/get/{taxRuleId}", new { controller = "taxclass", action = "gettaxrule" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("taxrule-update", "taxrule/update", new { controller = "taxclass", action = "updatetaxrule" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("taxrule-delete", "taxrule/delete", new { controller = "taxclass", action = "deletetaxrule" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Tax Rule Type Routes
            config.Routes.MapHttpRoute("taxruletype-get", "taxruletype/get/{taxRuleTypeId}", new { controller = "taxruletype", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), taxRuleTypeId = @"^\d+$" });
            config.Routes.MapHttpRoute("taxruletype-list", "taxruletype/list", new { controller = "taxruletype", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("taxruletype-create", "taxruletype/create", new { controller = "taxruletype", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("taxruletype-update", "taxruletype/update", new { controller = "taxruletype", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("taxruletype-delete", "taxruletype/delete", new { controller = "taxruletype", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("taxruletype-getalltaxruletypesnotindatabase", "taxruletype/getalltaxruletypesnotindatabase", new { controller = "taxruletype", action = "getalltaxruletypesnotindatabase" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("taxruletype-bulkenabledisabletaxruletypes", "taxruletype/bulkenabledisabletaxruletypes/{isEnable}", new { controller = "taxruletype", action = "bulkenabledisabletaxruletypes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Templates Routes
            config.Routes.MapHttpRoute("template-create", "template/create", new { controller = "template", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("template-get", "template/get/{cmsTemplateId}", new { controller = "template", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("template-update", "template/update", new { controller = "template", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("template-list", "template/list", new { controller = "template", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("template-delete", "template/delete", new { controller = "template", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Users Routes
            config.Routes.MapHttpRoute("accounts-create", "users/createusers", new { controller = "user", action = "createadminuseraccount" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("account-getuseraccount", "useraccounts/{accountId}/{portalId}", new { controller = "user", action = "getuseraccountdata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), accountId = @"^\d+$", portalId = @"^\d+$" });
            config.Routes.MapHttpRoute("account-updateuseraccount", "useraccount/update/{webstoreUser}", new { controller = "user", action = "updateuseraccountdata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("useraccount-list", "useraccounts/list/{loggedUserAccountId}", new { controller = "user", action = "getuseraccountlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("useraccount-delete", "useraccounts/delete", new { controller = "user", action = "deleteuseraccount" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("useraccount-enabledisable", "useraccounts/enabledisable/{lockUser}", new { controller = "user", action = "enabledisableaccount" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("user-updateusernameforregistereduser", "username/updateusernameforregistereduser", new { controller = "user", action = "UpdateUsernameForRegisteredUser" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //User Routes
            config.Routes.MapHttpRoute("users-login", "users/login", new { controller = "user", action = "login" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("users-payinvoice", "users/payinvoice", new { controller = "user", action = "payinvoice" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("users-changepassword", "users/changepassword", new { controller = "user", action = "changepassword" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("users-forgotpassword", "users/forgotpassword", new { controller = "user", action = "forgotpassword" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("users-bulkresetpassword", "users/bulkresetpassword", new { controller = "user", action = "bulkresetpassword" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("users-verifyresetpasswordlinkstatus", "users/verifyresetpasswordlinkstatus", new { controller = "user", action = "verifyresetpasswordlinkstatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("users-enabledisableaccount", "users/enabledisableaccount", new { controller = "user", action = "enabledisableaccount" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("users-getbyusername", "users/getbyusername", new { controller = "user", action = "getbyusername" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("users-getportalids", "users/getportalids/{aspNetUserId}", new { controller = "user", action = "getportalids" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("users-getSalesRepListForAssociation", "users/GetSalesRepListForAssociation", new { controller = "user", action = "GetSalesRepListForAssociation" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("users-saveportalsids", "users/saveportalsids", new { controller = "user", action = "saveportalsids" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("users-signupfornewsletter", "users/signupfornewsletter", new { controller = "user", action = "signupfornewsletter" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("users-sociallogin", "users/sociallogin", new { controller = "user", action = "sociallogin" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("users-getsocialloginproviders", "users/getsocialloginproviders", new { controller = "user", action = "getloginproviders" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("users-updateuserandquotedetails", "users/updateuserandquotedetails", new { controller = "user", action = "updateuserandquotedetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("users-convertshoppertoadmin", "users/convertshoppertoadmin", new { controller = "user", action = "convertshoppertoadmin" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("users-validatecsrtoken", "users/validatecsrtoken", new { controller = "user", action = "validatecsrtoken" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("users-CheckIsUserNameAnExistingShopper", "users/checkisusernameanexistingshopper", new { controller = "user", action = "checkisusernameanexistingshopper" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("users-getSalesRepListForAccount", "users/GetSalesRepListForAccount", new { controller = "user", action = "GetSalesRepListForAccount" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("user-getuserdetail", "user/getuserdetailbyid/{userId}/{portalId}", new { controller = "user", action = "getuserdetailbyid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), userId = @"^\d+$", portalId = @"^\d+$" });

            //User- Customer
            config.Routes.MapHttpRoute("users-createcustomeraccount", "users/createcustomeraccount", new { controller = "user", action = "createcustomeraccount" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("users-updatecustomeraccount", "customeraccount/update", new { controller = "user", action = "updatecustomeraccount" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("users-list", "customeraccount/list/{loggedUserAccountId}", new { controller = "user", action = "getcustomeraccountlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("unassociated-users-list", "unassociatedcustomeraccount/list/{portalId}", new { controller = "user", action = "getunassociatedcustomerlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("user-updatecustomeraccountmapping", "users/updateusersaccountmapping", new { controller = "user", action = "updateuseraccountmapping" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("users-updatecustomer", "customeraccount/updatecustomer", new { controller = "user", action = "updatecustomer" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("users-list-admin", "customeraccount/list/{loggedUserAccountId}/{currentUserName}/{roleName}", new { controller = "user", action = "GetCustomerAccountListForAdmin" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("admin-user-list", "customeraccount/getcustomerlistforadmin", new { controller = "user", action = "GetCustomerListForAdmin" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("users-getcustomeraccountdetails", "users/getcustomeraccountdetails/{userId}", new { controller = "User", action = "GetCustomerAccountdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute("users-isdefaultadminpasswordreset", "users/isdefaultadminpasswordreset", new { controller = "user", action = "isdefaultadminpasswordreset" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            // Web API routes
            config.MapHttpAttributeRoutes();

            //Email Templates
            config.Routes.MapHttpRoute("emailtemplate-list", "emailtemplate/list", new { controller = "emailtemplates", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("emailtemplate-create", "emailtemplate", new { controller = "emailtemplates", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("emailtemplate-get", "emailtemplates/{emailTemplateId}", new { controller = "emailtemplates", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("emailtemplate-update", "emailtemplate/update", new { controller = "emailtemplates", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("emailtemplate-delete", "emailtemplate/delete", new { controller = "emailtemplates", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("emailtemplate-templatetokens", "emailtemplate/getemailtemplatetokens", new { controller = "emailtemplates", action = "getemailtemplatetokens" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("emailtemplate-arealist", "emailtemplate/emailtemplatearealist", new { controller = "emailtemplates", action = "emailtemplatearealist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("emailtemplate-areamapperlist", "emailtemplate/emailtemplateareamapperlist/{portalId}", new { controller = "emailtemplates", action = "emailtemplateareamapperlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("emailtemplate-areamapperdelete", "emailtemplate/deletetemplateareaconfiguration", new { controller = "emailtemplates", action = "deleteemailtemplateareaconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("emailtemplate-areamappercreateupdate", "emailtemplate/createupdatetemplateareaconfiguration", new { controller = "emailtemplates", action = "saveemailtemplateareaconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Inventory SKU 
            config.Routes.MapHttpRoute("skuinventory-create", "skuinventory/create", new { controller = "inventory", action = "addskuinventory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("skuinventory-list", "skuinventory/list", new { controller = "inventory", action = "getskuinventorylist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("skuinventory-get", "skuinventory/get/{inventoryId}", new { controller = "inventory", action = "getskuinventory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("skuinventory-update", "skuinventory/update", new { controller = "inventory", action = "updateskuinventory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("skuinventory-delete", "skuinventory/delete", new { controller = "inventory", action = "deleteskuinventory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            // Digital Asset.
            config.Routes.MapHttpRoute("downloadableproduct-create", "inventory/adddownloadableproductkeys", new { controller = "inventory", action = "adddownloadableproductkeys" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("downloadableproduct-list", "inventory/downloadableproductkeylist", new { controller = "inventory", action = "getdownloadableproductkeylist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("product-deletedownloadableproductkeys", "inventory/deletedownloadableproductkeys", new { controller = "inventory", action = "deletedownloadableproductkeys" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("product-updatedownloadableproductkey", "inventory/updatedownloadableproductkey", new { controller = "inventory", action = "updatedownloadableproductkey" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });


            //Warehouse
            config.Routes.MapHttpRoute("warehouse-list", "warehouse/list", new { controller = "warehouse", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("warehouse-create", "warehouse/create", new { controller = "warehouse", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("warehouse-get", "warehouse/{warehouseId}", new { controller = "warehouse", action = "getwarehouse" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("warehouse-update", "warehouse/update", new { controller = "warehouse", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("warehouse-delete", "warehouse/delete", new { controller = "warehouse", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Warehouse: Associate inventory
            config.Routes.MapHttpRoute("associatedinventory-list", "warehouse/associatedinventory/list", new { controller = "warehouse", action = "getassociatedinventorylist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Dynamic Content
            config.Routes.MapHttpRoute("DynamicContent-GetEditorFormats", "geteditorformats/{portalId}", new { controller = "DynamicContent", action = "GetEditorFormats" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalId = @"^\d+$" });

            //Web Site Configuration
            config.Routes.MapHttpRoute("website-portallist", "websitelogo/portallist", new { controller = "website", action = "getportallist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("website-getlogo", "websitelogo/{portalId}", new { controller = "website", action = "getwebsitelogodetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalId = @"^\d+$" });
            config.Routes.MapHttpRoute("website-savewebsitelogo", "websitelogo/savewebsitelogo", new { controller = "website", action = "savewebsitelogo" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("website-getassociatedcatalog", "getassociatedcatalog/{portalId}", new { controller = "website", action = "getassociatedcatalogid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalId = @"^\d+$" });
            config.Routes.MapHttpRoute("website-publish", "publish/{portalId}", new { controller = "website", action = "publish" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalId = @"^\d+$" });
            config.Routes.MapHttpRoute("website-publish_new", "PublishWithPreview/{portalId}/{targetPublishState}/{publishContent}/{takeFromDraftFirst}", new { controller = "website", action = "PublishWithPreview" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalId = @"^\d+$" });
            config.Routes.MapHttpRoute("website-getwidgetidbycode", "website/{widgetCode}", new { controller = "website", action = "getwidgetidbycode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Web Site Configuration-content Page Routes
            config.Routes.MapHttpRoute("contentpage-create", "contentpage/createcontentpage", new { controller = "contentpage", action = "createcontentpage" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("contentpage-contentpagelist", "contentpage/contentpagelist", new { controller = "contentpage", action = "getcontentpagelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("contentpage-get", "contentpage/getcontentpage", new { controller = "contentpage", action = "getcontentpage" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("contentpage-update", "contentpage/updatecontentpage", new { controller = "contentpage", action = "updatecontentpage" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("contentpage-delete", "contentpage/deletecontentpage", new { controller = "contentpage", action = "DeletecontentPage" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("contentpage-tree", "contentpage/gettree", new { controller = "contentpage", action = "gettree" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("contentpage-renamefolder", "contentpage/renamefolder", new { controller = "contentpage", action = "renamefolder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("contentpage-addfolder", "contentpage/addfolder", new { controller = "contentpage", action = "addfolder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("contentpage-deletefolder", "contentpage/deletefolder", new { controller = "contentpage", action = "deletefolder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("contentpage-contentpageslist", "contentpage/contentpageslist", new { controller = "webstorecontentpage", action = "getcontentpageslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("contentpage-movecontentpagesfolder", "contentpage/movecontentpagesfolder", new { controller = "contentpage", action = "movecontentpagesfolder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("contentpage-movepagetofolder", "contentpage/move", new { controller = "contentpage", action = "movepage" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("contentpage-publish", "contentpage/publishcontentpages", new { controller = "contentpage", action = "PublishContentPages" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("contentpage-publishwithpreview", "contentpage/publishcontentpagewithpreview", new { controller = "contentpage", action = "publishcontentpagewithpreview" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Web Site Configuration-content Page Template Routes
            config.Routes.MapHttpRoute("contentpagetemplate-contentpagetemplatelist", "contentpage/contentpagetemplatelist", new { controller = "contentpage", action = "getcontentpagetemplatelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("contentpagetemplate-get", "contentpage/getcontentpagetemplate/{cmscontentpagetemplateId}", new { controller = "contentpage", action = "getcontentpagetemplate" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Store Locator
            config.Routes.MapHttpRoute("storelocator-list", "storelocator/list", new { controller = "storelocator", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("storelocator-create", "storelocator/create", new { controller = "storelocator", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("storelocator-get", "storelocator/get/{storeId}", new { controller = "storelocator", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("storelocator-getStoreLocatorByCode", "storelocator/getStoreLocatorByCode/{storeLocationCode}", new { controller = "storelocator", action = "getStoreLocatorByCode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("storelocator-update", "storelocator/update", new { controller = "storelocator", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("storelocator-delete", "storelocator/delete/{isDeleteByCode}", new { controller = "storelocator", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //WebSite : CMS Widget Category Association
            config.Routes.MapHttpRoute("cmswidgetcategory-getunassociatedcategory", "cmswidgetcategory/getunassociatedcategory", new { controller = "cmswidgetconfiguration", action = "GetUnAssociatedCategory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("cmswidgetcategory-getassociatedcategory", "cmswidgetcategory/getassociatedcategory", new { controller = "cmswidgetconfiguration", action = "GetAssociatedCategory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("cmswidgetcategory-removecategories", "cmswidgetcategory/deletecategories", new { controller = "cmswidgetconfiguration", action = "deletecategories" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cmswidgetcategory-associatecategory", "cmswidgetcategory/associatecategories", new { controller = "cmswidgetconfiguration", action = "associatecategories" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cmswidgetcategory-updatecategory", "cmswidgetcategory/updatecmswidgetcategory", new { controller = "cmswidgetconfiguration", action = "updatecmswidgetcategory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });


            //WebSite : CMS Widget Brand Association
            config.Routes.MapHttpRoute("cmswidgetbrand-getunassociatedbrand", "cmswidgetbrand/getunassociatedbrand", new { controller = "cmswidgetconfiguration", action = "getunassociatedbrand" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("cmswidgetbrand-getassociatedbrand", "cmswidgetbrand/getassociatedbrand", new { controller = "cmswidgetconfiguration", action = "getassociatedbrand" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("cmswidgetbrand-removebrands", "cmswidgetbrand/deletebrands", new { controller = "cmswidgetconfiguration", action = "deletebrands" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cmswidgetbrand-associatebrand", "cmswidgetbrand/associatebrands", new { controller = "cmswidgetconfiguration", action = "associatebrands" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cmswidgetbrand-updatebrand", "cmswidgetbrand/updatecmswidgetbrand", new { controller = "cmswidgetconfiguration", action = "updatecmswidgetbrand" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });



            //Web Site Configuration- CMSWidgetProduct
            config.Routes.MapHttpRoute("associatedproduct-list", "cmswidgetproduct/associatedproduct/list", new { controller = "CMSWidgetConfiguration", action = "getassociatedproductlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("unassociatedproduct-list", "cmswidgetproduct/unassociatedproduct/list", new { controller = "CMSWidgetConfiguration", action = "getunassociatedproductlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("cmsofferpageproduct-associateproduct", "cmswidgetproduct/associateproduct", new { controller = "CMSWidgetConfiguration", action = "associateproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cmsofferpageproduct-unassociateproduct", "cmswidgetproduct/unassociateproduct", new { controller = "CMSWidgetConfiguration", action = "unassociateproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cmsassociateproduct-updateassociateproduct", "cmsassociateproduct/updatecmsassociateproduct", new { controller = "cmswidgetconfiguration", action = "updatecmsassociateproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //WebSite : Product Page
            config.Routes.MapHttpRoute("website-getportalproductpagelist", "website/getportalproductpagelist/{portalId}", new { controller = "website", action = "getportalproductpagelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("website-updateportalproductpage", "website/updateportalproductpage", new { controller = "website", action = "updateportalproductpage" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //WebSite : Widgets Configuration - CMS Widget Slider Banner
            config.Routes.MapHttpRoute("websitewidgetsliderbanner-getcmswidgetsliderbanner", "websitewidgetsliderbanner/getcmswidgetsliderbanner", new { controller = "cmswidgetconfiguration", action = "getcmswidgetsliderbanner" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("websitewidgetsliderbanner-savecmswidgetsliderbanner", "websitewidgetsliderbanner/savecmswidgetsliderbanner", new { controller = "cmswidgetconfiguration", action = "savecmswidgetsliderbanner" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //WebSite : Widgets Configuration - CMS Widget Content Container
            config.Routes.MapHttpRoute("websitewidgetcontainer-savecmswidgetcontainer", "websitewidgetsliderbanner/savecmscontainerdetails", new { controller = "cmswidgetconfiguration", action = "savecmscontainerdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //WebSite : Widgets Configuration - CMS Link Widget configuration
            config.Routes.MapHttpRoute("linkwidgetconfiguration-create", "linkwidgetconfiguration/create", new { controller = "cmswidgetconfiguration", action = "createupdatelinkwidgetconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("linkwidgetconfiguration-list", "linkwidgetconfiguration/list", new { controller = "cmswidgetconfiguration", action = "linkwidgetconfigurationlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("linkwidgetconfiguration-delete", "linkwidgetconfiguration/delete/{localeId}", new { controller = "cmswidgetconfiguration", action = "deletelinkwidgetconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Sliders
            config.Routes.MapHttpRoute("slider-list", "slider/slider/list", new { controller = "slider", action = "sliderlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("slider-create", "slider/createslider", new { controller = "slider", action = "createslider" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("slider-get", "slider/getslider/{cmssliderid}", new { controller = "slider", action = "getslider" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("slider-update", "slider/updateslider", new { controller = "slider", action = "updateslider" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("slider-delete", "slider/deleteslider", new { controller = "slider", action = "deleteslider" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("slider-publish", "slider/publish", new { controller = "slider", action = "publishslider" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("slider-publishwithpreview", "slider/publishwithpreview", new { controller = "slider", action = "publishsliderwithpreview" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Url redirect Routes
            config.Routes.MapHttpRoute("urlredirect-create", "urlredirect/create", new { controller = "urlredirect", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("urlredirect-list", "urlredirect/geturlredirectlist", new { controller = "urlredirect", action = "geturlredirectlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("urlredirect-get", "urlredirect/geturlredirect", new { controller = "urlredirect", action = "geturlredirect" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("urlredirect-update", "urlredirect/update", new { controller = "urlredirect", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("urlredirect-delete", "urlredirect/delete", new { controller = "urlredirect", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Text Widget Configuration
            config.Routes.MapHttpRoute("textwidgetconfiguration-list", "textwidgetconfiguration/list", new { controller = "cmswidgetconfiguration", action = "textwidgetconfigurationlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("textwidgetconfiguration-get", "textwidgetconfiguration/{textwidgetConfigurationId}", new { controller = "cmswidgetconfiguration", action = "gettextwidgetconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), textWidgetConfigurationId = @"^\d+$" });
            config.Routes.MapHttpRoute("textwidgetconfiguration-create", "textwidgetconfiguration/create", new { controller = "cmswidgetconfiguration", action = "createtextwidgetconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("textwidgetconfiguration-update", "textwidgetconfiguration/update", new { controller = "cmswidgetconfiguration", action = "updatetextwidgetconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("mediawidgetconfiguration-saveandupdate", "mediawidgetconfiguration/saveandupdate", new { controller = "cmswidgetconfiguration", action = "saveandupdatemediawidgetconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("removewidgetconfiguration-delete", "cmswidgetConfiguration/removewidgetdatafromcontentpageconfiguration", new { controller = "cmswidgetconfiguration", action = "removewidgetdatafromcontentpageconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Form Widget Configuration
            config.Routes.MapHttpRoute("formwidgetconfiguration-create", "formwidgetconfiguration/create", new { controller = "CMSWidgetConfiguration", action = "CreateFormWidgetConfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("formwidgetconfiguration-list", "formwidgetconfiguration/list", new { controller = "CMSWidgetConfiguration", action = "FormWidgetConfigurationList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("formwidgetconfiguration-update", "formwidgetconfiguration/update", new { controller = "CMSWidgetConfiguration", action = "updateformwidgetconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            config.Routes.MapHttpRoute("formwidgetemailconfiguration-get", "formwidgetemailconfiguration/getformwidgetemailconfiguration/{cmscontentpagesid}", new { controller = "CMSWidgetConfiguration", action = "getFormWidgetemailConfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), cmscontentpagesid = @"^\d+$" });
            config.Routes.MapHttpRoute("formwidgetemailconfiguration-create", "formwidgetemailconfiguration/create", new { controller = "CMSWidgetConfiguration", action = "createFormWidgetemailConfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("formwidgetemailconfiguration-update", "formwidgetemailconfiguration/update", new { controller = "CMSWidgetConfiguration", action = "updateformwidgetemailconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            //Search widget configuration
            config.Routes.MapHttpRoute("searchwidgetconfiguration-get", "getsearchwidgetconfiguration", new { controller = "cmswidgetconfiguration", action = "getsearchwidgetconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("searchwidgetconfiguration-create", "searchwidgetconfiguration/create", new { controller = "cmswidgetconfiguration", action = "createsearchwidgetconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("searchwidgetconfiguration-update", "searchwidgetconfiguration/update", new { controller = "cmswidgetconfiguration", action = "updatesearchwidgetconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //ERP Configurator     
            config.Routes.MapHttpRoute("erpconfigurator-list", "erpconfigurator/list", new { controller = "erpconfigurator", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("erpconfigurator-getallerpconfiguratorclassesnotindatabase", "erpconfigurator/getallerpconfiguratorclassesnotindatabase", new { controller = "erpconfigurator", action = "getallerpconfiguratorclassesnotindatabase" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("erpconfigurator-create", "erpconfigurator/create", new { controller = "erpconfigurator", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("erpconfigurator-get", "erpconfigurator/{erpconfiguratorId}", new { controller = "erpconfigurator", action = "geterpconfigurator" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("erpconfigurator-update", "erpconfigurator/update", new { controller = "erpconfigurator", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("erpconfigurator-delete", "erpconfigurator/delete", new { controller = "erpconfigurator", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("erpconfigurator-enabledisableerpconfigurator", "erpconfigurator/enabledisableerpconfigurator/{eRPConfiguratorId}/{isActive}", new { controller = "erpconfigurator", action = "enabledisableerpconfigurator" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("erpconfigurator-getactiveerpclassname", "getactiveerpclassname", new { controller = "erpconfigurator", action = "getactiveerpclassname" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("erpconfigurator-geterpclassname", "geterpclassname", new { controller = "erpconfigurator", action = "geterpclassname" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //ERP Touch Point Configuration
            config.Routes.MapHttpRoute("touchpointconfiguration-list", "touchpointconfiguration/list", new { controller = "touchpointconfiguration", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("touchpointconfiguration-triggertaskscheduler", "touchpointconfiguration/triggertaskscheduler/{connectortouchpoints}", new { controller = "touchpointconfiguration", action = "triggertaskscheduler" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("touchpointconfiguration-getschedulerloglist", "touchpointconfiguration/getschedulerloglist", new { controller = "touchpointconfiguration", action = "getschedulerloglist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("touchpointconfiguration-sendscheduleractivitylog", "touchpointconfiguration/sendscheduleractivitylog", new { controller = "touchpointconfiguration", action = "sendscheduleractivitylog" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //ERP Task Scheduler
            config.Routes.MapHttpRoute("erptaskscheduler-list", "erptaskscheduler/list", new { controller = "erptaskscheduler", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("erptaskscheduler-create", "erptaskscheduler/create", new { controller = "erptaskscheduler", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("erptaskscheduler-delete", "erptaskscheduler/delete", new { controller = "erptaskscheduler", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("erptaskscheduler-triggeraction", "erptaskscheduler/triggerschedulertask/{erptaskschedulerid}", new { controller = "erptaskscheduler", action = "triggerschedulertask" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("erptaskscheduler-get", "erptaskscheduler/{erptaskschedulerid}", new { controller = "erptaskscheduler", action = "geterptaskscheduler" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("erptaskscheduler-update", "erptaskscheduler/update", new { controller = "erptaskscheduler", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("erptaskscheduler-getactiveerptaskschedulerid", "erptaskscheduler/getscheduleridbytouchpointname", new { controller = "erptaskscheduler", action = "getscheduleridBytouchpointname" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("erptaskscheduler-enabledisabletaskscheduler", "erptaskscheduler/enabledisabletaskscheduler/{erptaskschedulerid}/{isactive}", new { controller = "erptaskscheduler", action = "enabledisabletaskscheduler" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("erpconnector-erpconnectorcontrols", "erpconnector/erpconnectorcontrols", new { controller = "erpConnector", action = "geterpconnectorcontrols" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("erpconnector-createerpcontroldata", "erpconnector/createerpcontroldata", new { controller = "erpconnector", action = "createerpcontroldata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            #region WebStore

            //Message Routes
            config.Routes.MapHttpRoute("webstoremessage-getmessage", "webstoremessage/get", new { controller = "webstoremessage", action = "getmessage" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("webstoremessage-list", "webstoremessage/list/{localeId}", new { controller = "webstoremessage", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Portal Routes
            config.Routes.MapHttpRoute("webstoreportal-get", "webstoreportal/getportal/{portalId}", new { controller = "webstoreportal", action = "getportal" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalId = @"^\d+$" });
            config.Routes.MapHttpRoute("webstoreportal-getfromapplicationtype", "webstoreportal/getportal/{portalId}/{localeId}/{applicationType}", new { controller = "webstoreportal", action = "getportalfromapplicationtype" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalId = @"^\d+$", localeId = @"^\d+$" });

            //Portal Routes
            config.Routes.MapHttpRoute("webstoreportal-get-bydomain", "webstoreportal/getportal/{domainName}", new { controller = "webstoreportal", action = "getportalbydomain" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });


            //WebStore Product
            config.Routes.MapHttpRoute("webstoreproduct-list", "webstoreproducts/list", new { controller = "webstoreproduct", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("webstoreproduct-get", "webstoreproducts/get/{productId}", new { controller = "webstoreproduct", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("webstoreproduct-getassociatedproduct", "webstoreproducts/getassociatedproducts", new { controller = "webstoreproduct", action = "getassociatedproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("webstoreproduct-getproducthighlights", "webstoreproducts/getproducthighlights/{productId}/{localeId}", new { controller = "webstoreproduct", action = "getproducthighlights" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("webstoreproduct-sendcomparedproductmail", "webstoreproducts/sendcomparedproductmail", new { controller = "webstoreproduct", action = "sendcomparedproductmail" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("webstoreproduct-sendmailtofriend", "webstoreproducts/SendMailToFriend", new { controller = "webstoreproduct", action = "SendMailToFriend" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //WebStore Case Request
            config.Routes.MapHttpRoute("webstorecaserequest-create", "webstorecaserequest/createcontactus", new { controller = "webstorecaserequest", action = "createcontactus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("webstorecaserequest-list", "webstorecaserequest/list", new { controller = "webstorecaserequest", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("webstorecaserequest-createcaserequest", "webstorecaserequest/createcaserequest", new { controller = "webstorecaserequest", action = "createcaserequest" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("webstorecaserequest-get", "webstorecaserequest/getcaserequest/{caseRequestId}", new { controller = "webstorecaserequest", action = "getcaserequest" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("webstorecaserequest-update", "webstorecaserequest/updatecaserequest", new { controller = "webstorecaserequest", action = "updatecaserequest" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("webstorecaserequest-caseprioritylist", "webstorecaserequest/caseprioritylist", new { controller = "webstorecaserequest", action = "getcaseprioritylist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("webstorecaserequest-casestatuslist", "webstorecaserequest/casestatuslist", new { controller = "webstorecaserequest", action = "getcasestatuslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("webstorecaserequest-casetypelist", "webstorecaserequest/casetypelist", new { controller = "webstorecaserequest", action = "Getcasetypelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("webstorecaserequest-replyCustomer", "webstorecaserequest/replycustomer", new { controller = "webstorecaserequest", action = "replycustomer" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });



            //WebStore Locator
            config.Routes.MapHttpRoute("webstorelocator-list", "webstorelocator/list", new { controller = "webstorelocator", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Account Address
            config.Routes.MapHttpRoute("webstoreaccount-createaccountaddress", "webstoreaccount/createaccountaddress", new { controller = "webstoreaccount", action = "createaccountaddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("webstoreaccount-getuseraddresslist", "webstoreaccount/getuseraddresslist", new { controller = "webstoreaccount", action = "getuseraddresslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("webstoreaccount-getaddress", "webstoreaccount/getaddress/{addressId}", new { controller = "webstoreaccount", action = "getaddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), addressId = @"^\d+$" });
            config.Routes.MapHttpRoute("webstoreaccount-updateaccountaddress", "webstoreaccount/updateaccountaddress", new { controller = "webstoreaccount", action = "updateaccountaddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("webstoreaccount-deleteaddress", "webstoreaccount/deleteaddress/{addressId}/{userId}", new { controller = "webstoreaccount", action = "deleteaddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete), addressId = @"^\d+$", userId = @"^\d+$" });

            //Webstore Blog News
            config.Routes.MapHttpRoute("webstoreblognews-blognewslistforwebstore", "blognews/blognewslistforwebstore", new { controller = "webstoreblognews", action = "getblognewslistforwebstore" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("webstoreblognews-blognewsforwebstore", "blognews/blognewsforwebstore/{blogNewsId}/{localeId}/{portalId}/{activationDate}", new { controller = "webstoreblognews", action = "getblognewsforwebstore" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("webstoreblognews-savecomments", "blognews/savecomments", new { controller = "webstoreblognews", action = "savecomments" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("webstoreblognews-getusercommentlist", "blognews/getusercommentlist", new { controller = "webstoreblognews", action = "getusercommentlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            #endregion

            //WebStore Category Routes
            config.Routes.MapHttpRoute("webstorecategory-getcategorydetails", "webstorecategory/getcategorydetails", new { controller = "webstorecategory", action = "getcategorydetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //WebStore Widget Routes
            config.Routes.MapHttpRoute("webstorewidget-getslider", "webstorewidget/getslider/{key}", new { controller = "webstorewidget", action = "getslider" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("webstorewidget-getproducts", "webstorewidget/getproducts/{key}", new { controller = "webstorewidget", action = "getproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("webstorewidget-getlinkwidget", "webstorewidget/getlinkwidget/{key}", new { controller = "webstorewidget", action = "getlinkwidget" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("webstorewidget-getcategories", "webstorewidget/getcategories/{key}", new { controller = "webstorewidget", action = "getcategories" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("webstorewidget-getlinkproductlist", "webstorewidget/getlinkproductlist/{key}", new { controller = "webstorewidget", action = "getlinkproductlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("webstorewidget-gettagmanager", "webstorewidget/gettagmanager/{key}", new { controller = "webstorewidget", action = "gettagmanager" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("webstorewidget-getmediawidgetdetails", "webstorewidget/getmediawidgetdetails/{key}", new { controller = "webstorewidget", action = "getmediawidgetdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("webstorewidget-getcontainer", "webstorewidget/getcontainer", new { controller = "webstorewidget", action = "getcontainer" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            config.Routes.MapHttpRoute("webstorewidget-getbrands", "webstorewidget/getbrands/{key}", new { controller = "webstorewidget", action = "getbrands" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("webstorewidget-getforms", "webstorewidget/getformconfigurationbycmsmappingid", new { controller = "webstorewidget", action = "getformconfigurationbycmsmappingid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("webstorewidget-getsearchwidgetdata", "webstorewidget/getsearchwidgetdata", new { controller = "webstorewidget", action = "getsearchwidgetdata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            
            //Search routes.           
            config.Routes.MapHttpRoute("search-getseourldetails", "search/getseourldetails/{*seoUrl}", new { controller = "search", action = "getseourldetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("search-saveboostvalues", "search/saveboostvalues", new { controller = "search", action = "saveboostvalues" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("search-deleteboostvalue", "search/deleteboostvalue", new { controller = "search", action = "deleteboostvalue" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("search-getglobalproductboostlist", "search/getglobalproductboostlist", new { controller = "search", action = "getglobalproductboostlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("search-getglobalproductcategoryboostlist", "search/getglobalproductcategoryboostlist", new { controller = "search", action = "getglobalproductcategoryboostlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("search-getfieldboostlist", "search/getfieldboostlist", new { controller = "search", action = "getfieldboostlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("search-getproductindex", "search/getportalindexdata", new { controller = "search", action = "getsearchindexdata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("search-insertcreateindexdata", "search/insertcreateindexdata", new { controller = "search", action = "insertcreateindexdata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("search-createindex", "search/createindex/{indexName}/{revisionType}/{isPreviewProductionEnabled}/{isPublishDraftProductsOnly}/{portalId}/{searchIndexMonitorId}/{searchIndexServerStatusId}/{newIndexName}", new { controller = "search", action = "createindex" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("search-createindexwitholdname", "search/createindex/{indexName}/{revisionType}/{isPreviewProductionEnabled}/{isPublishDraftProductsOnly}/{portalId}/{searchIndexMonitorId}/{searchIndexServerStatusId}", new { controller = "search", action = "createindex" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("search-getsearchindexmonitorlist", "search/getsearchindexmonitorlist", new { controller = "search", action = "getsearchindexmonitorlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("search-getsearchindexserverstatuslist", "search/getsearchindexserverstatuslist", new { controller = "search", action = "getsearchindexserverstatuslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("search-createsearchsynonyms", "search/createsearchsynonyms", new { controller = "search", action = "createsearchsynonyms" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("search-getsearchsynonyms", "search/getsearchsynonyms/{searchSynonymsId}", new { controller = "search", action = "getsearchsynonyms" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("search-updatesearchsynonyms", "search/updatesearchsynonyms", new { controller = "search", action = "updatesearchsynonyms" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("search-getcatalogkeywordsredirectlist", "search/getcatalogkeywordsredirectlist", new { controller = "search", action = "getcatalogkeywordsredirectlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("search-getsearchsynonymslist", "search/getsearchsynonymslist", new { controller = "search", action = "getsearchsynonymslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("search-deletesearchsynonyms", "search/deletesearchsynonyms", new { controller = "search", action = "deletesearchsynonyms" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("search-createsearchkeywordsredirect", "search/createsearchkeywordsredirect", new { controller = "search", action = "createsearchkeywordsredirect" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("search-getsearchkeywordsredirect", "search/getsearchkeywordsredirect/{searchKeywordsRedirectId}", new { controller = "search", action = "getsearchkeywordsredirect" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("search-updatesearchkeywordsredirect", "search/updatesearchkeywordsredirect", new { controller = "search", action = "updatesearchkeywordsredirect" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("search-deletesearchkeywordsredirect", "search/deletesearchkeywordsredirect", new { controller = "search", action = "deletesearchkeywordsredirect" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("search-writesearchfile", "search/writesynonymsfile/{publishCatalogId}/{isSynonymsFile}", new { controller = "search", action = "WriteSearchFile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("search-deleteindex", "search/deleteindex/{catalogIndexId}", new { controller = "search", action = "deleteindex" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), catalogIndexId = @"^\d+$" });
            //To do: changes required if needed
            #region Elastic search  
            config.Routes.MapHttpRoute("search-getproductdetailsbysku", "search/getproductdetailsbysku", new { controller = "search", action = "getproductdetailsbysku" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("search-fulltextsearch", "search/fulltextsearch", new { controller = "search", action = "fulltextsearch" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("search-facetsearch", "search/facetsearch", new { controller = "search", action = "facetsearch" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("search-getkeywordsearchsuggestion", "search/getkeywordsearchsuggestion", new { controller = "search", action = "getkeywordsearchsuggestion" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cmssearchconfiguration-fulltextcontentpagesearch", "cmssearchconfiguration/fulltextcontentpagesearch", new { controller = "cmssearchconfiguration", action = "fulltextcontentpagesearch" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            #endregion

            //WebStore Wishlist routes.
            config.Routes.MapHttpRoute("webstoreproductwishlist-addtowishlist", "wishlist/addtowishlist", new { controller = "wishlist", action = "addtowishlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("webstoreproductwishlist-deletewishlist", "wishlist/deletewishlist/{wishListId}", new { controller = "wishlist", action = "deletewishlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            //Highlight
            config.Routes.MapHttpRoute("highlight-list", "highlight/list", new { controller = "highlight", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("highlight-create-highlight", "highlight/create", new { controller = "highlight", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("highlight-get", "gethighlight/{highlightId}/{productId}", new { controller = "highlight", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("highlight-update", "highlight/update", new { controller = "highlight", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("highlight-delete", "highlight/delete", new { controller = "highlight", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("highlight-codelist", "highlight/highlightcodelist/{attributeCode}", new { controller = "highlight", action = "gethighlightcodelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("highlight-getbycode", "gethighlightbycode/{highlightCode}", new { controller = "highlight", action = "getbycode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Highlight Type
            config.Routes.MapHttpRoute("highlighttype-list", "highlighttype/list", new { controller = "highlight", action = "gethighlighttypelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Highlight Product
            config.Routes.MapHttpRoute("associatehighlightproduct-product", "highlight/assocaitehighlightproduct", new { controller = "highlight", action = "associateandunassociateproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("highlight-unassociatehighlightproduct", "highlight/unassociatehighlightproduct", new { controller = "highlight", action = "unassociatehighlightproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Vendor
            config.Routes.MapHttpRoute("vendor-list", "vendor/list", new { controller = "vendor", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("vendor-create", "vendor/create", new { controller = "vendor", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("vendor-get", "vendor/getvendor/{PimVendorId}", new { controller = "vendor", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("vendor-update", "vendor/update", new { controller = "vendor", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("vendor-delete", "vendor/delete", new { controller = "vendor", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("vendor-codelist", "vendor/vendorcodelist/{attributeCode}", new { controller = "vendor", action = "getvendorcodelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("vendor-associateproduct", "vendor/associatevendorproduct", new { controller = "vendor", action = "associateandunassociateproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("vendor-activeinactivevendor", "vendor/activeinactivevendor", new { controller = "vendor", action = "activeinactivevendor" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Report
            config.Routes.MapHttpRoute("report-list", "report/list", new { controller = "myreports", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("report-getexportdata", "report/getexportdata/{dynamicreporttype}", new { controller = "myreports", action = "getexportdata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("report-getfilters", "report/getfilters", new { controller = "myreports", action = "getfilters" }, new { HttpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("report-getattributecolumns", "report/getattributecolumns/{product}", new { controller = "myreports", action = "GetAttributeColumns" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("report-generatedynamicreport", "report/generatedynamicreport", new { controller = "myreports", action = "GenerateDynamicReport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("report-deletedynamicreport", "report/deletedynamicreport", new { controller = "myreports", action = "deletedynamicreport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("report-getcustomreport", "report/getcustomreport/{customReportId}", new { controller = "myreports", action = "getcustomreport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //License
            config.Routes.MapHttpRoute("license-getlicenseinformation", "license/getlicenseinformation", new { controller = "license", action = "getlicenseinformation" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            // Diagnostics routes
            config.Routes.MapHttpRoute("diagnostics-checkemailaccount", "diagnostics/checkemailaccount", new { controller = "diagnostics", action = "checkemailaccount" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("diagnostics-getproductversiondetails", "diagnostics/getproductversiondetails", new { controller = "diagnostics", action = "getproductversiondetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("diagnostics-emaildiagnostics", "diagnostics/emaildiagnostics", new { controller = "diagnostics", action = "emaildiagnostics" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("diagnostics-CheckService", "diagnostics/CheckService/{ServiceName}", new { controller = "diagnostics", action = "CheckService" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("diagnostics-getdiagnosticslist", "diagnostics/getdiagnosticslist", new { controller = "diagnostics", action = "getdiagnosticslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });


            // CMS - BlogNews
            config.Routes.MapHttpRoute("blognews-create", "blognews/createblognews", new { controller = "blognews", action = "createblognews" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("blognews-list", "blognews/blognewslist", new { controller = "blognews", action = "getblognewslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("blognews-get", "blognews/getblognews/{blognewsId}/{localeId}", new { controller = "blognews", action = "getblognews" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("blognews-update", "blognews/updateblognews", new { controller = "blognews", action = "updateblognews" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("blognews-delete", "blognews/deleteblognews", new { controller = "blognews", action = "deleteblognews" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("blognews-getblognewscommentlist", "blognews/getblognewscommentlist", new { controller = "blognews", action = "getblognewscommentlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("blognews-updateblognewscomment", "blognews/updateblognewscomment", new { controller = "blognews", action = "updateblognewscomment" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("blognews-deleteblognewscomment", "blognews/deleteblognewscomment", new { controller = "blognews", action = "deleteblognewscomment" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("blognews-activedeactiveblognews", "blognews/activatedeactivateblognews", new { controller = "blognews", action = "activatedeactivateblognews" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("blognews-approvedisapproveblognewscomment", "blognews/approvedisapproveblognewscomment", new { controller = "blognews", action = "approvedisapproveblognewscomment" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("blognews-publishblognews", "blognews/publishblognews", new { controller = "blognews", action = "publishblognews" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Global Attributes - Route
            config.Routes.MapHttpRoute("globalattribute-create", "globalattribute/create", new { controller = "globalattribute", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("globalattribute-savelocales", "globalattribute/savelocales", new { controller = "globalattribute", action = "savelocales" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("globalattribute-savedefaultvalues", "globalattribute/savedefaultvalues", new { controller = "globalattribute", action = "savedefaultvalues" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("globalattribute-inputvalidations", "globalattribute/inputvalidations/{typeId}/{attributeId}", new { controller = "globalattribute", action = "inputvalidations" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), typeId = @"^\d+$" });
            config.Routes.MapHttpRoute("globalattribute-list", "globalattribute/list", new { controller = "globalattribute", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("globalattribute-get", "globalattribute/{id}", new { controller = "globalattribute", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"^\d+$" });
            config.Routes.MapHttpRoute("globalattribute-update", "globalattribute/update", new { controller = "globalattribute", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("globalattribute-delete", "globalattribute/delete", new { controller = "globalattribute", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("globalattribute-getattributelocale", "globalattribute/getattributelocale/{globalAttributeId}", new { controller = "globalattribute", action = "getattributelocale" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), globalAttributeId = @"^\d+$" });
            config.Routes.MapHttpRoute("globalattribute-getdefaultvalues", "globalattribute/getdefaultvalues/{globalAttributeId}", new { controller = "globalattribute", action = "getdefaultvalues" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), globalAttributeId = @"^\d+$" });
            config.Routes.MapHttpRoute("globalattribute-isattributecodeexist", "globalattribute/isattributecodeexist/{attributeCode}", new { controller = "globalattribute", action = "isattributecodeexist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("globalattribute-deletedefaultvalues", "globalattribute/deletedefaultvalues/{defaultvalueId}", new { controller = "globalattribute", action = "deletedefaultvalues" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete), defaultvalueId = @"^\d+$" });
            config.Routes.MapHttpRoute("globalattribute-isattributevalueunique", "globalattribute/isattributevalueunique", new { controller = "globalattribute", action = "isattributevalueunique" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Global Attributes Group - Route
            config.Routes.MapHttpRoute("globalattributegroup-list", "globalattributegroup/list", new { controller = "globalattributegroup", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("globalattributegroup-create", "globalattributegroup/create", new { controller = "globalattributegroup", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("globalattributegroup-get", "globalattributegroup/get/{id}", new { controller = "globalattributegroup", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"^\d+$" });
            config.Routes.MapHttpRoute("globalattributegroup-attributegrouplocales", "globalattributegroup/attributegrouplocales/{attributeGroupId}", new { controller = "globalattributegroup", action = "getattributegrouplocale" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("globalattributegroup-delete", "globalattributegroup/delete", new { controller = "globalattributegroup", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("globalattributegroup-update", "globalattributegroup/update", new { controller = "globalattributegroup", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("globalattributegroup-assignedattributes", "globalattributegroup/assignedattributes", new { controller = "globalattributegroup", action = "assignedattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("globalattributegroup-unassignedattributes", "globalattributegroup/unassignedattributes", new { controller = "globalattributegroup", action = "unassignedattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("globalattributegroup-associateattributes", "globalattributegroup/associateattributes", new { controller = "globalattributegroup", action = "associateattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("globalattributegroup-removeassociatedattributes", "globalattributegroup/removeassociatedattributes", new { controller = "globalattributegroup", action = "removeassociatedattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("globalattributegroup-updateattributedisplayorder", "globalattributegroup/updateattributedisplayorder", new { controller = "globalattributegroup", action = "updateattributedisplayorder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            //Global Attribute Group Entity- Route
            config.Routes.MapHttpRoute("globalattributeentity-getallentitylist", "globalattributeentity/getallentitylist", new { controller = "globalattributeentity", action = "getallentitylist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("globalattributeentity-getassignedentityattributegroups", "globalattributeentity/getassignedentityattributegroups", new { controller = "globalattributeentity", action = "getassignedentityattributegroups" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("globalattributeentity-getunassignedentityattributegroups", "globalattributeentity/getunassignedentityattributegroups", new { controller = "globalattributeentity", action = "getunassignedentityattributegroups" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("globalattributeentity-associateattributeentitytogroups", "globalattributeentity/associateattributeentitytogroups", new { controller = "globalattributeentity", action = "associateattributeentitytogroups" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("globalattributeentity-unassociateentitygroups", "globalattributeentity/unassociateentitygroups/{entityId}/{groupId}", new { controller = "globalattributeentity", action = "unassociateentitygroups" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("globalattributeentity-updateattributegroupdisplayorder", "globalattributeentity/updateattributegroupdisplayorder", new { controller = "globalattributeentity", action = "updateattributegroupdisplayorder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("globalattributeentity-getentityattributedetails", "globalattributeentity/getentityattributedetails/{entityId}/{entityType}", new { controller = "globalattributeentity", action = "getentityattributedetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), entityId = @"^\d+$" });
            config.Routes.MapHttpRoute("globalattributeentity-saveentityattributedetails", "globalattributeentity/saveentityattributedetails", new { controller = "globalattributeentity", action = "saveentityattributedetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("globalattributeentity-getglobalentityattributes", "globalattributeentity/getglobalentityattributes/{entityId}/{entityType}", new { controller = "globalattributeentity", action = "getglobalentityattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), entityId = @"^\d+$" });
            config.Routes.MapHttpRoute("globalattributeentity-getglobalattributesfordefaultvariantdata", "globalattributeentity/getglobalattributesfordefaultvariantdata/{familyCode}/{entityType}", new { controller = "globalattributeentity", action = "getglobalattributesfordefaultvariantdata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("globalattributeentity-getglobalattributesforassociatedvariant", "globalattributeentity/getglobalattributesforassociatedvariant/{variantId}/{entityType}/{localeId}", new { controller = "globalattributeentity", action = "getglobalattributesforassociatedvariant" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), variantId = @"^\d+$" });


            //Global Attribute Family 
            config.Routes.MapHttpRoute("globalattributefamily-list", "globalattributefamily/list", new { controller = "GlobalAttributeFamily", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("globalattributefamily-create", "globalattributefamily/create", new { controller = "GlobalAttributeFamily", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("globalattributefamily-getassignedattributegroups", "globalattributefamily/getassignedattributegroups/{familyCode}", new { controller = "GlobalAttributeFamily", action = "GetAssignedAttributeGroups" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("globalattributefamily-getunassignedattributegroups", "globalattributefamily/getunassignedattributegroups/{familyCode}", new { controller = "GlobalAttributeFamily", action = "GetUnassignedAttributeGroups" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("globalattributefamily-assignattributegroups", "globalattributefamily/assignattributegroups/{attributeGroupIds}/{familyCode}", new { controller = "GlobalAttributeFamily", action = "AssignAttributeGroups" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post)});
            config.Routes.MapHttpRoute("globalattributefamily-assignattributegroupsbygroupcode", "globalattributefamily/assignattributegroupsbygroupcode/{groupCode}/{familyCode}", new { controller = "GlobalAttributeFamily", action = "AssignAttributeGroupsByGroupCode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("globalattributefamily-unassignattributegroups", "globalattributefamily/unassignattributegroups/{groupCode}/{familyCode}", new { controller = "GlobalAttributeFamily", action = "UnassignAttributeGroups" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post)});
            config.Routes.MapHttpRoute("globalattributefamily-getattributefamily", "globalattributefamily/getattributefamily/{familyCode}", new { controller = "GlobalAttributeFamily", action = "GetAttributeFamily" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get)});
            config.Routes.MapHttpRoute("globalattributefamily-updateattributegroupdisplayorder", "globalattributefamily/updateattributegroupdisplayorder/{groupCode}/{familyCode}/{displayOrder}", new { controller = "GlobalAttributeFamily", action = "UpdateAttributeGroupDisplayOrder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post)});
            config.Routes.MapHttpRoute("globalattributefamily-attributefamilylocales", "globalattributefamily/getattributefamilylocale/{familyCode}", new { controller = "GlobalAttributeFamily", action = "GetAttributeFamilyLocale" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("globalattributefamily-update", "globalattributefamily/update", new { controller = "globalattributefamily", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("globalattributefamily-delete", "globalattributefamily/delete", new { controller = "globalattributefamily", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("globalattributefamily-deletebyfamilycode", "globalattributefamily/deletefamilybycode/{familyCode}", new { controller = "globalattributefamily", action = "DeleteFamilyByCode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("globalattributefamily-isfamilycodeexists", "globalattributefamily/isfamilycodeexist/{familyCode}", new { controller = "globalattributefamily", action = "IsFamilyCodeExist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Form Builder
            config.Routes.MapHttpRoute("formbuilder-create", "formbuilder/create", new { controller = "formbuilder", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("formbuilder-isformcodeexist", "formbuilder/isformcodeexist/{formCode}", new { controller = "formbuilder", action = "isformcodeexist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("formbuilder-get", "formbuilder/get/{id}", new { controller = "formbuilder", action = "get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), id = @"^\d+$" });
            config.Routes.MapHttpRoute("formbuilder-list", "formbuilder/list", new { controller = "formbuilder", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("formbuilder-delete", "formbuilder/delete", new { controller = "formbuilder", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("formbuilder-unassignedattributes", "formbuilder/unassignedattributes", new { controller = "formbuilder", action = "unassignedattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("formbuilder-getunassignedgroups", "formbuilder/getunassignedgroups", new { controller = "formbuilder", action = "getunassignedgroups" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("formbuilder-getformbuilderattributegroup", "formbuilder/getformattributegroup/{formBuilderId}/{localeId}/{mappingId}", new { controller = "formbuilder", action = "getformattributegroup" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), formBuilderId = @"^\d+$", localeId = @"^\d+$", mappingid = @"^\d+$" });
            config.Routes.MapHttpRoute("formbuilder-associategroups", "formbuilder/associategroups", new { controller = "formbuilder", action = "associategroups" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("formbuilder-associateattributes", "formbuilder/associateattributes", new { controller = "formbuilder", action = "associateattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("formbuilder-update", "formbuilder/update", new { controller = "formbuilder", action = "update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("formbuilder-updateattributedisplayorder", "formbuilder/updateattributedisplayorder", new { controller = "formbuilder", action = "updateattributedisplayorder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("formbuilder-updategroupdisplayorder", "formbuilder/updategroupdisplayorder", new { controller = "formbuilder", action = "updategroupdisplayorder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("formbuilder-unassociateformbuildergroups", "formbuilder/unassociateformbuildergroups/{formBuilderId}/{groupId}", new { controller = "formbuilder", action = "unassociateformbuildergroups" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("formbuilder-unassociateformbuilderattributes", "formbuilder/unassociateformbuilderattributes/{formBuilderId}/{attributeId}", new { controller = "formbuilder", action = "unassociateformbuilderattributes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });


            //Form builder Template.
            config.Routes.MapHttpRoute("formbuildertemplate-create", "formbuilder/createformtemplate", new { controller = "formbuilder", action = "createformtemplate" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("formbuildertemplate-formattributevalueunique", "formbuilder/formattributevalueunique", new { controller = "formbuilder", action = "formattributevalueunique" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });


            //Form Submission Route
            config.Routes.MapHttpRoute("formsubmission-list", "formsubmission/list", new { controller = "formsubmission", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("formsubmission-getformsubmitdetails", "formsubmission/getformsubmitdetails/{formsubmitid}", new { controller = "formsubmission", action = "getformsubmitdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), FormSubmitId = @"^\d+$" });
            config.Routes.MapHttpRoute("formsubmission-formsubmissionexportlist", "formsubmission/Formsubmissionexportlist/{exportType}", new { controller = "formsubmission", action = "Formsubmissionexportlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });


            // Log Message 

            config.Routes.MapHttpRoute("logmessage-list", "logmessage/list", new { controller = "logmessage", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("integrationlogmessage-list", "logmessage/integrationloglist", new { controller = "logmessage", action = "integrationloglist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("eventlogmessage-list", "logmessage/eventloglist", new { controller = "logmessage", action = "eventloglist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("databaselogmessage-list", "logmessage/databaseloglist", new { controller = "logmessage", action = "databaseloglist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("logmessage-getlogmessage", "logmessage/getlogmessage/{logMessageId}", new { controller = "logmessage", action = "getlogmessage" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("logmessage-getdatabaselogmessage", "logmessage/getdatabaselogmessage/{logMessageId}", new { controller = "logmessage", action = "getdatabaselogmessage" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("logmessage-getlogconfiguration", "logmessage/getlogconfiguration", new { controller = "logmessage", action = "getlogconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("logmessage-updatelogconfiguration", "logmessage/updatelogconfiguration", new { controller = "logmessage", action = "updatelogconfiguration" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("logmessage-purgelogs", "logmessage/purgelogs", new { controller = "logmessage", action = "purgelogs" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("impersonationlogmessage-list", "logmessage/impersonationloglist", new { controller = "logmessage", action = "impersonationloglist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Product Update Import
            config.Routes.MapHttpRoute("import-product-update-data", "products/importproductupdatedata", new { controller = "products", action = "importproductupdatedata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Publish History
            config.Routes.MapHttpRoute("publishhistory-list", "publishhistory/list", new { controller = "publishhistory", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("publishhistory-delete", "publishhistory/delete/{versionId}", new { controller = "publishhistory", action = "delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //CMS page index search
            config.Routes.MapHttpRoute("cmssearchconfiguration-getcmspagesearchindexmonitorlist", "cmssearchconfiguration/getcmspagesearchindexmonitorlist", new { controller = "cmssearchconfiguration", action = "getcmspagesearchindexmonitorlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("cmssearchconfiguration-insertcreatecmspageindexdata", "cmssearchconfiguration/insertcreatecmspageindexdata", new { controller = "cmssearchconfiguration", action = "insertcreatecmspageindexdata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("cmssearchconfiguration-getcmspageindex", "cmssearchconfiguration/getcmspageindexdata", new { controller = "cmssearchconfiguration", action = "getcmspageindexdata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //PowerBi
            config.Routes.MapHttpRoute("generalsetting-getpowerbisettings", "generalsetting/getpowerbisettings", new { controller = "generalsetting", action = "getpowerbisettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("generalsetting-updatepowerbisettings", "generalsetting/updatepowerbisettings", new { controller = "generalsetting", action = "updatepowerbisettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            #region RMA
            config.Routes.MapHttpRoute("rmareturn-getorderdetailsforreturn", "rmareturn/getorderdetailsforreturn/{userId}/{orderNumber}/{isFromAdmin}", new { controller = "rmareturn", action = "getorderdetailsforreturn" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("rmareturn-list", "rmareturn/list", new { controller = "rmareturn", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("rmareturn-isordereligibleforreturn", "rmareturn/isordereligibleforreturn/{userId}/{portalId}/{orderNumber}", new { controller = "rmareturn", action = "isordereligibleforreturn" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("rmareturn-getreturndetails", "rmareturn/getreturndetails/{returnNumber}", new { controller = "rmareturn", action = "getreturndetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("rmareturn-saveorderreturn", "rmareturn/saveorderreturn", new { controller = "rmareturn", action = "saveorderreturn" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("rmareturn-deleteorderreturnbyreturnnumber", "rmareturn/deleteorderreturnbyreturnnumber/{returnNumber}/{userId}", new { controller = "rmareturn", action = "deleteorderreturnbyreturnnumber" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("rmareturn-submitorderreturn", "rmareturn/submitorderreturn", new { controller = "rmareturn", action = "submitorderreturn" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("rmareturn-calculateorderreturn", "rmareturn/calculateorderreturn", new { controller = "rmareturn", action = "calculateorderreturn" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("rmareturn-getreturnstatuslist", "rmareturn/getreturnstatuslist", new { controller = "rmareturn", action = "getreturnstatuslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("rmareturn-getreturndetailsforadmin", "rmareturn/getreturndetailsforadmin/{returnNumber}", new { controller = "rmareturn", action = "getreturndetailsforadmin" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("rmareturn-createreturnhistory", "rmareturn/createreturnhistory", new { controller = "rmareturn", action = "createreturnhistory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("rmareturn-savereturnnotes", "rmareturn/savereturnnotes", new { controller = "rmareturn", action = "savereturnnotes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("rmareturn-getshiptoaddressbyportalid", "rmareturn/getshiptoaddressbyportalid/{portalId}/", new { controller = "rmareturn", action = "getshiptoaddressbyportalid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), portalId = @"^\d+$" });
            #endregion

            //Quote Request Routing
            config.Routes.MapHttpRoute("quote-list", "quote/list", new { controller = "quote", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("quote-createquote", "quote/create", new { controller = "quote", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("quote-getquotereceipt", "quote/getquotereceipt/{quoteId}/", new { controller = "quote", action = "getquotereceipt" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), quoteId = @"^\d+$" });
            config.Routes.MapHttpRoute("quote-getquotebyquotenumber", "quote/getquotebyquotenumber/{quotenumber}/", new { controller = "quote", action = "getquotebyquotenumber" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get)});

            config.Routes.MapHttpRoute("quote-convertquotetoorder", "quote/convertquotetoorder", new { controller = "quote", action = "convertquotetoorder" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("quote-getquotedetail", "quote/getquotebyid/{omsQuoteId}/", new { controller = "quote", action = "getquotebyid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), omsQuoteId = @"^\d+$" });
            config.Routes.MapHttpRoute("quote-lineitems", "quote/getquotelineitembyquoteid/{omsQuoteId}/", new { controller = "quote", action = "GetQuoteLineItemByQuoteId" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get), omsQuoteId = @"^\d+$" });
            config.Routes.MapHttpRoute("quote-updatequote", "quote/updatequote", new { controller = "quote", action = "updatequote" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put)});
            config.Routes.MapHttpRoute("quote-getquotetotal", "quote/getquotetotal/{quoteNumber}/", new { controller = "quote", action = "getquotetotal" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            #region Sitemap
            config.Routes.MapHttpRoute("sitemap-getsitemapcategorylist", "sitemap/getsitemapcategorylist/{includechildcategories}/", new { controller = "sitemap", action = "getsitemapcategorylist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("sitemap-getsitemapbrandlist", "sitemap/getsitemapbrandlist", new { controller = "sitemap", action = "getsitemapbrandlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("sitemap-getsitemapproductlist", "sitemap/getsitemapproductlist", new { controller = "sitemap", action = "getsitemapproductlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            #endregion

            //Stock Notification setting
            config.Routes.MapHttpRoute("generalsetting-getstocknoticesettings", "generalsetting/getstocknoticesettings", new { controller = "generalsetting", action = "getstocknoticesettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("generalsetting-updatestocknoticesettings", "generalsetting/updatestocknoticesettings", new { controller = "generalsetting", action = "updatestocknoticesettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
        }

        static void RegisterV2Routes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute("webstorecategory-getcategorydetails-v2", "v2/webstorecategory/getcategoryproducts", new { controller = "webstorecategoryv2", action = "getcategoryproducts" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute("publishproduct-listbyattribute-v2", "v2/publishproduct/productlistbyattributes", new { controller = "publishproductv2", action = "getpublishproductsbyattribute" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("publishproduct-getpricing-v2", "v2/publishproduct/getproductprice", new { controller = "publishproductv2", action = "getproductprice" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("publishproduct-getproductbyid-v2", "v2/publishproduct/get/{publishproductid}", new { controller = "publishproductv2", action = "getpublishproduct" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute("accounts-create-v2", "v2/users/create", new { controller = "userv2", action = "createuser" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("accounts-update-v2", "v2/users/update", new { controller = "userv2", action = "updateuser" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("accounts-createguestaccount-v2", "v2/users/createguestcustomer", new { controller = "userv2", action = "createguestcustomerv2" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute("shoppingcart-createcart-v2", "v2/shoppingcarts/upsertcart", new { controller = "shoppingcartv2", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("shoppingcart-calculate-v2", "v2/shoppingcarts/calculate", new { controller = "shoppingcartv2", action = "calculatev2" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("shoppingcart-removecartitems-v2", "v2/shoppingcarts/removecartitems", new { controller = "shoppingcartv2", action = "removecartitems" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });
            config.Routes.MapHttpRoute("shoppingcart-get-v2", "v2/shoppingcarts/get", new { controller = "shoppingcartv2", action = "getshoppingcart" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("shoppingcart-removecartlineitem", "shoppingcarts/removecartlineitem/{omsSavedCartLineItemId}", new { controller = "shoppingcart", action = "removecartlineitem" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            //Approval Routing
            config.Routes.MapHttpRoute("account-getlevelslist", "account/getlevelslist", new { controller = "account", action = "getlevelslist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("account-createapproverlevel", "account/createapproverlevel", new { controller = "account", action = "createapproverlevel" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("account-getapproverlevelList", "account/getapproverlevelList", new { controller = "account", action = "getapproverlevelList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("account-deleteapproverlevel", "account/deleteapproverlevel", new { controller = "account", action = "deleteapproverlevel" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("account-savepermissionsetting", "account/savepermissionsetting", new { controller = "account", action = "savepermissionsetting" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portal-getportalapprovaldetailsbyid", "portal/getportalapprovaldetailsbyid/{portalId}", new { controller = "portal", action = "getportalapprovaldetailsbyid" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portal-getportalapproverlist", "portal/getportalapproverlist/{portalApprovalId}", new { controller = "portal", action = "getportalapproverlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portal-GetPortalApprovalTypeList", "portal/GetPortalApprovalTypeList", new { controller = "portal", action = "GetPortalApprovalTypeList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portal-GetPortalApprovalLevelList", "portal/GetPortalApprovalLevelList", new { controller = "portal", action = "GetPortalApprovalLevelList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //portal Search Setting
            config.Routes.MapHttpRoute("portal-sort-list", "portal/sortlist", new { controller = "portal", action = "sortlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portal-removeassociatedsortsettings", "portal/removeassociatedsortsettings", new { controller = "portal", action = "removeassociatedsortsettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portal-associatesortsettings", "portal/associatesortsettings", new { controller = "portal", action = "associatesortsettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portal-page-list", "portal/pagelist", new { controller = "portal", action = "pagelist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("portal-removeassociatedpagesettings", "portal/removeassociatedpagesettings", new { controller = "portal", action = "removeassociatedpagesettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portal-associatepagesettings", "portal/associatepagesettings", new { controller = "portal", action = "associatepagesettings" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("portal-updateportalpagesetting", "portal/updateportalpagesetting", new { controller = "portal", action = "updateportalpagesetting" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute("portal-barcodescanner", "portal/getbarcodescanner", new { controller = "portal", action = "getbarcodescanner" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //ECertificate
            config.Routes.MapHttpRoute("ecert-available-list", "ecert/list", new { controller = "ECert", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("ecert-available-create", "ecert/create", new { controller = "ECert", action = "create" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("ecert-available-balance-data", "ecert/GetAvailableECertBalance", new { controller = "ECert", action = "GetAvailableECertBalance" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });

            config.Routes.MapHttpRoute("orders-gethistory-v2", "v2/orders/list", new { controller = "orderv2", action = "list" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("orders-updatepaymentstatus-v2", "v2/orders/updatepaymentstatus/{omsorderid}/{paymentstatusid}", new { controller = "orderv2", action = "updatepaymentstatusv2" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put), omsorderid = @"^\d+$", paymentstatusid = @"^\d+$" });
            config.Routes.MapHttpRoute("orders-createorder-v2", "v2/order/create", new { controller = "orderv2", action = "createv2" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("orders-updateorderstatus-v2", "v2/orders/updateshippinginfo", new { controller = "orderv2", action = "updateordershippingstatusv2" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("orders-getorderbyordernumber-v2", "v2/orders/{ordernumber}", new { controller = "orderv2", action = "getbyordernumber" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            #region Devexpress reports
            config.Routes.MapHttpRoute("reportcategories-list", "report/categorieslist", new { controller = "DevExpressReport", action = "GetReportCategories" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("reportdetails-list", "report/getreportdetails/{reportCategoryId}", new { controller = "DevExpressReport", action = "GetReportDetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("reportsetting-list", "report/GetReportSetting/{ReportCode}", new { controller = "DevExpressReport", action = "GetReportSetting" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("orderlist-report", "report/Orders", new { controller = "DevExpressReport", action = "GetOrdersReport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("orderItemlist-report", "report/getorderitemdetails/{OmsOrderId}", new { controller = "DevExpressReport", action = "GetOrdersItemsReport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("couponlist-report", "report/Coupons", new { controller = "DevExpressReport", action = "getcouponreport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("salestaxlist-report", "report/salestax", new { controller = "DevExpressReport", action = "getsalestaxreport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("affiliateorderlist-report", "report/affiliateorder", new { controller = "DevExpressReport", action = "getaffiliateorderreport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("orderpicklist-report", "report/orderpicklist", new { controller = "DevExpressReport", action = "getorderpickreport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("bestsellerproductlist-report", "report/bestsellerproduct", new { controller = "DevExpressReport", action = "getbestsellerproductreport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("inventoryreorderlist-report", "report/inventoryreorder", new { controller = "DevExpressReport", action = "getinventoryreorderreport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("popularsearchlist-report", "report/popularsearch", new { controller = "DevExpressReport", action = "getpopularsearchreport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("servicerequestlist-report", "report/servicerequest", new { controller = "rmarequest", action = "getservicerequestreport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("vendorsReportlist-report", "report/vendors", new { controller = "vendor", action = "getvendorsReport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("userslist-report", "report/users", new { controller = "DevExpressReport", action = "getusersreport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("emailoptincustomerlist-report", "report/emailoptincustomer", new { controller = "DevExpressReport", action = "getemailoptincustomerreport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("mostfrequentcustomerlist-report", "report/mostfrequentcustomer", new { controller = "DevExpressReport", action = "getmostfrequentcustomerreport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("topspendingcustomerslist-report", "report/topspendingcustomers", new { controller = "DevExpressReport", action = "gettopspendingcustomersreport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("abandonedcartlist-report", "report/abandonedcart", new { controller = "DevExpressReport", action = "getabandonedcartreport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("reportlayout-save", "report/save", new { controller = "DevExpressReport", action = "SaveReportLayout" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("reportlayout-load", "report/load", new { controller = "DevExpressReport", action = "LoadSavedReportLayout" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("reportlayout-load-reportname", "report/load/{reportCode}/{reportName}", new { controller = "DevExpressReport", action = "GetSavedReportLayout" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("reportlayout-delete", "report/deletesavedview", new { controller = "DevExpressReport", action = "DeleteSavedReportLayout" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("reportstoresdetails", "report/storesdetails", new { controller = "DevExpressReport", action = "getstoreswithcurrency" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("reportorderstatuses", "report/reportorderstatuses", new { controller = "DevExpressReport", action = "getorderstatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("reportdiscounttype", "report/reportdiscounttype", new { controller = "DevExpressReport", action = "getdiscounttype" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            #endregion

            //Typeahead Routing
            config.Routes.MapHttpRoute("typeahead-list", "typeahead/gettypeaheadresponse", new { controller = "typeahead", action = "gettypeaheadresponse" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute("searchreport-getnoresultsfoundreport", "searchreport/getnoresultsfoundreport", new { controller = "searchreport", action = "getnoresultsfoundreport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("searchreport-gettopkeywordsreport", "searchreport/gettopkeywordsreport", new { controller = "searchreport", action = "gettopkeywordsreport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("searchreport-savesearchreportdata", "searchreport/savesearchreport", new { controller = "searchreport", action = "savesearchreportdata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Maintenance
            config.Routes.MapHttpRoute("maintenance-purgeallpublisheddata", "maintenance/purgeallpublisheddata", new { controller = "maintenance", action = "purgeallpublisheddata" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            //Quick Order
            config.Routes.MapHttpRoute("quickorder-getquickorderproductlist", "quickorder/getquickorderproductlist", new { controller = "quickorder", action = "getquickorderproductlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("quickorder-getdummyquickorderproductlist", "quickorder/getdummyquickorderproductlist", new { controller = "quickorder", action = "getdummyquickorderproductlist" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //User registration attempt endpoint
            config.Routes.MapHttpRoute("user-removeuserregisterattempts", "user/removealluserregistrationattemptdetails", new { controller = "user", action = "removealluserregistrationattemptdetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            //Cart: Save For Later
            config.Routes.MapHttpRoute("Cart-SaveForLater", "SaveForLater/CreateCartForLater", new { controller = "SaveForLater", action = "CreateCartForLater" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("Cart-GetCartForLater", "SaveForLater/GetCartForLater/{userId}/{templateType}", new { controller = "SaveForLater", action = "GetCartForLater" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("Cart-DeleteCartItemtForLater", "SaveForLater/DeleteCartItem", new { controller = "SaveForLater", action = "DeleteCartItem" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("Cart-DeleteAllCartItemsForLater", "SaveForLater/DeleteAllCartItems/{omsTemplateId}/{isFromSavedCart}", new { controller = "SaveForLater", action = "DeleteAllCartItems" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //order
            config.Routes.MapHttpRoute("order-saveorderpaymentdetail", "order/saveorderpaymentdetail", new { controller = "order", action = "saveorderpaymentdetail" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            //Cart: SavedCart
            config.Routes.MapHttpRoute("Cart-SavedCart", "savedcart/createsavedcart", new { controller = "savedcart", action = "createsavedcart" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("Cart-EditSaveCart", "savedcart/EditSaveCart", new { controller = "savedcart", action = "EditSaveCart" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("Cart-AddProductToCartForSaveCart", "savedcart/AddProductToCartForSaveCart/{omsTemplateId}/{userId}/{portalId}", new { controller = "savedcart", action = "AddProductToCartForSaveCart" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("Cart-EditSaveCartName", "savedcart/EditSaveCartName/{templateName}/{templateId}", new { controller = "savedcart", action = "EditSaveCartName" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            #region TradeCentric
            config.Routes.MapHttpRoute("tradecentric-gettradecentricuser", "tradecentric/gettradecentricuser/{userId}", new { controller = "tradecentric", action = "gettradecentricuser" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("tradecentric-savetradecentricuser", "tradecentric/savetradecentricuser", new { controller = "tradecentric", action = "savetradecentricuser" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute("tradecentric-transfercart", "tradecentric/transfercart", new { controller = "tradecentric", action = "transfercart" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            #endregion
        }
    }
}
