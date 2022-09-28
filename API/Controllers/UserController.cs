using API.Data;
using API.DTO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class UserController : ControllerBase
{
    private readonly Context _context;

    public UserController(Context context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        var query = _context.Users
            .Select(u => new User
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Description = u.Description
            });

        var result = await query
            .AsNoTracking()
            .ToListAsync();

        return result;
    }
}
