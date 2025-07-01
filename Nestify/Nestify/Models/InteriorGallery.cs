using System;
using System.Collections.Generic;

namespace Nestify.Models;

public partial class InteriorGallery
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Style { get; set; }

    public string? ImageUrl { get; set; }
}
