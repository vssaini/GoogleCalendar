using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System;
using System.Linq;
using System.Threading;
using System.Web.Configuration;
using System.Web.Mvc;
using static Google.Apis.Auth.OAuth2.Web.AuthorizationCodeWebApp;

namespace ZenegyCalendar.GCalendarService
{
    /// <summary>
    /// This is the implementation for sending requests to the Google Calendar. The main focus of the blog is the Oauth token management so this class will be lacking as far as functionality is concerned. It simply pushes a calendar event for the current time to the user's Google Calendar.
    /// </summary>
    public class GoogleCalendarSyncer
    {
        /// <summary>
        /// Get a login link to which user will be redirected to for authorization.
        /// </summary>
        /// <param name="controller">The current controller.</param>
        /// <returns>Redirect Uri</returns>
        public static string GetOauthTokenUri(Controller controller)
        {
            var authResult = GetAuthResult(controller);
            return authResult.RedirectUri;
        }

        public static bool SyncToGoogleCalendar(Controller controller)
        {
            try
            {
                var authResult = GetAuthResult(controller);

                var service = InitializeService(authResult);

                var calendarId = GetMainCalendarId(service);

                var calendarEvent = GetCalendarEvent();

                SyncCalendarEventToCalendar(service, calendarEvent, calendarId);
                return true;
            }
            catch (Exception ex)
            {
                GoogleOauthTokenService.AccessToken = null;
                return false;
            }
        }

        /// <summary>
        /// Get AuthResult which contains the user's credentials if it was loaded successfully from the store. Otherwise it contains the redirect URI for the authorization server.
        /// </summary>
        /// <param name="controller">The current controller.</param>
        /// <returns>Instance of AuthResult.</returns>
        private static AuthResult GetAuthResult(Controller controller)
        {
            var clientID = WebConfigurationManager.AppSettings["GoogleClientID"];
            var clientSecret = WebConfigurationManager.AppSettings["GoogleClientSecret"];

            var dataStore = new EFDataStore();
            var appFlowMetaData = new GoogleAppFlowMetaData(dataStore, clientID, clientSecret);
            var authCodeMvcApp = new AuthorizationCodeMvcApp(controller, appFlowMetaData);

            var cancellationToken = new CancellationToken();
            var authResultTask = authCodeMvcApp.AuthorizeAsync(cancellationToken);
            authResultTask.Wait();

            return authResultTask.Result;
        }

        private static CalendarService InitializeService(AuthResult authResult)
        {
            return new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = authResult.Credential,
                ApplicationName = "Zenegy Calendar"
            });
        }

        private static string GetMainCalendarId(CalendarService service)
        {
            var calendarListRequest = new CalendarListResource.ListRequest(service);
            var calendars = calendarListRequest.Execute();
            var result = calendars.Items.First(i => i.AccessRole == "owner" || i.AccessRole == "writer").Id;
            return result;
        }

        private static Event GetCalendarEvent()
        {
            var result = new Event
            {
                Summary = "Test Calendar Event Summary 2",
                Description = "Test Calendar Event Description 2",
                Sequence = 1
            };
            var eventDate = new EventDateTime
            {
                DateTime = DateTime.UtcNow
            };
            result.Start = eventDate;
            result.End = eventDate;
            return result;
        }

        private static void SyncCalendarEventToCalendar(CalendarService service, Event calendarEvent, string calendarId)
        {
            var eventRequest = new EventsResource.InsertRequest(service, calendarEvent, calendarId);
            eventRequest.Execute();
        }
    }
}