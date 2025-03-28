using Microsoft.EntityFrameworkCore;
using PutzPilotApi.Context;

var builder = WebApplication.CreateBuilder(args);

// 👇 Add config from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// 👇 Register DbContext
builder.Services.AddDbContext<PutzPilotDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IONOS"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ).LogTo(Console.WriteLine, LogLevel.Information)
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// 👇 Add controllers
builder.Services.AddControllers();

// 👇 Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// 👇 Optional: Add CORS for frontend or MAUI App testing
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// 👇 Dev tools: Swagger & detailed errors
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
// 👇 Enable CORS (optional, je nach Bedarf)
app.UseCors("AllowAll");

// 👇 HTTPS Redirect (kann lokal nerven, also ggf. abschalten)
app.UseHttpsRedirection();

// 👇 Routing & controller endpoints
app.UseAuthorization();
app.MapControllers();

app.Run();
