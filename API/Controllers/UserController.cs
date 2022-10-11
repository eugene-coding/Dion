using API.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Shared.DTO;

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

    [Authorize]
    [HttpGet]
    public IActionResult Get()
    {
        var query = from c in User.Claims
                    select new { c.Type, c.Value };

        var result = new JsonResult(query);

        return result;
    }

    [HttpGet("user")]
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
