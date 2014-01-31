using System.Web.Mvc;

namespace Salsbokningssystem.Controllers
{
    [Authorize(Roles = "Administratör")]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            ViewBag.Message = "Här kan Gun administrera";

            return View();
        }

    }
}
