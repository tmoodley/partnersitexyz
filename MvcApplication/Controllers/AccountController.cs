using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using MvcApplication.Models;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace MvcApplication.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login(string returnUrl)
        {
            HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties
                {
                    RedirectUri = returnUrl ?? Url.Action("Index", "Home")
                },
                "Auth0");
            return new HttpUnauthorizedResult();
        }
        [HttpGet]
        public ActionResult SignUp()
        { 
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(User user)
        {
            // Get Token from auth0
            string token = GetToken();
            // Register user with auth0
            RegisterUser(user, token);
            // TODO - We can also register user to our internal all.

            return View();
        }

        private static void RegisterUser(User user, string token)
        {
            var clientToken = new RestClient("https://equitablelife.auth0.com/api/v2/users");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer " + token);

            request.AddParameter("application/json",
                                "{\"email\": \"" + user.Email
                                + "\",\"password\": \"" + user.Password
                                + "\",\"username\": \"" + user.Username
                                + "\",\"name\": \"" + user.Name
                                + "\",\"connection\": \"" + user.Connection
                                + "\", \"user_metadata\": {\"type\": \"plan-member\"}, \"app_metadata\": {\"equitableId\": \"132456\"}}",
                                ParameterType.RequestBody);


            IRestResponse response2 = clientToken.Execute(request);
        }

        private static string GetToken()
        {
            var client = new RestClient("https://equitablelife.auth0.com/oauth/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            string client_id = ConfigurationManager.AppSettings["client_id"];
            string client_secret = ConfigurationManager.AppSettings["client_secret"];
            request.AddParameter("application/json", "{\"client_id\":\"" + client_id
                                + "\",\"client_secret\":\"" + client_secret
                                + "\",\"audience\":\"https://equitablelife.auth0.com/api/v2/\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);


            JavaScriptSerializer js = new JavaScriptSerializer();
            dynamic blogObject = js.Deserialize<dynamic>(response.Content);
            string token = blogObject["access_token"];
            return token;
        }

        [Authorize]
        public void Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            HttpContext.GetOwinContext().Authentication.SignOut("Auth0");
        }

        [Authorize]
        public ActionResult Claims()
        {
            return View();
        }

        [HttpPut]
        public ActionResult LinkAccount(string userid, string email)
        {
            return Json(new { customId = "234324234" });
        }
    }
}