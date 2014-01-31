using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Salsbokningssystem.Controllers
{
    public class BookingController : Controller
    {
        //
        // GET: /Booking/

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

    }
}
