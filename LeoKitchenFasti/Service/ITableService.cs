// Services/ITableService.cs
using LeoKitchenFasti.DTOs;
using LeoKitchenFasti.Models;

namespace LeoKitchenFasti.Services
{
    public interface ITableService
    {
        // Áreas
        Task<List<Area>> GetAreasWithTablesAsync();
        Task<Area> CreateAreaAsync(AreaCreateDto dto);
        Task<bool> UpdateAreaAsync(int id, AreaCreateDto dto);
        Task<bool> DeleteAreaAsync(int id);

        // Mesas
        Task<Table> CreateTableAsync(TableCreateDto dto);
        Task<bool> UpdateTableAsync(int id, TableCreateDto dto);
        Task<bool> DeleteTableAsync(int id);
    }
}