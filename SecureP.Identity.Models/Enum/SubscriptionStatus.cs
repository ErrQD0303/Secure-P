namespace SecureP.Identity.Models.Enum;

/// <summary>
/// An enumeration that represents the various subscription statuses for users within the SecureP application. Each value in the SubscriptionStatus enum corresponds to a specific state of a user's subscription, such as active, expired, suspended, cancelled, or pending. This enum is used to manage and track the subscription status of users, allowing the application to enforce access control and provide appropriate features based on the user's current subscription state.
/// </summary>
public enum SubscriptionStatus
{
    Active = 1,
    Expired = 3,
    Suspended = 4,
    Cancelled = 5,
    Pending = 6
}