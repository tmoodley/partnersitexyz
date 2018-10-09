using System.Web.Mvc;

namespace MvcApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult SignInAuth0()
		{
			return View();
		}

        [HttpPost]
        public ActionResult Auth0()
        {
            return Json(new { customId = "234324234" });
        }
        
    }
}