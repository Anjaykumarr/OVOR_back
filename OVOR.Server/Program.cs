using OVOR.Repo.DataTools;
using OVOR.Repo.Repo;
using OVOR.Services.ProjectServices;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// ✅ DATABASE INITIALIZATION
// ======================================================
try
{
    DataAccessor.Initialize(builder.Configuration);
    Console.WriteLine("✅ Database initialized successfully");
}
catch (Exception ex)
{
    Console.WriteLine("❌ Database initialization failed: " + ex.Message);
}

// ======================================================
// ✅ DEPENDENCY INJECTION
// ======================================================
builder.Services.AddScoped<IMgnregaRepo, MgnregaRepo>();
builder.Services.AddScoped<IMgnregaServices, MgnregaServices>();
builder.Services.AddHttpClient<ProjectServices>();

// ======================================================
// ✅ SWAGGER
// ======================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ======================================================
// ✅ CONTROLLERS (with Newtonsoft for DataTables/DataSets)
// ======================================================
builder.Services.AddControllers().AddNewtonsoftJson();

// ======================================================
// ✅ GLOBAL CORS POLICY (Frontend + Local Dev)
// ======================================================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(
                "https://ovor-front.onrender.com", // ✅ Render Frontend
                "http://localhost:5173",           // ✅ Local Dev (Vite)
                "https://localhost:5173"           // ✅ Local Dev HTTPS
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// ======================================================
// ✅ RENDER PORT CONFIGURATION
// ======================================================
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{port}");

Console.WriteLine("===============================================");
Console.WriteLine($"🌐 Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"🚀 Listening on port: {port}");
Console.WriteLine($"🔓 CORS Enabled for: https://ovor-front.onrender.com");
Console.WriteLine("===============================================");

// ======================================================
// ✅ MIDDLEWARE PIPELINE (ORDER MATTERS!)
// ======================================================
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRouting();

// ✅ MUST come AFTER UseRouting and BEFORE Authorization
app.UseCors();

app.UseAuthorization();

// ✅ Swagger (works in all environments)
app.UseSwagger();
app.UseSwaggerUI();

// ======================================================
// ✅ ENDPOINTS
// ======================================================
app.MapControllers();

// ✅ Simple Health Check (for Render auto verification)
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// ======================================================
// ✅ RUN THE APP
// ======================================================
app.Run();
