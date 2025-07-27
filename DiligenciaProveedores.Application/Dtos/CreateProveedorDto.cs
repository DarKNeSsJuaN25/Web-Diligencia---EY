using System.ComponentModel.DataAnnotations;

namespace DiligenciaProveedores.Application.Dtos
{
    public class CreateProveedorDto
    {
        [Required(ErrorMessage = "La Razón Social es obligatoria.")]
        [StringLength(255, ErrorMessage = "La Razón Social no puede exceder los {1} caracteres.")]
        public string RazonSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Nombre Comercial es obligatorio.")]
        [StringLength(255, ErrorMessage = "El Nombre Comercial no puede exceder los {1} caracteres.")]
        public string NombreComercial { get; set; } = string.Empty;

        [Required(ErrorMessage = "La Identificación Tributaria (RUC) es obligatoria.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "La Identificación Tributaria (RUC) debe ser numérica y tener 11 dígitos.")]
        public string RUC { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Número Telefónico es obligatorio.")]
        [Phone(ErrorMessage = "El formato del Número Telefónico no es válido.")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Correo Electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del Correo Electrónico no es válido.")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Sitio Web es obligatorio.")]
        [Url(ErrorMessage = "El formato del Sitio Web no es válido.")]
        public string SitioWeb { get; set; } = string.Empty;

        [Required(ErrorMessage = "La Dirección Física es obligatoria.")]
        [StringLength(500, ErrorMessage = "La Dirección Física no puede exceder los {1} caracteres.")]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El País es obligatorio.")]
        [StringLength(100, ErrorMessage = "El País no puede exceder los {1} caracteres.")]
        public string Pais { get; set; } = string.Empty;

        [Required(ErrorMessage = "La Facturación Anual en USD es obligatoria.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La Facturación Anual en USD debe ser un valor positivo.")]
        public decimal FacturacionAnualUSD { get; set; }
    }
}