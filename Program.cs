using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WorkOutAPI.Data;
using WorkOutAPI.Repositories;
using WorkOutAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var encodedKey = Encoding.ASCII.GetBytes(TokenService.GetKey());

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var app = builder.Build();

if(app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();

app.Run();