using System.Threading.Tasks;
using ZenegyCalendar.GCalendarService;
using System.Web.Mvc;

namespace ZenegyCalendar.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SyncToGoogleCalendar(string url)
        {
            // Google Calendar integration ref - https://www.sparkhound.com/blog/google-oauth-integration-using-an-mvc-asp-net-app-1

            if (string.IsNullOrWhiteSpace(GoogleOauthTokenService.AccessToken))
            {
                var redirectUri = await GoogleCalendarSyncer.GetOauthTokenUriAsync(this).ConfigureAwait(false);
                return Redirect(redirectUri);
            }

            return Redirect("~/");
        }

        [HttpGet]
        public async Task<ActionResult> SyncToGoogleCalendar()
        {
            //var success = await GoogleCalendarSyncer.SaveEventToGoogleCalendarAsync(this).ConfigureAwait(false);
            //var success = await GoogleCalendarSyncer.UpdateEventInGoogleCalendarAsync(this).ConfigureAwait(false);
            var success = await GoogleCalendarSyncer.DeleteEventInGoogleCalendarAsync(null).ConfigureAwait(false);

            if (!success)
            {
                return Json("Token was revoked. Try again.");
            }

            TempData["Success"] = "An event was saved to Google Calendar successfully!";
            return Redirect("~/");
        }

    }
}