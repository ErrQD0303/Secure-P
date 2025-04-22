using System.Text.Json.Serialization;

namespace SecureP.Identity.Models.Dto.SortModels;

/// <summary>
/// Specifies the criteria by which parking subscription can be ordered.
/// </summary>
/// <remarks>
/// This enum can be used in APIs to define sorting preferences for parking subscription.
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
public enum AppUserParkingSubscriptionOrderBy
{
    /// <summary>
    /// Order by subscription start date.
    /// </summary>
    SubscriptionStartDate,

    /// <summary>
    /// Order by subscription end date.
    /// </summary>
    SubscriptionEndDate,

    /// <summary>
    /// Order by subscription status.
    /// </summary>
    SubscriptionStatus,

    /// <summary>
    /// Order by User ID.
    /// </summary>
    UserId,

    /// <summary>
    /// Order by Parking Zone ID.
    /// </summary>
    ParkingZoneId,

    /// <summary>
    /// Order by Product Type.
    /// /// </summary>
    ProductType,

    /// <summary>
    /// Order by License Plate.
    /// </summary>
    LicensePlate,

    /// <summary>
    /// Order by Subscription Fee.
    /// </summary>
    SubscriptionFee,

    /// <summary>
    /// Order by Clamping Fee.
    /// </summary>
    ClampingFee,

    /// <summary>
    /// Order by ChangeSignage Fee.
    /// </summary>
    ChangeSignageFee,

    /// <summary>
    /// Order by Payment Date.
    /// </summary>
    PaymentDate,

    /// <summary>
    /// Order by Is Paid status.
    /// </summary>
    IsPaid,

    /// <summary>
    /// Order by Id.
    /// </summary>
    Id
}