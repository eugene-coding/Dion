﻿namespace Shared.DTO;

public sealed class User
{
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? Description { get; set; }
}