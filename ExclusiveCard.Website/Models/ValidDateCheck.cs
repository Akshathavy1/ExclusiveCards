using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ExclusiveCard.Website.Models
{
    public class ValidDateCheck : ValidationAttribute
    {
        public ValidDateCheck()
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
                    if (datetime > minDate && TimeZoneInfo.ConvertTimeToUtc(datetime, TimeZoneInfo.Local) <= DateTime.UtcNow)
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
    }
}