using System;
using System.Collections.Generic;

namespace Nestify.Models;

public partial class Sublocation
{
    public int Id { get; set; }

    public string SublocationName { get; set; } = null!;

    public int LocationId { get; set; }

    public virtual Location Location { get; set; } = null!;
}
