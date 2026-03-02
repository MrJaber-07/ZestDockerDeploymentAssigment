using Application.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IProductService 
    {
        Task<IEnumerable<ProductResponse>> GetAllAsync();
        Task<ProductResponse?> GetByIdAsync(int id);
        Task<ProductResponse> CreateAsync(ProductRequest request);
        Task<ProductResponse> UpdateAsync(int id, ProductRequest request);
        Task<bool> DeleteAsync(int id);
        Task<int> CommitAsync();

    }
}
