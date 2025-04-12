using System;
using System.Collections.Generic;

namespace Nestify.Models;

public partial class ContactInquiry
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string ServiceType { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string AssignedTo { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? SubmittedAt { get; set; }
}
