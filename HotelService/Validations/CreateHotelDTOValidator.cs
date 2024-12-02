using FluentValidation;
using HotelService.DTOs.HotelDTOs;

namespace HotelService.Validations
{
    public class CreateHotelDTOValidator : AbstractValidator<CreateHotelDTO>
    {
        public CreateHotelDTOValidator()
        {
            RuleFor(hotel => hotel.Name)
                .NotEmpty().WithMessage("Hotel name is required.")
                .MaximumLength(100).WithMessage("Hotel name must not exceed 100 characters.");

            RuleFor(hotel => hotel.Location)
                .NotEmpty().WithMessage("Hotel location is required.")
                .MaximumLength(100).WithMessage("Hotel location must not exceed 100 characters.");
        }
    }

    public class UpdateHotelDTOValidator : AbstractValidator<UpdateHotelDTO>
    {
        public UpdateHotelDTOValidator()
        {
            RuleFor(hotel => hotel.Name)
                .NotEmpty().WithMessage("Hotel name is required.")
                .MaximumLength(100).WithMessage("Hotel name must not exceed 100 characters.");

            RuleFor(hotel => hotel.Location)
                .NotEmpty().WithMessage("Hotel location is required.")
                .MaximumLength(100).WithMessage("Hotel location must not exceed 100 characters.");
        }
    }
}
