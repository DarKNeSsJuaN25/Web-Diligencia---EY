using DiligenciaProveedores.Application.Interfaces;
using DiligenciaProveedores.Application.Dtos; 
using Microsoft.Extensions.Configuration; 

namespace DiligenciaProveedores.Infrastructure.Services
{
    public class ScrapingCredentialService : IScrapingCredentialService
    {
        private readonly IConfiguration _configuration;

        public ScrapingCredentialService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ScrapingApiCredentialsDto GetScrapingApiCredentials()
        {
            var username = _configuration["ScrapingApi:Username"];
            var password = _configuration["ScrapingApi:Password"];
            var tenantId = _configuration["ScrapingApi:TenantId"];

            return new ScrapingApiCredentialsDto
            {
                Username = username,
                Password = password,
                TenantId = tenantId
            };
        }
    }
}