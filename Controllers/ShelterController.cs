using Microsoft.AspNetCore.Mvc;
using Serilog;

[ApiController]
[Route("api/shelter")]
public class ShelterController : ControllerBase
{
    private List<Shelter> _shelterList;
    private IConfiguration _configuration;

    public ShelterController(IConfiguration configuration)
    {
        _shelterList = new List<Shelter>();
        _configuration = configuration;
        List<string[]> data = CSVHelper.ReadCSV(_configuration["Data:Location"]);

        // 1,Refugio 1,Direccion 1,Pincher => [1, Refugio 1, Direccion 1]
        // 2,Refugio 2,Direccion 2
        for (int i = 0; i < data.Count; i++)
        {
            Shelter shelter = new Shelter
            {
                Id = int.Parse(data[i][0]),
                Name = data[i][1],
                Address = data[i][2],
                DogsBreedAllowed = data[i][3]
            };
            _shelterList.Add(shelter);
        }
    }
    // C: Create
    [HttpPost]
    public IActionResult Post([FromBody] Shelter shelterToAdd)
    {
        // validaciones de datos
        DogBreedService dogBreedService = new DogBreedService(_configuration);
        List<DogBreed> dogBreeds = dogBreedService.GetDogBreeds().Result;

        shelterToAdd.DogsBreedAllowed = dogBreeds[0].Attributes.Name;
        _shelterList.Add(shelterToAdd);

        Log.Debug($"Se agrego un nuevo refugio: {shelterToAdd.Id} - {shelterToAdd.Name} - {shelterToAdd.Address}");

        Log.Information($"Se agrego un nuevo refugio: {shelterToAdd.Name}");

        List<string[]> data = new List<string[]>();
        for (int i = 0; i < _shelterList.Count; i++)
        {
            string[] shelterData = new string[]
            {
                _shelterList[i].Id.ToString(),
                _shelterList[i].Name,
                _shelterList[i].Address,
                _shelterList[i].DogsBreedAllowed
            };
            data.Add(shelterData);
        }
        CSVHelper.WriteCSV(_configuration["Data:Location"], data);

        return Ok(_shelterList);
    }

    // R: Read ALL / Retrieve ALL
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_shelterList);
    }

    // R: Read by id / Retrieve by id
    [HttpGet]
    [Route("{id}")]
    public IActionResult Get([FromRoute] int id)
    {
        Shelter foundShelter = _shelterList.Find(item => item.Id == id);

        if (foundShelter == null)
        {
            Log.Error($"No se encontro un refugio con el id: {id}");
            return Ok($"No se encontro un refucion con el id: {id} {id}");
        }
        else
        {
            return Ok(foundShelter);
        }
    }

    // U: Update
    [HttpPut]
    [Route("{id1}/{id2}")]
    public IActionResult Put([FromRoute] int id1, [FromRoute] int id2, [FromBody] Shelter shelterData)
    {
        Shelter shelterToUpdate = _shelterList.Find(s => s.Id == id1);
        if (shelterToUpdate == null)
        {
            return Ok($"No se encontro un refucion con el id: {id1} {id1}");
        }
        else
        {
            shelterToUpdate.Id = shelterData.Id;
            shelterToUpdate.Name = shelterData.Name;
            shelterToUpdate.Address = shelterData.Address;
            CSVHelper.WriteCSV(_configuration["Data:Location"], _shelterList.Select(s => new string[] { s.Id.ToString(), s.Name, s.Address }).ToList());
            return Ok(_shelterList);
        }
    }

    // D: Delete
    [HttpDelete]
    [Route("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        Shelter shelterToRemove = _shelterList.Find(s => s.Id == id);
        if (shelterToRemove == null)
        {
            return Ok("No se borro!");
        }
        else
        {
            _shelterList.Remove(shelterToRemove);
            CSVHelper.WriteCSV(_configuration["Data:Location"], _shelterList.Select(s => new string[] { s.Id.ToString(), s.Name, s.Address }).ToList());
            return Ok(shelterToRemove);
        }
    }
}