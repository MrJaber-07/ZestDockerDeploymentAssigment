using Application.DTOs.Item;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IItemService
    {
        Task<IEnumerable<ItemResponse>> GetAllAsync();
        Task<ItemResponse?> GetByIdAsync(int id);
        Task<ItemResponse?> CreateAsync(ItemRequest request);
        Task<ItemResponse?> UpdateAsync(int id, ItemRequest request);
        Task<bool> DeleteAsync(int id);
    }
}