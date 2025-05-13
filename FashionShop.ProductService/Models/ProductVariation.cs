using FashionShop.ProductService.Protos;
using FashionShop.ProductService.SyncDataService.GrpcClient;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FashionShop.ProductService.Models
{
    public class ProductVariation:BaseEntity
    {
        private readonly IProductProtoClient _productProtoClient;

        public ProductVariation()
        {
            
        }
        public ProductVariation(IProductProtoClient productProtoClient)
        {
            _productProtoClient  = productProtoClient;
        }

        [Required]
        public string Size { get; set; }
        public string? Description { get; set; }
        //I use it because I want to use grpc take data from inventory service
        //I will configure to map with Quantity in Database throught fluent api 
        //It still keep code base and changeless code
        private int _quantity;
        public int Quantity {
            get { 
                if(_productProtoClient != null)
                {
                    Console.WriteLine("---> Get Quantity from grpc");
                    _quantity = _productProtoClient.GetQuantity(Id).Quantity;    
                }
                return _quantity;
            }
            set {
                if(_productProtoClient != null)
                {
                    Console.WriteLine("---> Update Quantity from grpc");
                    UpdateQuantityAsync(Id, value).Wait();
                
                }   
            } }

        private async Task UpdateQuantityAsync(Guid id, int value)
        {
            if (_productProtoClient != null)
            {
                var response =await _productProtoClient.UpdateQuantity(id, value);
                if (response.Success)
                {
                    Console.WriteLine("---> Update Quantity from grpc success");
                }
                else
                {
                    Console.WriteLine("---> Update Quantity from grpc failed");
                }
            }
        }

        public string? ImageUrlVariation { get; set; }
        //configure the relationship with Product   
        public ProductColor? ProductColor { get; set; }
        [Required]
        public Guid ProductColorId { get; set; }


    }
}
