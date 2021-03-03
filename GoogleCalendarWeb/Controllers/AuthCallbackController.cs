using System;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Web;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using ZenegyCalendar.GCalendarService;

namespace ZenegyCalendar.Controllers
{
    public class AuthCallbackController : Google.Apis.Auth.OAuth2.Mvc.Controllers.AuthCallbackController
    {
        private FlowMetadata _flowMetaData { get; }
        protected override FlowMetadata FlowData => _flowMetaData;

        public AuthCallbackController()
        {
            var dataStore = new EFDataStore();
            _flowMetaData = new GoogleAppFlowMetaData(dataStore);
        }

        public override async Task<ActionResult> IndexAsync(AuthorizationCodeResponseUrl authorizationCode, CancellationToken taskCancellationToken)
        {
            if (string.IsNullOrEmpty(authorizationCode.Code))
            {
                var errorResponse = new TokenErrorResponse(authorizationCode);
                Logger.Info("Received an error. The response is: {0}", errorResponse);
                Debug.WriteLine("Received an error. The response is: {0}", errorResponse);
                return OnTokenError(errorResponse);
            }

            Logger.Debug("Received \"{0}\" code", authorizationCode.Code);
            Debug.WriteLine("Received \"{0}\" code", authorizationCode.Code);

            // Exchange the auth code for an access token

            var returnUrl = Request.Url?.ToString();
            returnUrl = returnUrl?.Substring(0, returnUrl.IndexOf("?", StringComparison.Ordinal));

            // The token is being saved at this point
            await Flow.ExchangeCodeForTokenAsync(UserId, authorizationCode.Code, returnUrl, taskCancellationToken).ConfigureAwait(false);

            // Extract the right state.
            var redirectUri = await AuthWebUtility.ExtracRedirectFromState(Flow.DataStore, UserId,
                authorizationCode.State).ConfigureAwait(false);
            return new RedirectResult(redirectUri); // Return to url to which need to redirect
        }

        protected override ActionResult OnTokenError(TokenErrorResponse errorResponse)
        {
            return Redirect(Url.Content("~/"));
        }
    }
}