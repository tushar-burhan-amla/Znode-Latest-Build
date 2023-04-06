using System;
using RestSharp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using RestSharp.Authenticators;

namespace Znode.Multifront.PaymentApplication.Providers
{
        public class CardConnectRestClient
        {
            private String url;
            private String userpass;
            private String username;
            private String password;

            // Endpoint names
            private static String ENDPOINT_AUTH = "cardconnect/rest/auth";
            private static String ENDPOINT_CAPTURE = "cardconnect/rest/capture";
            private static String ENDPOINT_VOID = "cardconnect/rest/void";
            private static String ENDPOINT_REFUND = "cardconnect/rest/refund";
            private static String ENDPOINT_INQUIRE = "cardconnect/rest/inquire";
            private static String ENDPOINT_SETTLESTAT = "cardconnect/rest/settlestat";
            private static String ENDPOINT_DEPOSIT = "cardconnect/rest/deposit";
            private static String ENDPOINT_PROFILE = "cardconnect/rest/profile";

            private enum OPERATIONS { GET, PUT, POST, DELETE };

            public CardConnectRestClient(String url, String username, String password)
            {
                if (IsEmpty(url)) throw new ArgumentException("url parameter is required");
                if (IsEmpty(username)) throw new ArgumentException("username parameter is required");
                if (IsEmpty(password)) throw new ArgumentException("password parameter is required");

                if (!url.EndsWith("/")) url = url + "/";
                this.url = url;
                this.username = username;
                this.password = password;
                this.userpass = username + ":" + password;
            }


            /**
            * Authorize transaction
            * @param request JObject representing an Authorization transaction request
            * @return JObject representing an Authorization transaction response
            */
            public JObject AuthorizeTransaction(JObject request)
            {
                return (JObject)Send(ENDPOINT_AUTH, OPERATIONS.PUT, request);
            }

            /**
            * Capture transaction
            * @param request JObject representing a Capture transaction request
            * @return JObject representing a Capture transaction response
            */
            public JObject CaptureTransaction(JObject request)
            {
                return (JObject)Send(ENDPOINT_CAPTURE, OPERATIONS.PUT, request);
            }

            /**
             * Void transaction
             * @param request JObject representing a Void transaction request
             * @return JObject representing a Void transaction response
             */
            public JObject VoidTransaction(JObject request)
            {
                return (JObject)Send(ENDPOINT_VOID, OPERATIONS.PUT, request);
            }

            /**
             * Refund Transaction
             * @param request JObject representing a Refund transaction request
             * @return JObject represeting a Refund transaction response
             */
            public JObject RefundTransaction(JObject request)
            {
                return (JObject)Send(ENDPOINT_REFUND, OPERATIONS.PUT, request);
            }

            /**
             * Inquire Transaction
             * @param merchid Merchant ID
             * @param retref RetRef to inquire
             * @return JObject representing the request transaction
             * @throws IllegalArgumentException
             */
            public JObject InquireTransaction(String merchid, String retref)
            {
                if (IsEmpty(merchid)) throw new ArgumentException("Missing required parameter: merchid");
                if (IsEmpty(retref)) throw new ArgumentException("Missing required parameter: retref");

                String url = ENDPOINT_INQUIRE + "/" + retref + "/" + merchid;
                return (JObject)Send(url, OPERATIONS.GET, null);
            }


            /**
             * Gets the settlement status for transactions
             * @param merchid Mechant ID
             * @param date Date in MMDD format
             * @return JArray of JObjects representing Settlement batches, each batch containing a JArray of 
             * JObjects representing the settlement status of each transaction
             * @throws IllegalArgumentException
             */
            public JArray SettlementStatus(String merchid, String date)
            {
                if ((!IsEmpty(merchid) && IsEmpty(date)) || (IsEmpty(merchid) && !IsEmpty(date)))
                    throw new ArgumentException("Both merchid and date parameters are required, or neither");

                String url = null;
                if (IsEmpty(merchid) || IsEmpty(date))
                {
                    url = ENDPOINT_SETTLESTAT;
                }
                else
                {
                    url = ENDPOINT_SETTLESTAT + "?merchid=" + merchid + "&date=" + date;
                }

                return (JArray)Send(url, OPERATIONS.GET, null);
            }


