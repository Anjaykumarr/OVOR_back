using OVOR.Repo.DataTools;
using OVOR.Repo.Repo;
using OVOR.Services.ProjectServices;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

var builder = WebApplication.CreateBuilder(args);

// ✅ Initialize Data Access
DataAccessor.Initialize(builder.Configuration);

// ✅ Register Dependencies
builder.Services.AddScoped<IMgnregaRepo, MgnregaRepo>();
builder.Services.AddScoped<IMgnregaServices, MgnregaServices>();
builder.Services.AddHttpClient<ProjectServices>();

// ✅ Add MVC + Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Add CORS — allow your frontend Render domain
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
            "https://ovor-front.onrender.com",  // ✅ your frontend Render URL
            "http://localhost:5173"             // ✅ local dev (Vite)
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// ✅ Add controllers + Newtonsoft JSON
builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();

// ✅ Bind to Render’s PORT environment variable
var portEnv = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(portEnv) && int.TryParse(portEnv, out var port))
{
    app.Urls.Clear();
    app.Urls.Add($"http://0.0.0.0:{port}");
}

// ✅ Use Middleware (order matters)
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRouting();

// ✅ Enable Swagger in dev only
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ API routes
app.MapControllers();

// ✅ Health check endpoint for Render
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// ✅ Fallback for client-side routing (optional, safe to keep)
app.MapFallbackToFile("index.html");

// ✅ Run
app.Run();
