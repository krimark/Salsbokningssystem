﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
            if (Roles.IsUserInRole("Administratör"))
            {
                ViewBag.Message = "Här kan Gun administrera";


                return View();
            }

            else
                return RedirectToAction("Index", "Booking");
        }
    }
}
