using ClaimService.Model;
using ClaimService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClaimService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("policy")]
    public class PolicyController : ControllerBase
    {
        private readonly ILogger<PolicyController> _logger;
        private readonly IPolicyService _policyService;

        public PolicyController(ILogger<PolicyController> logger, IPolicyService policyService)
        {
            _logger = logger;
            _policyService = policyService;
        }
        [HttpGet]
        [Route("{userId:int}")]
        [Produces(typeof(List<PolicyResponse>))]
        public async Task<IActionResult> GetPolicyAsync([FromRoute] int userId)
        {
            var user = User;
            var policies = await _policyService.GetPolicyAsync(userId);
            return Ok(policies);
        }

        [HttpGet]
        [Route("{policyNumber}")]
        [Produces(typeof(PolicyResponse))]
        public async Task<IActionResult> GetPolicyAsync([FromRoute] string policyNumber)
        {
            var policy = await _policyService.GetPolicyAsync(policyNumber);
            if (policy == null) {
                return BadRequest("Invalid Policy");
            }
            return Ok(policy);
        }
    }
}
