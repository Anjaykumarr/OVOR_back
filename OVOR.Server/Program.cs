using System.Diagnostics;
using OVOR.Repo.DataTools;
using OVOR.Repo.Repo;
using OVOR.Services.ProjectServices;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;


var builder = WebApplication.CreateBuilder(args);

// Initialize Data Access
DataAccessor.Initialize(builder.Configuration);

// Register Dependencies
builder.Services.AddScoped<IMgnregaRepo, MgnregaRepo>();
builder.Services.AddScoped<IMgnregaServices, MgnregaServices>();
//builder.Services.AddScoped<ProjectServices>();
builder.Services.AddHttpClient<ProjectServices>();


// Add MVC + Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  Add CORS policy (cross-origin-resource-sharing) blocking react api requests, solution below...
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy
                .WithOrigins("https://localhost:57483")  // your React app URL
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // if you use cookies or auth headers
        });
});

// This thing does is it converts everyting into json before sending through api (new +fast)
//builder.Services.AddControllers();

// Now waht this does is it allows datatable, dataset, datarow through api (old + slow + more features)
builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();

// ===== STATIC & CORE MIDDLEWARE =====
app.UseDefaultFiles();
app.UseStaticFiles();

// Enable CORS before MapControllers
app.UseCors("AllowReactApp");

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRouting();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Handle all API routes first
app.MapControllers();

// Only handle NON-API routes through Vite/React
if (app.Environment.IsDevelopment())
{
    app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"), spaApp =>
    {
        spaApp.UseSpa(spa =>
        {
            spa.Options.SourcePath = "../ovor.client";

            const string viteServerUrl = "http://localhost:57483";
            var viteUri = new Uri(viteServerUrl);
            var clientPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\ovor.client"));
            var nodePath = Path.Combine(clientPath, "node_modules");

            bool isViteRunning = false;
            try
            {
                using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(1) };
                var res = httpClient.GetAsync(viteUri).Result;
                isViteRunning = res.IsSuccessStatusCode;
            }
            catch { isViteRunning = false; }

            if (!isViteRunning)
            {
                Console.WriteLine("Vite is not running — starting automatically...");

                string FindNodeTool(string toolName)
                {
                    var localTool = Path.Combine(nodePath, ".bin", $"{toolName}.cmd");
                    if (File.Exists(localTool)) return localTool;
                    return toolName;
                }

                var npmPath = FindNodeTool("npm");
                Console.WriteLine($"[DEBUG] Using npm from: {npmPath}");

                var psi = new ProcessStartInfo
                {
                    FileName = npmPath,
                    Arguments = "run dev",
                    WorkingDirectory = clientPath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var viteProcess = Process.Start(psi);
                viteProcess.EnableRaisingEvents = true;

                AppDomain.CurrentDomain.ProcessExit += (s, e) =>
                {
                    try
                    {
                        if (!viteProcess.HasExited)
                        {
                            viteProcess.Kill(true);
                            Console.WriteLine("Cleaned up Vite process on exit.");
                        }
                    }
                    catch { }
                };
            }

            spa.UseProxyToSpaDevelopmentServer(viteServerUrl);
        });
    });
}
else
{
    // ===== PRODUCTION (serves built files) =====
    app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"), spaApp =>
    {
        spaApp.UseSpa(spa =>
        {
            spa.Options.SourcePath = "../ovor.client/dist";
        });
    });
}

// React Router fallback
app.MapFallbackToFile("index.html");

app.Run();
