using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class CitizensController : ControllerBase
{
    private List<Citizen> _citizensList;
    private IConfiguration _configuration;
    private ExternalAssetService _assetService;
    private const string CSV_HEADER = "FirstName,LastName,CI,BloodGroup,PersonalAsset";

    private readonly string[] _bloodGroups = { "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-" };

    public CitizensController(IConfiguration configuration)
    {
        _citizensList = new List<Citizen>();
        _configuration = configuration;
        _assetService = new ExternalAssetService(_configuration);
        
        string csvPath = _configuration["Data:CitizensFile"] ?? "CitizensDatastore.csv";
        LoadCitizensFromCSV(csvPath);
    }

    /// <summary>
    /// Load citizens from CSV file.
    /// </summary>
    private void LoadCitizensFromCSV(string csvPath)
    {
        try
        {
            if (!System.IO.File.Exists(csvPath))
            {
                Log.Information("Citizens CSV file not found: {Path}", csvPath);
                return;
            }

            var lines = System.IO.File.ReadAllLines(csvPath);
            _citizensList.Clear();

            // Skip header if present
            int startIndex = (lines.Length > 0 && lines[0] == CSV_HEADER) ? 1 : 0;
            
            for (int i = startIndex; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                if (parts.Length >= 5)
                {
                    _citizensList.Add(new Citizen
                    {
                        FirstName = parts[0].Trim(),
                        LastName = parts[1].Trim(),
                        CI = int.Parse(parts[2].Trim()),
                        BloodGroup = parts[3].Trim(),
                        PersonalAsset = parts[4].Trim()
                    });
                }
            }

            Log.Information("Loaded {Count} citizens from CSV", _citizensList.Count);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error reading citizens file");
        }
    }

    /// <summary>
    /// Save citizens to CSV file.
    /// </summary>
    private void SaveCitizensToCSV(string csvPath)
    {
        try
        {
            var lines = new List<string> { CSV_HEADER };
            foreach (var citizen in _citizensList)
            {
                var line = $"{citizen.FirstName},{citizen.LastName},{citizen.CI},{citizen.BloodGroup},{citizen.PersonalAsset}";
                lines.Add(line);
            }

            System.IO.File.WriteAllLines(csvPath, lines);
            Log.Information("Saved {Count} citizens to CSV", _citizensList.Count);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error writing citizens file");
        }
    }

    /// <summary>
    /// Get all citizens.
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        Log.Information("Retrieved all citizens - Count: {Count}", _citizensList.Count);
        return Ok(_citizensList);
    }

    /// <summary>
    /// Get citizen by CI (Citizen Identification).
    /// </summary>
    [HttpGet]
    [Route("{ci}")]
    public IActionResult Get([FromRoute] int ci)
    {
        Citizen foundCitizen = _citizensList.Find(c => c.CI == ci);

        if (foundCitizen == null)
        {
            Log.Warning("Citizen not found: {CI}", ci);
            return NotFound(new { message = "Citizen not found" });
        }

        Log.Information("Retrieved citizen: {CI}", ci);
        return Ok(foundCitizen);
    }

    /// <summary>
    /// Create a new citizen with random blood group and asset.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCitizenRequest request)
    {
        try
        {
            // Validate CI uniqueness
            if (_citizensList.Any(c => c.CI == request.CI))
            {
                Log.Warning("Attempt to create citizen with duplicate CI: {CI}", request.CI);
                return BadRequest(new { message = "Citizen with this CI already exists" });
            }

            // Assign random blood group
            var randomBloodGroup = _bloodGroups[new Random().Next(_bloodGroups.Length)];

            // Get random asset from external API
            var personalAsset = await _assetService.GetRandomAssetAsync();

            var newCitizen = new Citizen
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                CI = request.CI,
                BloodGroup = randomBloodGroup,
                PersonalAsset = personalAsset
            };

            _citizensList.Add(newCitizen);
            
            string csvPath = _configuration["Data:CitizensFile"] ?? "CitizensDatastore.csv";
            SaveCitizensToCSV(csvPath);

            Log.Information("Citizen created: {FirstName} {LastName} ({CI})", request.FirstName, request.LastName, request.CI);

            return CreatedAtAction(nameof(Get), new { ci = newCitizen.CI }, newCitizen);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating citizen");
            return StatusCode(500, new { message = "Error creating citizen" });
        }
    }

    /// <summary>
    /// Update citizen (only FirstName and LastName can be updated).
    /// </summary>
    [HttpPut]
    [Route("{ci}")]
    public IActionResult Put([FromRoute] int ci, [FromBody] UpdateCitizenRequest request)
    {
        try
        {
            Citizen citizenToUpdate = _citizensList.Find(c => c.CI == ci);
            
            if (citizenToUpdate == null)
            {
                Log.Warning("Attempt to update non-existent citizen: {CI}", ci);
                return NotFound(new { message = "Citizen not found" });
            }

            // Only update FirstName and LastName (business logic)
            citizenToUpdate.FirstName = request.FirstName;
            citizenToUpdate.LastName = request.LastName;
            // BloodGroup and PersonalAsset are immutable

            string csvPath = _configuration["Data:CitizensFile"] ?? "CitizensDatastore.csv";
            SaveCitizensToCSV(csvPath);

            Log.Information("Citizen updated: {CI}", ci);
            return Ok(citizenToUpdate);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating citizen");
            return StatusCode(500, new { message = "Error updating citizen" });
        }
    }

    /// <summary>
    /// Delete a citizen by CI.
    /// </summary>
    [HttpDelete]
    [Route("{ci}")]
    public IActionResult Delete([FromRoute] int ci)
    {
        try
        {
            Citizen citizenToRemove = _citizensList.Find(c => c.CI == ci);
            
            if (citizenToRemove == null)
            {
                Log.Warning("Attempt to delete non-existent citizen: {CI}", ci);
                return NotFound(new { message = "Citizen not found" });
            }

            _citizensList.Remove(citizenToRemove);

            string csvPath = _configuration["Data:CitizensFile"] ?? "CitizensDatastore.csv";
            SaveCitizensToCSV(csvPath);

            Log.Information("Citizen deleted: {CI}", ci);
            return NoContent();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting citizen");
            return StatusCode(500, new { message = "Error deleting citizen" });
        }
    }
}

/// <summary>
/// Request model for creating a citizen.
/// </summary>
public class CreateCitizenRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int CI { get; set; }
}

/// <summary>
/// Request model for updating a citizen.
/// </summary>
public class UpdateCitizenRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}