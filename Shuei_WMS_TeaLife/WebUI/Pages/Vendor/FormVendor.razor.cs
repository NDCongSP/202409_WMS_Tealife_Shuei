using Application.DTOs.Request.Vendor;
using Application.Extentions;
using Microsoft.AspNetCore.Components;
using WebUI.Core;
using VendorEntity = Domain.Entity.Commons.Vendor;
using Domain.Enums;

namespace WebUI.Pages.Vendor
{
    public partial class FormVendor
    {
        [Parameter] public required string Mode { get; set; }
        [Parameter] public VendorEntity? VendorDto { get; set; }
        [SupplyParameterFromForm] private VendorRequestDTO? Vendor { get; set; }

        protected override void OnInitialized()
        {
            Vendor ??= new VendorRequestDTO();

            if (VendorDto != null)
            {
                Vendor.Id = VendorDto.Id;
                Vendor.VendorCode = VendorDto.VendorCode;
                Vendor.VendorName = VendorDto.VendorName;
                Vendor.VendorImage = string.Empty;
                Vendor.Abbreviation = string.Empty;
                Vendor.Type = string.Empty;
                Vendor.Address = VendorDto.DeadOfficeAddress;
                Vendor.Email = VendorDto.BillingAddress;
                Vendor.PhoneNumber = VendorDto.BillingAddress;
                Vendor.Fax = VendorDto.HeadOfficeFax;
                Vendor.Status = VendorDto.Status;
                Vendor.Remarks = VendorDto.Remarks;
                Vendor.BankName = VendorDto.BankName;
                Vendor.BranchName = VendorDto.BankBranch;
                Vendor.AccountNumber = VendorDto.BankAccountNumber;
                Vendor.AccountHolderName = VendorDto.BankAccountHolder;
                Vendor.ShippingAddress = VendorDto.BillingAddress;
            }
        }

        private async Task AddNewItemAsync() => await _vendor.InsertAsync(VendorDto);

        private async Task EditItemAsync() => await _vendor.UpdateAsync(VendorDto);

        private async Task DeleteItemAsync() => await _vendor.DeleteAsync(VendorDto);

        private async Task SaveVendor()
        {
            try
            {
                if (Vendor != null)
                {
                    VendorDto ??= new VendorEntity();
                    VendorDto.VendorCode = Vendor.VendorCode;
                    VendorDto.VendorName = Vendor.VendorName;
                    VendorDto.DeadOfficeAddress = Vendor.Address;
                    VendorDto.BillingMail = Vendor.Email;
                    VendorDto.BillingPhone = Vendor.PhoneNumber;
                    VendorDto.HeadOfficeFax = Vendor.Fax;
                    VendorDto.Status = Vendor.Status;
                    VendorDto.Remarks = Vendor.Remarks;
                    VendorDto.BankName = Vendor.BankName;
                    VendorDto.BankBranch = Vendor.BranchName;
                    VendorDto.BankAccountNumber = Vendor.AccountNumber;
                    VendorDto.BankAccountHolder = Vendor.AccountHolderName;
                    VendorDto.BillingAddress = Vendor.ShippingAddress;

                    if (Mode == ConstantExtention.ViewMode.Edit)
                    {
                        await EditItemAsync();
                    }
                    else
                    {
                        await AddNewItemAsync();
                    }
                }

                _navigation.NavigateTo($"{Constants.Routes.Vendor}");
            }
            catch (Exception ex)
            {

            }
        }

        private void AdjustAction()
        {
            switch (Mode)
            {
                case ConstantExtention.ViewMode.Edit:
                    Mode = ConstantExtention.ViewMode.View;
                    break;
                case ConstantExtention.ViewMode.View:
                    Mode = ConstantExtention.ViewMode.Edit;
                    break;
                case ConstantExtention.ViewMode.Create:
                    _navigation.NavigateTo($"{Constants.Routes.Vendor}");
                    break;
                default:
                    break;
            }
        }
    }
}
