using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option)
        {

        }

        public string GetConnectionString()
        {
            return this.Database.GetDbConnection().ConnectionString;
        }

        #region common
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApiCode> ApiCodes { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<ChannelMaster> ChannelMasters { get; set; }
        public DbSet<CompanyTenant> Companies { get; set; }
        public DbSet<CountryMaster> CountryMasters { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrencyPairSetting> CurrencyPairSettings { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<OrderDispatch> OrderDispatches { get; set; }
        public DbSet<OrderDispatchProduct> OrderDispatchProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        //public DbSet<OrderReturnItem> OrderReturnItems { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<ProductBundle> ProductBundles { get; set; }
        public DbSet<ProductJanCode> ProductJanCodes { get; set; }
        public DbSet<ProductStatus> ProductStatuses { get; set; }
        public DbSet<ProductStock> ProductStocks { get; set; }

        public DbSet<SalesData> SalesDatas { get; set; }
        public DbSet<ShippingCountry> ShippingCountries { get; set; }
        public DbSet<SystemClassCompany> SystemClassCompanies { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }
        public DbSet<UserVendor> UserVendors { get; set; }
        public DbSet<TaskModel> TaskModels { get; set; }
        public DbSet<TaskChatHistory> TaskChatHistories { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        //public DbSet<WarehouseUserSetting> WarehouseUserSettings { get; set; }

        public DbSet<VendorBilling> VendorBillings { get; set; }
        public DbSet<VendorBillingDetail> VendorBillingDetails { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ArrivalInstruction> ArrivalInstructions { get; set; }
        public DbSet<ArrivalInstructionDetail> ArrivalInstructionDetails { get; set; }
        #endregion

        #region authp
        public DbSet<TenantAuth> TenantAuth { get; set; }
        #endregion

        #region WMS
        public DbSet<MstUserSetting> MstUserSettings { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<RefreshTokens> RefreshTokens { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<PermissionsTenant> PermissionsTenants { get; set; }
        public DbSet<RoleToPermission> RoleToPermissions { get; set; }
        public DbSet<RoleToPermissionTenant> RoleToPermissionTenants { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Bin> Bins { get; set; }
        //public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<UserToTenant> UserToTenants { get; set; }

        public DbSet<WarehouseTran> WarehouseTrans { get; set; }
        public DbSet<WarehousePutAway> WarehousePutAways { get; set; }
        public DbSet<WarehousePutAwayLine> WarehousePutAwayLines { get; set; }
        public DbSet<WarehousePutAwayStaging> WarehousePutAwayStagings { get; set; }
        public DbSet<WarehouseReceiptOrder> WarehouseReceiptOrders { get; set; }
        public DbSet<WarehouseReceiptOrderLine> WarehouseReceiptOrderLines { get; set; }
        public DbSet<WarehouseReceiptStaging> WarehouseReceiptStagings { get; set; }
        public DbSet<NumberSequences> SequencesNumber { get; set; }
        public DbSet<Batches> Batches { get; set; }
        public DbSet<LogTime> LogTimes { get; set; }
        public DbSet<InventTransfer> InventTransfers { get; set; }
        public DbSet<InventTransferLine> InventTransferLines { get; set; }
        public DbSet<InventAdjustment> InventAdjustments { get; set; }
        public DbSet<InventAdjustmentLine> InventAdjustmentLines { get; set; }
        public DbSet<InventBundle> InventBundles { get; set; }
        public DbSet<InventBundleLine> InventBundleLines { get; set; }
        public DbSet<InventStockTake> InventStockTakes { get; set; }
        public DbSet<InventStockTakeLine> InventStockTakeLines { get; set; }
        public DbSet<InventStockTakeRecording> InventStockTakeRecordings { get; set; }
        public DbSet<InventStockTakeRecordingLine> InventStockTakeRecordingLines { get; set; }

        #region Outbound
        public DbSet<ReturnOrder> ReturnOrders { get; set; }
        public DbSet<ReturnOrderLine> ReturnOrderLines { get; set; }
        public DbSet<ShippingBox> ShippingBoxes { get; set; }
        public DbSet<ShippingCarrier> ShippingCarriers { get; set; }
        public DbSet<WarehousePickingLine> WarehousePickingLines { get; set; }
        public DbSet<WarehousePickingList> WarehousePickingLists { get; set; }
        public DbSet<WarehousePickingStaging> WarehousePickingStagings { get; set; }
        public DbSet<WarehouseShipment> WarehouseShipments { get; set; }
        public DbSet<WarehouseShipmentLine> WarehouseShipmentLines { get; set; }
        public DbSet<WarehouseParameter> WarehouseParameters { get; set; }
        #endregion

        public DbSet<ImageStorage> ImageStorages { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region DB Chinh ko migration
            // Ánh xạ bảng "Orders" tới schema "sales"
            //modelBuilder.Entity<PermissionsListModel>().ToTable("PermissionsListModels", "dbo", x => x.ExcludeFromMigrations());//ko cho migration cac bang hien co cua FBT_DEV

            modelBuilder.Entity<TenantAuth>()
               .ToTable("Tenants", "authp");
            #endregion

            foreach (var item in modelBuilder.Model.GetEntityTypes())
            {
                if (item.Name.Contains("RoleToPermissionTenant"))
                {
                    var a = item.Name;
                }

                if (!string.IsNullOrEmpty(item.ClrType.Namespace))
                {
                    if (item.ClrType.Namespace.Contains("WMS"))
                    {
                        item.SetSchema("wms");
                        continue;
                    }
                }
            }

            //override lai cac bang identity
            //modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "wms", x => x.ExcludeFromMigrations());
            //modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles", "wms", x => x.ExcludeFromMigrations());
            //modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "wms", x => x.ExcludeFromMigrations());
            //modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "wms", x => x.ExcludeFromMigrations());
            //modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "wms", x => x.ExcludeFromMigrations());
            //modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "wms", x => x.ExcludeFromMigrations());

            //modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers", "wms", x => x.ExcludeFromMigrations());

            modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers", "wms");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "wms");
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles", "wms");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "wms");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "wms");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "wms");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "wms");

            // Ghi đè bảng __EFMigrationsHistory
            //modelBuilder.HasAnnotation("Relational:Schema", "wms", x => x.ExcludeFromMigrations());  // Đặt schema tùy chỉnh
            // modelBuilder.HasAnnotation("Relational:HistoryTableName", "__EFMigrationsHistoryWMS");  // Đặt tên bảng tùy chỉnh
        }
    }
}
