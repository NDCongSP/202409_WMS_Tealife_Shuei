using Application.DTOs.Request;
using Application.Extentions.Pagings;

namespace WebUIFinal.Pages.ReturnOrder
{
    public partial class ReturnOrderList
    {
        private PageList<ReturnOrderDto> returnOrders = new();
        private RadzenDataGrid<ReturnOrderDto> _profileGrid;
        private bool _showPagerSummary = true;
        private bool allowRowSelectOnRowClick = false;
        private int _count, _pageNumber = 1, _pageSize = 30;
        ReturnOrderSearchModel _searchModel = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();
                Constants.PagingSummaryFormat = $"{_CLoc["DisplayPage"]} {{0}} {_CLoc["Of"]} {{1}} <b>({_CLoc["Total"]} {{2}} {_CLoc["Records"]})</b>";

                await RefreshDataAsync();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception e)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], e.Message);
            }
        }

        async Task LoadData(LoadDataArgs args)
        {
            _pageNumber = (int)((args.Skip / args.Top) + 1);
            _pageSize = (int)args.Top;
            await RefreshDataAsync();
        }

        private async Task RefreshDataAsync()
        {
            try
            {
                if ((_searchModel.ReturnOrderFrom.HasValue && _searchModel.ReturnOrderTo.HasValue) 
                    && (_searchModel.ReturnOrderFrom > _searchModel.ReturnOrderTo))
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], "The 'From Date' must be less than the 'To Date'.");
                    return;
                }

                var model = new QueryModel<ReturnOrderSearchModel>
                {
                    Entity = _searchModel,
                    PageNumber = _pageNumber,
                    PageSize = _pageSize
                };
                var result = await _returnOrderService.SearchReturnOrder(model);
                if (result.Succeeded)
                {
                    returnOrders = result.Data;
                }
                else
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], result.Messages.ToString());
                    return;
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.Message);
            }
        }

        private void ShowNotification(NotificationSeverity severity, string summary, string detail, int duration = 5000)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = severity,
                Summary = summary,
                Detail = detail,
                Duration = duration
            });
        }

        private void AddNewItemAsync() => _navigation.NavigateTo($"/return-order/{"create"}");

        private List<EnumDisplay<EnumReturnOrderStatus>> GetDisplayReturnOrderStatus()
        {
            return Enum.GetValues(typeof(EnumReturnOrderStatus)).Cast<EnumReturnOrderStatus>().Select(_ => new EnumDisplay<EnumReturnOrderStatus>
            {
                Value = _,
                DisplayValue = GetValueReturnOrderStatus(_)
            }).ToList();
        }

        private string GetValueReturnOrderStatus(EnumReturnOrderStatus status) => status switch
        {
            EnumReturnOrderStatus.Open => _localizer["Open"],
            EnumReturnOrderStatus.Receiving => _localizer["Receiving"],
            EnumReturnOrderStatus.Received => _localizer["Received"],
            EnumReturnOrderStatus.Putaway => _localizer["Putaway"],
            EnumReturnOrderStatus.Completed => _localizer["Completed"],
            _ => status.ToString(),
        };

        private string GetLocalizedStatus(EnumReturnOrderStatus status)
        {
            return status switch
            {
                EnumReturnOrderStatus.Open => _localizer["Open"],
                EnumReturnOrderStatus.Receiving => _localizer["Receiving"],
                EnumReturnOrderStatus.Received => _localizer["Received"],
                EnumReturnOrderStatus.Putaway => _localizer["Putaway"],
                EnumReturnOrderStatus.Completed => _localizer["Completed"],
                _ => status.ToString(),
            };
        }

        private void ClearFilter()
        {
            _searchModel = new();
            _profileGrid.Reload();
            StateHasChanged();
        }
    }
}
