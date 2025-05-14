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
            //this constructor is not working because ef core only use the default contructor 
            //everytime ef core modify with the databse this constructor is not called 
           //that reseason I didn't see setter and getter take data from inventory 
            _productProtoClient = productProtoClient;
        }

        [Required]
        public string Size { get; set; }
        public string? Description { get; set; }
        //I use it because I want to use grpc take data from inventory service
        //I will configure to map with Quantity in Database throught fluent api 
        //It still keep code base and changeless code
        public int Quantity { get; set; }

        public string? ImageUrlVariation { get; set; }
        //configure the relationship with Product   
        public ProductColor? ProductColor { get; set; }
        [Required]
        public Guid ProductColorId { get; set; }


    }
}
