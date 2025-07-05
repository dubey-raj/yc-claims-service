namespace ClaimService.Model.APIResponses
{
    public class UpdateAssignedCaseCountRequest
    {
        public int UserId { get; set; }
        public int DeltaCount { get; set; }
    }
}
