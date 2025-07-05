using Amazon.Auth.AccessControlPolicy;
using Amazon.Rekognition.Model;
using ClaimService.DataStorage.DAO;
using ClaimService.External;
using ClaimService.Model;
using ClaimService.Model.APIResponses;
using ClaimService.Model.Claim;
using ClaimService.Model.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClaimService.Services
{
    public class ClaimsService : IClaimService
    {
        private readonly ILogger<ClaimsService> _logger;
        private readonly YcEclaimsDbContext _dbContext;
        private readonly IUserAPIClient _userAPIClient;
        private readonly IEventPublisher _eventPublisher;

        ///<inheritdoc/>
        public ClaimsService(ILogger<ClaimsService> logger, YcEclaimsDbContext YcEclaimsDbContext, IUserAPIClient userAPIClient, IEventPublisher eventPublisher)
        {
            _logger = logger;
            _dbContext = YcEclaimsDbContext;
            _userAPIClient = userAPIClient;
            _eventPublisher = eventPublisher;
        }

        ///<inheritdoc/>
        public async Task<SubmitClaimResponse> AddClaimAsync(SubmitClaimDto claim)
        {
            var policy = await _dbContext.Policies
                .Include(p => p.Vehicles)
                .FirstOrDefaultAsync(p => p.PolicyNumber == claim.PolicyNumber);

            if (policy == null)
                return new SubmitClaimResponse { IsSuccess= false, Message = "Invalid policy number" };

            var claimNumber = GenerateClaimNumber();
            var inspectors = await GetAvailableAssigneeAsync("Inspector");
            var inspectorId = inspectors.FirstOrDefault().Id;
            var claimDao = new Claim
            {
                UserId = claim.UserId,
                PolicyId = policy.Id,
                ClaimNumber = claimNumber,
                IncidentDate = DateOnly.FromDateTime(claim.IncidentDate),
                IncidentLocation = claim.IncidentLocation,
                IncidentDescription = claim.IncidentDescription,
                Status = ClaimStatus.Submitted.ToString(),
                SubmittedAt = DateTime.Now
            };

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            
            try
            {
                await _dbContext.Claims.AddAsync(claimDao);
                await _dbContext.SaveChangesAsync();
                var claimAssessment = new ClaimAssessment
                {
                    ClaimId = claimDao.Id,
                    InspectorId = inspectorId
                };

                await _dbContext.ClaimAssessments.AddAsync(claimAssessment);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                var res = await UpdateInspectorAssignmentAsync(inspectorId);

                var user = await _userAPIClient.GetAsync<UserResponse>($"user/{claim.UserId}");
                await PublishClaimNotificationEvent("Claim.Submitted", user.Email, policy.Vehicles.FirstOrDefault().RegistrationNumber,
                    claimNumber, ClaimStatus.Submitted.ToString(), $"{user.FirstName} {user.LastName}", string.Empty, claim.IncidentDate.ToString(), policy.PolicyNumber,
                    string.Empty, string.Empty, DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while submitting the claim");
                await transaction.RollbackAsync();
                return new SubmitClaimResponse { IsSuccess = false, Message = "Error occurred while submitting claim" };
            }

            return new SubmitClaimResponse { IsSuccess= true, ClaimNumber = claimNumber };
        }

        ///<inheritdoc/>
        public async Task<bool> AddClaimDocumentsAsync(string claimNumber, List<string> docPaths, string docCategory)
        {
            var claim = _dbContext.Claims.FirstOrDefault(c => c.ClaimNumber == claimNumber);
            var claimDocuments = docPaths.Select(file => new ClaimDocument
            {
                ClaimId = claim.Id,
                UploadedAt = DateTime.Now,
                FileUrl = file,
                FileType = docCategory
            });
            await _dbContext.ClaimDocuments.AddRangeAsync(claimDocuments);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        ///<inheritdoc/>
        public async Task<SubmitClaimResponse> AssessClaimAsync(AssessClaimRequest assessClaim)
        {
            var claim = await _dbContext.Claims
                .Include(c => c.ClaimAssessments)
                .FirstOrDefaultAsync(c => c.ClaimNumber == assessClaim.ClaimNumber);
            var managers = await GetAvailableAssigneeAsync("Manager");
            var managerId = managers.FirstOrDefault().Id;
            claim.Status = ClaimStatus.UnderReview.ToString();
            claim.LastUpdated = DateTime.Now;

            var claimAssessment = claim.ClaimAssessments.FirstOrDefault();
            claimAssessment.InspectionDate = DateOnly.FromDateTime(assessClaim.InspectionDate);
            claimAssessment.EstimatedAmount = assessClaim.DamageEstimate;
            claimAssessment.Status = assessClaim.ClaimStatus;
            claimAssessment.DamageAssessed = assessClaim.Notes;
            claimAssessment.ManagerId = managerId;
            await _dbContext.SaveChangesAsync();

            await UpdateInspectorAssignmentAsync(managerId, 1);
            await UpdateInspectorAssignmentAsync(assessClaim.UserId, -1);
            return new SubmitClaimResponse { IsSuccess = true, ClaimNumber = assessClaim.ClaimNumber };
        }

        ///<inheritdoc/>
        public async Task<SubmitClaimResponse> ReviewClaimAsync(AssessClaimRequest reviewClaim)
        {
            var claim = await _dbContext.Claims
                .Include(c => c.ClaimAssessments)
                .Include(c => c.Policy)
                .ThenInclude(c => c.Vehicles)
                .FirstOrDefaultAsync(c => c.ClaimNumber == reviewClaim.ClaimNumber);
            claim.Status = reviewClaim.ClaimStatus;
            claim.LastUpdated = DateTime.Now;

            var claimAssessment = claim.ClaimAssessments.FirstOrDefault();
            claimAssessment.Status = reviewClaim.ClaimStatus;
            claimAssessment.ReviewNote = reviewClaim.Notes;
            await _dbContext.SaveChangesAsync();

            await UpdateInspectorAssignmentAsync(reviewClaim.UserId, -1);
            var user = await _userAPIClient.GetAsync<UserResponse>($"user/{claim.UserId}");
            await PublishClaimNotificationEvent("Claim.Reviewed", user.Email, claim.Policy.Vehicles.FirstOrDefault().RegistrationNumber,
                    claim.ClaimNumber, reviewClaim.ClaimStatus, $"{user.FirstName} {user.LastName}", claim.ClaimAssessments.FirstOrDefault().DamageAssessed,
                    claim.IncidentDate.ToString(), claim.Policy.PolicyNumber, DateTime.Now.ToString(), reviewClaim.Notes, claim.SubmittedAt.ToString());
            return new SubmitClaimResponse { IsSuccess = true, ClaimNumber = reviewClaim.ClaimNumber };
        }

        ///<inheritdoc/>
        public async Task<ClaimResponse> GetClaimAsync(string claimNumber)
        {
            var claim = await _dbContext.Claims
                .Include(c => c.ClaimAssessments)
                .Include(c => c.ClaimDocuments)
                .Include(p => p.Policy)
                .ThenInclude(p => p.Vehicles)
                .FirstOrDefaultAsync(c => c.ClaimNumber == claimNumber);
            var claimResponse = new ClaimResponse
            {
                ClaimNumber = claimNumber,
                ApprovedAmount = claim.ClaimAssessments.FirstOrDefault()?.EstimatedAmount,
                EstimatedAmount = claim.ClaimAssessments.FirstOrDefault()?.EstimatedAmount,
                IncidentDate = claim.IncidentDate,
                IncidentDescription = claim.IncidentDescription,
                IncidentLocation = claim.IncidentLocation,
                InspectionDate = claim.ClaimAssessments.FirstOrDefault().InspectionDate.GetValueOrDefault(),
                InspectionNote = claim.ClaimAssessments.FirstOrDefault().DamageAssessed,
                InspectorRecommendation = claim.ClaimAssessments.FirstOrDefault().Status,
                Status = claim.Status,
                SubmittedAt = claim.SubmittedAt.GetValueOrDefault(),
                VehicleNumber = claim.Policy.Vehicles.FirstOrDefault()?.RegistrationNumber,
                DocumentUrls = claim.ClaimDocuments.Select(cd => cd.FileUrl).ToList()
            };

            return claimResponse;
        }

        ///<inheritdoc/>
        public async Task<ClaimListResponse> GetClaimsAsync(long userId)
        {
            var claimListResponse = new ClaimListResponse() { };
            var claims = await _dbContext.Claims
                .Include(c => c.ClaimAssessments)
                .Include(c => c.ClaimDocuments)
                .Include(c => c.Policy)
                .ThenInclude(p => p.Vehicles)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            var claimsList = claims.Select(claim =>
            {
                var assessment = claim.ClaimAssessments.FirstOrDefault();
                var vehicle = claim.Policy?.Vehicles.FirstOrDefault();

                return new ClaimResponse
                {
                    ClaimNumber = claim.ClaimNumber,
                    ApprovedAmount = assessment?.EstimatedAmount ?? 0,
                    EstimatedAmount = assessment?.EstimatedAmount,
                    IncidentDate = claim.IncidentDate,
                    IncidentDescription = claim.IncidentDescription,
                    IncidentLocation = claim.IncidentLocation,
                    Status = claim.Status,
                    SubmittedAt = claim.SubmittedAt ?? DateTime.MinValue,
                    VehicleNumber = vehicle?.RegistrationNumber ?? "N/A",
                    DocumentUrls = claim.ClaimDocuments.Select(cd => cd.FileUrl).ToList()
                };
            }).ToList();
            claimListResponse.Claims = claimsList;

            return claimListResponse;
        }

        ///<inheritdoc/>
        public async Task<ClaimListResponse> GetAssignedClaimsAsync(long userId)
        {
            var claimListResponse = new ClaimListResponse();

            var claimsList = await _dbContext.Claims
                .Where(c => c.ClaimAssessments.Any(ca => (c.Status == ClaimStatus.Submitted.ToString() && ca.InspectorId == userId) || 
                (c.Status == ClaimStatus.UnderReview.ToString() && ca.ManagerId == userId)))
                .Select(claim => new ClaimResponse
                {
                    ClaimNumber = claim.ClaimNumber,
                    ApprovedAmount = claim.ClaimAssessments.Where(ca => ca.InspectorId == userId || ca.ManagerId == userId)
                    .Select(ca => ca.EstimatedAmount).FirstOrDefault() ?? 0,
                    IncidentDate = claim.IncidentDate,
                    IncidentDescription = claim.IncidentDescription,
                    IncidentLocation = claim.IncidentLocation,
                    Status = claim.Status,
                    SubmittedAt = claim.SubmittedAt ?? DateTime.MinValue,
                    VehicleNumber = claim.Policy.Vehicles.Select(v => v.RegistrationNumber).FirstOrDefault(),
                    DocumentUrls = claim.ClaimDocuments.Select(cd => cd.FileUrl).ToList()
                }).ToListAsync();
            claimListResponse.Claims = claimsList;

            return claimListResponse;
        }

        private static string GenerateClaimNumber()
        {
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            var uniquePart = Guid.NewGuid().ToString("N")[..8].ToUpper();
            return $"CLM-{date}-{uniquePart}";
        }

        private async Task<List<UserAvailabilityResponse>> GetAvailableAssigneeAsync(string userType)
        {
            try
            {
                var inspectors = await _userAPIClient.GetAsync<List<UserAvailabilityResponse>>($"user-api/user/{userType}/availability?region=delhi&limit=1");
                return inspectors;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<bool> UpdateInspectorAssignmentAsync(int userId, int deltaCount = 1)
        {
            var request = new UpdateAssignedCaseCountRequest { UserId = userId, DeltaCount = deltaCount };
            var result = await _userAPIClient.PatchAsync<UpdateAssignedCaseCountRequest, bool>("user-api/user/update-assigned-count", request);
            return result;
        }

        private async Task PublishClaimNotificationEvent(string eventType, string email, string vehicleNumber, string claimNumber,
            string claimStatus, string fullName, string approvedAmount, string incidentDt, string poilicyNumber, string reviewDate, string remarks,
            string submissionDate)
        {
            var notificationEvent = new NotificationEvent<ClaimEvent>()
            {
                EventType = eventType,
                TimeStamp = DateTime.UtcNow,
                Payload = new ClaimEvent
                {
                    Email = email,
                    FullName = fullName,
                    ApprovedAmount = approvedAmount,
                    ClaimId = claimNumber,
                    ClaimStatus = claimStatus,
                    IncidentDate = incidentDt,
                    PolicyNumber = poilicyNumber,
                    ReviewDate = reviewDate,
                    ReviewRemarks = remarks,
                    SubmissionDate = submissionDate,
                    VehicleNumber = vehicleNumber
                }
            };
            await _eventPublisher.PublishEventAsync(notificationEvent);
        }
    }
}