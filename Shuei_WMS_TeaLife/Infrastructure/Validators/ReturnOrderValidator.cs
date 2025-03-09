using Application.DTOs;
using FluentValidation;

namespace Infrastructure.Validators
{
    public class ReturnOrderValidator : AbstractValidator<ReturnOrderDto>
    {
        public ReturnOrderValidator()
        {
            RuleFor(ro => ro.ReturnOrderNo)
                .NotEmpty().WithMessage("ReturnOrderNo is required!");
            RuleFor(ro => ro.ShipmentNo)
                .NotEmpty().WithMessage("ShipmentNo is required!");
        }
    }
}
