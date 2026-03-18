using Newtonsoft.Json;

public class DogBreedService
{
    private HttpClient _httpClient;

    public DogBreedService(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(configuration["ExternalServices:BreedesApi:BaseUrl"]);
    }

    public async Task<List<DogBreed>> GetDogBreeds()
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "api/v2/breeds");
        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            string responseContent = response.Content.ReadAsStringAsync().Result;
            Response dogBreeds = JsonConvert.DeserializeObject<Response>(responseContent);
            return dogBreeds.Data;
        }
        else
        {
            throw new Exception("Error al obtener las razas de perros");
        }
    }
}