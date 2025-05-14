using FashionShop.ProductService.DTOs.ProductDTO;
using FashionShop.ProductService.Models;
using FashionShop.ProductService.Repo.Interface;
using FashionShop.ProductService.Service.Interface;
using FashionShop.ProductService.SyncDataService.GrpcClient;

namespace FashionShop.ProductService.Service
{
    public class ProductsService:BaseService<Product,ProductCreateDTO,ProductUpdateNormal>, IProductService
    {
        private readonly IProductRepo _productRepo;
        private readonly IProductProtoClient _productProtoClient;
        public ProductsService(IProductRepo productRepo, IProductProtoClient productProtoClient) : base(productRepo)
        {
            _productRepo = productRepo;
            _productProtoClient = productProtoClient;
        }

        public async Task<IEnumerable<ProductCreateDetailDTO>> AddRangeProductService(List<ProductCreateDetailDTO> list)
        {
            if(list == null || list.Count == 0)
            {
                throw new ArgumentNullException(nameof(list), "List cannot be null or empty");
            }
            var productCreateDetails = await _productRepo.AddRangeProduct(list);
            return productCreateDetails;
        }

        public async Task<ProductDetailsDTO> GetProductDetailService(Guid id)
        {
            if(id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be empty");
            }
            var productDetails =await _productRepo.GetProductDetail(id);
            if (productDetails == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            return productDetails;
        }

        public Task<IEnumerable<ProductDisplayDTO>> GetProductsByCategoryService(Guid categoryId)
        {
            if(Guid.Empty == categoryId)
            {
                throw new ArgumentNullException(nameof(categoryId), "Category ID cannot be empty"); 
            }
            return _productRepo.GetProductsByCategory(categoryId);
        }

        public override Product MapCreateToEntity(ProductCreateDTO dto)
        {
            return new Product
            {
                BasePrice = dto.Price,

                Name = dto.Name,
                Description = dto.Description,
                SKU = dto.SKU,
                MainImageUrl = dto.MainImageUrl,


            };
        }

        public override void MapToExistingEntity(Product entity, ProductUpdateNormal dto)
        {
            entity.Name = dto.Name;
            entity.BasePrice = dto.Price;
            entity.Description = dto.Description;
            entity.MainImageUrl = dto.MainImageUrl;
        }
    }
}
