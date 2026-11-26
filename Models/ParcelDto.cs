namespace ParcelTrack.Models
{
    public class ParcelDto
    {
        public int Id { get; set; }
        public string TrackingCode { get; set; } = "";
        public string Recipient { get; set; } = "";
        public double WeightKg { get; set; }
        public string Status { get; set; } = "";
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public decimal DeliveryFee { get; set; }
    }
}
