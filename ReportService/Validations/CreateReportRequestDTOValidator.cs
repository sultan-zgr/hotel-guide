using FluentValidation;
using ReportService.DTOs;

namespace ReportService.Validations
{
    public class CreateReportRequestDTOValidator : AbstractValidator<CreateReportRequestDTO>
    {
        public CreateReportRequestDTOValidator()
        {
            RuleFor(report => report.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(100).WithMessage("Location must not exceed 100 characters.");
        }
    }
}
