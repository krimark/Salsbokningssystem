﻿using System;
using Salsbokningssystem.Helpers;
using Salsbokningssystem.Models;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace Salsbokningssystem.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        //
        // GET: /Booking/
        DataClasses1DataContext db = new DataClasses1DataContext();

        public ActionResult Index()
        {
            var bookings = from b in db.Bookings
                           orderby b.StartTime
                           select b;
            return View(bookings);
        }

        [HttpGet]
        public ActionResult Book()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Book(Booking booking)
        {
            if (ModelState.IsValid && !Roles.IsUserInRole("Användare"))
            {
                booking.UserID = WebSecurity.CurrentUserId;
                db.Bookings.InsertOnSubmit(booking);
                db.SubmitChanges();
            }
            else if (ModelState.IsValid)
            {
                int dayCount = 0;
              
                for (DateTime iterator = DateTime.Today; iterator <= booking.StartTime; iterator = iterator.AddDays(1))
                {
                    if (iterator.IsWorkingDay())
                    {
                        dayCount++;
                    }
                }

                if (dayCount > 6)
                {
                    ViewBag.Error = "Du kan endast boka en vecka i förväg.";
                    return View();
                }
                if (booking.EndTime > booking.StartTime.AddHours(4))
                {
                    ViewBag.Error = "Bokningstiden får ej överstiga 4 timmar.";
                    return View();
                }
                booking.UserID = WebSecurity.CurrentUserId;
                db.Bookings.InsertOnSubmit(booking);
                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administratör")]
        [HttpGet]
        public ActionResult Remove(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = (from b in db.Bookings
                               where b.ID == id
                               select b).FirstOrDefault();
            if (booking == null)
            {
                return HttpNotFound();
            }

            return View(booking);
        }

        [Authorize(Roles = "Administratör")]
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = (from b in db.Bookings
                               where b.ID == id
                               select b).FirstOrDefault();
            if (booking != null)
            {
                db.Bookings.DeleteOnSubmit(booking);
                db.SubmitChanges();
            }
            return RedirectToAction("Index", "Booking");
        }

    }
}
