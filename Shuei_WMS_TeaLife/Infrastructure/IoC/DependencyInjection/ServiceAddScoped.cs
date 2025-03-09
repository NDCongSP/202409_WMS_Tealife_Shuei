using Application.Services;
using Application.Services.Authen;
using Application.Services.Inbound;
using Application.Services.Inventory;
using Application.Services.Outbound;
using Application.Services.Suppliers;
using Application.Services.Vendors;
using Infrastructure.Repos;
using Infrastructure.Repos.Inventory;
using Infrastructure.Repos.Outbound;
using Microsoft.Extensions.DependencyInjection;
namespace Infrastructure.IoC.DependencyInjection
{
    public static class ServiceAddScoped
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IAccount, RepositoryAccountServices>();
            services.AddScoped<IProducts, RepositoryProductsServices>();

            services.AddScoped<IPermissions, RepositoryPermissionsServices>();
            services.AddScoped<IRoleToPermissions, RepositoryRoleToPermissionsServices>();
            services.AddScoped<IRoleToPermissionTenant, RepositoryRoleToPermissionTenantTenantServices>();

            services.AddScoped<IProductJanCodes, RepositoryProductJanCodesServices>();
            services.AddScoped<IVendors, RepositoryVendorsServices>();
            services.AddScoped<IUserVendors, RepositoryUserVendorServices>();
            services.AddScoped<IVendorBillings, RepositoryVendorBillingsServices>();
            services.AddScoped<IVendorBillingDetail, RepositoryVendorBillingDetailServices>();
            services.AddScoped<ILocations, RepositoryLocationsServices>();
            services.AddScoped<IBins, RepositoryBinsServices>();
            services.AddScoped<IDevices, RepositoryDevicesServices>();
            services.AddScoped<IProductCategory, RepositoryProducCategorysServices>();
            services.AddScoped<IUnits, RepositoryUnitsServices>();

            services.AddScoped<ICurrency, RepositoryCurrencyServices>();
            services.AddScoped<ICurrencyPairSetting, RepositoryCurrencyPairSettingServices>();
            services.AddScoped<ITenants, RepositoryTenantsServices>();
            services.AddScoped<IUserToTenant, RepositoryUserToTenantServices>();
            services.AddScoped<ISuppliers, RepositorySuppliersServices>();
            services.AddScoped<IArrivalInstructions,RepositoryArrivalInstructionServices>();
            services.AddScoped<IArrivalInstructionDetails,RepositoryArrivalInstructionDetailsServices>();

            services.AddScoped<IWarehousePutAway, RepositoryWarehousePutAwayServices>();
            services.AddScoped<IWarehousePutAwayLine, RepositoryWarehousePutAwayLineServices>();
            services.AddScoped<IWarehousePutAwayStaging, RepositoryWarehousePutAwayStagingServices>();
            services.AddScoped<IWarehouseReceiptOrder, RepositoryWarehouseReceiptOrderServices>();
            services.AddScoped<IWarehouseReceiptOrderLine, RepositoryWarehouseReceiptOrderLineServices>();
            services.AddScoped<IWarehouseReceiptStaging, RepositoryWarehouseReceiptStagingServices>();
            services.AddScoped<IWarehouseTran, RepositoryWarehouseTranServices>();
            services.AddScoped<INumberSequences, RepositorySequenceNumberServices>();
            services.AddScoped<IBatches, RepositoryBatchesService>();
            services.AddScoped<IWarehousePickingList,RepositoryWarehousePickingListServices>();
            services.AddScoped<IWarehousePickingLine, RepositoryWarehousePickingLineServices>();
            services.AddScoped<IWarehousePickingStaging, RepositoryWarehousePickingStagingServices>();
            services.AddScoped<IWarehouseShipment, RepositoryWarehouseShipment>();
            services.AddScoped<IWarehouseShipmentLine, RepositoryWarehouseShipmentLine>();
            services.AddScoped<IShippingBox, RepositoryShippingBox>();
            services.AddScoped<IShippingCarrier, RepositoryShippingCarrier>();
            services.AddScoped<IPackingList,RepositoryPackingListServices>();
            services.AddScoped<IInventTransfer, RepositoryInventTransferService>();
            services.AddScoped<IInventTransferLines, RepositoryInventTransferLineService>();
            services.AddScoped<IWarehouseParameters, RepoWarehouseParameterServices >();
            services.AddScoped<IReturnOrder, RepositoryReturnOrderService>();
            services.AddScoped<IInventAdjustment,RepositoryInventAdjustmentService>();
            services.AddScoped<IInventAdjustmentLines,RepositoryInventAdjustmentLineService>();
            services.AddScoped<IInventBundle, RepositoryInventBundleService>();
            services.AddScoped<IInventBundleLines, RepositoryInventBundleLineService>();
            services.AddScoped<IInventStockTakeRecording,RepositoryInventStockTakeRecordingService>();
            services.AddScoped<ICategories, RepositoryCategoryServices>();
            services.AddScoped<IImageStorage, RepositoryImageStorageServices>();
            services.AddScoped<IInventStockTake, RepositoryInventStockTakeService>();
            services.AddScoped<ICompanies, RepositoryCompaniesServices>();
            services.AddScoped<IProductBundles, RepositoryProductBundlesServicess>();
            services.AddScoped<IOrder, RepositoryOrderService>();
            services.AddScoped<ITaskModel, RepositoryTaskServices>();
            services.AddScoped<ICommon, RepositoryCommonServices>();
            services.AddScoped<ICountryMaster, RepositoryCountryMasterServices>();
            services.AddScoped<Repository>();
        }
    }
}
