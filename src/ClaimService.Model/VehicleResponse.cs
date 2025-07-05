namespace ClaimService.Model
{
    /// <summary>
    /// A model to represent Vehicle details
    /// </summary>
    public partial class VehicleResponse
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public string RegistrationNumber { get; set; } = null!;

        public string? Make { get; set; }

        public string? Model { get; set; }

        public int? Year { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<PolicyResponse> Policies { get; set; } = new List<PolicyResponse>();
    }
}
