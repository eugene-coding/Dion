using API.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

const string bearerSchemeName = "Bearer";

services.AddAuthentication(bearerSchemeName)
    .AddJwtBearer(bearerSchemeName, options =>
    {
        options.Authority = "https://localhost:5001";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;

    SeedData.Initialize(serviceProvider);
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
