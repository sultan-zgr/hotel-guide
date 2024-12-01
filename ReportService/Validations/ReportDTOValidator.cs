using FluentValidation;
using ReportService.DTOs;

namespace ReportService.Validations
{
    public class ReportDTOValidator : AbstractValidator<ReportDTO>
    {
        public ReportDTOValidator()
        {
            RuleFor(report => report.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(100).WithMessage("Location must not exceed 100 characters.");

            RuleFor(report => report.HotelCount)
                .GreaterThanOrEqualTo(0).WithMessage("Hotel count must be zero or a positive number.");

            RuleFor(report => report.ContactCount)
                .GreaterThanOrEqualTo(0).WithMessage("Contact count must be zero or a positive number.");
        }
    }
}
