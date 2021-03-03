using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2.Web;
using static Google.Apis.Auth.OAuth2.Web.AuthorizationCodeWebApp;

namespace ZenegyCalendar.GCalendarService
{
    /// <summary>
    /// This is the implementation for sending requests to the Google Calendar. The main focus of the blog is the Oauth token management so this class will be lacking as far as functionality is concerned. It simply pushes a calendar event for the current time to the user's Google Calendar.
    /// </summary>
    public class GoogleCalendarSyncer
    {
        const string savedEventId = "nq69rqugqtb440nh1o2rah9oic";

        /// <summary>
        /// Get a login link to which user will be redirected to for authorization.
        /// </summary>
        /// <param name="controller">The current controller.</param>
        /// <returns>Redirect Uri</returns>
        public static async Task<string> GetOauthTokenUriAsync(Controller controller)
        {
            var authResult = await GetAuthResultAsync(controller).ConfigureAwait(false);
            return authResult.RedirectUri;
        }

        public static async Task<bool> SaveEventToGoogleCalendarAsync(Controller controller)
        {
            try
            {
                var authResult = await GetAuthResultAsync(controller).ConfigureAwait(false);

                var calService = new CalendarService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = authResult.Credential,
                    ApplicationName = "Zenegy Calendar"
                });

                var calendarId = GetMainCalendarId(calService);

                // Create event
                var startDate = new EventDateTime { DateTime = DateTime.UtcNow };
                var endDate = new EventDateTime { DateTime = DateTime.UtcNow.AddDays(3) };
                var calEvent = new Event
                {
                    Summary = "Vikram Saini - Absence",
                    Description = "On leave for medical reason",
                    Start = startDate,
                    End = endDate
                };

                var insertRequest = new EventsResource.InsertRequest(calService, calEvent, calendarId);
                var savedEvent = await insertRequest.ExecuteAsync().ConfigureAwait(false);
                var eventId = savedEvent.Id;

                return true;
            }
            catch (Exception)
            {
                GoogleOauthTokenService.AccessToken = null;
                return false;
            }
        }

        public static async Task<bool> UpdateEventInGoogleCalendarAsync(Controller controller)
        {
            try
            {
                var authResult = await GetAuthResultAsync(controller).ConfigureAwait(false);

                var calService = new CalendarService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = authResult.Credential,
                    ApplicationName = "Zenegy Calendar"
                });

                var calendarId = GetMainCalendarId(calService);

                // Create event
                var startDate = new EventDateTime { DateTime = DateTime.UtcNow };
                var endDate = new EventDateTime { DateTime = DateTime.UtcNow.AddDays(10) };
                var calEvent = new Event
                {
                    Summary = "Vikram Saini - Holiday",
                    Description = "On leave for personal reason",
                    Start = startDate,
                    End = endDate
                };

                var eventResource = new EventsResource(calService);
                var erListRequest = eventResource.List(calendarId);
                var eventsResponse = await erListRequest.ExecuteAsync().ConfigureAwait(false);
                var existingEvent = eventsResponse.Items.FirstOrDefault(e => e.Id == savedEventId);
                
                if (existingEvent != null)
                {
                    var updateRequest = eventResource.Update(calEvent, calendarId, savedEventId);
                    var updatedEvent = await updateRequest.ExecuteAsync().ConfigureAwait(false);
                    return true;
                }

                return false;
            }
            catch (Exception exc)
            {
                GoogleOauthTokenService.AccessToken = null;
                return false;
            }
        }
        public static async Task<bool> DeleteEventInGoogleCalendarAsync(Controller controller)
        {
            try
            {
                var authResult = await GetAuthResultAsync(controller).ConfigureAwait(false);

                var calService = new CalendarService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = authResult.Credential,
                    ApplicationName = "Zenegy Calendar"
                });

                var calendarId = GetMainCalendarId(calService);

                var deleteRequest = new EventsResource.DeleteRequest(calService, calendarId, savedEventId);
                var deleteResponse = await deleteRequest.ExecuteAsync().ConfigureAwait(false);

                return true;
            }
            catch (Exception exc)
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
        private static async Task<AuthResult> GetAuthResultAsync(Controller controller)
        {
            var dataStore = new EFDataStore();
            var appFlowMetaData = new GoogleAppFlowMetaData(dataStore);
            var authCodeMvcApp = new AuthorizationCodeMvcApp(controller, appFlowMetaData);

            var authResultTask = await authCodeMvcApp.AuthorizeAsync(new CancellationToken()).ConfigureAwait(false);
            return authResultTask;
        }

        private static string GetMainCalendarId(IClientService service)
        {
            var calendarListRequest = new CalendarListResource.ListRequest(service);
            var calendars = calendarListRequest.Execute();
            return calendars.Items.First(i => i.AccessRole == "owner" || i.AccessRole == "writer").Id;
        }

    }
}