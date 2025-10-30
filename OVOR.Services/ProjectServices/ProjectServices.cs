using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Mysqlx.Resultset;
using Newtonsoft.Json.Linq;
using OVOR.Models.ProjectData;
using OVOR.Repo.Repo;

namespace OVOR.Services.ProjectServices
{
    public class ProjectServices
    {
        private readonly HttpClient _httpClient;
        private ProjectData _token;
        private DateTime _tokenExpiry;

        public ProjectServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            // If token still valid, reuse it
            if (_token != null && DateTime.UtcNow < _tokenExpiry)
                return _token.access_token;

            var request = new HttpRequestMessage(HttpMethod.Post, "https://outpost.mapmyindia.com/api/security/oauth/token");
            var keyValues = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", "96dHZVzsAuusxlOHioObQVYsBOXiw2AoxW1pqFNJFl6kfiHdbKrD8abqgXo-FEDE4MgFoR7GEWiHhzPJ2mgmLHqx5p_1AWgE" },
                { "client_secret", "lrFxI-iSEg8qDX2ZGHEOyZYf6dnY4YgIdgsylTHmnICPkMZNUQJx9auq27fBinDyD_pyYXUtVd4YQ4NXLHuySj-AjAGydIX7zVdkip6YL-8=" }
            };
            request.Content = new FormUrlEncodedContent(keyValues);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            _token = JsonSerializer.Deserialize<ProjectData>(json);

            _tokenExpiry = DateTime.UtcNow.AddSeconds(_token.expires_in - 60); // refresh 1 min early

            return _token.access_token;
        }

        public async Task<GetAutoDistrict> GetDistrictFromCoordinatesAsync(GetAutoDistrict data)
        {
            var token = await GetAccessTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //var response = await _httpClient.GetAsync($"https://atlas.mapmyindia.com/api/places/geocode?lat={lat}&lng={lon}");
            var response = await _httpClient.GetAsync($"https://apis.mapmyindia.com/advancedmaps/v1/{_token.access_token}/rev_geocode?lat={data.Lat}&lng={data.Lon}");
            response.EnsureSuccessStatusCode();

            var finaldata = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(finaldata);

            data.district = json["results"]?[0]?["district"]?.ToString();
            data.state = json["results"]?[0]?["state"]?.ToString();

            return data;
        }
    }
}
