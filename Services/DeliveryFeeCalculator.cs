namespace ParcelTrack.Services
{
    public class DeliveryFeeCalculator : IDeliveryFeeCalculator
    {
        public decimal CalculateFee(double weightKg, string? parish)
        {
            decimal wholeKg = (decimal)Math.Ceiling(weightKg);
            decimal fee = 500m + (150m * wholeKg);

            bool isKingstonOrStAndrew =
                string.Equals(parish, "Kingston", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(parish, "St Andrew", StringComparison.OrdinalIgnoreCase);

            if (!isKingstonOrStAndrew)
            {
                fee += 300m;
            }

            return fee;
        }
    }
}
