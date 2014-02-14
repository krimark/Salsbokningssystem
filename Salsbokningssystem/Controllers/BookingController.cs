using System;
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
          [HttpGet]
        public ActionResult Index(FormCollection form)
        {
            var bookings = from b in db.Bookings
                           orderby b.StartTime
                           select b;       

            ViewBag.Room = (from f in db.Rooms
                            select f).ToList();

            ViewBag.Booking = (from f in db.Bookings
                               where f.StartTime.Date==DateTime.Now.Date
                               select f).ToList();
            //DateTime t = DateTime.Now;
            // string st=t.ToString("yyyy-MM-dd");
            // ViewBag.date = st;
            
            return View(bookings);

        }
         [HttpPost]
          public ActionResult Index(FormCollection form, string bookingDateDDL, string datepicker)
          {
              var bookings = from b in db.Bookings
                             orderby b.StartTime
                             select b;

              ViewBag.Room = (from f in db.Rooms
                              select f).ToList();
              DateTime t;
              if (bookingDateDDL != null)
              {

                   t = Convert.ToDateTime(bookingDateDDL);
              }
              else
              {
                  t = Convert.ToDateTime(datepicker);
              }
             string s = bookingDateDDL;
             ViewBag.Booking = (from f in db.Bookings
                                where f.StartTime.Date == t
                                select f).ToList();
           

              return View(bookings);

          }

        [HttpGet]
        public ActionResult Book()
        {
          
            return View();
        }

        [HttpPost]
        public ActionResult Book(Booking booking, FormCollection form, bool? reccuringDate)
        {
            if (ModelState.IsValid && !Roles.IsUserInRole("Användare"))
            {
                var bookingDate = form["datepicker"];
                var startTime = form["bookingStartDDL"];
                var stoppTime = form["bookingStoppDDL"];

                var startDateTime = bookingDate + " " + startTime;
                var stoppDateTime = bookingDate + " " + stoppTime;

                booking.StartTime = Convert.ToDateTime(startDateTime);
                booking.EndTime = Convert.ToDateTime(stoppDateTime);

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
                if (PreviousBookingExists(booking.RoomID, booking.StartTime, booking.EndTime))
                {
                    TempData["UserMessage"] = "Bokningstiden du angav krockar med en existerande bokning.";
                    return RedirectToAction("Index");
                }

                if (reccuringDate == true)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                booking.UserID = WebSecurity.CurrentUserId;
                                db.Bookings.InsertOnSubmit(booking);
                                db.SubmitChanges();
                                break;

                            case 1:
                                Booking booking2 = new Booking();
                                booking2.StartTime = booking.StartTime.AddDays(7);
                                booking2.EndTime = booking.EndTime.AddDays(7);
                                booking2.RoomID = booking.RoomID;
                                booking2.UserID = WebSecurity.CurrentUserId;
                                db.Bookings.InsertOnSubmit(booking2);
                                db.SubmitChanges();
                                break;

                            case 2:
                                Booking booking3 = new Booking();
                                booking3.StartTime = booking.StartTime.AddDays(14);
                                booking3.EndTime = booking.EndTime.AddDays(14);
                                booking3.RoomID = booking.RoomID;
                                booking3.UserID = WebSecurity.CurrentUserId;
                                db.Bookings.InsertOnSubmit(booking3);
                                db.SubmitChanges();
                                break;
                        }
                    }
                }
                else
                {
                    booking.UserID = WebSecurity.CurrentUserId;
                    db.Bookings.InsertOnSubmit(booking);
                    db.SubmitChanges();
                }
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
                if (PreviousBookingExists(booking.RoomID, booking.StartTime, booking.EndTime))
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
                var user = (from u in db.Users
                            where u.ID == booking.UserID
                            select u).FirstOrDefault();

                db.Bookings.DeleteOnSubmit(booking);
                db.SubmitChanges();
                if (user != null)
                {
                    Mailer.SendMail(user.Email, "Din bokning har avbrutits av Gun!", "Bokning avbruten");
                }
            }
            return RedirectToAction("Index", "Admin");
        }

        private bool PreviousBookingExists(int roomId, DateTime bookingStart, DateTime bookingEnd)
        {
            var result = (from b in db.Bookings
                          where b.RoomID == roomId && b.StartTime < bookingEnd && bookingStart < b.EndTime
                          select b).FirstOrDefault();

            return result != null;
        }
    }
}
