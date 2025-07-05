using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace ClaimService.Model.Claim
{
    public class AssessClaimRequest
    {
        [JsonIgnore]
        public int UserId { get; set; }
        public string ClaimNumber { get; set; } = string.Empty;
        public DateTime InspectionDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string ClaimStatus { get; set; } = string.Empty;
        public decimal DamageEstimate { get; set; }
        public List<IFormFile>? SupportingFiles { get; set; }
    }
}
