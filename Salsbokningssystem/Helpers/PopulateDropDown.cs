using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Salsbokningssystem.Models;

namespace Salsbokningssystem.Helpers
{
    public static class PopulateDropDown
    {
        public static List<SelectListItem> Rooms()
        {
            var roomItems = new List<SelectListItem>();

            var db = new DataClasses1DataContext();

            var lm = db.Rooms;
            foreach (var item in lm)
            {
                roomItems.Add(new SelectListItem { Text = item.Name, Value = item.ID.ToString(CultureInfo.InvariantCulture) });
            }
            return roomItems;
        }

        public static List<SelectListItem> Dates()
        {
            var dateItems = new List<SelectListItem>();
            DateTime date = DateTime.Today;
            int dayCount = 0;
           
            while (dayCount < 5)
            {
                if (date.IsWorkingDay())
                {
                    dateItems.Add(new SelectListItem { Text = String.Format("{0:d\\/M ddd}", date), Value = date.ToString("d", CultureInfo.InvariantCulture) });
                    dayCount++;
                }
                date = date.AddDays(1);
            }
            return dateItems;
        }

        public static List<SelectListItem> FromTimes()
        {
            
            var fromTimeItems = new List<SelectListItem>
            {
                new SelectListItem {Text = "08.00", Value = "08.00"},
                new SelectListItem {Text = "09.00", Value = "09.00"},
                new SelectListItem {Text = "10.00", Value = "10.00"},
                new SelectListItem {Text = "11.00", Value = "11.00"},
                new SelectListItem {Text = "12.00", Value = "12.00"},
                new SelectListItem {Text = "13.00", Value = "13.00"},
                new SelectListItem {Text = "14.00", Value = "14.00"},
                new SelectListItem {Text = "15.00", Value = "15.00"}
            };
            return fromTimeItems;
        }

        public static List<SelectListItem> ToTimes()
        {
            var toTimeItems = new List<SelectListItem>
            {
                new SelectListItem {Text = "09.00", Value = "09.00"},
                new SelectListItem {Text = "10.00", Value = "10.00"},
                new SelectListItem {Text = "11.00", Value = "11.00"},
                new SelectListItem {Text = "12.00", Value = "12.00"},
                new SelectListItem {Text = "13.00", Value = "13.00"},
                new SelectListItem {Text = "14.00", Value = "14.00"},
                new SelectListItem {Text = "15.00", Value = "15.00"},
                new SelectListItem {Text = "16.00", Value = "16.00"}
            };
            return toTimeItems;
        }
    }
}