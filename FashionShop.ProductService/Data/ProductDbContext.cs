﻿using FashionShop.ProductService.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace FashionShop.ProductService.Data
{
    public class ProductDbContext: DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductVariation> ProductVariations { get; set; }
        public DbSet<Discount> Discounts { get; set; }

        public ProductDbContext( DbContextOptions<ProductDbContext> options):base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Discount>()
                .HasMany(p => p.Products)
                .WithOne(p => p.Discount)
                .HasForeignKey(p => p.DiscountId);

            modelBuilder.Entity<ProductCategory>()
                .HasMany(p => p.Products)
                .WithOne(p => p.ProductCategory)
                .HasForeignKey(p => p.ProductCategoryId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Discount)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.DiscountId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductCategory)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.ProductCategoryId);

            modelBuilder.Entity<ProductVariation>()
                .HasOne(p => p.Product)
                .WithMany(p => p.Variations)
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<Discount>()
                .Property(p => p.DiscountPercent)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProductVariation>()
                .Property(p => p.AdditionalPrice)
                .HasColumnType("decimal(18,2)");

           
        }
  
    }
}
