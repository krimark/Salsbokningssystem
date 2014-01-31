using System.Web.Mvc;

namespace Salsbokningssystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Kontakta admin";

            return View();
        }
    }
}
