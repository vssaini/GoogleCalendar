namespace ZenegyCalendar.GCalendarService
{
    /// <summary>
    /// The GoogleOauthTokenService.cs will be used for token storage for this demo.
    /// </summary>
    public class GoogleOauthTokenService
    {
        //private static string _accessToken;

        /// <summary>
        /// Gets or sets the access token issued by the authorization server.
        /// </summary>
        public static string AccessToken { get; set; }
        
        /// <summary>
        /// Gets or sets the refresh token which can be used to obtain new access token.
        /// </summary>
        public static string RefreshToken { get; set; }


        //TODO: Save oauth token in database or session
    }
}