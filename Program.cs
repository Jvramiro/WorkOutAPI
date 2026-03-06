using Microsoft.EntityFrameworkCore;
using WorkOutAPI.Data;
using WorkOutAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddTransient<IUnityOfWork, UnityOfWork>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ICheckInRepository, CheckInRepository>();

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
app.UseAuthorization();
app.MapControllers();

app.Run();