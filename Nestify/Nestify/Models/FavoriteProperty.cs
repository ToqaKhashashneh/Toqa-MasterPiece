using System;
using System.Collections.Generic;

namespace Nestify.Models;

public partial class FavoriteProperty
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PropertyId { get; set; }

    public virtual Property Property { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
