using ClaimService.DataStorage.DAO;
using ClaimService.Model;
using Microsoft.EntityFrameworkCore;

namespace ClaimService.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly YcEclaimsDbContext _dbContext;
        private readonly IEventPublisher _eventPublisher;

        ///<inheritdoc/>
        public PolicyService(YcEclaimsDbContext YcEclaimsDbContext, IEventPublisher eventPublisher)
        {
            _dbContext = YcEclaimsDbContext;
            _eventPublisher = eventPublisher;
        }

        ///<inheritdoc/>
        public async Task<List<PolicyResponse>> GetPolicyAsync(int userId)
        {
           var policies = await _dbContext.Policies
                .Include(p => p.Vehicles)
                .Where(p => p.UserId == userId)
                .Select(policy => new PolicyResponse
                {
                    Id = policy.Id,
                    CoverageAmount = policy.CoverageAmount,
                    CreatedAt = policy.CreatedAt,
                    DeductibleAmount = policy.DeductibleAmount,
                    EffectiveDate = policy.EffectiveDate.ToString(),
                    ExpiryDate = policy.ExpiryDate.ToString(),
                    InsurerName = policy.InsurerName,
                    PolicyNumber = policy.PolicyNumber,
                    PolicyType = policy.PolicyType,
                    Status = policy.Status,
                    Vehicles = policy.Vehicles.Select(v => new VehicleResponse
                    {
                        Id = v.Id,
                        CreatedAt = v.CreatedAt,
                        Make = v.Make,
                        Model = v.Model,
                        RegistrationNumber = v.RegistrationNumber,
                        Year = v.Year
                    }
                ).ToList()
                }).ToListAsync();
            return policies;
        }

        public async Task<PolicyResponse> GetPolicyAsync(string policyNumber)
        {
            var policy = _dbContext.Policies
                .Include(p => p.Vehicles)
                .FirstOrDefault(p => p.PolicyNumber == policyNumber);
            if (policy != null)
            {
                var policyDetail = new PolicyResponse
                {
                    CoverageAmount = policy.CoverageAmount,
                    CreatedAt = policy.CreatedAt,
                    DeductibleAmount = policy.DeductibleAmount,
                    EffectiveDate = policy.EffectiveDate.ToString(),
                    ExpiryDate = policy.ExpiryDate.ToString(),
                    InsurerName = policy.InsurerName,
                    PolicyNumber = policy.PolicyNumber,
                    PolicyType = policy.PolicyType,
                    Status = policy.Status,
                    Vehicles = policy.Vehicles.Select(v => new VehicleResponse
                    {
                        Id = v.Id,
                        CreatedAt = v.CreatedAt,
                        Make = v.Make,
                        Model = v.Model,
                        RegistrationNumber = v.RegistrationNumber,
                        Year = v.Year
                    }
                ).ToList()
                };
                return policyDetail;
            }
            return null;
        }
    }
}
