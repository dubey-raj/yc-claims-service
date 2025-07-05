using ClaimService.Model;

namespace ClaimService.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPolicyService
    {
        /// <summary>
        /// Get Policy details for user
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <returns></returns>
        Task<List<PolicyResponse>> GetPolicyAsync(int userId);

        /// <summary>
        /// Get Policy details by policy number
        /// </summary>
        /// <param name="policyNumber">UserId</param>
        /// <returns></returns>
        Task<PolicyResponse> GetPolicyAsync(string policyNumber);
    }
}
