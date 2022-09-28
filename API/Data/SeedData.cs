using API.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace API.Data;

internal static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new Context(serviceProvider.GetRequiredService<DbContextOptions<Context>>());

        if (!context.Users.Any())
        {
            context.AddRange(
                new User
                {
                    FirstName = "Андрей",
                    LastName = "Игнатов",
                    Description = "Тренер по боксу",
                    UserCredential = new UserCredential
                    {
                        Username = "andrey",
                        Password = "qwe123"
                    }
                },
                new User
                {
                    FirstName = "Тимур",
                    LastName = "Лихачев",
                    Description = "Учитель географии",
                    UserCredential = new UserCredential
                    {
                        Username = "timur",
                        Password = "qwe123"
                    }
                });
        }

        context.SaveChanges();
    }
}
