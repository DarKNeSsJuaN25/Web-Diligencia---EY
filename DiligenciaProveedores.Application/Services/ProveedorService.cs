using DiligenciaProveedores.Application.Dtos;
using DiligenciaProveedores.Application.Exceptions;
using DiligenciaProveedores.Application.Interfaces;
using DiligenciaProveedores.Domain.Dtos.Screening;
using DiligenciaProveedores.Domain.Entities;
using DiligenciaProveedores.Domain.Entities.Pagination;
using DiligenciaProveedores.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DiligenciaProveedores.Application.Services
{
    public class ProveedorService : IProveedorService
    {
        private readonly IProveedorRepository _proveedorRepository;
        private readonly IScrapingApiClient _scrapingApiClient;
        private readonly IScrapingCredentialService _scrapingCredentialService;
        private readonly UserManager<IdentityUser> _userManager;

        public ProveedorService(IProveedorRepository proveedorRepository, IScrapingApiClient scrapingApiClient,
            IScrapingCredentialService scrapingCredentialService, UserManager<IdentityUser> userManager)
        {
            _proveedorRepository = proveedorRepository;
            _scrapingApiClient = scrapingApiClient;
            _scrapingCredentialService = scrapingCredentialService;
            _userManager = userManager;
        }

        public async Task<PaginatedResultDto<GetProveedorDto>> ObtenerTodosAsync(PaginationParams paginationParams)
        {
            var (proveedores, totalCount) = await _proveedorRepository.GetAllAsync(paginationParams);

            var proveedorDtos = proveedores.Select(p => new GetProveedorDto
            {
                Id = p.Id,
                RazonSocial = p.RazonSocial,
                NombreComercial = p.NombreComercial,
                RUC = p.RUC,
                Telefono = p.Telefono,
                CorreoElectronico = p.CorreoElectronico,
                SitioWeb = p.SitioWeb,
                Direccion = p.Direccion,
                Pais = p.Pais,
                FacturacionAnualUSD = p.FacturacionAnualUSD,
                FechaCreacion = p.FechaCreacion,
                FechaActualizacion = p.FechaActualizacion
            }).ToList();

            return new PaginatedResultDto<GetProveedorDto>
            {
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / paginationParams.PageSize),
                Items = proveedorDtos
            };
        }

        public async Task<GetProveedorDto?> ObtenerPorIdAsync(Guid id)
        {
            var proveedor = await _proveedorRepository.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Proveedor), id);

            return new GetProveedorDto
            {
                Id = proveedor.Id,
                RazonSocial = proveedor.RazonSocial,
                NombreComercial = proveedor.NombreComercial,
                RUC = proveedor.RUC,
                Telefono = proveedor.Telefono,
                CorreoElectronico = proveedor.CorreoElectronico,
                SitioWeb = proveedor.SitioWeb,
                Direccion = proveedor.Direccion,
                Pais = proveedor.Pais,
                FacturacionAnualUSD = proveedor.FacturacionAnualUSD,
                FechaCreacion = proveedor.FechaCreacion,
                FechaActualizacion = proveedor.FechaActualizacion
            };
        }

        public async Task<GetProveedorDto?> ObtenerPorRazonSocialAsync(string razonSocial)
        {
            var proveedor = await _proveedorRepository.GetByRazonSocialAsync(razonSocial);
            if (proveedor == null) return null;

            return new GetProveedorDto
            {
                Id = proveedor.Id,
                RazonSocial = proveedor.RazonSocial,
                NombreComercial = proveedor.NombreComercial,
                RUC = proveedor.RUC,
                Telefono = proveedor.Telefono,
                CorreoElectronico = proveedor.CorreoElectronico,
                SitioWeb = proveedor.SitioWeb,
                Direccion = proveedor.Direccion,
                Pais = proveedor.Pais,
                FacturacionAnualUSD = proveedor.FacturacionAnualUSD,
                FechaCreacion = proveedor.FechaCreacion,
                FechaActualizacion = proveedor.FechaActualizacion
            };
        }

        public async Task<GetProveedorDto> CrearAsync(CreateProveedorDto dto)
        {
            var rucExists = await _proveedorRepository.RucExistsAsync(dto.RUC);
            if (rucExists)
                throw new ValidationException("RUC", "La Identificación Tributaria (RUC) ya está registrada para otro proveedor.");

            var proveedor = new Proveedor
            {
                Id = Guid.NewGuid(),
                RazonSocial = dto.RazonSocial,
                NombreComercial = dto.NombreComercial,
                RUC = dto.RUC,
                Telefono = dto.Telefono,
                CorreoElectronico = dto.CorreoElectronico,
                SitioWeb = dto.SitioWeb,
                Direccion = dto.Direccion,
                Pais = dto.Pais,
                FacturacionAnualUSD = dto.FacturacionAnualUSD,
                FechaCreacion = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow
            };

            await _proveedorRepository.AddAsync(proveedor);
            await _proveedorRepository.SaveChangesAsync();

            return new GetProveedorDto
            {
                Id = proveedor.Id,
                RazonSocial = proveedor.RazonSocial,
                NombreComercial = proveedor.NombreComercial,
                RUC = proveedor.RUC,
                Telefono = proveedor.Telefono,
                CorreoElectronico = proveedor.CorreoElectronico,
                SitioWeb = proveedor.SitioWeb,
                Direccion = proveedor.Direccion,
                Pais = proveedor.Pais,
                FacturacionAnualUSD = proveedor.FacturacionAnualUSD,
                FechaCreacion = proveedor.FechaCreacion,
                FechaActualizacion = proveedor.FechaActualizacion
            };
        }

        public async Task<bool> ActualizarAsync(Guid id, UpdateProveedorDto dto)
        {
            var proveedor = await _proveedorRepository.GetByIdAsync(id);
            if (proveedor == null)
                throw new NotFoundException(nameof(Proveedor), id);

            var rucExists = await _proveedorRepository.RucExistsAsync(dto.RUC, id);
            if (rucExists)
                throw new ValidationException("RUC", "La Identificación Tributaria (RUC) ya está registrada para otro proveedor.");

            proveedor.RazonSocial = dto.RazonSocial;
            proveedor.NombreComercial = dto.NombreComercial;
            proveedor.RUC = dto.RUC;
            proveedor.Telefono = dto.Telefono;
            proveedor.CorreoElectronico = dto.CorreoElectronico;
            proveedor.SitioWeb = dto.SitioWeb;
            proveedor.Direccion = dto.Direccion;
            proveedor.Pais = dto.Pais;
            proveedor.FacturacionAnualUSD = dto.FacturacionAnualUSD;
            proveedor.FechaActualizacion = DateTime.UtcNow;

            await _proveedorRepository.UpdateAsync(proveedor);
            await _proveedorRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarAsync(Guid id)
        {
            var proveedor = await _proveedorRepository.GetByIdAsync(id);
            if (proveedor == null)
                throw new NotFoundException(nameof(Proveedor), id);

            await _proveedorRepository.DeleteAsync(proveedor);
            await _proveedorRepository.SaveChangesAsync();

            return true;
        }

        public async Task<ScrapingResponseDto> PerformScreeningAsync(Guid proveedorId, ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Usuario no autenticado o ID de usuario no encontrado.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("Usuario no encontrado.");

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Admin"))
                throw new UnauthorizedAccessException("Acceso denegado. Solo los usuarios con rol 'Admin' pueden realizar el screening.");

            var scrapingCreds = _scrapingCredentialService.GetScrapingApiCredentials();
            if (string.IsNullOrEmpty(scrapingCreds.Username) || string.IsNullOrEmpty(scrapingCreds.Password))
                throw new ApplicationException("Las credenciales de la API de scraping no están configuradas correctamente.");

            var loginResponse = await _scrapingApiClient.LoginScraperApiAsync(
                scrapingCreds.Username,
                scrapingCreds.Password,
                scrapingCreds.TenantId
            );

            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
                throw new ApplicationException("No se pudo obtener un token de la API de scraping. Verifique credenciales o conectividad.");

            var proveedor = await _proveedorRepository.GetByIdAsync(proveedorId);
            if (proveedor == null)
                throw new NotFoundException(nameof(Proveedor), proveedorId);

            return await _scrapingApiClient.ScrapeCompanyAsync(proveedor.NombreComercial, loginResponse.Token);
        }
    }
}
