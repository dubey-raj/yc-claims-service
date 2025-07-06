using ClaimService.Model.Dashboard;

namespace ClaimService.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardResponse> GetDashboardDataAsync();
    }
}
