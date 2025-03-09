namespace Application.Extentions
{
    public static class ApiRoutes
    {
        /// <summary>
        /// httpGet.
        /// </summary>
        public const string GetAll = "";
        /// <summary>
        /// httpget.
        /// </summary>
        public const string GetById = "{id}";
        /// <summary>
        /// httppost.
        /// </summary>
        public const string Update = "update";
        /// <summary>
        /// httppost.
        /// </summary>
        public const string Insert = "insert";
        public const string Delete = "delete";
        public const string AddRange = "AddRange";
        public const string DeleteRange = "DeleteRange";
        public const string DeleteById = "DeleteById/{id}";

        public static class Identity
        {
            public const string BasePath = "api/account";
            public const string Login = "identity/loginasync";
            public const string LoginHt = "identity/loginht";
            public const string CreateAccount = "identity/create";
            public const string RefreshToken = "identity/refresh-token";
            public const string CreateRole = "identity/role/create";
            public const string RoleList = "identity/role/list";
            public const string CreateSuperAdminAccount = "identity/setting";
            public const string UserWithRole = "identity/user-with-role";
            public const string ChangePassword = "identity/change-pass";
            public const string ChangeUserRole = "identity/change-role";
            public const string AssignUserRole = "identity/assign_user_role";
            public const string DeleteUser = "identity/delete-user";
            public const string DeleteUserRole = "identity/delete-user-role";
            public const string UpdateRole = "identity/update-role-name";
            public const string UpdateRoleDTO = "identity/update-role";
            public const string UpdateUserInfo = "identity/update-user-info";
            public const string UserGetById = "identity/{id}";
            public const string DeleteRole = "identity/role/delete";
            public const string UserGetByEmail = "identity/UserGetByEmail/{email}";
            public const string RoleGetById = "identity/Role/{id}";
            public const string GetReportBase64 = "identity/GetReportBase64/{id}";
            public const string GeneratePdf = "identity/GeneratePdf";
            public const string GetLabelById = "identity/GetLabelById/{id}";
            public const string GetLabelsAll = "identity/GetLabelsAll";
            public const string JobApi = "JobApi";
        }
        public static class Permissions
        {
            public const string BasePath = "api/Permissions";
            public const string GetAllPermissionWithAssignedRole = "Get-All-Permission-With-Assigned-To-Role";
            public const string AddOrEdit = "add-or-edit";
        }
        public static class PermissionTenant
        {
            public const string BasePath = "api/PermissionTenant";
        }

        public static class RoleToPermissions
        {
            public const string BasePath = "api/RoleToPermissions";
            public const string GetByPermissionId = "GetByPermissionId/{id}";
        }
        public static class RoleToPermissionTenant
        {
            public const string BasePath = "api/RoleToPermissionsTenant";
        }

        public static class Product
        {
            public const string BasePath = "api/Products";
            public const string GetFillter = "GetFillter";
            public const string UploadProductImage = "UploadProductImage";
            public const string GetProductListAsync = "get-product-list";
            public const string GetByProductCodeAsync = "get-by-product-code";
            public const string SearchByProductCodeAsync = "search-by-product-code";
            public const string AutocompleteProductAsync = "autocomplete-product";
            public const string GetAllDtoAsync = "GetAllDtoAsync";
            public const string GetByIdDtoAsync = "GetByIdDtoAsync/{id}";
            public const string InsertDtoAsync = "InsertDtoAsync";
            public const string UpdateDtoAsync = "UpdateDtoAsync";
            public const string DeleteDtoAsync = "DeleteDtoAsync/{id}";
            public const string GetProductCategoriesAsync = "get-product-categories";
            public const string GetByUnitAsync = "GetByUnit/{unitId}";
            public const string GetByCatetgoryAsync = "GetByCatetgory/{categoryId}";
            public const string GetBySupplierAsync = "GetBySupplier/{supplierId}";


        }
        public static class ProductBundle
        {
            public const string BasePath = "api/ProductBundles";
            public const string GetPlannedShipmentBundlesAsync = "GetPlannedShipmentBundlesAsync";
            public const string GetAllDistinctAsync = "GetAllDistinctAsync";
            public const string GetProductCodesByBundleCodeAsync = "GetProductCodesByBundleCodeAsync/{BundleCode}";
            public const string DeleteAllBundleLineAsync = "DeleteAllBundleLineAsync/{id}";
            
        }
        public static class ProductCategories
        {
            public const string BasePath = "api/ProductCategories";
        }
        public static class Unit
        {
            public const string BasePath = "api/Units";
        }

        public static class Locations
        {
            public const string BasePath = "api/Locations";
            public const string DeleteLocation = "deletelocation";
        }
        public static class Bins
        {
            public const string BasePath = "api/Bins";
            public const string GetByLocationId = "GetByLocationId/{locationId}";
            public const string AddOrUpdate = "AddOrUpdate";
            public const string GetLabelById = "GetLabelById/{id}";
            public const string GetLabelByLocationId = "GetLabelByLocationId/{locationId}";
        }
        public static class ProductJanCodes
        {
            public const string BasePath = "api/ProductJanCodes";
            public const string GetByProductId = "GetByProductId/{productId}";
            public const string AddOrUpdateAsync = "add-or-update";
        }
        public static class Vendors
        {
            public const string BasePath = "api/Vendors";

        }
        public static class VendorBillings
        {
            public const string BasePath = "api/VendorBillings";

        }
        public static class VendorBillingDetail
        {
            public const string BasePath = "api/VendorBillingDetail";

        }
        public static class UserVendors
        {
            public const string BasePath = "api/UserVendors";

        }

        public static class Devices
        {
            public const string BasePath = "api/Devices";
            public const string GetByNameAsync = "GetByNameAsync/{name}";
            public const string CheckNameExists = "CheckNameExists/{name}";
        }

        public static class Currency
        {
            public const string BasePath = "api/Currency";
        }

        public static class CurrencyPairSetting
        {
            public const string BasePath = "api/CurrencyPairSetting";
        }

        public static class Tenants
        {
            public const string BasePath = "api/Tenants";

        }
        public static class UserToTenant
        {
            public const string BasePath = "api/UserToTenant";
            public const string GetByUserId = "GetByUserId/{userId}";
            public const string GetUsersAsync = "get-users";
        }

        public static class Suppliers
        {
            public const string BasePath = "api/Suppliers";
            public const string GetSupplierWithTenantAsync = "GetSupplierWithTenantAsync";
        }
        //inbound
        public static class ArrivalInstructions
        {
            public const string BasePath = "api/ArrivalInstructions";
            public const string SearchAsync = "SearchAsync";
        }
        public static class ArrivalInstructionDetails
        {
            public const string BasePath = "api/ArrivalInstructionDetails";
        }
        public static class WarehouseTran
        {
            public const string BasePath = "api/WarehouseTran";
            public const string GetAllProductInventoryInfo = "GetAllInventoryInfo";
            public const string GetByProductCodeInventoryInfo = "GetByProductCodeInventoryInfo/{productCode}";

            public const string GetAllProductInventoryInfoFlowBinLotAsync = "GetAllProductInventoryInfoFlowBinLotAsync";
            public const string GetByProductCodeInventoryInfoFlowBinLotAsync = "GetByProductCodeInventoryInfoFlowBinLotAsync/{productCode}";
            public const string GetInventoryInfoFlowBinLotFlowModelSearchAsync = "GetInventoryInfoFlowBinLotFlowModelSearchAsync";
            public const string GetProductInventoryInformationAsync = "get-inventory-history";
            public const string GetProductInventoryInformationDetailsAsync = "get-inventory-history-details";
            public const string GetInventoryAdjustmentDetailsAsync = "get-inventory-adjustment-details";
            public const string GetInventoryBundleAdjustmentDetailsAsync = "get-inventory-bundle-adjustment-details";
            public const string UpdateInventoryAdjustmentAsync = "update-inventory-adjustment";
            public const string GetInventoryAdjustmentAsync = "get-inventory-adjustment";
            public const string GetQtyFromWarehouseTrans = "get-qty-fromwarehouse-trans";
            public const string GetInventoryAdjustmentLinesDefaultProduct = "get-inventory-adjustmentline-default-products";

        }
        public static class WarehousePutAway
        {
            public const string BasePath = "api/WarehousePutAway";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{putAwayNo}";
            public const string InsertWarehousePutAwayOrder = "insert-warehouse-put-away-order";
            public const string GetPutAwayAsync = "get-put-away";
            public const string SyncHTData = "sync-ht-data";
            public const string AdjustActionPutAway = "adjust-action-put-away";
            public const string UpdateWarehousePutAwaysStatus = "update-warehouse-put-away-status";
            public const string GetPutawayReportAsync = "get-put-away-report";
            public const string CompletePutawayFlow = "complete-putaway";

        }
        public static class WarehousePutAwayLine
        {
            public const string BasePath = "api/WarehousePutAwayLine";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{putAwayNo}";
            public const string GetLabelById = "GetLabelById/{id}";
            public const string GetLabelByPutAwayNo = "GetLabelByPutAwayNo/{putAwayNo}";
            public const string GetByPutAwayLineIdAsync = "get-by-putAwayLineid";
        }
        public static class WarehousePutAwayStaging
        {
            public const string BasePath = "api/WarehousePutAwayStaging";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{putAwayNo}";
            public const string GetByPutAwayLineIdAsync = "get-by-putAwayLineid";
        }
       
        public static class WarehouseReceiptOrder
        {
            public const string BasePath = "api/WarehouseReceiptOrder";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{receiptNo}";
            public const string InsertWarehouseReceiptOrder = "insert-warehouse-receipt";
            public const string UpdateWarehouseReceiptOrder = "update-warehouse-receipt";
            public const string GetReceiptOrderAsync = "get-receipt-order/{receiptNo}";
            public const string GetReceiptOrderListAsync = "get-receipt-order-list";
            public const string SyncHTData = "sync-ht-data";
            public const string AdjustActionReceiptOrder = "adjust-action-receipt-order";
            public const string CreateLineFromArrivalNo = "CreateLineFromArrivalNo";
            public const string CreateReceiptFromReceiptPlan = "CreateReceiptFromReceiptPlan/{arrivalNo}";
            public const string SearchReceiptOrderListAsync = "search-receipt-order";
            public const string GenerateReceiptFromReceiptPlan = "generate-receipt-from-receipt-plan/{tenantIds}";
            public const string GetReceiptReportAsync = "get-receipt-report";
            public const string CompleteAndPutAwayReceipts = "complete-putaway-receipts";

        }
        public static class WarehouseReceiptOrderLine
        {
            public const string BasePath = "api/WarehouseReceiptOrderLine";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{receiptNo}";
            public const string UploadProductImage = "UploadProductImage";
            public const string GetAllDTOAsync = "GetAllDTOAsync";
            public const string GetByIdDTOAsync = "GetByReceiptLineIdDTOAsync/{id}";
            public const string GetByProductCodeAsync = "GetByProductAsync/{productCode}";
        }
        public static class WarehouseReceiptStaging
        {
            public const string BasePath = "api/WarehouseReceiptStaging";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{receiptNo}";
            public const string GetByReceiptLineIdAsync = "GetByReceiptLineIdAsync/{receiptLineId}";
            public const string UploadProductErrorImageAsync = "UploadProductErrorImageAsync";
            public const string GetAllDTOAsync = "GetAllDTOAsync";
            public const string GetByReceiptLineIdDTOAsync = "GetByReceiptLineIdDTOAsync/{receiptLineId}";
        }

        public static class NumberSequences
        {
            public const string BasePath = "api/SequenceNumbers";
            public const string GetNumberSequenceByType = "GetNumberSequenceByType/{type}";
            public const string GetNoByType = "GetNoByType/{type}";
            public const string IncreaseNumberSequenceByType = "IncreaseNumberSequenceByType/{type}";
        }

        public static class Reports
        {
            public const string BasePath = "api/Reports";
            public const string GetReportBase64 = "GetReportBase64";
        }

        public static class Batches
        {
            public const string BasePath = "api/Batches";
            public const string GetBatchByLotNo = "api/GetBatchByLotNo";
            public const string SaveBatchByLotNo = "api/SaveBatchByLotNo";
        }

        public static class ShippingBoxs
        {
            public const string BasePath = "api/ShippingBoxs";
            public const string GetShippingBoxListAsync = "api/GetShippingBoxListAsync";
        }

        public static class WarehouseShipment
        {
            public const string BasePath = "api/WarehouseShipment";
            public const string GetAsync = "GetAsync/{id}";
            public const string SearchAsync = "SearchAsync";
            public const string CreateAsync = "CreateAsync";
            public const string UpdateAsync = "UpdateAsync";
            public const string DeleteAsync = "DeleteAsync/{id}";
            public const string ConfirmShipmentAsync = "ConfirmShipmentAsync/{id}";
            public const string CreatePickingAsync = "CreatePickingAsync";
            public const string CheckMultipleShipmentCreatePicking = "CheckMultipleShipmentCreatePicking";
            public const string GetDeliveryNote = "GetDeliveryNote/{id}";
            public const string GetDeliveryNotes = "GetDeliveryNotes";
            public const string CompletedShipmentAsync = "CompletedShipmentAsync/{id}";
            public const string CreatePickingManualAsync = "CreatePickingManualAsync";

            public const string SearchMovementAsync = "SearchMovementAsync";
            public const string CreateMovementAsync = "CreateMovementAsync";
            public const string UpdateMovementAsync = "UpdateMovementAsync";
            public const string ShipMovementAsync = "ShipMovementAsync/{id}";
            public const string DHLPickup = "DHLPickup";

            public const string GetProductByShipmentNoAsync = "get-product-by-shipment";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{shipmentNo}";
            public const string CompletedShipments = "completed-shipments";

            public const string GetShippingReport = "get-shipping-report";
            public const string GenerateShipmentFromOrder = "generate-shipment-from-order/{tenantIds}";

            public const string AutoCreatePickingAsync = "AutoCreatePickingAsync/{remarks}/{tenantIds}";
        }
        public static class WarehouseShipmentLine
        {
            public const string BasePath = "api/WarehouseShipmentLine";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync";
            public const string GetByProducCodetAsync = "GetByProducCodetAsync/{productCode}";
        }
        public static class WarehousePickingList
        {
            public const string BasePath = "api/WarehousePickingList";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{pickNo}";
            public const string GetWarehousePickingDTOAsync = "GetWarehousePickingDTOAsync";
            public const string DeletePickingAsync = "DeletePickingAsync/{pickNo}";
            public const string CompletePickingAsync = "CompletePickingAsync/{pickNo}";
            public const string SyncToHTAsync = "SyncToHTAsync";
            public const string GetBarcodeDataAsync = "GetBarcodeDataAsync/{No}";
            public const string GetDataCoverSheetNDeliveryNote = "GetDataCoverSheetNDeliveryNote";
            public const string CompletedPickings = "CompletedPickings";
            public const string AutoCompletedPickings = "complete-pickings";
        }
        public static class WarehousePickingLine
        {
            public const string BasePath = "api/WarehousePickingLine";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{pickNo}";
            public const string GetPickingLineDTOAsync = "GetPickingLineDTOAsync/{pickNo}";
            public const string GetShipmentsByPickAsync = "GetShipmentsByPickAsync/{pickNo}";
            public const string UpdateWarehousePickingLinesAsync = "UpdateWarehousePickingLinesAsync";
        }
        public static class WarehousePickingStaging
        {
            public const string BasePath = "api/WarehousePickingStaging";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{pickNo}";
        }
        public static class PackingList
        {
            public const string BasePath = "api/PackingList";
            public const string GetDataMasterAsync = "GetDataMasterAsync";
            public const string UpdatePackedQtyAsync = "UpdatePackedQtyAsync";
            public const string CompletePackingAsync = "CompletePackingAsync";
            public const string GetAllInventoryInfo = "GetAllInventoryInfo";
            public const string GetDataMasterByTrackingNoAsync = "GetDataMasterByTrackingNoAsync/{trackingNo}";
            public const string GetDataMasterByShipmentNoAsync = "GetDataMasterByShipmentNoAsync/{shipmentNo}";
            public const string GetPdfAsBase64Async = "GetPdfAsBase64Async/{filePath}";
            public const string GetPackingLabelFilePathAsync = "GetPackingLabelFilePath/{trackingNo}";
            public const string UpdateOrderDispatchesFilePathAsync = "UpdateOrderDispatchesFilePathAsync";
        }
        public static class ShippingBox
        {
            public const string BasePath = "api/ShippingBox";
            public const string GetByShippingCarrierCodeAsync = "GetByShippingCarrierCode/{shippingCarrierCode}";
            public const string GetAllWithCarrierAsync = "get-all-with-shipping-carrier";
            public const string GetAllShippingCarrierAsync = "get-all-shipping-carrier-for-box";
        }
        public static class ShippingCarier
        {
            public const string BasePath = "api/ShippingCarrier";
        }

        public static class Categories
        {
            public const string BasePath = "api/categories";
            public const string GetUsers = "api/categories/users";
            public const string GetBinByLocationId = "api/categories/bin-by-locationid";
            public const string GetOrders = "api/get-orders";
        }

        public static class ReturnOrder
        {
            public const string BasePath = "api/ReturnOrder";
            public const string GetAllReturnOrdersAsync = "get-all-return-orders";
            public const string GetReturnOrderByReturnNoAsync = "get-return-order-by-return-no";
            public const string InsertReturnOrderAsync = "insert-return-order";
            public const string UpdateReturnOrderAsync = "update-return-order";
            public const string DeleteReturnOrderAsync = "delete-return-order/{id}";
            public const string SearchReturnOrder = "search-return-order";
            public const string GetReturnByShipmentNo = "GetReturnByShipmentNo";
        }

        public static class InventTransfer
        {
            public const string BasePath = "api/InventTransfer";
            public const string GetAllDTO = "GetAllDTO";
            public const string GetByIdDTO = "GetByIdDTO/{id}";
            public const string GetByTransferNoDTO = "GetByTransferNoDTO/{transferNo}";
            public const string CompletedTransfer = "CompletedTransfer/{id}";
        }

        public static class InventTransferLines
        {
            public const string BasePath = "api/InventTransferLine";
        }

        public static class WarehouseParameter
        {
            public const string BasePath = "api/WarehouseParameter";
            public const string GetFirstOrDefault = "FirstOrDefault";
        }

        public static class InventAdjustment
        {
            public const string BasePath = "api/InventAdjustment";
            public const string GetAllDTO = "GetAllDTO";
            public const string GetByIdDTO = "GetByIdDTO/{id}";
            public const string GetByAdjustmentNoDTO = "GetByTransferNoDTO/{adjustmentNo}";
            public const string CompletedAdjustment = "CompletedAdjustment/{id}";
            public const string DeleteAllAdjustmentLineAsync = "DeleteAllAdjustmentLineAsync/{id}";

        }

        public static class InventAdjustmentLines
        {
            public const string BasePath = "api/InventAdjustmentLine";
        }

        public static class InventBundleLines
        {
            public const string BasePath = "api/InventBundleLine";
            public const string GetProductsByTransNo = "GetProductsByTransNo/{TransNo}";
            public const string GetProductsByBundleCode = "GetProductsByBundleCode/{BundleCode}";


        }

        public static class InventBundle
        {
            public const string BasePath = "api/InventBundle";
            public const string GetAllDTO = "GetAllDTO";
            public const string GetByIdDTO = "GetByIdDTO/{id}";
            public const string GetByTransNoDTOAsync = "GetByTransNoDTOAsync/{transNo}";
            public const string GetAllMasterByBundleCode = "GetAllMasterByBundleCode/{ProductBundleCode}";
            public const string CompletedBundle = "CompletedBundle";
            public const string CreatePutawayAsync = "CreatePutawayAsync/{id}";
            public const string UploadFromHandheldAsync = "UploadFromHandheldAsync";
            public const string DeleteAllBundleLineAsync = "DeleteAllBundleLineAsync/{id}";

            
        }

        public static class InventStockTake
        {
            public const string BasePath = "api/InventStockTake";
            public const string GetAll = "get-all";
            public const string GetByStockTakeNo = "/get-by-stock-take-no";
            public const string Insert = "insert-async";
            public const string Update = "update-async";
            public const string Delete = "delete-async";
            public const string GetStockTakeAsync = "GetStockTakeAsync";
            public const string CompleteInventStockTake = "complete-invent-stock-take";
            public const string GetStockTakeLineByIdAsync = "GetStockTakeLineByIdAsync/{StockTakeNo}";
            public const string CheckProductExistenceinStocktake = "CheckProductExistenceinStocktake";
            public const string UpdateLine = "UpdateLine";
            public const string RemoveLine = "RemoveLine";
            public const string GetNewStocktakeNo = "GetNewStocktakeNo";
            public const string GetCurrentUserAsync = "GetCurrentUserAsync";
        }

        public static class InventStockTakeRecording
        {
            public const string BasePath = "api/InventStockTakeRecording";
            public const string GetAllDTO = "GetAllDTO";
            public const string GetByIdDTO = "GetByIdDTO/{id}";
            public const string GetListByStockTakeNoDTOAsync = "GetListByStockTakeNoDTOAsync/{StockTakeNo}";
            public const string GetStockTakeRecordingAsync = "GetStockTakeRecordingAsync";
            public const string CreateStockTakeRecordingAsync = "CreateStockTakeRecordingAsync";
            public const string DeleteStockTakeRecordingAsync = "DeleteStockTakeRecordingAsync/{StockTakeRecordingId}";
            public const string CompleteStockTakeRecordingAsync = "CompleteStockTakeRecordingAsync/{StockTakeRecordingId}";
            // lines
            public const string GetLineByStockTakeRecordingIdAsync = "GetLineByStockTakeRecordingIdAsync/{StockTakeRecordingId}";
            public const string GetLineByStockTakeNoDTOAsync = "GetLineByStockTakeNoDTOAsync/{StockTakeNo}";
            public const string UpdateStockTakeRecordingLinesAsync = "UpdateStockTakeRecordingLinesAsync";
        }

        public static class Tasks
        {
            public const string BasePath = "api/Tasks";
        }

        public static class ImageStorage
        {
            public const string BasePath = "api/ImageStorage";
            public const string GetByResourceIdAndTypeAsync = "GetByResourceIdAndTypeAsync";
        }

        public static class CompanyTenant
        {
            public const string BasePath = "api/CompanyTenant";
        }
        
        public static class Order
        {
            public const string BasePath = "api/Order";
            public const string GetOrderReport = "get-order-report";
        }
        
        public static class TaskModel
        {
            public const string BasePath = "api/TaskModel";
            public const string GetTaskReport = "get-task-report";
        }

        public static class Common
        {
            public const string BasePath = "api/Common";
            public const string UpdateHHTStatusAsync = "UpdateHHTStatusAsync";
            public const string GetPdfAsBase64Async = "GetPdfAsBase64Async";
            public const string GetLaterPaymentAsync = "GetLaterPaymentAsync";
            public const string GetDataForExportCsvAsync = "GetDataForExportCsvAsync";
            public const string ImportCsvAsync1 = "ImportCsvAsync1";
            public const string ImportCsvAsync = "ImportCsvAsync";
        }
        public static class LaterPayment
        {
            public const string BasePath = "webapi/servlet";
            public const string GetLaterPayment = "WEBAPI";
            public const string GetPdfAsBase64FromDownloadUrlAsync = "DOWNLOAD";
        }


        public static class CountryMaster
        {
            public const string BasePath = "api/CountryMaster";
        }
    }
}
