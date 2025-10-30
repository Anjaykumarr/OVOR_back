using OVOR.Repo.DataTools;
using OVOR.Repo.Repo;
using OVOR.Services.ProjectServices;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

var builder = WebApplication.CreateBuilder(args);

// ✅ Initialize DB (uses your connection string from environment)
DataAccessor.Initialize(builder.Configuration);

// ✅ Register dependencies
builder.Services.AddScoped<IMgnregaRepo, MgnregaRepo>();
builder.Services.AddScoped<IMgnregaServices, MgnregaServices>();
builder.Services.AddHttpClient<ProjectServices>();

// ✅ Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Add Controllers
builder.Services.AddControllers().AddNewtonsoftJson();

// ✅ Global CORS Policy (no named policy, always applied)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(
                "https://ovor-front.onrender.com", // ✅ your frontend Render URL
                "http://localhost:5173"            // ✅ for local dev
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // optional if using cookies or auth
    });
});

var app = builder.Build();

// ✅ Bind to Render dynamic port
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{port}");

Console.WriteLine($"✅ Running in environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"✅ Listening on port: {port}");
Console.WriteLine($"✅ CORS: Enabled for https://ovor-front.onrender.com");

// ✅ Apply CORS globally (very important!)
app.UseCors();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
