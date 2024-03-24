using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Product;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class ProductRepository : IProductRepository
    {
        // lấy dữ liệu ra
        private readonly ApplicationDBContext _context;
        // viết hàm lấy dữ liệu trong db
        public ProductRepository(ApplicationDBContext context) {
            _context = context;
        }

        public async Task<Product> CreateAsync(Product productModel)
        {
            await _context.Products.AddAsync(productModel);
            await _context.SaveChangesAsync();
            return productModel;
        }

        public async Task<Product?> DeleteAsync(int id)
        {
            var productModel = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if(productModel == null) {
                return null;
            }
            _context.Products.Remove(productModel);
            await _context.SaveChangesAsync();
            return productModel;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product?> SoftDeleteAsync(int id)
        {
            var productModel = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if(productModel == null) {
                return null;
            }
            productModel.Active = false;
            await _context.SaveChangesAsync();

            return productModel;
        }

        public async Task<Product?> UpdateAsync(int id, UpdateProductRequestDto productDto)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if(existingProduct == null) {
                return null;
            }
            existingProduct.Name =productDto.Name;
            existingProduct.Price = productDto.Price;
            existingProduct.Thumbnail = productDto.Thumbnail;
            existingProduct.Description = productDto.Description;

            await _context.SaveChangesAsync();
            
            return existingProduct;
         }
    }
}