using System.Diagnostics;
using OVOR.Repo.DataTools;
using OVOR.Repo.Repo;
using OVOR.Services.ProjectServices;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

var builder = WebApplication.CreateBuilder(args);

// ✅ Initialize Database / Data Access
DataAccessor.Initialize(builder.Configuration);

// ✅ Register Dependencies
builder.Services.AddScoped<IMgnregaRepo, MgnregaRepo>();
builder.Services.AddScoped<IMgnregaServices, MgnregaServices>();
builder.Services.AddHttpClient<ProjectServices>();

// ✅ Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Add Controllers + Newtonsoft JSON
builder.Services.AddControllers().AddNewtonsoftJson();

// ✅ Configure CORS for Render + local dev
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
            "https://ovor-front.onrender.com",  // ✅ Your Render frontend
            "http://localhost:5173"             // ✅ For local testing (optional)
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

// ✅ Render dynamic port binding
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{port}");

// ✅ Middleware (order matters)
app.UseDefaultFiles();
app.UseStaticFiles();

// ✅ CORS must come before routing
app.UseCors("AllowReactApp");

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// ✅ Swagger for API testing (available even in production if needed)
app.UseSwagger();
app.UseSwaggerUI();

// ✅ Map API controllers
app.MapControllers();

// ✅ Health check endpoint for Render
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// ✅ Run the app
app.Run();
