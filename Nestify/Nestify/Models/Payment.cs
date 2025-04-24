using System;
using System.Collections.Generic;

namespace Nestify.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? PaymentDate { get; set; }

    public int? PackageId { get; set; }

    public bool IsBusinessPlan { get; set; }

    public virtual Package? Package { get; set; }

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    public virtual User User { get; set; } = null!;
}
