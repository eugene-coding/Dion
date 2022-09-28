using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

namespace API.Data.Models;

[Index(nameof(Username), IsUnique = true)]
public sealed class UserCredential
{
    public int Id { get; set; }
    public int UserId { get; set; }

    [StringLength(32, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [StringLength(32, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    public User User { get; set; } = default!;
}