            /**
             * Retrieves deposit status information for the given merchant and date
             * @param merchid Merchant ID
             * @param date in MMDD format
             * @return
             * @throws IllegalArgumentException
             */
            public JObject DepositStatus(String merchid, String date)
            {
                if ((!IsEmpty(merchid) && IsEmpty(date)) || (IsEmpty(merchid) && !IsEmpty(date)))
                    throw new ArgumentException("Both merchid and date parameters are required, or neither");

                String url = null;
                if (IsEmpty(merchid) || IsEmpty(date))
                {
                    url = ENDPOINT_DEPOSIT;
                }
                else
                {
                    url = ENDPOINT_DEPOSIT + "?merchid=" + merchid + "&date=" + date;
                }
                return (JObject)Send(url, OPERATIONS.GET, null);
            }

            /**
             * Retrieves a profile
             * @param profileId ProfileID to retrieve
             * @param accountId Optional account id within profile
             * @param merchId Merchant ID
             * @return JArray of JObjects each represeting a profile
             * @throws IllegalArgumentException
             */
            public JArray ProfileGet(String profileId, String accountId, String merchId)
            {
                if (IsEmpty(profileId)) throw new ArgumentException("Missing required parameter: profileid");
                if (IsEmpty(merchId)) throw new ArgumentException("Missing required parameter: merchid");
                if (accountId == null) accountId = "";

                String url = ENDPOINT_PROFILE + "/" + profileId + "/" + accountId + "/" + merchId;
                return (JArray)Send(url, OPERATIONS.GET, null);
            }

            /**
             * Deletes a profile
             * @param profileid ProfileID to delete
             * @param accountid Optional accountID within the profile
             * @param merchid Merchant ID
             * @return
             * @throws IllegalArgumentException
             */
            public JObject ProfileDelete(String profileid, String accountid, String merchid)
            {
                if (IsEmpty(profileid)) throw new ArgumentException("Missing required parameter: profileid");
                if (IsEmpty(merchid)) throw new ArgumentException("Missing required parameter: merchid");
                if (accountid == null) accountid = "";

                String url = ENDPOINT_PROFILE + "/" + profileid + "/" + accountid + "/" + merchid;
                return (JObject)Send(url, OPERATIONS.DELETE, null);
            }


            /**
             * Creates a new profile
             * @param request JObject representing the Profile creation request
             * @return JSONObject representing the newly created profile
             * @throws IllegalArgumentException
             */
            public JObject ProfileCreate(JObject request)
            {
                return (JObject)Send(ENDPOINT_PROFILE, OPERATIONS.PUT, request);
            }


            /**
             * Updates an existing profile
             * @param request JObject representing the Profile Update request
             * @return JObject representing the updated Profile
             */
            public JObject ProfileUpdate(JObject request)
            {
                return ProfileCreate(request);
            }


            private Boolean IsEmpty(String s)
            {
                if (s == null) return true;
                if (s.Length <= 0) return true;
                if ("".Equals(s)) return true;
                return false;
            }

            private Object Send(String endpoint, OPERATIONS operation, JObject request)
            {
                // Create REST client
                RestClient client = new RestClient(url);

                // Set authentication credentials
                client.Authenticator = new HttpBasicAuthenticator(username, password);

                // Create REST request
                RestRequest rest = null;
                switch (operation)
                {
                    case OPERATIONS.PUT: rest = new RestRequest(endpoint, Method.PUT); break;
                    case OPERATIONS.GET: rest = new RestRequest(endpoint, Method.GET); break;
                    case OPERATIONS.POST: rest = new RestRequest(endpoint, Method.POST); break;
                    case OPERATIONS.DELETE: rest = new RestRequest(endpoint, Method.DELETE); break;
                }

                rest.RequestFormat = DataFormat.Json;
                rest.AddHeader("Content-Type", "application/json");

                String data = (request != null) ? request.ToString() : "";
                rest.AddParameter("application/json", data, ParameterType.RequestBody);
                IRestResponse response = client.Execute(rest);
                JsonTextReader jsReader = new JsonTextReader(new StringReader(response.Content));

                try
                {
                    return new JsonSerializer().Deserialize(jsReader);
                }
                catch (JsonReaderException jx)
                {
                    return null;
                }
            }
        }
    }

