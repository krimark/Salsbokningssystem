using System;
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
        public ActionResult Book(Booking booking, FormCollection form)
        {
            if (ModelState.IsValid && !Roles.IsUserInRole("Användare"))
            {
                if (booking.EndTime < DateTime.Now || booking.StartTime < DateTime.Now)
                {
                    ViewBag.Error = "Du kan inte boka DÅTID";
                    return View();
                }
                if (booking.EndTime <= booking.StartTime)
                {
                    ViewBag.Error = "Fel bookning START TIME måste vara tidgare än END TIME";
                    return View();
                }
                if (PreviousBookingExists(booking.StartTime, booking.EndTime, booking.RoomID))
                {
                    TempData["UserMessage"] = "Bokningstiden du angav krockar med en existerande bokning.";
                    return RedirectToAction("Index");
                }
                booking.UserID = WebSecurity.CurrentUserId;
                db.Bookings.InsertOnSubmit(booking);
                db.SubmitChanges();
            }
            else if (ModelState.IsValid)
            {
                var bookingDate = form["bookingDateDDL"];
                var startTime = form["bookingStartDDL"];
                var stoppTime = form["bookingStoppDDL"];

                var startDateTime = bookingDate + " " + startTime;
                var stoppDateTime = bookingDate + " " + stoppTime;

                booking.StartTime = Convert.ToDateTime(startDateTime);
                booking.EndTime = Convert.ToDateTime(stoppDateTime);

                if (booking.EndTime > booking.StartTime.AddHours(4))
                {
                    ViewBag.Error = "Bokningstiden får ej överstiga 4 timmar.";
                    return View();
                }

                //if (booking.EndTime <= booking.StartTime)
                //{
                //    ViewBag.Error = "Fel bookning START TIME måste vara tidgare än END TIME";
                //    return View();
                //}
                if (booking.EndTime < booking.StartTime.AddHours(1))
                {
                    ViewBag.Error = "Bokningstiden får ej mindre än 1 timma.";
                    return View();
                }
                if (booking.EndTime < DateTime.Now || booking.StartTime < DateTime.Now)
                {
                    ViewBag.Error = "Du kan inte boka DÅTID";
                    return View();
                }



                var bookingList = from b in db.Bookings
                                  select b.UserID;

                if (bookingList.Contains(WebSecurity.CurrentUserId))
                {
                    TempData["UserMessage"] = "Du har redan gjort en bokning!";
                    return RedirectToAction("Index");
                }
                if (PreviousBookingExists(booking.StartTime, booking.EndTime, booking.RoomID))
                {
                    TempData["UserMessage"] = "Bokningstiden du angav krockar med en existerande bokning.";
                    return RedirectToAction("Index");
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

        private bool PreviousBookingExists(DateTime bookingStart, DateTime bookingEnd, int roomId)
        {
            var result = (from b in db.Bookings
                          where b.RoomID == roomId && b.StartTime < bookingEnd && bookingStart < b.EndTime
                          select b).FirstOrDefault();

            return result != null;
        }
    }
}
