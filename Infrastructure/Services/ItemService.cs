using Application.Abstractions;
using Application.DTOs.Item;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class ItemService : IItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ItemService> _logger;

        public ItemService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ItemService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ItemResponse>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all items");
            var items = await _unitOfWork.Repository<Item>().GetAllAsync();
            return _mapper.Map<IEnumerable<ItemResponse>>(items);
        }

        public async Task<ItemResponse?> GetByIdAsync(int id)
        {
            // Fixed the logger template syntax
            _logger.LogInformation("Fetching item with ID: {Id}", id);
            var item = await _unitOfWork.Repository<Item>().GetByIdAsync(id);
            return item == null ? null : _mapper.Map<ItemResponse>(item);
        }

        public async Task<ItemResponse?> CreateAsync(ItemRequest request)
        {
            _logger.LogInformation("Creating new item for Product {ProductId}", request.ProductId);

            // Business Rule: Ensure parent product exists before creating child item
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.ProductId);
            if (product == null) return null;

            var item = _mapper.Map<Item>(request);
            await _unitOfWork.Repository<Item>().AddAsync(item);

            await _unitOfWork.Commit();
            return _mapper.Map<ItemResponse>(item);
        }

        public async Task<ItemResponse?> UpdateAsync(int id, ItemRequest request)
        {
            _logger.LogInformation("Updating item with ID {Id}", id);
            var repo = _unitOfWork.Repository<Item>();
            var item = await repo.GetByIdAsync(id);

            if (item == null) return null;

            // Maps the request values onto the existing tracked entity
            _mapper.Map(request, item);

            await repo.UpdateAsync(item);
            await _unitOfWork.Commit();

            return _mapper.Map<ItemResponse>(item);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting item with ID {Id}", id);
            var repo = _unitOfWork.Repository<Item>();
            var item = await repo.GetByIdAsync(id);

            if (item == null) return false;

            await repo.DeleteAsync(item.Id);
            await _unitOfWork.Commit();

            return true;
        }
    }
}