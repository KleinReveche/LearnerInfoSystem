using System.Text.Json;
using System.Text.Json.Serialization;
using Reveche.SimpleLearnerInfoSystem.Models;

namespace Reveche.SimpleLearnerInfoSystem.Data;

public class CountryJsonRepo
{
    private static readonly JsonSerializerOptions SourceGenOptions = new()
    {
        TypeInfoResolver = CountryJsonContext.Default,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        AllowTrailingCommas = true,
        IncludeFields = true
    };
    
    private static readonly CountryJsonContext Context = new(SourceGenOptions);

    public static CountryInfo GetCountryInfos()
    {
        using var stream = typeof(CountryJsonRepo).Assembly
            .GetManifestResourceStream("Reveche.SimpleLearnerInfoSystem.Resources.Countries.json")!;
        using var reader = new StreamReader(stream, System.Text.Encoding.UTF8);
        return JsonSerializer.Deserialize(reader.ReadToEnd(), Context.CountryInfo)!;
    }
    
    public static Country? GetCountryInfo(string countryCode) => GetCountryInfos().Countries.GetValueOrDefault(countryCode);
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(CountryInfo))]
internal partial class CountryJsonContext : JsonSerializerContext;