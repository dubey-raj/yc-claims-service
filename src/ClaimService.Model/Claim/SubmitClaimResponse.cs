using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace ClaimService.Model.Claim
{
    public class SubmitClaimResponse
    {
        public bool IsSuccess { get; set; }
        public string ClaimNumber { get; set; }
        public string Message { get; set; }
    }
}
