using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using IdentityModel;

namespace okta_aspnet_mvc_example.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [Authorize]
        public ActionResult Profile()
        {
            System.Diagnostics.Debug.WriteLine("isAuthenticated: " + HttpContext.GetOwinContext().Authentication.User.Identity.IsAuthenticated);
            System.Diagnostics.Debug.WriteLine("AuthenticationType: " + HttpContext.GetOwinContext().Authentication.User.Identity.AuthenticationType);
            var identity = (ClaimsIdentity)HttpContext.GetOwinContext().Authentication.User.Identity;
            System.Diagnostics.Debug.WriteLine("Claims: " + identity.Claims);
            foreach (var claim in identity.Claims)
            {
                System.Diagnostics.Debug.WriteLine("tyoe: " + claim.Type + ", value: " + claim.Value);
            }
            System.Diagnostics.Debug.WriteLine("Expiration: " + identity.FindFirst(JwtClaimTypes.Expiration));
            return View(HttpContext.GetOwinContext().Authentication.User.Claims);
        }
    }
}