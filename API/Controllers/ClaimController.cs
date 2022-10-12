using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public sealed class ClaimController : ControllerBase
{
    public IActionResult Get()
    {
        var query = from c in User.Claims
                    select new { c.Type, c.Value };

        var result = new JsonResult(query);

        return result;
    }
}
