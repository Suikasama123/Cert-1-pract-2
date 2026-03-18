/// <summary>
/// Represents a citizen in the city registry system.
/// </summary>
public class Citizen
{
    /// <summary>
    /// Gets or sets the first name of the citizen.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the citizen.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the unique citizen identification number.
    /// </summary>
    public int CI { get; set; }

    /// <summary>
    /// Gets or sets the blood group of the citizen (A+, A-, B+, B-, O+, O-, AB+, AB-).
    /// </summary>
    public string? BloodGroup { get; set; }

    /// <summary>
    /// Gets or sets the personal asset assigned from external API.
    /// </summary>
    public string? PersonalAsset { get; set; }
}