namespace ClaimService.Model.Dashboard
{
    public class DashboardResponse
    {
        public int TotalClaims { get; set; }

        public ClaimsSummaryResponse ClaimsSummary { get; set; }

        public PendingApprovalsResponse PendingApprovals { get; set; }

        public QuickStatsResponse QuickStats { get; set; }

        public List<ClaimAssignmentResponse> RecentAssignments { get; set; }
    }

    public class ClaimsSummaryResponse
    {
        public int InInspection { get; set; }
        public int Approved { get; set; }
        public int InReview { get; set; }
        public int Rejected { get; set; }
    }

    public class PendingApprovalsResponse
    {
        public int Total { get; set; }
        public int New { get; set; }
        public int Escalated { get; set; }
        public int Delayed { get; set; }
    }

    public class QuickStatsResponse
    {
        public string AverageProcessingTime { get; set; }
        public string ApprovalRate { get; set; }
        public int DelayedClaims { get; set; }
        public int ActiveInspectors { get; set; }
    }

    public class ClaimAssignmentResponse
    {
        public string InspectorName { get; set; }
        public string InspectorAvatar { get; set; }
        public string ClaimId { get; set; }
        public DateTime AssignedDate { get; set; }
        public string Status { get; set; }
    }

}
