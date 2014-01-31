﻿using System.Web.Mvc;

namespace Salsbokningssystem.Controllers
{
    public class HomeController : Controller
    {

        [Authorize]

        public ActionResult Index()
        {
            ViewBag.Message = "Välkommen!";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Kontakta admin";

            return View();
        }

        public ActionResult Admin()
        {
            ViewBag.Message = "Här kan Gun administrera";

            return View();
        }
    }
}
