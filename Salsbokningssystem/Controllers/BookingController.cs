using Salsbokningssystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (ModelState.IsValid)
            {
                booking.UserID = WebSecurity.CurrentUserId;
                db.Bookings.InsertOnSubmit(booking);
                db.SubmitChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
