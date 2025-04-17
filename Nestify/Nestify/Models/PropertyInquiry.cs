using System;
using System.Collections.Generic;

namespace Nestify.Models;

public partial class PropertyInquiry
{
    public int Id { get; set; }

    public int PropertyId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime? SubmittedAt { get; set; }

    public int? UserId { get; set; }

    public string? Phone { get; set; }

    public DateTime? PreferredDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Property Property { get; set; } = null!;

    public virtual User? User { get; set; }
}
