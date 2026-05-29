namespace GymSupport.Repository.Models.DTOs.Customer
{
    public class CreateCustomerRequest
    {
        public string UserId { get; set; } = null!;
        public int? HeightCm { get; set; }
        public int? WeightKg { get; set; }
        public string? Goal { get; set; }
        public string? ExperienceLevel { get; set; }
        public string? InjuryNotes { get; set; }
        public string? Subscription { get; set; }
    }
}
