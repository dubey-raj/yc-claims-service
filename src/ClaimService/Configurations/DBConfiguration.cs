using ClaimService.DataStorage.DAO;
using Microsoft.EntityFrameworkCore;

namespace ClaimService.Configurations
{
    public static class DBConfiguration
    {
        public static IServiceCollection AddUsersDBContext(this IServiceCollection services, IConfiguration configuration) {
            services.AddTransient<YcEclaimsDbContext>();
            services.AddDbContext<YcEclaimsDbContext>(opt => {
                var dbConnectionSetting = configuration.GetSection("DBConnectionInfo");
                if (dbConnectionSetting == null)
                {
                    throw new NotImplementedException("DbConnectionInfo configuration was not found");
                }
                var dbConnectionInfo = new DbConnectionInfo();
                dbConnectionSetting.Bind(dbConnectionInfo);
                opt.UseNpgsql(dbConnectionInfo.ToConnectionString());
            });
            return services;
        }
    }
}
