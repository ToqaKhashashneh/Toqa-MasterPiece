using System;
using System.Collections.Generic;

namespace Nestify.Models;

public partial class Property
{
    public int Id { get; set; }

    public string PropertyName { get; set; } = null!;

    public int UserId { get; set; }

    public int Bedrooms { get; set; }

    public int Bathrooms { get; set; }

    public decimal Size { get; set; }

    public decimal Price { get; set; }

    public bool? IsFeatured { get; set; }

    public string? Description { get; set; }

    public int? YearBuilt { get; set; }

    public decimal? LotArea { get; set; }

    public string? LotDimensions { get; set; }

    public string PropertyStatus { get; set; } = null!;

    public string? VideoUrl { get; set; }

    public string? ImageUrl1 { get; set; }

    public string? ImageUrl2 { get; set; }

    public string? ImageUrl3 { get; set; }

    public string? ImageUrl4 { get; set; }

    public string? ImageUrl5 { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public DateTime? PublishDate { get; set; }

    public virtual ICollection<FavoriteProperty> FavoriteProperties { get; set; } = new List<FavoriteProperty>();

    public virtual ICollection<PropertyFeature> PropertyFeatures { get; set; } = new List<PropertyFeature>();

    public virtual ICollection<PropertyInquiry> PropertyInquiries { get; set; } = new List<PropertyInquiry>();

    public virtual User User { get; set; } = null!;
}
