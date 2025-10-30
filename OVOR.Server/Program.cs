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

// ✅ Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ CORS setup — allow your frontend (Render + local dev)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
            "https://ovor-front.onrender.com",  // ✅ your actual frontend Render URL
            "http://localhost:5173"             // ✅ allow local dev too
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();

// ✅ Make it listen to Render’s PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{port}");

// ✅ Order matters — this order is correct
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// ✅ Swagger (only in dev)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Map your API controllers
app.MapControllers();

// ✅ Health check for Render
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
