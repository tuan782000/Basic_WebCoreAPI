using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Product;
using api.Models;

namespace api.Mappers
{
    public static class ProductMappers
    {
        public static ProductDto ToProductDto(this Product productModel) {
            return new ProductDto {
                Id = productModel.Id,
                Name = productModel.Name,
                Price = productModel.Price,
                Thumbnail = productModel.Thumbnail,
                Description = productModel.Description,
                Active = productModel.Active,

                // Bổ dung List Comment
                Comments = productModel.Comments.Select(c => c.ToCommentDto()).ToList()
            };
        }

        public static Product ToProductFromCreateDTO(this CreateProductRequestDto productDto) {
            return new Product {
                Name = productDto.Name,
                Price = productDto.Price,
                Thumbnail = productDto.Thumbnail,
                Description = productDto.Description,
                Active = productDto.Active
            };
        }

    }
}