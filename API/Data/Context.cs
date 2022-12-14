using Microsoft.EntityFrameworkCore;

namespace API.Data;

public sealed class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options)
    {
    }
}
