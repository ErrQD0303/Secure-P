using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SecureP.Shared.Helpers;

public class AppModelValidator
{
    private class ValidatorUserProfile
    {
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format. Email must have '@' and '.' characters. Email must be between 5 and 254 characters long.")]
        public string Email { get; set; } = string.Empty;
        [RegularExpression(@"^(\+?[1-9]|0)\d{1,14}$", ErrorMessage = "Invalid phone number format. Phone number can only contain digits and an optional leading '+' with country code. Phone number must be between 1 and 15 digits long.")]
        public string PhoneNumber { get; set; } = string.Empty;
        [DataType(DataType.Date)]
        public DateTime DayOfBirth { get; set; } = DateTime.MinValue;
    }

    public static bool ValidateUserProfile(string email, string phoneNumber, DateTime dayOfBirth, out List<IdentityError> errors)
    {
        errors = [];
        if (string.IsNullOrEmpty(email))
        {
            errors.Add(new IdentityError
            {
                Code = "Email",
                Description = "Email is required."
            });
        }

        if (string.IsNullOrEmpty(phoneNumber))
        {
            errors.Add(new IdentityError
            {
                Code = "Phone",
                Description = "Phone number is required."
            });
        }

        var validator = new ValidatorUserProfile
        {
            Email = email,
            PhoneNumber = phoneNumber,
            DayOfBirth = dayOfBirth
        };

        var context = new ValidationContext(validator);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(validator, context, results, true);

        if (!isValid)
        {
            errors = [.. results.Select(r => new IdentityError
            {
                Code = r.ErrorMessage switch
                {
                    string errorMessage when errorMessage.ToLower().Contains("email") => "Email",
                    string errorMessage when errorMessage.ToLower().Contains("phone") => "Phone",
                    _ => "summary"
                },
                Description = r.ErrorMessage ?? string.Empty
            })];
        }

        if (dayOfBirth > DateTime.Now)
        {
            errors.Add(new IdentityError
            {
                Code = "DayOfBirth",
                Description = "Day of birth cannot be in the future."
            });
        }

        return errors.Count == 0;
    }
}
