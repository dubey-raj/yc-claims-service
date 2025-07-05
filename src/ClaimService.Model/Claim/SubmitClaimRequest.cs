using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace ClaimService.Model.Claim
{
    public class SubmitClaimDto
    {
        [JsonIgnore]
        public long UserId { get; set; }
        public string PolicyNumber { get; set; }
        public DateTime IncidentDate { get; set; }
        public string IncidentLocation { get; set; } = string.Empty;
        public string IncidentDescription { get; set; } = string.Empty;
        public List<IFormFile>? SupportingFiles { get; set; }
    }
}
