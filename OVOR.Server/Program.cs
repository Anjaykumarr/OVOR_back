using System.Diagnostics;
using OVOR.Repo.DataTools;
using OVOR.Repo.Repo;
using OVOR.Services.ProjectServices;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

var builder = WebApplication.CreateBuilder(args);

// ✅ Initialize Data Access (uses your connection string from Render)
DataAccessor.Initialize(builder.Configuration);

// ✅ Register Dependencies
builder.Services.AddScoped<IMgnregaRepo, MgnregaRepo>();
builder.Services.AddScoped<IMgnregaServices, MgnregaServices>();
builder.Services.AddHttpClient<ProjectServices>();

// ✅ Add Swagger (always available)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Add Controllers
builder.Services.AddControllers().AddNewtonsoftJson();

// ✅ Add CORS Policy (works in any environment)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
            "https://ovor-front.onrender.com",  // ✅ Your frontend
            "http://localhost:5173"             // ✅ Local dev (optional)
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

// ✅ Bind to Render’s dynamic port (important)
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{port}");

Console.WriteLine($"✅ Running in environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"✅ Listening on port: {port}");

// ✅ CORS must be applied globally (before routing)
app.UseCors("AllowReactApp");

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// ✅ Enable Swagger (always)
app.UseSwagger();
app.UseSwaggerUI();

// ✅ Map API Controllers
app.MapControllers();

// ✅ Health check for Render
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
