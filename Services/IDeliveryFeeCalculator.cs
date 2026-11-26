namespace ParcelTrack.Services
{
    public interface IDeliveryFeeCalculator
    {
        decimal CalculateFee(double weightKg, string? parish);
    }
}
