using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using ProductService.Models;
using ProductService.Providers;
using System;

namespace ProductService
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                //AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(60),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(1),
                //AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp =true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                //PRODUCTION localhost:8081
                //ClientId = "962877420729-eau0ke54pafsk8qqinqhs46nnaigqai2.apps.googleusercontent.com",
                //ClientSecret = "TjCIb9FnsRhCYWkzuH67eTM7"

                //DEVELOPEMENT
                ClientId = "840333866669-0nk64oeu2bra89dpabrk3jrsludlnjsf.apps.googleusercontent.com",
                ClientSecret = "AH44xKwp4FdnZCgf5vJNJnsf"
            });


            var facebookOptions = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationOptions()
            {

                //Production
                //AppId = "1235806606540142",
                //AppSecret = "0e519bffc62d179814ce18bbbbbc65b3",

                //Development
                AppId = "263747314063867",
                AppSecret = "58c66e84611d678f43a042e6bfe04d5e",



                //I had to write this handler to deal with facebook redirect parameters in v2.4
                BackchannelHttpHandler = new Facebook.FacebookBackChannelHandler(),
                UserInformationEndpoint = "https://graph.facebook.com/v2.4/me?fields=id,email"
            };
            facebookOptions.Scope.Add("email");
            app.UseFacebookAuthentication(facebookOptions);
        }
    }
}
