namespace DiligenciaProveedores.Domain.Entities.Pagination
{
    public class PaginatedResultDto<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public long TotalCount { get; set; }
        public List<T> Items { get; set; } = new List<T>();
    }
}
