using DiligenciaProveedores.Application.Dtos;
using DiligenciaProveedores.Domain.Entities.Pagination;
using DiligenciaProveedores.Application.Dtos.Screening;
using DiligenciaProveedores.Domain.Dtos.Screening;
using DiligenciaProveedores.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiligenciaProveedores.Api.Controllers{
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedoresController : ControllerBase
    {
        private readonly IProveedorService _proveedorService;

        public ProveedoresController(IProveedorService proveedorService)
        {
            _proveedorService = proveedorService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationParams paginationParams)
        {
            var paginatedResult = await _proveedorService.ObtenerTodosAsync(paginationParams);
            return Ok(paginatedResult);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var proveedor = await _proveedorService.ObtenerPorIdAsync(id);
            return Ok(proveedor);
        }

        [HttpGet("name")]
        public async Task<ActionResult<GetProveedorDto>> GetByRazonSocial(string razonSocial)
        {
            if (string.IsNullOrWhiteSpace(razonSocial))
            {
                return BadRequest("La razón social no puede estar vacía.");
            }

            var proveedor = await _proveedorService.ObtenerPorRazonSocialAsync(razonSocial);

            if (proveedor == null)
            {
                return NotFound($"Proveedor con razón social '{razonSocial}' no encontrado.");
            }

            return Ok(proveedor);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateProveedorDto dto)
        {
            var nuevo = await _proveedorService.CrearAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = nuevo.Id }, nuevo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdateProveedorDto dto)
        {
            await _proveedorService.ActualizarAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Delete(Guid id)
        {
            await _proveedorService.EliminarAsync(id);
            return NoContent();
        }
        [HttpPost("{id}/screening")]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<ScrapingResponseDto>> PerformScreening(Guid id, [FromBody] ScreeningRequestDto request)
        {
            var screeningResults = await _proveedorService.PerformScreeningAsync(id, User);
            return Ok(screeningResults);
        }
    }
}