using System.Text.Json.Serialization;

namespace SecureP.Identity.Models.Dto.SortModels;

/// <summary>
/// Specifies the criteria by which parking rates can be ordered.
/// </summary>
/// <remarks>
/// This enum can be used in APIs to define sorting preferences for parking rates.
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
public enum ParkingRateOrderBy
{
    /// <summary>
    /// Order by hourly rate.
    /// </summary>
    HourlyRate,

    /// <summary>
    /// Order by daily rate.
    /// </summary>
    DailyRate,

    /// <summary>
    /// Order by monthly rate.
    /// </summary>
    MonthlyRate,
}