// Controllers/TablesController.cs
using LeoKitchenFasti.Attributes;
using LeoKitchenFasti.DTOs;
using LeoKitchenFasti.Models;
using LeoKitchenFasti.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeoKitchenFasti.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Exige JWT
    public class TablesController : ControllerBase
    {
        private readonly ITableService _tableService;

        public TablesController(ITableService tableService)
        {
            _tableService = tableService;
        }

        // --- LECTURA (Cualquiera con acceso al módulo de mesas) ---
        [HttpGet("areas")]
        [RequierePermiso("view.tables")]
        public async Task<ActionResult<ApiResponse<List<Area>>>> GetAreas()
        {
            var areas = await _tableService.GetAreasWithTablesAsync();
            return Ok(ApiResponse<List<Area>>.Exito(areas, "Áreas cargadas exitosamente"));
        }

        // --- ADMINISTRACIÓN DE ÁREAS ---
        [HttpPost("areas")]
        [RequierePermiso("manage.areas")]
        public async Task<ActionResult<ApiResponse<Area>>> CreateArea([FromBody] AreaCreateDto dto)
        {
            var area = await _tableService.CreateAreaAsync(dto);
            return Ok(ApiResponse<Area>.Exito(area, "Área creada exitosamente"));
        }

        [HttpPut("areas/{id}")]
        [RequierePermiso("manage.areas")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateArea(int id, [FromBody] AreaCreateDto dto)
        {
            var exito = await _tableService.UpdateAreaAsync(id, dto);
            if (!exito) return NotFound(ApiResponse<object>.Error("Área no encontrada"));
            return Ok(ApiResponse<object>.Exito(null, "Área actualizada correctamente"));
        }

        [HttpDelete("areas/{id}")]
        [RequierePermiso("manage.areas")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteArea(int id)
        {
            try
            {
                var exito = await _tableService.DeleteAreaAsync(id);
                if (!exito) return NotFound(ApiResponse<object>.Error("Área no encontrada"));
                return Ok(ApiResponse<object>.Exito(null, "Área eliminada correctamente"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        // --- ADMINISTRACIÓN DE MESAS ---
        [HttpPost("tables")]
        [RequierePermiso("manage.areas")] // Usamos el mismo permiso general para administrar el salón
        public async Task<ActionResult<ApiResponse<Table>>> CreateTable([FromBody] TableCreateDto dto)
        {
            var table = await _tableService.CreateTableAsync(dto);
            return Ok(ApiResponse<Table>.Exito(table, "Mesa creada exitosamente"));
        }

        [HttpPut("tables/{id}")]
        [RequierePermiso("manage.areas")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateTable(int id, [FromBody] TableCreateDto dto)
        {
            var exito = await _tableService.UpdateTableAsync(id, dto);
            if (!exito) return NotFound(ApiResponse<object>.Error("Mesa no encontrada"));
            return Ok(ApiResponse<object>.Exito(null, "Mesa actualizada correctamente"));
        }

        [HttpDelete("tables/{id}")]
        [RequierePermiso("manage.areas")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteTable(int id)
        {
            try
            {
                var exito = await _tableService.DeleteTableAsync(id);
                if (!exito) return NotFound(ApiResponse<object>.Error("Mesa no encontrada"));
                return Ok(ApiResponse<object>.Exito(null, "Mesa eliminada correctamente"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }
    }
}