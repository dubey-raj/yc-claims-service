using ClaimService.Model;
using ClaimService.Model.Claim;
using ClaimService.Services;
using ClaimService.Services.Documents;
using ClaimService.Services.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClaimService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("claim")]
    public class ClaimController : ControllerBase
    {
        private readonly ILogger<ClaimController> _logger;
        private readonly IClaimService _claimService;
        private readonly IFileUploader _fileUploader;

        public ClaimController(ILogger<ClaimController> logger, IClaimService  claimService, IFileUploader fileUploader)
        {
            _logger = logger;
            _claimService = claimService;
            _fileUploader = fileUploader;
        }

        [HttpPost]
        [Produces(typeof(SubmitClaimResponse))]
        public async Task<IActionResult> AddClaimAsync([FromForm] SubmitClaimDto claim)
        {
            var user = User;
            var userId = UserClaimHelper.GetUserId(user);
            claim.UserId = userId;
            var claimResult = await _claimService.AddClaimAsync(claim);
            if (claimResult.IsSuccess)
            {
                var uploadedFilePaths = await _fileUploader.UploadFilesAsync(claimResult.ClaimNumber, claim.SupportingFiles);
                await _claimService.AddClaimDocumentsAsync(claimResult.ClaimNumber, uploadedFilePaths, "ClaimDoc");
                return Ok(claimResult);
            }
            return BadRequest(claimResult.Message);
        }

        [HttpGet()]
        [Route("/claims")]
        [Produces(typeof(ClaimListResponse))]
        public async Task<IActionResult> GetClaimsAsync()
        {
            var user = User;
            var userId = UserClaimHelper.GetUserId(user);
            var claims = await _claimService.GetClaimsAsync(userId);
            return Ok(claims);
        }

        [HttpGet("/claims/assigned")]
        [Produces(typeof(ClaimListResponse))]
        public async Task<IActionResult> GetAssignedClaimsAsync()
        {
            var user = User;
            var userId = UserClaimHelper.GetUserId(user);
            var assignedClaims = await _claimService.GetAssignedClaimsAsync(userId);
            return Ok(assignedClaims);
        }

        [HttpGet("{claimNumber}")]
        [Produces(typeof(ClaimResponse))]
        public async Task<IActionResult> GetClaimAsync(string claimNumber)
        {
            var claim = await _claimService.GetClaimAsync(claimNumber);
            return Ok(claim);
        }
    }
}
