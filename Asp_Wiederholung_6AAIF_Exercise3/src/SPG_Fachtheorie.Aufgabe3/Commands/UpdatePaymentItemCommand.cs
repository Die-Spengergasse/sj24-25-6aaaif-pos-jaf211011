using SPG_Fachtheorie.Aufgabe1.Model;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe3.Commands
{
    public record UpdatePaymentItemCommand(
    [Range(1, 999999, ErrorMessage = "Invalid ID")]
    int id,
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Invalid Article Name")]
    string articleName,
    [Range(1, 999999, ErrorMessage = "Invalid Amount")]
    int amount,
    [Range(1, 999999, ErrorMessage = "Invalid Price")]
    decimal price,
    [Range(1, 999999, ErrorMessage = "Invalid Payment ID")]
    Payment payment,
    DateTime LastUpdated
        ) : IValidatableObject

    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        // Validate: Method called to validate the object
        // ValidationContext: Provides context about the object being validated
        {
            if (LastUpdated > payment.PaymentDateTime)
            {
                yield return new ValidationResult("Date for update cannot be more than inital Payment", new[] { nameof(LastUpdated) });
            }
        }
    }
}
