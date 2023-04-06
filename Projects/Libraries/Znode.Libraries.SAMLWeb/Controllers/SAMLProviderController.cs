using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using Znode.Libraries.SAML;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Libraries.SAMLWeb.Controllers
{
    public class SAMLProviderController :Controller
    {
        /// <summary>
        /// Attempt to access the resource on the Resource server
        /// </summary>
        public ActionResult GetResource()
        {
            string OAuthToken = "";

            if (Request.Cookies["OAuthToken"] != null)
            {
                OAuthToken = Request.Cookies["OAuthToken"].Value;
            }

            var client = new HttpClient(new OAuthRequestHttpHandler(OAuthToken));

            try
            {
                ViewBag.ApiResponse = client.GetStringAsync(new Uri(SAMLKeys.GetSettingsByMappingKey("resourceAddress").Value)).Result;
            }

            //User is not authorized e.g. invalid/expired OAuthToken
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException is HttpRequestException)
                {
                    var samlRequest = CreateSAMLRequest();

                    string redirectURL = SAMLKeys.GetSettingsByMappingKey("samlRequestAddress").Value +
                        "?samlRequest=" + HttpUtility.UrlEncode(samlRequest);

                    return Redirect(redirectURL);
                }
                else
                {
                    return View("Error");
                }
            }

            return View("Main");
        }

        /// <summary>
        /// Receives the OAuth code from the OAuth server and sends back a request for the OAuth token
        /// </summary>
        public ActionResult OAuthRedirect()
        {
            if (Request.Params["code"] != null && Request.Params["state"] != null)
            {
                //Extract the session for the user
               
                SessionObj sessionObj = SessionHelper.GetDataFromSession<SessionObj>(Request.Params["state"]);

                //Request the OAuth token
                var authorizeGetTokenURI = new Uri(SAMLKeys.GetSettingsByMappingKey("authorizationServerTokenAddress").Value);
                var client = new HttpClient(new OAuthCodeHttpHandler(sessionObj.User,
                    sessionObj.SAMLToken.GetHashCode().ToString())
                    );
                StringContent stringContent = new StringContent(
                    "grant_type=authorization_code" +
                    "&client_id=" + sessionObj.User +
                    "&code=" + Request.Params["code"] +
                    "&redirect_uri=" + HttpUtility.UrlEncode(SAMLKeys.GetSettingsByMappingKey("clientOAuthCodeReturnURL").Value)
                    , System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
                var result = client.PostAsync(authorizeGetTokenURI, stringContent).Result;

                //Persist OAuth token
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string content = result.Content.ReadAsStringAsync().Result;
                    JObject jObject = JObject.Parse(content);
                    string accessToken = jObject.Value<string>("access_token");
                    CookieHelper.SetCookie("OAuthToken", accessToken, 0, true);
                }
            }

            return RedirectToAction("Main");
        }

        /// <summary>
        /// Passes the response of the SAML server on to the OAuth server and stores the user state
        /// </summary>
        /// <param name="username">The federated user name agreed between the SAML server and the OAuth server</param>
        /// <param name="samlResponse">SAML Response XML returned by the server</param>
        [HttpPost]
        public ActionResult AuthnResponse(string username, string samlResponse,string userid)
        {
            //Handle response from the SAML server
            if (samlResponse != null)
            {
                //Store user state so we can construct appropriate URL 
                //when requesting the OAuth token after receiving the OAuth code
                SessionObj stateObj = new SessionObj
                {
                    User = username,
                    SAMLToken = samlResponse
                };
                string stateId = Guid.NewGuid().ToString();

                SessionHelper.SaveDataInSession<SessionObj>(stateId, stateObj);
             
                // Craft url to request OAuth code
                ViewBag.AuthServerUrl = SAMLKeys.GetSettingsByMappingKey("authorizationServerSAMLAuthorizeAddress").Value +
                    "?redirect_uri=" + SAMLKeys.GetSettingsByMappingKey("clientOAuthCodeReturnURL").Value +
                    "&state=" + stateId +
                    "&id=" + userid+
                    "&response_type=code";

                //The encoded SAML Response to be embedded in the form
                ViewBag.SAMLResponse = samlResponse;

                //Redirect to authorization prompt
                return View("OAuthRedirect");
            }

            return RedirectToAction("Main");
        }

        /// <summary>
        /// Creates a SAMLRequest object and serializes it.
        /// </summary>
        /// <returns>The serialized SAMLRequest object</returns>
        private string CreateSAMLRequest()
        {
            AuthnRequestType samlRequest = new AuthnRequestType()
            {
                ID = Guid.NewGuid().ToString(),
                Version = "2.0",
                IssueInstant = DateTime.UtcNow,
                Issuer = new NameIDType
                {
                    Value = SAMLKeys.GetSettingsByMappingKey("Client").Value
                },
                NameIDPolicy = new NameIDPolicyType
                {
                    AllowCreate = true,
                    Format = "urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified"
                }
            };

            XmlSerializer serializer = new XmlSerializer(typeof(AuthnRequestType));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, samlRequest);

            string base64EncodedRequest = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(writer.ToString()));

            return base64EncodedRequest;
        }
    }
}
