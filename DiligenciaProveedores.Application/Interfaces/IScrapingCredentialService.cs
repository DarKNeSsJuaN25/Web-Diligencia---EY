using DiligenciaProveedores.Application.Dtos;

namespace DiligenciaProveedores.Application.Interfaces
{
    public interface IScrapingCredentialService
    {
        ScrapingApiCredentialsDto GetScrapingApiCredentials();
    }
}