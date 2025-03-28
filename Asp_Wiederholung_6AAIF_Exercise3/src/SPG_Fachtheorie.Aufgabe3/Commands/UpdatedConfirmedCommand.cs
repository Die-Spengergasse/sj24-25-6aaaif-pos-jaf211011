﻿using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe3.Commands
{
    public record UpdatedConfirmedCommand
    (
            [Range(1, 999999, ErrorMessage = "Invalid cash desk numbner")]
                int CashDeskNumber,
                DateTime PaymentDateTime,
            [StringLength(255, MinimumLength = 1, ErrorMessage = "Invalid payment type")]
                string PaymentType,
            [Range(1, 999999, ErrorMessage = "Invalid registration number")]
                int EmployeeRegistrationNumber,
                DateTime Confirmed
        ) : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        // Validate: Method called to validate the object
        // ValidationContext: Provides context about the object being validated
        {
            if (PaymentDateTime > DateTime.Now.AddMinutes(1))
            {
                yield return new ValidationResult("Payment date cannot be more than 1 minute in the future", new[] { nameof(PaymentDateTime) });
            }
            if (Confirmed > PaymentDateTime)
            {
                yield return new ValidationResult("Date for update cannot be more than inital Payment", new[] { nameof(Confirmed) });
            }
        }
    }
}
