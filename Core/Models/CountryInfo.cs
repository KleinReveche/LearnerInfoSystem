namespace Reveche.SimpleLearnerInfoSystem.Models;

public class CountryInfo
{
    public required Dictionary<string, Country> Countries { get; set; }
}

public class Country
{
    public required string Name { get; set; }
    public required string Region { get; set; }
    public required Dictionary<string, string> Timezones { get; set; }
    public required Dictionary<string, string> Iso { get; set; }
    public required List<string> Phone { get; set; }
    public required string Emoji { get; set; }
    public required string Image { get; set; }
}