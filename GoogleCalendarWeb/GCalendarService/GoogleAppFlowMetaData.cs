using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Calendar.v3;
using Google.Apis.Util.Store;
using System;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ZenegyCalendar.GCalendarService
{
    /// <summary>
    /// This class maintains a reference to Google OAuth 2.0 authorization code flow that manages and persists end-user credentials.
    /// </summary>
    public class GoogleAppFlowMetaData : FlowMetadata
    {
        private IAuthorizationCodeFlow _flow { get; }

        public override IAuthorizationCodeFlow Flow => _flow;

        public GoogleAppFlowMetaData(IDataStore dataStore)
        {
            var clientId = WebConfigurationManager.AppSettings["GoogleClientID"];
            var clientSecret = WebConfigurationManager.AppSettings["GoogleClientSecret"];

            var flowInitializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                Scopes = new[] { CalendarService.Scope.Calendar },
                DataStore = dataStore
            };
            _flow = new ForceOfflineGoogleAuthorizationCodeFlow(flowInitializer);
        }

        public override string GetUserId(Controller controller)
        {
            // In this sample we use the session to store the user identifiers.
            // That's not the best practice, because you should have a logic to identify
            // a user. You might want to use "OpenID Connect".
            // You can read more about the protocol in the following link:
            // https://developers.google.com/accounts/docs/OAuth2Login.

            var user = controller.Session["user"];
            if (user == null)
            {
                user = Guid.NewGuid();
                controller.Session["user"] = user;
            }

            // TODO: We need to get logged in user id here
            // so that we can use that as key for saving token

            return user.ToString();
                       
        }

        public override string AuthCallback => "/AuthCallback/IndexAsync";
    }    
}