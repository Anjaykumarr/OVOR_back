using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using OVOR.Models.ProjectData;
using OVOR.Repo.Repo;

namespace OVOR.Services.ProjectServices
{
    public class ProjectServices
    {
        private readonly HttpClient _httpClient;
        private readonly string _geoapifyKey = "b6080df7569444569478a68af8ee863e";

        public ProjectServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GetAutoDistrict> GetDistrictFromCoordinatesAsync(GetAutoDistrict data)
        {
            // ✅ Geoapify Reverse Geocoding API
            var url = $"https://api.geoapify.com/v1/geocode/reverse?lat={data.Lat}&lon={data.Lon}&apiKey={_geoapifyKey}";

            // Add a User-Agent header (good practice)
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("OVORApp/1.0 (contact@example.com)");

            // Send request
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // Read response
            var jsonString = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(jsonString);

            // Parse response according to Geoapify structure
            var props = json["features"]?[0]?["properties"];

            data.state = props?["state"]?.ToString();
            data.district =
                props?["district"]?.ToString() ??
                props?["county"]?.ToString() ??
                props?["region"]?.ToString() ??
                props?["city"]?.ToString() ??
                props?["suburb"]?.ToString() ??
                "Unknown";

            return data;
        }
    }
}
