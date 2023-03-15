using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ExclusiveCard.WebAdmin.ViewModels
{
    public class ValidToDateCheck : ValidationAttribute
    {
        public ValidToDateCheck()
        {
        }

        public override bool IsValid(object value)
        {
            bool result = false;
            DateTime minDate = Convert.ToDateTime("01-Jan-1900", CultureInfo.InvariantCulture);
            try
            {
                if (value == null)
                {
                    result = true;
                }
                else
                {
                    DateTime datetime = Convert.ToDateTime(value);
                    if (datetime > minDate && datetime <= DateTime.UtcNow.AddYears(1))
                    {
                        result = true;
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}