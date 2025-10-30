using System.Diagnostics;
using OVOR.Repo.DataTools;
using OVOR.Repo.Repo;
using OVOR.Services.ProjectServices;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

var builder = WebApplication.CreateBuilder(args);

// ✅ Initialize Data Access
DataAccessor.Initialize(builder.Configuration);

// ✅ Register Services
builder.Services.AddScoped<IMgnregaRepo, MgnregaRepo>();
builder.Services.AddScoped<IMgnregaServices, MgnregaServices>();
builder.Services.AddHttpClient<ProjectServices>();

// ✅ Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Add Controllers + JSON
builder.Services.AddControllers().AddNewtonsoftJson();

// ✅ Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
            "https://ovor-front.onrender.com",  // ✅ exact frontend URL (Render)
            "http://localhost:5173"             // ✅ local Vite dev
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

// ✅ Bind to Render’s dynamic port
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{port}");

Console.WriteLine($"✅ App starting on port {port}");
Console.WriteLine("✅ CORS Policy: AllowReactApp registered for https://ovor-front.onrender.com");

// ✅ Force CORS for *all* requests — even before static files
app.UseCors("AllowReactApp");

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// ✅ Swagger (enabled for debug)
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// ✅ Health check endpoint for Render
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
