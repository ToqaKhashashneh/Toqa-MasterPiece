using System;
using System.Collections.Generic;

namespace Nestify.Models;

public partial class Location
{
    public int Id { get; set; }

    public string LocationName { get; set; } = null!;

    public virtual ICollection<Sublocation> Sublocations { get; set; } = new List<Sublocation>();
}
