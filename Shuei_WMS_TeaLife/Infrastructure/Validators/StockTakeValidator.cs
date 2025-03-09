using Application.DTOs;
using FluentValidation;

namespace Infrastructure.Validators
{
    public class StockTakeValidator : AbstractValidator<InventStockTakeDto>
    {
        public StockTakeValidator()
        {
            RuleFor(ro => ro.StockTakeNo)
                .NotEmpty().WithMessage("StockTakeNo is required!");
            RuleFor(ro => ro.Location)
                .NotEmpty().WithMessage("Location is required!");
        }
    }
}
