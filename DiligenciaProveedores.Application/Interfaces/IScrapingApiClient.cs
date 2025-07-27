using DiligenciaProveedores.Application.Dtos.Scraping;
using DiligenciaProveedores.Domain.Dtos.Screening;

namespace DiligenciaProveedores.Application.Interfaces
{
    public interface IScrapingApiClient
    {
        Task<ScrapingLoginResponseDto?> LoginScraperApiAsync(string username, string password, string tenantId);
        Task<ScrapingResponseDto> ScrapeCompanyAsync(string companyName, string authToken);
    }
}