using MarketData.DAL.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace MarketData.BLL.Models.DtoModels;

public class AssetMappingDto
{
    [JsonPropertyName("provider")]
    public string Provider { get; set; } = string.Empty;

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("exchange")]
    public string Exchange { get; set; } = string.Empty;

}

public class AssetMappingConverter : JsonConverter<List<AssetMappingDto>>
{
    public override List<AssetMappingDto> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var mappings = new List<AssetMappingDto>();

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject for mappings");
        }

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected PropertyName");
            }

            string provider = reader.GetString()!;
            reader.Read();

            var mapping = JsonSerializer.Deserialize<AssetMappingDto>(ref reader, options)!;
            mapping.Provider = provider;
            mappings.Add(mapping);
        }

        return mappings;
    }

    public override void Write(Utf8JsonWriter writer, List<AssetMappingDto> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var mapping in value)
        {
            writer.WritePropertyName(mapping.Provider);
            JsonSerializer.Serialize(writer, mapping, options);
        }

        writer.WriteEndObject();
    }
}