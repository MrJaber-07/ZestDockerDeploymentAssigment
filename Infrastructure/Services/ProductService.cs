using Application.Abstractions; 
using Application.ViewModels.Product;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductResponse>> GetAllAsync()
        {
            var products = await _unitOfWork.Repository<Product>().GetAllAsync();
            return _mapper.Map<IEnumerable<ProductResponse>>(products);
        }

        public async Task<ProductResponse?> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            return product == null ? null : _mapper.Map<ProductResponse>(product);
        }

        public async Task<ProductResponse> CreateAsync(ProductRequest request)
        {
            var entity = _mapper.Map<Product>(request);

            await _unitOfWork.Repository<Product>().AddAsync(entity);

            await _unitOfWork.Commit();

            _logger.LogInformation("Product created with ID: {Id}", entity.Id);
            return _mapper.Map<ProductResponse>(entity);
        }

        public async Task<ProductResponse> UpdateAsync(int id, ProductRequest request)
        {
            var repo = _unitOfWork.Repository<Product>();
            var existing = await repo.GetByIdAsync(id);

            if (existing == null)
            {
                _logger.LogWarning("Update failed: Product {Id} not found.", id);
                throw new KeyNotFoundException($"Product with id {id} not found.");
            }

            _mapper.Map(request, existing);
            await repo.UpdateAsync(existing);

            await _unitOfWork.Commit();

            _logger.LogInformation("Product {Id} updated successfully.", id);
            return _mapper.Map<ProductResponse>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var repo = _unitOfWork.Repository<Product>();
            var existing = await repo.GetByIdAsync(id);

            if (existing == null) return false;

            await repo.DeleteAsync(id);
            await _unitOfWork.Commit(); 

            _logger.LogInformation("Product {Id} deleted.", id);
            return true;
        }

        public async Task<int> CommitAsync()
        {
            return await _unitOfWork.Commit();
        }
    }
}