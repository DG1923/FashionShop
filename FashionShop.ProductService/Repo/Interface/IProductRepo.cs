﻿using FashionShop.ProductService.DTOs.ProductDTO;
using FashionShop.ProductService.Models;
using System.Collections.Generic;

namespace FashionShop.ProductService.Repo.Interface
{
    public interface IProductRepo : IGenericRepo<Product>
    {
        Task<IEnumerable<ProductDisplayDTO>> GetProductsByCategory(Guid categoryId);
        Task<ProductDetailsDTO> GetProductDetail(Guid id);
        Task<IEnumerable<ProductCreateDetailDTO>> AddRangeProduct(List<ProductCreateDetailDTO> list);

    }
}
