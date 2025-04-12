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

    public virtual Property Property { get; set; } = null!;
}
