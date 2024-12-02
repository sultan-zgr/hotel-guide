using FluentValidation;
using HotelService.DTOs.ContactDTOs;

namespace HotelService.Validations
{
    public class CreateContactDTOValidator : AbstractValidator<CreateContactDTO>
    {
        public CreateContactDTOValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Contact Type is required.")
                .Must(type => new[] { "Phone", "Email" }.Contains(type))
                .WithMessage("Contact Type must be 'Phone' or 'Email'.");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Contact Value is required.")
                .MaximumLength(100).WithMessage("Contact Value cannot exceed 100 characters.")
                .When(x => x.Type == "Phone")
                .Matches(@"^\+?[0-9\s]+$").WithMessage("Phone numbers must contain only digits and spaces.")
                .When(x => x.Type == "Phone");

            RuleFor(x => x.Value)
                .EmailAddress().WithMessage("Invalid email format.")
                .When(x => x.Type == "Email");
        }
    }
}
