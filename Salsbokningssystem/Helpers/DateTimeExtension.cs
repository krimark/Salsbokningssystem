using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Salsbokningssystem.Helpers
{
    public static class DateTimeExtension
    {
        public static bool IsWorkingDay(this DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday
                && date.DayOfWeek != DayOfWeek.Sunday;
        }
    }
}