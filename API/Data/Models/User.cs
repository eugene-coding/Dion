using System.ComponentModel.DataAnnotations;

namespace API.Data.Models;

public sealed class User
{
    public int Id { get; set; }

    [MaxLength(64)]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(128)]
    public string? LastName { get; set; }

    [MaxLength(256)]
    public string? Description { get; set; }
}
