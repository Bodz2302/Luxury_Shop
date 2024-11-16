using Luxury_Shop.Models;
using System;

public class Product
{
    public int ProductID { get; set; }
    public string ProductName { get; set; }
    public string Description { get; set; }
    public int? CategoryID { get; set; }
    public int? BrandID { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public int? StockQuantity { get; set; }
    public string ImageURL { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public virtual Brand Brand { get; set; }
    public virtual Category Category { get; set; }
}
