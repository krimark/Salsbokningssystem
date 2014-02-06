using Salsbokningssystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace Salsbokningssystem.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {

        //Dropdown lista för start tider, behöver förbättras

        public static List<SelectListItem> GetDropDownRoom()
        {
            List<SelectListItem> listItem = new List<SelectListItem>();

            DataClasses1DataContext db = new DataClasses1DataContext();

            var lm = db.Rooms;
            foreach (var item in lm)
            {
                listItem.Add(new SelectListItem() { Text = item.Name, Value = item.ID.ToString() });
            }
            return listItem;
        }


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
                if (booking.EndTime <= booking.StartTime.AddHours(4))
                {
                    booking.UserID = WebSecurity.CurrentUserId;
                    db.Bookings.InsertOnSubmit(booking);
                    db.SubmitChanges();
                }
                else
                {
                    ViewBag.text = "Du måste boka mindre period";
                    return View();
                }
            }

            return RedirectToAction("Index");
        }

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
