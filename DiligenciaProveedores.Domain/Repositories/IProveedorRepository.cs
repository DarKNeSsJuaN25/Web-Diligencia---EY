using DiligenciaProveedores.Domain.Entities;
using DiligenciaProveedores.Domain.Entities.Pagination;
namespace DiligenciaProveedores.Domain.Repositories
{
    public interface IProveedorRepository
    {
        Task<(IEnumerable<Proveedor> items, int totalCount)> GetAllAsync(PaginationParams paginationParams);
        Task<Proveedor?> GetByIdAsync(Guid id);
        Task AddAsync(Proveedor proveedor);
        Task UpdateAsync(Proveedor proveedor); 
        Task DeleteAsync(Proveedor proveedor);
        Task SaveChangesAsync();
        Task<bool> RucExistsAsync(string ruc, Guid? excludeId = null);
        Task<Proveedor?> GetByRazonSocialAsync(string razonSocial);


    }
}