using System;
using System.Globalization;

namespace ETLib.Helpers
{
    public static class CheckTimeHelper
    {
        public static bool IsInQuarter(this DateTime? dateTime, int numberOfQuarter)
        {
            if (dateTime == null) return false;
            var date = dateTime.Value;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("au-AU");
            switch (numberOfQuarter)
            {
                case 1:
                    return dateTime >= DateTime.Parse($"01/01/{date.Year}", culture) &&
                           dateTime < DateTime.Parse($"01/04/{date.Year}", culture);

                case 2:
                    return dateTime >= DateTime.Parse($"01/04/{date.Year}", culture) &&
                           dateTime < DateTime.Parse($"01/07/{date.Year}", culture);

                case 3:
                    return dateTime >= DateTime.Parse($"01/07/{date.Year}", culture) &&
                           dateTime < DateTime.Parse($"01/10/{date.Year}", culture);

                case 4:
                    return dateTime >= DateTime.Parse($"01/10/{date.Year}", culture) &&
                           dateTime <= DateTime.Parse($"31/12/{date.Year}", culture);
            }

            return false;
        }

        public static bool IsInQuarter(this DateTime dateTime)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("au-AU");
            var currentQuarter = 0;
            var currentMonth = DateTime.Now.Month;
            if (currentMonth >= 1 && currentMonth <= 3) currentQuarter = 1;
            else if (currentMonth > 3 && currentMonth <= 6) currentQuarter = 2;
            else if (currentMonth > 6 && currentMonth <= 9) currentQuarter = 3;
            else if (currentMonth > 9 && currentMonth <= 12) currentQuarter = 4;

            switch (currentQuarter)
            {
                case 1:
                    return dateTime >= DateTime.Parse($"01/01/{dateTime.Year}", culture) &&
                           dateTime < DateTime.Parse($"01/04/{dateTime.Year}", culture);

                case 2:
                    return dateTime >= DateTime.Parse($"01/04/{dateTime.Year}", culture) &&
                           dateTime < DateTime.Parse($"01/07/{dateTime.Year}", culture);

                case 3:
                    return dateTime >= DateTime.Parse($"01/07/{dateTime.Year}", culture) &&
                           dateTime < DateTime.Parse($"01/10/{dateTime.Year}", culture);

                case 4:
                    return dateTime >= DateTime.Parse($"01/10/{dateTime.Year}", culture) &&
                           dateTime <= DateTime.Parse($"31/12/{dateTime.Year}", culture);
            }

            return false;
        }

        public static bool IsInCurrentMonth(this DateTime? dateTime)
        {
            if (dateTime == null) return false;

            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            return dateTime.Value.Month == currentMonth && dateTime.Value.Year == currentYear;
        }

        public static bool IsInCurrentMonth(this DateTime dateTime)
        {
            

            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            return dateTime.Month == currentMonth && dateTime.Year == currentYear;
        }
        public static int[] GetQuarter(int month)
        {
            int[] monthsInQuater = new int[3];
            if (month >= 1 && month <= 3) monthsInQuater = new int[] { 1, 2, 3 };
            else if (month > 3 && month <= 6) monthsInQuater = new int[] { 4, 5, 6 };
            else if (month > 6 && month <= 9) monthsInQuater = new int[] { 7, 8, 9 };
            else if (month > 9 && month <= 12) monthsInQuater = new int[] { 10, 11, 12 };
            return monthsInQuater;

        }

    }
}