using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Product;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(QueryObject query);
        Task<Product?>GetByIdAsync(int id); // FirstOrDefault can be null - FirstOrDefault tìm không thấy có thể null cho nên để "?"
        Task<Product>CreateAsync(Product productModel);
        Task<Product?>UpdateAsync(int id, UpdateProductRequestDto productDto);
        Task<Product?> DeleteAsync(int id);
        Task<Product?> SoftDeleteAsync(int id);

        // Kiểm tra product có tồn tại hay chưa
        Task<bool> ProductExists(int id);
    }
}