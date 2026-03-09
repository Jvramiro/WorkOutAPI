using System.Text.Json;
using System.Text.Json.Serialization;
using WorkOutAPI.Enums;
using WorkOutAPI.Models;
using WorkOutAPI.Services;

namespace WorkOutAPI.Data
{
    public static class DbSeeder
    {
        public static async Task Seed(AppDbContext context)
        {
            await SeedAdmin(context);
            await SeedExercises(context);
        }

        private static async Task SeedAdmin(AppDbContext context)
        {
            if (context.Users.Any(i => i.Email == "admin@mail.com"))
                return;

            var admin = new User
            {
                Username = "admin",
                Email = "admin@mail.com",
                PasswordHash = PasswordHashService.HashPassword("12345678"),
                Role = Role.Admin
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }

        private static async Task SeedExercises(AppDbContext context)
        {
            if (context.Exercises.Any())
                return;

            var path = Path.Combine(AppContext.BaseDirectory, "base_exercises.json");

            if (!File.Exists(path))
                return;

            var json = await File.ReadAllTextAsync(path);

            var exercises = JsonSerializer.Deserialize<List<Exercise>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                }
            );

            if (exercises == null)
                return;

            context.Exercises.AddRange(exercises);
            await context.SaveChangesAsync();
        }
    }
}