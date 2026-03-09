using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SuaApi.Configurations;
using WorkOutAPI.Data;
using WorkOutAPI.Repositories;
using WorkOutAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var key = builder.Configuration["Security:JwtKey"];
if (string.IsNullOrEmpty(key)){
    throw new Exception("Security:JwtKey environment variable not configured");
}

var encodedKey = Encoding.ASCII.GetBytes(key);

builder.Services.AddAuthentication(i => {
    i.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    i.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(i =>
{
    i.RequireHttpsMetadata = false;
    i.SaveToken = true;
    i.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(encodedKey),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddTransient<IUnityOfWork, UnityOfWork>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ICheckInRepository, CheckInRepository>();
builder.Services.AddTransient<IExerciseRepository, ExerciseRepository>();
builder.Services.AddSingleton<TokenService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var app = builder.Build();

// if(app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    await DbSeeder.Seed(db);
}

app.Run();