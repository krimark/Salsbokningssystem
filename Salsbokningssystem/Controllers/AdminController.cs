using System;
using System.Linq;
using System.Web.Mvc;
using Salsbokningssystem.Models;


namespace Salsbokningssystem.Controllers
{
    [Authorize(Roles = "Administratör")]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        readonly DataClasses1DataContext db = new DataClasses1DataContext();

        public ActionResult Index()
        {
            ViewBag.Message = "Här kan Gun administrera";

            return View();
        }

        public ActionResult BookingHistory(string searchString)
        {

            var bookings = from b in db.Bookings
                           orderby b.StartTime
                           select b;


            if (!String.IsNullOrEmpty(searchString))
            {
                bookings = from r in db.Bookings
                           where r.Room.Name.Contains(searchString) || r.User.UserName.Contains(searchString)
                           orderby r.StartTime
                           select r;
            }

            return View(bookings);

        }

    }
}
