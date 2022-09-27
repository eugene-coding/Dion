using Microsoft.EntityFrameworkCore;

namespace API.Data;

internal sealed class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options)
    {
    }
}
