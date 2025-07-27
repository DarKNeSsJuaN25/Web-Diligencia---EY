namespace DiligenciaProveedores.Domain.Entities.Pagination
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string SortBy { get; set; } = "FechaCreacion";
        public string SortOrder { get; set; } = "desc";
        public string? SearchTerm { get; set; }
    }
}
