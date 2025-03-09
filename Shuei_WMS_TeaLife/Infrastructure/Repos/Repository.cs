using Application.Services;
using Application.Services.Authen;
using Application.Services.Inbound;
using Application.Services.Inventory;
using Application.Services.Outbound;
using Application.Services.Suppliers;
using Application.Services.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using static Application.Extentions.ApiRoutes;

namespace Infrastructure.Repos
{
    public class Repository
    {
        public IProducts SProducts { get; set; }
        public IPermissions SPermissions { get; set; }
        public IPermissionTenant SPermissionTenant { get; set; }
        public IRoleToPermissions SRoleToPermissions { get; set; }
        public IRoleToPermissionTenant SRoleToPermissionTenant { get; set; }

        public IProductJanCodes SProductJanCodes { get; set; }

        public IVendors SVendors { get; set; }
        public IUserVendors SUserVendors { get; set; }
        public IVendorBillings SVendorBillings { get; set; }
        public IVendorBillingDetail SVendorBillingDetail { get; set; }


        public ILocations SLocations { get; set; }

        public IDevices SDevices { get; set; }

        public IBins SBins { get; set; }
        public IProductCategory SProductCategories { get; set; }
        public IUnits SUnits { get; set; }
        public ICurrency SCurrency { get; set; }
        public ICurrencyPairSetting SCurrencyPairSetting { get; set; }

        public ITenants STennats { get; set; }
        public IUserToTenant SUserToTenant { get; set; }

        public ISuppliers SSuppliers { get; set; }

        public IArrivalInstructions SArrivalInstructions { get; set; }
        public IArrivalInstructionDetails SArrivalInstructionDetails { get; set; }
        public IWarehousePutAway SWarehousePutAways { get; set; }
        public IWarehousePutAwayLine SWarehousePutAwayLines { get; set; }
        public IWarehousePutAwayStaging SWarehousePutAwayStagings { get; set; }
        public IWarehouseReceiptOrder SWarehouseReceiptOrders { get; set; }
        public IWarehouseReceiptOrderLine SWarehouseReceiptOrderLines { get; set; }
        public IWarehouseReceiptStaging SWarehouseReceiptStagings { get; set; }
        public IWarehouseTran SWarehouseTrans { get; set; }
       
        public INumberSequences SNumberSequences { get; set; }
        public IBatches SBatches { get; set; }
        public IShippingBox ShippingBoxs { get; set; }

        public IWarehousePickingList SWarehousePickingList { get; set; }
        public IWarehousePickingLine SWarehousePickingLine { get; set; }
        public IWarehousePickingStaging SWarehousePickingStaging { get; set; }
        public IWarehouseShipment SWarehouseShipment { get; set; }
        public IWarehouseShipmentLine SWarehouseShipmentLine { get; set; }
        public IShippingBox SShippingBox { get; set; }
        public IShippingCarrier SShippingCarrier { get; set; }
        public IPackingList SPackingList { get; set; }
        public ICategories SCategories { get; set; }
        public IReturnOrder SReturnOrder { get; set; }
        public IInventTransfer  SInventTransfer { get; set; }
        public IInventTransferLines  SInventTransferLine { get; set; }
        public IWarehouseParameters SWarehouseParameters { get; set; }
        public IInventAdjustment SInventAdjustments { get; set; }
        public IInventAdjustmentLines SInventAdjustmentLines { get; set; }
        public IInventBundle SInventBundles { get; set; }
        public IInventBundleLines SInventBundleLines { get; set; }
        public IInventStockTakeRecording  SInventStockTakeRecordings { get; set; }
        public IImageStorage SImageStorage { get; set; }
        public IInventStockTake SStockTake { get; set; }
        public ICompanies SCompanies { get; set; }

        public IProductBundles SProductBundles { get; set; }
        public IOrder SOrder { get; set; }
        public ITaskModel STask { get; set; }
        public ICommon SCommon { get; set; }

        public ICountryMaster SCountryMaster { get; set; }

