using API.Data;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddDbContext<Context>(options =>
{
    const string connectionStringName = "Database";

    var connectionString = builder.Configuration.GetConnectionString(connectionStringName);
    var serverVersion = ServerVersion.AutoDetect(connectionString);

    options.UseMySql(connectionString, serverVersion, options =>
    {
        options.EnableRetryOnFailure();
    });
});

services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
