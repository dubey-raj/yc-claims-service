namespace ClaimService.Model.Claim
{
    /// <summary>
    /// Class to represent claim
    /// </summary>
    public class ClaimResponse
    {
        public int ClaimId {  get; set; }
        public string ClaimNumber { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;

        public DateOnly IncidentDate { get; set; }
        public string IncidentLocation { get; set; } = string.Empty;
        public string IncidentDescription { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }

        public DateOnly InspectionDate { get; set; }
        public string InspectionNote { get; set; }
        public string InspectorRecommendation { get; set; }
        public decimal? EstimatedAmount { get; set; }
        public decimal? ApprovedAmount { get; set; }

        public List<string> DocumentUrls { get; set; } = new();
    }

    public class ClaimListResponse
    {
        public List<ClaimResponse> Claims { get; set; }
    }
}
