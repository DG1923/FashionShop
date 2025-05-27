using FashionShop.ProductService.Models;
using System.ComponentModel.DataAnnotations;

public class ProductRating : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }

    public Guid UserId { get; set; }


    [Range(1, 5)]
    public decimal? Rating { get; set; }

    public string? Comment { get; set; }
}