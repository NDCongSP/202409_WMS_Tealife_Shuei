using Application.DTOs;
using FluentValidation;

namespace Infrastructure.Validators
{
    public class WareHouseShipmentValidator : AbstractValidator<WarehouseShipmentDto>
    {
        public WareHouseShipmentValidator()
        {
        //    RuleFor(user => user.ShipmentNo)
        //        .NotEmpty().WithMessage("ShipmentNo is require");

            RuleFor(user => user.TenantId)
                .NotEmpty().WithMessage("TenantId is require");
        }
    }
}
