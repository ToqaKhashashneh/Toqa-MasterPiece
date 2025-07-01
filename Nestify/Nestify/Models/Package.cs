using System;
using System.Collections.Generic;

namespace Nestify.Models;

public partial class Package
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int PostLimit { get; set; }

    public int DurationInDays { get; set; }

    public int? FeaturedPostLimit { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
