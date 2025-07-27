using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiligenciaProveedores.Domain.Entities
{
    [Table("Proveedores")]
    public class Proveedor
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "La Razón Social es obligatoria.")]
        [MaxLength(255)]
        public string RazonSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Nombre Comercial es obligatorio.")]
        [MaxLength(255)]
        public string NombreComercial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El RUC es obligatorio.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "El RUC debe tener exactamente 11 dígitos.")]
        public string RUC { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Teléfono es obligatorio.")]
        [Phone(ErrorMessage = "El formato del teléfono es inválido.")] // [Phone] es para validación de formato, no obligatoriedad.
        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Correo Electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico es inválido.")]
        [MaxLength(255)]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Sitio Web es obligatorio.")] // ¡CAMBIO AQUÍ!
        [Url(ErrorMessage = "El formato de la URL del Sitio Web es inválido.")]
        [MaxLength(255)]
        public string SitioWeb { get; set; } = string.Empty; // ¡CAMBIO AQUÍ! Eliminar '?' para que no sea nullable

        [Required(ErrorMessage = "La Dirección es obligatoria.")] // ¡CAMBIO AQUÍ!
        [MaxLength(255)] // Sincronizado con el frontend y tu backend actual
        public string Direccion { get; set; } = string.Empty; // ¡CAMBIO AQUÍ! Eliminar '?' para que no sea nullable

        [Required(ErrorMessage = "El País es obligatorio.")] // ¡CAMBIO AQUÍ!
        [MaxLength(100)]
        public string Pais { get; set; } = string.Empty; // ¡CAMBIO AQUÍ! Eliminar '?' para que no sea nullable

        [Required(ErrorMessage = "La Facturación Anual (USD) es obligatoria.")] // ¡CAMBIO AQUÍ!
        [Column(TypeName = "decimal(18,2)")]
        public decimal FacturacionAnualUSD { get; set; }

        // FechaCreacion y FechaActualizacion suelen gestionarse automáticamente y no necesitan [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }
    }
}