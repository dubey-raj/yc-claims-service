using ClaimService.Model;
using ClaimService.Model.Claim;

namespace ClaimService.Services
{
    public interface IClaimService
    {
        /// <summary>
        /// Register a new customer
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        Task<SubmitClaimResponse> AddClaimAsync(SubmitClaimDto claim);

        /// <summary>
        /// Add documents to claim
        /// </summary>
        /// <param name="claimNumber"></param>
        /// <param name="docPaths"></param>
        /// <returns></returns>
        Task<bool> AddClaimDocumentsAsync(string claimNumber, List<string> docPaths, string docCategory);

        /// <summary>
        /// Update the claim
        /// </summary>
        /// <param name="assessClaim"></param>
        /// <returns></returns>
        Task<SubmitClaimResponse> AssessClaimAsync(AssessClaimRequest assessClaim);

        /// <summary>
        /// Update the claim
        /// </summary>
        /// <param name="assessClaim"></param>
        /// <returns></returns>
        Task<SubmitClaimResponse> ReviewClaimAsync(AssessClaimRequest reviewClaim);

        /// <summary>
        /// Get claim details by claim number
        /// </summary>
        /// <param name="claimNumber"></param>
        /// <returns></returns>
        Task<ClaimResponse> GetClaimAsync(string claimNumber);

        /// <summary>
        /// Get list of claims for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ClaimListResponse> GetClaimsAsync(long userId);

        /// <summary>
        /// Get list of assigned claims
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ClaimListResponse> GetAssignedClaimsAsync(long userId);
    }
}
