public class DogBreed
{
    public string? Id { get; set; }
    public string? Type { get; set; }
    public DogBreedAttributes? Attributes { get; set; }
}

public class DogBreedAttributes
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}