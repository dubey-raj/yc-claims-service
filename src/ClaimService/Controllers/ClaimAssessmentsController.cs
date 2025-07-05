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
    [Route("claim/assessment")]
    public class ClaimAssessmentController : ControllerBase
    {
        private readonly ILogger<ClaimAssessmentController> _logger;
        private readonly IClaimService _claimService;
        private readonly IFileUploader _fileUploader;
        private readonly IEventPublisher _eventPublisher;
        public ClaimAssessmentController(ILogger<ClaimAssessmentController> logger, IClaimService  claimService, IFileUploader fileUploader,
            IEventPublisher eventPublisher)
        {
            _logger = logger;
            _claimService = claimService;
            _fileUploader = fileUploader;
            _eventPublisher = eventPublisher;
        }

        [HttpPost]
        [Produces(typeof(SubmitClaimResponse))]
        public async Task<IActionResult> SubmitClaimAssessmentAsync([FromForm] AssessClaimRequest assessClaimReqest)
        {
            var user = User;
            var userId = UserClaimHelper.GetUserId(user);
            assessClaimReqest.UserId = (int) userId;
            var assessmentResult = await _claimService.AssessClaimAsync(assessClaimReqest);
            
            if (assessmentResult.IsSuccess)
            {
                var uploadedFilePaths = await _fileUploader.UploadFilesAsync(assessmentResult.ClaimNumber, assessClaimReqest.SupportingFiles);
                await _claimService.AddClaimDocumentsAsync(assessmentResult.ClaimNumber, uploadedFilePaths, "AssessmentDoc");
                return Ok(assessClaimReqest);
            }

            return BadRequest(assessmentResult.Message);
        }

        [HttpPost("/claim/review")]
        [Produces(typeof(SubmitClaimResponse))]
        public async Task<IActionResult> SubmitClaimReviewAsync([FromForm] AssessClaimRequest assessClaimReqest)
        {
            var user = User;
            var userId = UserClaimHelper.GetUserId(user);
            assessClaimReqest.UserId = (int)userId;
            var reviewResult = await _claimService.ReviewClaimAsync(assessClaimReqest);

            if (reviewResult.IsSuccess)
            {
                return Ok(assessClaimReqest);
            }

            return BadRequest(reviewResult.Message);
        }

        private void PublishNotificationEvent()
        {
            _eventPublisher.PublishEventAsync<string>(string.Empty);
        }
    }
}
