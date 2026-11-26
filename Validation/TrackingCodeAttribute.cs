using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ParcelTrack.Validation
{
    public class TrackingCodeAttribute : ValidationAttribute
    {
        public TrackingCodeAttribute()
        {
            ErrorMessage = "Tracking code must look like PT-123456";
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true;
            }

            string? text = value as string;
            if (string.IsNullOrEmpty(text))
            {
                return true;
            }

            return Regex.IsMatch(text, @"^PT-\d{6}$");
        }
    }
}
