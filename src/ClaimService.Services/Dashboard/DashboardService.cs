using ClaimService.DataStorage.DAO;
using ClaimService.Model.Constants;
using ClaimService.Model.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace ClaimService.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly YcEclaimsDbContext _dbContext;

        public DashboardService(YcEclaimsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        ///<inheritdoc/>
        public async Task<DashboardResponse> GetDashboardDataAsync()
        {
            var totalClaims = await _dbContext.Claims.CountAsync();
            var inInspection = await _dbContext.Claims.CountAsync(c => c.Status == ClaimStatus.Submitted.ToString());
            var approved = await _dbContext.Claims.CountAsync(c => c.Status == ClaimStatus.Approved.ToString());
            var inReview = await _dbContext.Claims.CountAsync(c => c.Status == ClaimStatus.UnderReview.ToString());
            var rejected = await _dbContext.Claims.CountAsync(c => c.Status == ClaimStatus.Rejected.ToString());

            var pendingApprovals = await _dbContext.Claims.Where(a => a.Status == ClaimStatus.UnderReview.ToString()).ToListAsync();

            var newPending = pendingApprovals.Count(pa => pa.SubmittedAt >= DateTime.Now.AddDays(-7));
            var escalated = pendingApprovals.Count(pa => pa.Status == "Escalated");
            var delayed = pendingApprovals.Count(pa => DateTime.Now.Subtract(pa.SubmittedAt.Value).Days > 7);

            var assessments = await _dbContext.ClaimAssessments
                .Where(a => a.Status == ClaimStatus.Approved.ToString() && a.InspectionDate != null && a.CreatedAt != null).ToListAsync();

            var avgProcessingTimeDays = assessments
                .Select(a => a.InspectionDate!.Value.ToDateTime(TimeOnly.MinValue).Subtract(a.CreatedAt!.Value).TotalDays)
                .DefaultIfEmpty(0)
                .Average();

            var totalReviewed = await _dbContext.Claims.CountAsync(c => c.Status == ClaimStatus.Approved.ToString() || c.Status == ClaimStatus.Rejected.ToString());
            var approvalRate = totalReviewed == 0 ? "0%" : $"{(approved * 100 / totalReviewed)}%";

            var thresholdDate = DateTime.Now.AddDays(-7);

            var delayedClaims = await _dbContext.Claims
                .CountAsync(c => c.Status == ClaimStatus.UnderReview.ToString()
                              && c.SubmittedAt != null
                              && c.SubmittedAt < thresholdDate);

            var activeInspectors = await _dbContext.ClaimAssessments
                .Where(a => a.CreatedAt >= DateTime.Now.AddDays(-1))
                .Select(a => a.InspectorId)
                .Distinct()
                .CountAsync();

            var recentAssignments = await _dbContext.ClaimAssessments
                .Include(a => a.Claim)
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .Select(a => new ClaimAssignmentResponse
                {
                    InspectorName = $"Inspector {a.InspectorId}",
                    ClaimId = a.Claim.ClaimNumber,
                    AssignedDate = a.CreatedAt.Value,
                    Status = a.Claim.Status
                })
                .ToListAsync();

            return new DashboardResponse
            {
                TotalClaims = totalClaims,
                ClaimsSummary = new ClaimsSummaryResponse
                {
                    InInspection = inInspection,
                    Approved = approved,
                    InReview = inReview,
                    Rejected = rejected
                },
                PendingApprovals = new PendingApprovalsResponse
                {
                    Total = pendingApprovals.Count,
                    New = newPending,
                    Escalated = escalated,
                    Delayed = delayed
                },
                QuickStats = new QuickStatsResponse
                {
                    AverageProcessingTime = $"{Math.Round(avgProcessingTimeDays, 1)} days",
                    ApprovalRate = approvalRate,
                    DelayedClaims = delayedClaims,
                    ActiveInspectors = activeInspectors
                },
                RecentAssignments = recentAssignments
            };
        }
    }
}
