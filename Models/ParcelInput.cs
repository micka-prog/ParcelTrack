using System.ComponentModel.DataAnnotations;
using ParcelTrack.Validation;

namespace ParcelTrack.Models
{
    public class ParcelInput
    {
        [Display(Name = "Tracking code")]
        [Required(ErrorMessage = "Give the tracking code, e.g. PT-100101")]
        [StringLength(9, ErrorMessage = "The tracking code is at most 9 characters")]
        [TrackingCode]
        public string TrackingCode { get; set; } = "";

        [Required(ErrorMessage = "Give the name of the person it is for")]
        [StringLength(40, ErrorMessage = "The name is at most 40 characters")]
        public string Recipient { get; set; } = "";

        [Range(0.1, 30.0, ErrorMessage = "Weight must be from 0.1 kg to 30 kg")]
        public double WeightKg { get; set; }

        [Required(ErrorMessage = "Give the status")]
        [StringLength(20)]
        public string Status { get; set; } = "Received";

        [Required(ErrorMessage = "Give the branchId")]
        public int? BranchId { get; set; }
    }
}
