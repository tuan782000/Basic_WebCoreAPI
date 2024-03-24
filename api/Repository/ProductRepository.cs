using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Product;
using api.Helpers;
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

        public async Task<List<Product>> GetAllAsync(QueryObject query)
        {
            // return await _context.Products.ToListAsync();

            // bổ sung thêm việc lấy ra danh sách comment
            // return await _context.Products.Include(c => c.Comments).ToListAsync();
        
            /*
            .Include(c => c.Comments): Phương thức Include được sử dụng để chỉ định rằng khi truy vấn dữ liệu từ bảng Products, cần bao gồm dữ liệu từ bảng Comments liên quan đến mỗi sản phẩm. Điều này giúp tải dữ liệu liên quan (dữ liệu của các comments) cùng với dữ liệu của sản phẩm một cách hiệu quả mà không cần phải thực hiện các truy vấn tách biệt sau đó.

            .ToListAsync(): Phương thức này thực hiện truy vấn dữ liệu từ cơ sở dữ liệu và chuyển đổi kết quả trả về thành một danh sách (List) các đối tượng. Trong trường hợp này, nó chuyển đổi kết quả trả về từ tất cả các sản phẩm và comments liên quan thành một danh sách danh sách List<Product>.
            */ 

            var products =  _context.Products.Include(c => c.Comments).AsQueryable();

            // string.IsNullOrWhiteSpace() là một phương thức trong C# dùng để kiểm tra xem một chuỗi có chứa giá trị nào đó không, mà không phải là null hoặc chỉ chứa khoảng trắng. Nó trả về true nếu chuỗi là null, rỗng hoặc chỉ chứa khoảng trắng; ngược lại, trả về false.
            // Nếu có giá trị IsNullOrWhiteSpace trả về false nghịch đảo của false là true nó sẽ cho vào
            if(!string.IsNullOrWhiteSpace(query.ProductName)) {
                products = products.Where(s => s.Name.Contains(query.ProductName));
            }

            // Nếu có nhiều thông tin search thì viết thêm if tương tự

            /*
            if (!string.IsNullOrWhiteSpace(query.SortBy)): Điều kiện này kiểm tra xem trường SortBy đã được định nghĩa trong yêu cầu và không phải là null hoặc rỗng. Nếu điều kiện này đúng (tức là SortBy không null và không rỗng), nó sẽ tiếp tục kiểm tra và thực hiện việc sắp xếp.

            if (query.SortBy.Equals("ProductName", StringComparison.OrdinalIgnoreCase)): Điều kiện này kiểm tra xem trường SortBy có giá trị là "ProductName" không. Nếu điều kiện này đúng, nó sẽ tiếp tục thực hiện việc sắp xếp theo tên sản phẩm (Name).
            */

            // đoạn này đang sai logic
            // if(!string.IsNullOrWhiteSpace(query.SortBy)) {
            //     if(query.SortBy.Equals("ProductName", StringComparison.OrdinalIgnoreCase)) {
            //         products = query.IsDecsending ? products.OrderByDescending(s => s.Name) : products.OrderBy(s => s.Name);
            //     }
            // }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                switch (query.SortBy.ToLowerInvariant())
                {
                    case "name":
                        products = query.IsDecsending ? products.OrderByDescending(s => s.Name) 
                        : 
                        products.OrderBy(s => s.Name);
                        break;
                    // Add more cases for additional sorting options if needed
                }
            }

            return await products.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            // return await _context.Products.FindAsync(id);

            // bổ sung thêm việc lấy ra danh sách comment
             return await _context.Products.Include(c => c.Comments).FirstOrDefaultAsync(i => i.Id == id);
            /*
            .Include(c => c.Comments): Phương thức Include được sử dụng để chỉ định rằng khi truy vấn dữ liệu từ bảng Products, cần bao gồm dữ liệu từ bảng Comments liên quan đến mỗi sản phẩm. Điều này giúp tải dữ liệu liên quan (dữ liệu của các comments) cùng với dữ liệu của sản phẩm một cách hiệu quả mà không cần phải thực hiện các truy vấn tách biệt sau đó.

            .FirstOrDefaultAsync(i => i.Id == id): Phương thức này thực hiện truy vấn dữ liệu từ cơ sở dữ liệu để lấy ra sản phẩm đầu tiên trong tập hợp (Products) mà Id của sản phẩm đó bằng với Id được chỉ định (id). Nó được gọi là FirstOrDefaultAsync vì nó sẽ trả về sản phẩm đầu tiên tìm thấy hoặc giá trị mặc định (null) nếu không tìm thấy bất kỳ sản phẩm nào thỏa mãn điều kiện. Phương thức này được thực hiện bất đồng bộ và trả về một Task.
            */
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

        public Task<bool> ProductExists(int id)
        {
            return _context.Products.AnyAsync(s => s.Id == id);
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