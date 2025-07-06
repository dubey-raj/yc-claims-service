using ClaimService.Model.Dashboard;
using ClaimService.Services.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClaimService.Controllers
{
    [ApiController]
    [Route("dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IDashboardService _dashboardService;

        public DashboardController( IDashboardService dashboardService, ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        [HttpGet]
        [Produces(typeof(DashboardResponse))]
        public async Task<IActionResult> GetManagerDashboard()
        {
            var dashboardData = await _dashboardService.GetDashboardDataAsync();
            return Ok(dashboardData);
        }
    }

}
