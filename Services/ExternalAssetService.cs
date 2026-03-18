using Newtonsoft.Json;
using Serilog;
using System.Text.Json;

/// <summary>
/// Service to handle external API integration for citizen assets.
/// Retrieves objects from https://api.restful-api.dev/objects
/// </summary>
public class ExternalAssetService
{
    private HttpClient _httpClient;
    private IConfiguration _configuration;

    public ExternalAssetService(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _configuration = configuration;
    }

    /// <summary>
    /// Retrieves a random asset from the external API for a citizen.
    /// Maintains naming for consistency with existing structure.
    /// </summary>
    public async Task<string> GetRandomAssetAsync()
    {
        try
        {
            var apiUrl = _configuration["ExternalApi:ObjectsUrl"] ?? "https://api.restful-api.dev/objects";
            
            Log.Information("Calling external API for assets: {ApiUrl}", apiUrl);
            
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(responseContent))
                {
                    var root = doc.RootElement;
                    if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                    {
                        // Select a random object
                        var randomIndex = new Random().Next(root.GetArrayLength());
                        var randomAsset = root[randomIndex];
                        
                        // Try to get the "name" property
                        if (randomAsset.TryGetProperty("name", out var nameElement))
                        {
                            var assetName = nameElement.GetString();
                            Log.Information("Asset retrieved from API: {AssetName}", assetName);
                            return assetName ?? "Unknown Asset";
                        }
                    }
                }

                Log.Warning("No valid assets found in API response");
                return "Unknown Asset";
            }
            else
            {
                Log.Error("External API returned status code: {StatusCode}", response.StatusCode);
                return "Unknown Asset";
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error calling external API for assets");
            return "Unknown Asset";
        }
    }
}