using System;
using System.Collections.Generic;

namespace Nestify.Models;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? ProfileImageUrl { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string Role { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public virtual ICollection<FavoriteProperty> FavoriteProperties { get; set; } = new List<FavoriteProperty>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    public virtual ICollection<PropertyInquiry> PropertyInquiries { get; set; } = new List<PropertyInquiry>();
}
