using System;
using System.Collections.Generic;

namespace Nestify.Models;

public partial class PropertyFeature
{
    public int Id { get; set; }

    public int PropertyId { get; set; }

    public string FeatureName { get; set; } = null!;

    public string? Size { get; set; }

    public virtual Property Property { get; set; } = null!;
}
