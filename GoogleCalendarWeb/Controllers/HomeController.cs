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
        public ActionResult SyncToGoogleCalendar()
        {
            // Google Calendar integration ref - https://www.sparkhound.com/blog/google-oauth-integration-using-an-mvc-asp-net-app-1

            if (string.IsNullOrWhiteSpace(GoogleOauthTokenService.AccessToken))
            {
                var redirectUri = GoogleCalendarSyncer.GetOauthTokenUri(this);
                return Redirect(redirectUri);
            }
            else
            {
                var success = GoogleCalendarSyncer.SyncToGoogleCalendar(this);
                if (!success)
                {
                    return Json("Token was revoked. Try again.");
                }
            }
            return Redirect("~/");
        }       
    }
}