        public Repository(IProducts sProduct = null, ILocations sLocations = null, IDevices sDevices = null
            , IProductJanCodes sProductJanCodes = null, IVendors sVendors = null, IBins sBins = null
            , IProductCategory sProductCategories = null, IUnits sUnits = null, IUserVendors sUserVendors = null
            , IVendorBillings sVendorBillings = null, IVendorBillingDetail sVendorBillingDetail = null
            , ICurrency sCurrencys = null, ICurrencyPairSetting sCurrencyPairSetting = null
            , IPermissions sPermissions = null, IPermissionTenant sPermissionTenant = null
            , IRoleToPermissions sRoleToPermissions = null, IRoleToPermissionTenant sRoleToPermissionTenant = null
            , ITenants sTennats = null, IUserToTenant sUserToTenant = null, ISuppliers iSuppliers = null
            , IArrivalInstructions sArrivalInstructions = null, IArrivalInstructionDetails sArrivalInstructionDetails = null
            , IWarehousePutAway sWarehousePutAways = null, IWarehousePutAwayLine sWarehousePutAwayLines = null
            , IWarehousePutAwayStaging sWarehousePutAwayStagings = null, IWarehouseReceiptOrder sWarehouseReceiptOrders = null
            , IWarehouseReceiptOrderLine sWarehouseReceiptOrderLines = null, IWarehouseReceiptStaging sWarehouseReceiptStagings = null
            , IWarehouseTran sWarehouseTrans = null, INumberSequences sNumberSequences = null, IBatches sBatches = null
            , IWarehousePickingList sWarehousePickingList = null, IWarehousePickingLine sWarehousePickingLine = null
            , IWarehousePickingStaging sWarehousePickingStaging = null
            , IPackingList sPackingList = null, IWarehouseShipment sWarehouseShipment = null

            , IWarehouseShipmentLine sWarehouseShipmentLine = null, IShippingBox sShippingBox = null, IShippingCarrier sShippingCarrier = null
            , ICategories sICategories = null, IReturnOrder sReturnOrder = null, IInventTransfer sInventTransfer = null, IInventTransferLines sInventTransferLine = null
            , IWarehouseParameters sWarehouseParameters = null, IInventAdjustment sInventAdjustments = null, IInventAdjustmentLines sInventAdjustmentLines = null, IInventBundle sInventBundles = null, IInventBundleLines sInventBundleLines = null
            , IInventStockTakeRecording sInventStockTakeRecordings = null
            , IImageStorage sImageStorage = null, IInventStockTake sStockTake = null, ICompanies sCompanies = null, IProductBundles sProductBundles = null, IOrder sOrder = null, ITaskModel sTask = null, ICommon sCommon = null
            , ICountryMaster sCountryMaster = null)
        {
            SProducts = sProduct;
            SLocations = sLocations;
            SDevices = sDevices;
            SProductJanCodes = sProductJanCodes;
            SVendors = sVendors;
            SBins = sBins;
            SProductCategories = sProductCategories;
            SUnits = sUnits;
            SUserVendors = sUserVendors;
            SVendorBillings = sVendorBillings;
            SVendorBillingDetail = sVendorBillingDetail;

            SCurrency = sCurrencys;
            SCurrencyPairSetting = sCurrencyPairSetting;
            SPermissions = sPermissions;
            SPermissionTenant = sPermissionTenant;
            SRoleToPermissions = sRoleToPermissions;
            SRoleToPermissionTenant = sRoleToPermissionTenant;
            STennats = sTennats;
            SUserToTenant = sUserToTenant;
            SSuppliers = iSuppliers;
            SArrivalInstructions = sArrivalInstructions;
            SArrivalInstructionDetails = sArrivalInstructionDetails;
            SWarehousePutAways = sWarehousePutAways;
            SWarehousePutAwayLines = sWarehousePutAwayLines;
            SWarehousePutAwayStagings = sWarehousePutAwayStagings;
            SWarehouseReceiptOrders = sWarehouseReceiptOrders;
            SWarehouseReceiptOrderLines = sWarehouseReceiptOrderLines;
            SWarehouseReceiptStagings = sWarehouseReceiptStagings;
            SWarehouseTrans = sWarehouseTrans;
            SNumberSequences = sNumberSequences;
            SBatches = sBatches;
            SWarehousePickingList = sWarehousePickingList;
            SWarehousePickingLine = sWarehousePickingLine;
            SWarehousePickingStaging = sWarehousePickingStaging;
            SWarehouseShipment = sWarehouseShipment;
            SWarehouseShipmentLine = sWarehouseShipmentLine;
            SShippingBox = sShippingBox;
            SShippingCarrier = sShippingCarrier;
            SCategories = sICategories;
            SPackingList = sPackingList;
            SReturnOrder = sReturnOrder;
            SInventTransfer = sInventTransfer;
            SInventTransferLine = sInventTransferLine;
            SWarehouseParameters = sWarehouseParameters;
            SInventAdjustments = sInventAdjustments;
            SInventAdjustmentLines = sInventAdjustmentLines;
            SInventBundles = sInventBundles;
            SInventBundleLines = sInventBundleLines;
            SInventStockTakeRecordings = sInventStockTakeRecordings;
            SImageStorage = sImageStorage;

            SStockTake = sStockTake;
            SCompanies = sCompanies;
            SProductBundles = sProductBundles;
            SOrder = sOrder;
            STask = sTask;
            SCommon = sCommon;
            SCountryMaster = sCountryMaster;
        }
    }
}
