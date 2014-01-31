using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Salsbokningssystem.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        //
        // GET: /Booking/

        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Admin()
        {
            ViewBag.Message = "Här kan Gun administrera";

            return View();
        }
    }
}
