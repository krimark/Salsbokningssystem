using System;
using System.Linq;
using System.Web.Mvc;
using Salsbokningssystem.Models;

namespace Salsbokningssystem.Controllers
{
    public class RoomController : Controller
    {

        DataClasses1DataContext db = new DataClasses1DataContext();
        //
        // GET: /Room/

        public ActionResult Index(string searchString, string projektRoom)
        {
            var rooms = from r in db.Rooms
                        select r;
            if (!String.IsNullOrEmpty(searchString))
            {
                rooms = rooms.Where(s => s.Name.Contains(searchString) || s.info.Contains(searchString));
            }
            else if (!String.IsNullOrEmpty(projektRoom))
            {
                rooms = rooms.Where(s => s.Capacity == int.Parse(projektRoom));
            }


            return View(rooms);
        }



        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Room room)
        {

            if (ModelState.IsValid)
            {

                db.Rooms.InsertOnSubmit(room);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }

            return View(room);
        }

        public ActionResult Delete(int id = 0)
        {
            var room = (from r in db.Rooms.Where(r => r.ID == id) select r).FirstOrDefault();
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        public ActionResult Details(int id = 0)
        {
            var room = (from r in db.Rooms.Where(r => r.ID == id) select r).FirstOrDefault();
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            var room = (from m in db.Rooms.Where(c => c.ID == id) select m).FirstOrDefault();
            if (room != null)
            {
                db.Rooms.DeleteOnSubmit(room);
            }
            db.SubmitChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id = 0)
        {

            var room = (from r in db.Rooms.Where(c => c.ID == id) select r).FirstOrDefault();
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Room room)
        {

            if (ModelState.IsValid)
            {
                var projectRoom = from r in db.Rooms where r.ID == room.ID select r;
                foreach (var rooms in projectRoom)
                {
                    rooms.Name = room.Name;
                    rooms.info = room.info;
                    rooms.Capacity = room.Capacity;


                }

                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View(room);

        }


    }
}
