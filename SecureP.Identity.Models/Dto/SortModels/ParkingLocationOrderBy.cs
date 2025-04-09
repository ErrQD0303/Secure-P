using System.Text.Json.Serialization;

namespace SecureP.Identity.Models.Dto.SortModels;

/// <summary>
/// Specifies the criteria by which parking locations can be ordered.
/// </summary>
/// <remarks>
/// This enum can be used in APIs to define sorting preferences for parking locations.
/// </remarks>
/// <example>
/// Example usage in Swagger:
/// To display the names of these enum values in Swagger, you can use the 
/// <see cref="System.Text.Json.Serialization.JsonConverterAttribute"/> or 
/// <see cref="Newtonsoft.Json.JsonConverterAttribute"/> with a custom converter.
/// Alternatively, you can configure Swagger to use the enum names by enabling 
/// the `DescribeAllEnumsAsStrings` or `DescribeStringEnumsInCamelCase` options 
/// in the Swagger configuration.
/// </example>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ParkingLocationOrderBy
{
    Name,
    Address,
    Capacity,
    AvailableSpaces,
    HourlyRate,
    DailyRate,
    MonthlyRate,
}