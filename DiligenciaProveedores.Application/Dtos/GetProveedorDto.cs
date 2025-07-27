namespace DiligenciaProveedores.Application.Dtos
{
    public class GetProveedorDto
    {
        public Guid Id { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string NombreComercial { get; set; } = string.Empty;
        public string RUC { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string? SitioWeb { get; set; }
        public string? Direccion { get; set; }
        public string? Pais { get; set; }
        public decimal FacturacionAnualUSD { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}
