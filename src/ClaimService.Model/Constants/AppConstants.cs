namespace ClaimService.Model.Constants
{
    public enum ClaimStatus
    {
        Submitted = 0,
        AssignedToInspector = 1,
        InspectionCompleted = 2,
        UnderReview = 3,
        Approved = 4,
        Rejected = 5,
        Settled = 6,
        Closed = 7
    }
}
