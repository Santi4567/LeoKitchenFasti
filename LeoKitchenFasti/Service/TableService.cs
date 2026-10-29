// Services/TableService.cs
using LeoKitchenFasti.Data;
using LeoKitchenFasti.DTOs;
using LeoKitchenFasti.Models;
using Microsoft.EntityFrameworkCore;

namespace LeoKitchenFasti.Services
{
    public class TableService : ITableService
    {
        private readonly AppDbContext _context;

        public TableService(AppDbContext context)
        {
            _context = context;
        }

        // --- ÁREAS ---
        public async Task<List<Area>> GetAreasWithTablesAsync()
        {
            // Retorna las áreas incluyendo sus mesas (como lo espera el frontend)
            return await _context.Areas
                .Include(a => a.Tables)
                .ToListAsync();
        }

        public async Task<Area> CreateAreaAsync(AreaCreateDto dto)
        {
            var nombreLimpio = dto.Name.Trim();
            var existe = await _context.Areas.AnyAsync(a => a.Name.ToLower() == nombreLimpio.ToLower());

            if (existe)
                throw new Exception($"El área '{nombreLimpio}' ya está registrada.");

            var area = new Area { Name = nombreLimpio };
            _context.Areas.Add(area);
            await _context.SaveChangesAsync();
            return area;
        }

        public async Task<bool> UpdateAreaAsync(int id, AreaCreateDto dto)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area == null) return false;

            var nombreLimpio = dto.Name.Trim();
            var existe = await _context.Areas.AnyAsync(a => a.Name.ToLower() == nombreLimpio.ToLower() && a.Id != id);

            if (existe)
                throw new Exception($"Ya existe otra área con el nombre '{nombreLimpio}'.");

            area.Name = nombreLimpio;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAreaAsync(int id)
        {
            var area = await _context.Areas.Include(a => a.Tables).FirstOrDefaultAsync(a => a.Id == id);
            if (area == null) return false;

            // Validación de integridad: no borrar un área que tiene mesas
            if (area.Tables.Any())
                throw new Exception("No puedes eliminar un área que aún tiene mesas asignadas.");

            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();
            return true;
        }

        // --- MESAS ---
        public async Task<Table> CreateTableAsync(TableCreateDto dto)
        {
            var nombreLimpio = dto.Name.Trim();

            // Verificamos si la mesa ya existe globalmente en el restaurante
            var existe = await _context.Tables.AnyAsync(t => t.Name.ToLower() == nombreLimpio.ToLower());

            if (existe)
                throw new Exception($"La mesa '{nombreLimpio}' ya existe en el sistema.");

            var table = new Table
            {
                Name = nombreLimpio,
                AreaId = dto.AreaId,
                Status = TableStatus.Libre
            };

            _context.Tables.Add(table);
            await _context.SaveChangesAsync();
            return table;
        }

        public async Task<bool> UpdateTableAsync(int id, TableCreateDto dto)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null) return false;

            table.Name = dto.Name;
            table.AreaId = dto.AreaId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTableAsync(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null) return false;

            // Aquí podrías agregar validación de si la mesa tiene cuentas activas antes de borrarla
            if (table.Status == TableStatus.Ocupada)
                throw new Exception("No puedes eliminar una mesa que está actualmente en servicio.");

            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}