using DiligenciaProveedores.Domain.Entities;
using DiligenciaProveedores.Domain.Entities.Pagination;
using DiligenciaProveedores.Domain.Repositories;
using DiligenciaProveedores.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DiligenciaProveedores.Infrastructure.Repositories
{
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly ApplicationDbContext _context;

        public ProveedorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Proveedor> items, int totalCount)> GetAllAsync(PaginationParams paginationParams)
        {
            var query = _context.Proveedores.AsQueryable();

            if (!string.IsNullOrWhiteSpace(paginationParams.SearchTerm))
            {
                query = query.Where(p => p.NombreComercial.Contains(paginationParams.SearchTerm) ||
                                         p.RazonSocial.Contains(paginationParams.SearchTerm));
            }

            var totalCount = await query.CountAsync();

            query = ApplySorting(query, paginationParams.SortBy, paginationParams.SortOrder);

            var items = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        private static IQueryable<Proveedor> ApplySorting(IQueryable<Proveedor> query, string sortBy, string sortOrder)
        {
            Expression<Func<Proveedor, object?>> keySelector = sortBy.ToLowerInvariant() switch
            {
                "fechaactualizacion" => p => p.FechaActualizacion,
                "nombrecomercial" => p => p.NombreComercial,
                "razonsocial" => p => p.RazonSocial,
                _ => p => p.FechaActualizacion
            };

            return sortOrder.ToLowerInvariant() == "desc"
                ? query.OrderByDescending(keySelector)
                : query.OrderBy(keySelector);
        }

        public async Task<Proveedor?> GetByIdAsync(Guid id)
        {
            return await _context.Proveedores.FindAsync(id);
        }

        public async Task<Proveedor?> GetByRazonSocialAsync(string razonSocial)
        {
            return await _context.Proveedores
                                 .FirstOrDefaultAsync(p => p.RazonSocial.ToLower().Contains(razonSocial.ToLower()));
        }

        public async Task AddAsync(Proveedor proveedor)
        {
            await _context.Proveedores.AddAsync(proveedor);
            await _context.SaveChangesAsync();
        }

        public Task UpdateAsync(Proveedor proveedor)
        {
            _context.Proveedores.Update(proveedor);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Proveedor proveedor)
        {
            _context.Proveedores.Remove(proveedor);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RucExistsAsync(string ruc, Guid? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Proveedores
                                    .AnyAsync(p => p.RUC == ruc && p.Id != excludeId.Value);
            }
            else
            {
                return await _context.Proveedores
                                    .AnyAsync(p => p.RUC == ruc);
            }
        }
    }
}
