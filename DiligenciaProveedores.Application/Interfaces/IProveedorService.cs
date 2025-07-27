using DiligenciaProveedores.Application.Dtos;
using DiligenciaProveedores.Domain.Dtos.Screening;
using DiligenciaProveedores.Domain.Entities.Pagination;
using System.Security.Claims;

namespace DiligenciaProveedores.Application.Interfaces
{
    public interface IProveedorService
    {
        Task<PaginatedResultDto<GetProveedorDto>> ObtenerTodosAsync(PaginationParams paginationParams);

        Task<GetProveedorDto?> ObtenerPorIdAsync(Guid id);
        Task<GetProveedorDto?> ObtenerPorRazonSocialAsync(string razonSocial);
        Task<GetProveedorDto> CrearAsync(CreateProveedorDto dto);
        Task<bool> ActualizarAsync(Guid id, UpdateProveedorDto dto);
        Task<bool> EliminarAsync(Guid id);
        Task<ScrapingResponseDto> PerformScreeningAsync(Guid proveedorId, ClaimsPrincipal userClaims);


    }
}
