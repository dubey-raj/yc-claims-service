using System.Collections.Immutable;

namespace ClaimService.Model
{
    /// <summary>
    /// A model to represent Policy details
    /// </summary>
    public partial class PolicyResponse
    {
        public long Id { get; set; }

        public string PolicyNumber { get; set; } = null!;

        public long UserId { get; set; }

        public long VehicleId { get; set; }

        public string InsurerName { get; set; } = null!;

        public string? PolicyType { get; set; }

        public decimal? CoverageAmount { get; set; }

        public decimal? DeductibleAmount { get; set; }

        public string? EffectiveDate { get; set; }

        public string? ExpiryDate { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual List<VehicleResponse> Vehicles { get; set; } = new List<VehicleResponse>();
    }
}
