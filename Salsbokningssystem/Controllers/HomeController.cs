using System.Web.Mvc;

namespace Salsbokningssystem.Controllers
{
    public class HomeController : Controller
    {

        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Message = "Hej Gun!";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Välkommen till din nya bokningssajt.";

            return View();
        }
    }
}
