USE [master]
GO
/****** Object:  Database [FBT_DEV2]    Script Date: 24/02/2025 6:41:33 am ******/
CREATE DATABASE [FBT_DEV2]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'FBT_DEV2', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\FBT_DEV2.mdf' , SIZE = 139264KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'FBT_DEV2_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\FBT_DEV2_log.ldf' , SIZE = 729088KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [FBT_DEV2] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [FBT_DEV2].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [FBT_DEV2] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [FBT_DEV2] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [FBT_DEV2] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [FBT_DEV2] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [FBT_DEV2] SET ARITHABORT OFF 
GO
ALTER DATABASE [FBT_DEV2] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [FBT_DEV2] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [FBT_DEV2] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [FBT_DEV2] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [FBT_DEV2] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [FBT_DEV2] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [FBT_DEV2] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [FBT_DEV2] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [FBT_DEV2] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [FBT_DEV2] SET  ENABLE_BROKER 
GO
ALTER DATABASE [FBT_DEV2] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [FBT_DEV2] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [FBT_DEV2] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [FBT_DEV2] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [FBT_DEV2] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [FBT_DEV2] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [FBT_DEV2] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [FBT_DEV2] SET RECOVERY FULL 
GO
ALTER DATABASE [FBT_DEV2] SET  MULTI_USER 
GO
ALTER DATABASE [FBT_DEV2] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [FBT_DEV2] SET DB_CHAINING OFF 
GO
ALTER DATABASE [FBT_DEV2] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [FBT_DEV2] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [FBT_DEV2] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [FBT_DEV2] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [FBT_DEV2] SET QUERY_STORE = OFF
GO
USE [FBT_DEV2]
GO
/****** Object:  Schema [authp]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE SCHEMA [authp]
GO
/****** Object:  Schema [wms]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE SCHEMA [wms]
GO
/****** Object:  Table [wms].[WarehouseTrans]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehouseTrans](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductCode] [nvarchar](max) NULL,
	[Qty] [float] NOT NULL,
	[DatePhysical] [date] NULL,
	[TransType] [int] NOT NULL,
	[TransNumber] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[Bin] [nvarchar](max) NULL,
	[LotNo] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
	[StatusIssue] [int] NULL,
	[StatusReceipt] [int] NULL,
	[TransId] [uniqueidentifier] NULL,
	[TransLineId] [uniqueidentifier] NULL,
	[Status] [int] NOT NULL,
	[TenantId] [int] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_WarehouseTrans] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductCode] [nvarchar](100) NULL,
	[CompanyId] [int] NOT NULL,
	[ProductShortCode] [nvarchar](max) NULL,
	[VendorId] [int] NOT NULL,
	[SupplierId] [int] NOT NULL,
	[ProductType] [int] NOT NULL,
	[SaleProductCode] [nvarchar](max) NULL,
	[SaleProductName] [nvarchar](max) NULL,
	[ProductName] [nvarchar](max) NULL,
	[ProductEname] [nvarchar](max) NULL,
	[ProductIname] [nvarchar](max) NULL,
	[CategoryId] [int] NOT NULL,
	[ProductModelNumber] [nvarchar](max) NULL,
	[ProductImageName] [nvarchar](max) NULL,
	[ProductImageUrl] [nvarchar](max) NULL,
	[StockAvailableQuanitty] [int] NOT NULL,
	[UnitId] [int] NOT NULL,
	[CurrencyCode] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[HsCode] [nvarchar](max) NULL,
	[JanCode] [nvarchar](max) NULL,
	[Net] [float] NULL,
	[Weight] [float] NULL,
	[Height] [float] NULL,
	[Length] [float] NULL,
	[Depth] [float] NULL,
	[BaseCost] [float] NULL,
	[BaseCostOther] [float] NULL,
	[RegularPrice] [float] NULL,
	[Currency] [nvarchar](max) NULL,
	[CountryOfOrigin] [nvarchar](max) NULL,
	[ProductStatus] [int] NOT NULL,
	[RegistrationDate] [datetime2](7) NULL,
	[Remark] [nvarchar](max) NULL,
	[InventoryMethod] [nvarchar](max) NULL,
	[ShippingLimitDays] [int] NOT NULL,
	[FromApplyPreBundles] [datetime2](7) NULL,
	[ToApplyPreBundles] [datetime2](7) NULL,
	[StockReceiptProcessInstruction] [nvarchar](max) NULL,
	[StockThreshold] [int] NULL,
	[IndividuallyShippedItem] [bit] NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[MakerManagementCode] [nvarchar](max) NULL,
	[ProductShortName] [nvarchar](max) NULL,
	[ProductUrl] [nvarchar](max) NULL,
	[ShopifyAdminGraphqlApiId] [nvarchar](max) NULL,
	[ShopifyInventoryItemId] [nvarchar](max) NULL,
	[ShopifyLocationId] [nvarchar](max) NULL,
	[StandardPrice] [float] NULL,
	[VendorProductName] [nvarchar](max) NULL,
	[WarehouseProcessingFlag] [bit] NULL,
	[Width] [float] NULL,
	[IsFdaRegistration] [bit] NULL,
	[IsAttachedItem] [bit] NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[view_wmsGetInventoryInfo]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[view_wmsGetInventoryInfo] AS
with cteStock as (
	SELECT ProductCode
		,TenantId
		,sum(Qty) InventoryStock
	FROM [FBT_DEV2].[wms].WarehouseTrans
	where 
		DatePhysical is not null
		--and ProductCode='CLOTH-001'
		--and (TransNumber = 'WHR000085' or TransNumber = 'WHP000036')
	group by ProductCode,TenantId
)
,cteOnOrder as (
	SELECT ProductCode
		,TenantId
		,sum(Qty) OnOrder
	FROM [FBT_DEV2].[wms].WarehouseTrans
	where 
		DatePhysical is null
		and TransType = 1 --Shipment
		--and ProductCode='CLOTH-001'
		--and (TransNumber = 'WHR000085' or TransNumber = 'WHP000036')
		--and 
	group by ProductCode,TenantId
)

select
	_cteStock.InventoryStock
	,abs(isnull(_cteOnOrder.OnOrder,0)) OnOrder
	,_cteStock.InventoryStock - abs(ISNULL(_cteOnOrder.OnOrder,0)) InventoryAvailable
	,_pro.*
from cteStock _cteStock
	left join cteOnOrder _cteOnOrder on _cteOnOrder.ProductCode = _cteStock.ProductCode and _cteOnOrder.TenantId = _cteStock.TenantId
	left join dbo.products _pro on _pro.ProductCode = _cteStock.ProductCode  and _pro.CompanyId = _cteStock.TenantId
GO
/****** Object:  Table [wms].[Batches]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[Batches](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductCode] [nvarchar](max) NULL,
	[TenantId] [int] NOT NULL,
	[LotNo] [nvarchar](max) NULL,
	[ManufacturingDate] [date] NULL,
	[ExpirationDate] [date] NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Batches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[Locations]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[Locations](
	[Id] [uniqueidentifier] NOT NULL,
	[LocationCD] [nvarchar](max) NULL,
	[LocationName] [nvarchar](max) NULL,
	[Abbreviation] [nvarchar](max) NULL,
	[Type] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Phone] [nvarchar](max) NULL,
	[Fax] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[view_wmsGetInventoryInfoFlowBinLot]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[view_wmsGetInventoryInfoFlowBinLot] AS
	with cteStock as (
		SELECT ProductCode
			,LotNo
			,[Location]
			,Bin
			,TenantId
			,sum(Qty) InventoryStock
		FROM [FBT_DEV2].[wms].WarehouseTrans
		where 
			DatePhysical is not null
		group by ProductCode,LotNo,[Location],Bin,TenantId
	 )
	 ,cteOnOrder as (
		SELECT ProductCode
			,LotNo
			,[Location]
			,Bin
			,TenantId
			,sum(Qty) OnOrder
		FROM [FBT_DEV2].[wms].WarehouseTrans
		where 
			DatePhysical is null
			and TransType = 1 --Shipment
		group by ProductCode,LotNo,[Location],Bin,TenantId
	 )

	 select _cteStock.LotNo
			,_location.LocationName
			,_cteStock.Bin BinCode
			,_batches.ExpirationDate Expired
			,_cteStock.InventoryStock
			,abs(isnull(_cteOnOrder.OnOrder,0)) OnOrder
			,_cteStock.InventoryStock - abs(ISNULL(_cteOnOrder.OnOrder,0)) AvailableStock
			,_cteStock.[Location]
			,_cteStock.TenantId
			,_pro.*
	 from cteStock _cteStock
		left join cteOnOrder _cteOnOrder on _cteOnOrder.ProductCode = _cteStock.ProductCode and _cteOnOrder.Bin = _cteStock.Bin and _cteOnOrder.LotNo = _cteStock.LotNo
		left join dbo.products _pro on _pro.ProductCode = _cteStock.ProductCode 
		left join wms.[Batches] _batches on _batches.ProductCode = _cteStock.ProductCode and _batches.LotNo = _cteStock . LotNo
		left join wms.Locations _location on _location.Id = TRY_CONVERT(uniqueidentifier, _cteStock.Location)
GO
/****** Object:  View [dbo].[view_wmsGetInventoryInfoFlowBinLotTenant]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE VIEW [dbo].[view_wmsGetInventoryInfoFlowBinLotTenant] AS
	with cteStock as (
		SELECT ProductCode
			,LotNo
			,[Location]
			,Bin
			,TenantId
			,sum(Qty) InventoryStock
		FROM [FBT_DEV2].[wms].WarehouseTrans
		where 
			DatePhysical is not null
			--and ProductCode in ('A_TestInventory1','A_TestInventory2')
		group by ProductCode,LotNo,[Location],Bin,TenantId
	 )
	 ,cteOnOrder as (
		SELECT ProductCode
			,LotNo
			,[Location]
			,Bin
			,TenantId
			,sum(Qty) OnOrder
		FROM [FBT_DEV2].[wms].WarehouseTrans
		where 
			DatePhysical is null
			and TransType = 1 --Shipment
			--and ProductCode in ('A_TestInventory1','A_TestInventory2')
		group by ProductCode,LotNo,[Location],Bin,TenantId
	 )

	 select _cteStock.LotNo
			,_location.LocationName
			,_cteStock.Bin BinCode
			,_batches.ExpirationDate Expired
			,_cteStock.InventoryStock
			,abs(isnull(_cteOnOrder.OnOrder,0)) OnOrder
			,_cteStock.InventoryStock - abs(ISNULL(_cteOnOrder.OnOrder,0)) AvailableStock
			,_cteStock.[Location]
			,_cteStock.TenantId
			--,_comp.FullName
			,_pro.*
	 from cteStock _cteStock
		left join cteOnOrder _cteOnOrder on _cteOnOrder.ProductCode = _cteStock.ProductCode and _cteOnOrder.Bin = _cteStock.Bin and _cteOnOrder.LotNo = _cteStock.LotNo
			and _cteStock.TenantId=_cteOnOrder.TenantId
		left join dbo.products _pro on _pro.ProductCode = _cteStock.ProductCode 
			and _cteStock.TenantId=_pro.CompanyId
		left join wms.[Batches] _batches on _batches.ProductCode = _cteStock.ProductCode and _batches.LotNo = _cteStock . LotNo
			and _cteStock.TenantId=_batches.TenantId
		left join wms.Locations _location on _location.Id = TRY_CONVERT(uniqueidentifier, _cteStock.Location)
		--left join Companies _comp on _comp.AuthPTenantId = _cteStock.TenantId
	--where _cteStock.ProductCode	in ('A_TestInventory1','A_TestInventory2')
GO
/****** Object:  Table [authp].[AuthUsers]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [authp].[AuthUsers](
	[UserId] [nvarchar](256) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[UserName] [nvarchar](128) NULL,
	[TenantId] [int] NULL,
	[ConcurrencyToken] [timestamp] NULL,
	[IsDisabled] [bit] NOT NULL,
 CONSTRAINT [PK_AuthUsers] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [authp].[RefreshTokens]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [authp].[RefreshTokens](
	[TokenValue] [varchar](50) NOT NULL,
	[UserId] [nvarchar](256) NOT NULL,
	[JwtId] [nvarchar](max) NULL,
	[IsInvalid] [bit] NOT NULL,
	[AddedDateUtc] [datetime2](7) NOT NULL,
	[ConcurrencyToken] [timestamp] NULL,
 CONSTRAINT [PK_RefreshTokens] PRIMARY KEY CLUSTERED 
(
	[TokenValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [authp].[RoleToPermissions]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [authp].[RoleToPermissions](
	[RoleName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[PackedPermissionsInRole] [nvarchar](max) NOT NULL,
	[ConcurrencyToken] [timestamp] NULL,
	[RoleType] [tinyint] NOT NULL,
 CONSTRAINT [PK_RoleToPermissions] PRIMARY KEY CLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [authp].[RoleToPermissionsTenant]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [authp].[RoleToPermissionsTenant](
	[TenantRolesRoleName] [nvarchar](100) NOT NULL,
	[TenantsTenantId] [int] NOT NULL,
	[ConcurrencyToken] [timestamp] NULL,
 CONSTRAINT [PK_RoleToPermissionsTenant] PRIMARY KEY CLUSTERED 
(
	[TenantRolesRoleName] ASC,
	[TenantsTenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [authp].[Tenants]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [authp].[Tenants](
	[TenantId] [int] IDENTITY(1,1) NOT NULL,
	[ParentDataKey] [varchar](250) NULL,
	[TenantFullName] [nvarchar](400) NOT NULL,
	[IsHierarchical] [bit] NOT NULL,
	[ParentTenantId] [int] NULL,
	[ConcurrencyToken] [timestamp] NULL,
	[DatabaseInfoName] [nvarchar](max) NULL,
	[HasOwnDb] [bit] NOT NULL,
 CONSTRAINT [PK_Tenants] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [authp].[UserToRoles]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [authp].[UserToRoles](
	[UserId] [nvarchar](256) NOT NULL,
	[RoleName] [nvarchar](100) NOT NULL,
	[ConcurrencyToken] [timestamp] NULL,
 CONSTRAINT [PK_UserToRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[__AppDbContextMigrationHistory]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__AppDbContextMigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___AppDbContextMigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[__AuthPermissionsMigrationHistory]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__AuthPermissionsMigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___AuthPermissionsMigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApiCodes]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApiCodes](
	[Code] [nvarchar](450) NOT NULL,
	[Message] [nvarchar](max) NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ApiCodes] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApiLogs]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApiLogs](
	[Id] [uniqueidentifier] NOT NULL,
	[CompanyId] [int] NOT NULL,
	[OrderIds] [nvarchar](max) NULL,
	[CourierCode] [nvarchar](max) NULL,
	[ApiEndpoint] [nvarchar](max) NULL,
	[RequestData] [nvarchar](max) NULL,
	[ResponseData] [nvarchar](max) NULL,
	[StatusCode] [int] NULL,
	[ErrorMessage] [nvarchar](max) NULL,
	[RequestTimestamp] [datetime2](7) NOT NULL,
	[ResponseTimestamp] [datetime2](7) NULL,
	[DurationMs] [int] NULL,
	[IsSuccessful] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ApiLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ArrivalInstructions]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ArrivalInstructions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[ScheduledArrivalDate] [datetime2](7) NOT NULL,
	[ProductCode] [nvarchar](max) NULL,
	[Quantity] [int] NOT NULL,
	[OrderNumber] [nvarchar](max) NULL,
	[OrderDate] [datetime2](7) NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ArrivalInstructions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AttachedItemCategories]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttachedItemCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AttachedItemId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[DataKey] [nvarchar](450) NULL,
	[CompanyId] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_AttachedItemCategories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AttachedItemChannels]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttachedItemChannels](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AttachedItemId] [int] NOT NULL,
	[ChannelMasterCode] [nvarchar](max) NULL,
	[DataKey] [nvarchar](450) NULL,
	[CompanyId] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_AttachedItemChannels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AttachedItemDetails]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttachedItemDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AttachedItemId] [int] NOT NULL,
	[ProductCode] [nvarchar](max) NULL,
	[DataKey] [nvarchar](450) NULL,
	[CompanyId] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_AttachedItemDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AttachedItemProductTypes]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttachedItemProductTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AttachedItemId] [int] NOT NULL,
	[ProductTypeId] [int] NOT NULL,
	[DataKey] [nvarchar](450) NULL,
	[CompanyId] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_AttachedItemProductTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AttachedItems]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttachedItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AttachedItemPriority] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[StartDate] [datetime2](7) NULL,
	[EndDate] [datetime2](7) NULL,
	[IsApplyAllTime] [bit] NOT NULL,
	[IsApplyAllChannels] [bit] NOT NULL,
	[IsApplyAllProductTypes] [bit] NOT NULL,
	[IsApplyAllCategories] [bit] NOT NULL,
	[DataKey] [nvarchar](450) NULL,
	[CompanyId] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_AttachedItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BatchCalendars]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BatchCalendars](
	[Date] [nvarchar](450) NOT NULL,
	[ScheduleType] [nvarchar](1) NOT NULL,
 CONSTRAINT [PK_BatchCalendars] PRIMARY KEY CLUSTERED 
(
	[Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Batches]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Batches](
	[BatchId] [nvarchar](450) NOT NULL,
	[BatchName] [nvarchar](max) NULL,
	[ExecFile] [nvarchar](max) NULL,
	[StartupStatus] [bit] NOT NULL,
	[Args] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Batches] PRIMARY KEY CLUSTERED 
(
	[BatchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BatchSchedules]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BatchSchedules](
	[BatchScheduleId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NULL,
	[BatchId] [varchar](50) NOT NULL,
	[StartTime] [time](7) NOT NULL,
	[EndTime] [time](7) NULL,
	[ScheduleDivision] [varchar](2) NOT NULL,
	[ScheduleTime] [time](7) NULL,
	[CreateOperatorId] [varchar](10) NULL,
	[CreateAt] [datetime] NOT NULL,
	[UpdateOperatorId] [varchar](10) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[ScheduleType] [varchar](1) NULL,
	[DayOfWeek] [varchar](1) NULL,
	[DayOfMonth] [nvarchar](2) NULL,
 CONSTRAINT [PK_BacthSchedulers] PRIMARY KEY CLUSTERED 
(
	[BatchScheduleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BatchSchedulesNew]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BatchSchedulesNew](
	[BatchScheduleId] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NULL,
	[BatchId] [varchar](50) NOT NULL,
	[StartTime] [time](7) NOT NULL,
	[EndTime] [time](7) NULL,
	[ScheduleDivision] [varchar](2) NOT NULL,
	[ScheduleTime] [time](7) NULL,
	[CreateOperatorId] [varchar](10) NULL,
	[CreateAt] [datetime] NOT NULL,
	[UpdateOperatorId] [varchar](10) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[ScheduleType] [varchar](1) NULL,
	[DayOfWeek] [varchar](1) NULL,
	[DayOfMonth] [nvarchar](2) NULL,
 CONSTRAINT [PK_BacthSchedulersNew] PRIMARY KEY CLUSTERED 
(
	[BatchScheduleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChannelMasters]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChannelMasters](
	[ChannelMasterCode] [nvarchar](450) NOT NULL,
	[ChannelMasterName] [nvarchar](max) NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ChannelMasters] PRIMARY KEY CLUSTERED 
(
	[ChannelMasterCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Channels]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Channels](
	[ChannelCode] [nvarchar](450) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[ChannelName] [nvarchar](max) NULL,
	[ChannelMasterCode] [nvarchar](450) NULL,
	[ChannelMasterName] [nvarchar](max) NULL,
	[PaymentMethod] [nvarchar](max) NULL,
	[BillCalcType] [nvarchar](max) NULL,
	[Currency] [nvarchar](max) NULL,
	[LanguageCode] [nvarchar](max) NULL,
	[IsActive] [bit] NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Channels] PRIMARY KEY CLUSTERED 
(
	[ChannelCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Companies]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Companies](
	[CompanyTenantId] [uniqueidentifier] NOT NULL,
	[FullName] [nvarchar](max) NULL,
	[ShortName] [nvarchar](max) NULL,
	[DataKey] [nvarchar](450) NULL,
	[AuthPTenantId] [int] NOT NULL,
	[IsDeleted] [bit] NULL,
	[CompanyName] [nvarchar](max) NULL,
	[ContactEmailAddress] [nvarchar](max) NULL,
	[DeliveryNoteMessage] [nvarchar](max) NULL,
	[LogoImage] [nvarchar](max) NULL,
	[ShopName] [nvarchar](max) NULL,
	[SiteAddress] [nvarchar](max) NULL,
	[SiteAddressName] [nvarchar](max) NULL,
 CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED 
(
	[CompanyTenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CountryMasters]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CountryMasters](
	[Id] [int] NOT NULL,
	[CountryIsoNumeric] [nvarchar](max) NULL,
	[CountryIso2] [nvarchar](max) NULL,
	[CountryIso3] [nvarchar](max) NULL,
	[CountryNameEn] [nvarchar](max) NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_CountryMasters] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CourierApis]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CourierApis](
	[CompanyId] [int] NOT NULL,
	[CourierCd] [nvarchar](450) NOT NULL,
	[ApiParam] [nvarchar](450) NOT NULL,
	[ApiValue] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_CourierApis] PRIMARY KEY CLUSTERED 
(
	[CompanyId] ASC,
	[CourierCd] ASC,
	[ApiParam] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Currencies]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Currencies](
	[CurrencyCode] [nvarchar](450) NOT NULL,
	[Country] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[IsDisplayCurrency] [bit] NOT NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Currencies] PRIMARY KEY CLUSTERED 
(
	[CurrencyCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CurrencyPairSettings]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CurrencyPairSettings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CurrencyCodeFrom] [nvarchar](max) NULL,
	[CurrencyCodeTo] [nvarchar](max) NULL,
	[RateDecimalPlaces] [int] NOT NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_CurrencyPairSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeliveryCompanies]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeliveryCompanies](
	[DeliveryCompanyId] [nvarchar](450) NOT NULL,
	[DeliveryCompanyCode] [nvarchar](max) NULL,
	[DeliveryCompanyName] [nvarchar](max) NULL,
	[ParentDeliveryCompanyCode] [nvarchar](max) NULL,
	[ParentDeliveryCompanyName] [nvarchar](max) NULL,
	[UsingApi] [bit] NOT NULL,
	[WeightMin] [int] NULL,
	[WeightMax] [int] NULL,
	[ServiceLevel] [nvarchar](max) NULL,
	[MobileRequirement] [nvarchar](max) NULL,
	[ChCustomerRequirement] [nvarchar](max) NULL,
	[ChAddressRequirement] [nvarchar](max) NULL,
	[NavDeliveryCompanyCode] [nvarchar](max) NULL,
	[MgtCarrierCode] [nvarchar](max) NULL,
	[MgtTitle] [nvarchar](max) NULL,
	[LabelExportModel] [nvarchar](max) NULL,
	[DefaultPriceRestriction] [decimal](18, 2) NULL,
	[DefaultWeightRestriction] [decimal](18, 2) NULL,
	[DefaultQtyRestriction] [bigint] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_DeliveryCompanies] PRIMARY KEY CLUSTERED 
(
	[DeliveryCompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeliverySplitPriceRestriction]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeliverySplitPriceRestriction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[DeliveryCompanyCode] [varchar](100) NULL,
	[CountryCode] [varchar](2) NULL,
	[Threshold] [decimal](15, 3) NULL,
	[Currency] [varchar](3) NULL,
	[CreateOperatorId] [varchar](10) NOT NULL,
	[CreateAt] [datetime] NOT NULL,
	[UpdateOperatorId] [varchar](10) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_mst_split_restriction_price] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeliverySplitQtyRestriction]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeliverySplitQtyRestriction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[DeliveryCompanyCode] [varchar](100) NULL,
	[CountryCode] [varchar](2) NULL,
	[Threshold] [int] NULL,
	[CreateOperatorId] [varchar](10) NOT NULL,
	[CreateAt] [datetime] NOT NULL,
	[UpdateOperatorId] [varchar](10) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_DeliverySplitQtyRestriction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DeliverySplitWeightRestriction]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeliverySplitWeightRestriction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[DeliveryCompanyCode] [varchar](100) NULL,
	[CountryCode] [varchar](2) NULL,
	[Threshold] [decimal](15, 3) NULL,
	[CreateOperatorId] [varchar](10) NOT NULL,
	[CreateAt] [datetime] NOT NULL,
	[UpdateOperatorId] [varchar](10) NULL,
	[UpdateAt] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_DeliverySplitWeightRestriction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExchangeRates]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExchangeRates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CurrencyCodeFrom] [nvarchar](max) NULL,
	[CurrencyCodeTo] [nvarchar](max) NULL,
	[Period] [nvarchar](max) NULL,
	[Rate] [float] NOT NULL,
	[AcquisitionDate] [datetime2](7) NOT NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ExchangeRates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FdaRegistrations]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FdaRegistrations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[ChannelCode] [nvarchar](max) NULL,
	[OrderId] [nvarchar](max) NULL,
	[RegistrationDate] [datetime2](7) NULL,
	[PriorNoticeConfirmationNumber] [nvarchar](max) NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[DeliveryId] [nvarchar](max) NULL,
 CONSTRAINT [PK_FdaRegistrations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ForecastedSalesDatas]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ForecastedSalesDatas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[ForecastedDate] [nvarchar](max) NULL,
	[ForecastedChannel] [nvarchar](max) NULL,
	[ForecastedSalesAmount] [real] NOT NULL,
	[ForecastedProfitAmount] [real] NOT NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ForecastedSalesDatas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MstSystemClasses]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MstSystemClasses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TableName] [nvarchar](max) NULL,
	[StatusTittle] [nvarchar](max) NULL,
	[StatusValue] [int] NOT NULL,
	[JWording] [nvarchar](max) NULL,
	[EWording] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_MstSystemClasses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDispatches]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDispatches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[DeliveryId] [nvarchar](max) NULL,
	[MarketDeliveryNo] [nvarchar](max) NULL,
	[ChannelCode] [nvarchar](max) NULL,
	[OrderId] [nvarchar](max) NULL,
	[TrackingNo] [nvarchar](max) NULL,
	[DeliveryCompanyCode] [nvarchar](max) NULL,
	[ShipmentDate] [nvarchar](max) NULL,
	[DispatchStatus] [nvarchar](max) NULL,
	[IsCutOff] [bit] NOT NULL,
	[CutoffDate] [datetime2](7) NULL,
	[CutoffId] [nvarchar](max) NULL,
	[IsMarketShipped] [bit] NOT NULL,
	[IsCourierAssigned] [bit] NULL,
	[LabelFilePath] [nvarchar](max) NULL,
	[LabelFileExtension] [nvarchar](max) NULL,
	[InvoiceFilePath] [nvarchar](max) NULL,
	[InvoiceFileExtension] [nvarchar](max) NULL,
	[CallingApiDeliveryStatus] [int] NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[FdaRegistrationStatus] [int] NOT NULL,
	[StockUpStatus] [int] NOT NULL,
	[ReferenceId] [varchar](max) NULL,
	[OrderDispatchStatus] [nvarchar](5) NULL,
	[IsMarketUpdated] [bit] NULL,
 CONSTRAINT [PK_OrderDispatches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDispatchProducts]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDispatchProducts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DispatchHeaderId] [int] NOT NULL,
	[CompanyId] [int] NOT NULL,
	[DeliveryId] [nvarchar](max) NULL,
	[OrderId] [nvarchar](max) NULL,
	[LineNo] [int] NULL,
	[ProductSku] [nvarchar](max) NULL,
	[ItemName] [nvarchar](max) NULL,
	[ShippedQty] [int] NULL,
	[MarketShippedQty] [int] NULL,
	[Price] [float] NULL,
	[DeclaredValue] [float] NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[ParentItemCode] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NULL,
	[ProductName] [nvarchar](max) NULL,
	[isAttachedItem] [bit] NOT NULL,
	[StockUpStatus] [int] NOT NULL,
	[UnitPrice] [float] NULL,
 CONSTRAINT [PK_OrderDispatchProducts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItems]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderHeaderId] [int] NOT NULL,
	[CompanyId] [int] NOT NULL,
	[OrderId] [nvarchar](max) NULL,
	[LineNo] [int] NULL,
	[ProductCode] [nvarchar](max) NULL,
	[ProductName] [nvarchar](max) NULL,
	[SalesProductCode] [nvarchar](max) NULL,
	[SalesProductName] [nvarchar](max) NULL,
	[Quantity] [int] NULL,
	[PurchaseUnitPrice] [float] NULL,
	[DeclaredValue] [float] NULL,
	[Currency] [nvarchar](max) NULL,
	[ReturnQuantity] [int] NULL,
	[ParentItemCode] [nvarchar](max) NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[isAttachedItem] [bit] NOT NULL,
	[UnitPrice] [float] NULL,
 CONSTRAINT [PK_OrderItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[WarehouseCode] [nvarchar](max) NULL,
	[ChannelCode] [nvarchar](450) NULL,
	[OrderId] [nvarchar](max) NULL,
	[OrderStatus] [nvarchar](max) NULL,
	[OrderDate] [datetime2](7) NULL,
	[ShippedDate] [datetime2](7) NULL,
	[Total] [float] NULL,
	[SubTotal] [float] NULL,
	[HandlingCharge] [float] NULL,
	[Giftvoucher] [float] NULL,
	[Point] [float] NULL,
	[Shipping] [float] NULL,
	[CodCharge] [float] NULL,
	[DiscountAmount] [float] NULL,
	[OtherDiscount] [float] NULL,
	[TaxAmount] [float] NULL,
	[Currency] [nvarchar](max) NULL,
	[DeliveryCity] [nvarchar](max) NULL,
	[DeliveryState] [nvarchar](max) NULL,
	[DeliveryCountryCode] [nvarchar](max) NULL,
	[DeliveryCountryName] [nvarchar](max) NULL,
	[DeliveryMail] [nvarchar](max) NULL,
	[DeliveryAddress1] [nvarchar](max) NULL,
	[DeliveryAddress2] [nvarchar](max) NULL,
	[DeliveryAddress3] [nvarchar](max) NULL,
	[DeliveryName] [nvarchar](max) NULL,
	[DeliveryPhone] [nvarchar](max) NULL,
	[DeliveryZipcode] [nvarchar](max) NULL,
	[CustomerId] [nvarchar](max) NULL,
	[OrderedCompanyName] [nvarchar](max) NULL,
	[BillCity] [nvarchar](max) NULL,
	[BillState] [nvarchar](max) NULL,
	[BillCountry] [nvarchar](max) NULL,
	[BillMail] [nvarchar](max) NULL,
	[OrderDeliveryCompany] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
	[HoldJudgmentMemo] [nvarchar](max) NULL,
	[OnHoldStatus] [int] NOT NULL,
	[SubscriptionStatus] [int] NOT NULL,
	[HadCheckAttachItem] [int] NOT NULL,
	[InternalRemarks] [nvarchar](max) NULL,
	[ReturnStatus] [nvarchar](max) NULL,
	[StockUpStatus] [int] NOT NULL,
	[ShopifyTags] [nvarchar](max) NULL,
	[IsDeltaData] [bit] NULL,
	[AutoSplitDeliveryStatus] [int] NULL,
	[IsMarketUpdated] [bit] NULL,
	[MarketUpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[OrderName] [nvarchar](max) NULL,
	[DeliveryStateCode] [nvarchar](20) NULL,
	[OrderDate1] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderStatuses]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderStatuses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StatusOrder] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [int] NOT NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_OrderStatuses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PreNoticeShippingLimits]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PreNoticeShippingLimits](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PrePeriodDays] [int] NOT NULL,
	[DataKey] [nvarchar](450) NOT NULL,
	[CreateOperatorId] [nvarchar](450) NOT NULL,
	[CreateAt] [datetime2](7) NOT NULL,
	[UpdateOperatorId] [nvarchar](450) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PreSetProductSettings]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PreSetProductSettings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[SaleProductCode] [nvarchar](max) NULL,
	[SaleProductName] [nvarchar](max) NULL,
	[ProductBundleCode] [nvarchar](max) NULL,
	[ProductBundleName] [nvarchar](max) NULL,
	[RegularPrice] [float] NOT NULL,
	[FromApplyPreBundles] [datetime2](7) NULL,
	[ToApplyPreBundles] [datetime2](7) NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[IsConvertPrice] [bit] NOT NULL,
	[NumberOfTimes] [int] NOT NULL,
	[UnitPrice] [float] NULL,
 CONSTRAINT [PK_PreSetProductSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductBundles]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductBundles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductBundleCode] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NULL,
	[CompanyId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[BundlePriceRatio] [real] NOT NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ProductBundles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductCategories]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [int] NULL,
	[CompanyId] [int] NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[SlipDeliveryPrinting] [nvarchar](500) NULL,
 CONSTRAINT [PK_ProductCategories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductJanCodes]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductJanCodes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JanCode] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[ProductId] [int] NOT NULL,
 CONSTRAINT [PK_ProductJanCodes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductStatuses]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductStatuses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StatusProduct] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ProductStatuses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductStocks]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductStocks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductCode] [nvarchar](max) NULL,
	[CompanyId] [int] NOT NULL,
	[WarehouseCode] [nvarchar](max) NULL,
	[StockQuantity] [int] NULL,
	[DataKey] [nvarchar](450) NULL,
	[BinCode] [nvarchar](50) NULL,
	[LOT] [nvarchar](50) NULL,
	[Expried] [datetime2](7) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ProductStocks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SalesData]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalesData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](max) NULL,
	[CompanyId] [int] NOT NULL,
	[Channel] [nvarchar](max) NULL,
	[OrderId] [nvarchar](max) NULL,
	[OrderDate] [datetime2](7) NOT NULL,
	[OrderYear] [int] NOT NULL,
	[OrderQty] [int] NOT NULL,
	[CostOfSales] [float] NOT NULL,
	[Sales] [float] NOT NULL,
	[Profit] [float] NOT NULL,
	[Currency] [nvarchar](max) NULL,
	[ProductName] [nvarchar](max) NULL,
	[ProductCategory] [nvarchar](max) NULL,
	[State] [nvarchar](max) NULL,
	[City] [nvarchar](max) NULL,
	[Country] [nvarchar](max) NULL,
	[CustomerEmail] [nvarchar](max) NULL,
	[RepeatPurchase] [nvarchar](max) NULL,
	[DataKey] [nvarchar](450) NULL,
	[CustomerId] [nvarchar](max) NULL,
	[OrderMonth] [int] NOT NULL,
 CONSTRAINT [PK_SalesData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ShippingCountries]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShippingCountries](
	[CountryCode] [nvarchar](450) NOT NULL,
	[CountryName] [nvarchar](max) NULL,
	[DisplayName] [nvarchar](max) NULL,
	[DisplayOrder] [int] NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ShippingCountries] PRIMARY KEY CLUSTERED 
(
	[CountryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ShippingRules]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShippingRules](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CountryCode] [nvarchar](max) NULL,
	[StateCode] [nvarchar](max) NULL,
	[Weight] [int] NULL,
	[Courier] [nvarchar](max) NULL,
 CONSTRAINT [PK_ShippingRules] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Suppliers]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Suppliers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SupplierName] [nvarchar](max) NULL,
	[CompanyId] [int] NULL,
	[DataKey] [nvarchar](450) NULL,
	[SupplierId] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Suppliers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemClassCompanies]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemClassCompanies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[LargeClassCode] [nvarchar](max) NULL,
	[SmallClassCode] [nvarchar](max) NULL,
	[LargeClassCodeName] [nvarchar](max) NULL,
	[SmallClassCodeName] [nvarchar](max) NULL,
	[Code1] [nvarchar](max) NULL,
	[Code2] [nvarchar](max) NULL,
	[Numeric1] [int] NULL,
	[Numeric2] [float] NULL,
	[Date1] [datetime2](7) NULL,
	[Date2] [datetime2](7) NULL,
	[Text1] [nvarchar](max) NULL,
	[Text2] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_SystemClassCompanies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemMaxPKNos]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemMaxPKNos](
	[ObjectCode] [nvarchar](450) NOT NULL,
	[MaxNo] [int] NOT NULL,
	[Digit] [int] NOT NULL,
	[Remarks] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_SystemMaxPKNos] PRIMARY KEY CLUSTERED 
(
	[ObjectCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskChatHistories]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskChatHistories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskId] [int] NOT NULL,
	[ChatContent] [nvarchar](max) NULL,
	[DataKey] [nvarchar](450) NULL,
	[IsModified] [bit] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_TaskChatHistories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tasks]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tasks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[Title] [nvarchar](max) NULL,
	[Status] [int] NULL,
	[Priority] [int] NOT NULL,
	[Assignee] [nvarchar](max) NULL,
	[TaskType] [int] NOT NULL,
	[TaskContent] [nvarchar](max) NULL,
	[StartTime] [datetime2](7) NOT NULL,
	[EndTime] [datetime2](7) NULL,
	[DataKey] [nvarchar](450) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[FdaRegistrationId] [int] NULL,
	[ImageUrl] [nvarchar](max) NULL,
	[ChannelCode] [nvarchar](max) NULL,
	[OrderId] [nvarchar](max) NULL,
	[LotNo] [nvarchar](500) NULL,
	[ProductCode] [nvarchar](100) NULL,
	[ProductName] [nvarchar](500) NULL,
	[ReceiptNo] [nvarchar](100) NULL,
	[ReceiptOrderLineId] [uniqueidentifier] NOT NULL,
	[StatusProductError] [int] NULL,
	[ImagesBase64] [nvarchar](max) NULL,
	[IsTaskCreatedByBatch] [bit] NOT NULL,
	[DeliveryId] [nvarchar](max) NULL,
 CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TaskSchedules]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskSchedules](
	[ScheduleId] [int] IDENTITY(1,1) NOT NULL,
	[ScheduleDatetime] [datetime2](7) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[MarketplaceCode] [nvarchar](max) NULL,
	[StartDatetime] [datetime2](7) NULL,
	[FinishDatetime] [datetime2](7) NULL,
	[BatchId] [nvarchar](max) NULL,
	[Priority] [nvarchar](max) NULL,
	[IsFailed] [bit] NOT NULL,
	[IsStopped] [bit] NOT NULL,
	[RequestId] [nvarchar](max) NULL,
	[IsBatchInRequest] [bit] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_TaskSchedules] PRIMARY KEY CLUSTERED 
(
	[ScheduleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TempShopifyOrderProducts]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TempShopifyOrderProducts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[OrderId] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[Grams] [nvarchar](max) NULL,
	[Price] [nvarchar](max) NULL,
	[ProductId] [nvarchar](max) NULL,
	[Quantity] [nvarchar](max) NULL,
	[Sku] [nvarchar](max) NULL,
	[Title] [nvarchar](max) NULL,
	[TotalDiscount] [nvarchar](max) NULL,
	[VariantId] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_TempShopifyOrderProducts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TempShopifyOrders]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TempShopifyOrders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[OrderId] [nvarchar](max) NULL,
	[OrderName] [nvarchar](max) NULL,
	[FinancialStatus] [nvarchar](max) NULL,
	[FulfillmentStatus] [nvarchar](max) NULL,
	[Gateway] [nvarchar](max) NULL,
	[LocationId] [nvarchar](max) NULL,
	[Currency] [nvarchar](max) NULL,
	[TotalPrice] [nvarchar](max) NULL,
	[SubTotalPrice] [nvarchar](max) NULL,
	[TotalDiscounts] [nvarchar](max) NULL,
	[TotalTax] [nvarchar](max) NULL,
	[Number] [nvarchar](max) NULL,
	[OrderNumber] [nvarchar](max) NULL,
	[PresentmentCurrency] [nvarchar](max) NULL,
	[TotalWeight] [nvarchar](max) NULL,
	[Created] [nvarchar](max) NULL,
	[Updated] [nvarchar](max) NULL,
	[UserId] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Phone] [nvarchar](max) NULL,
	[FirstName] [nvarchar](max) NULL,
	[LastName] [nvarchar](max) NULL,
	[Address1] [nvarchar](max) NULL,
	[Address2] [nvarchar](max) NULL,
	[Province] [nvarchar](max) NULL,
	[Country] [nvarchar](max) NULL,
	[City] [nvarchar](max) NULL,
	[Zip] [nvarchar](max) NULL,
	[CountryCode] [nvarchar](max) NULL,
	[ProvinceCode] [nvarchar](max) NULL,
	[Shipping] [nvarchar](max) NULL,
	[CustomerId] [nvarchar](max) NULL,
	[Note] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Tags] [nvarchar](max) NULL,
	[OrderedCompanyName] [nvarchar](max) NULL,
 CONSTRAINT [PK_TempShopifyOrders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserSettings]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSettings](
	[UserId] [nvarchar](450) NOT NULL,
	[CurrencyCode] [nvarchar](max) NULL,
	[PageLength] [int] NOT NULL,
	[DataKey] [nvarchar](450) NULL,
 CONSTRAINT [PK_UserSettings] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserVendors]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserVendors](
	[UserId] [nvarchar](450) NOT NULL,
	[VendorCode] [nvarchar](max) NULL,
	[DataKey] [nvarchar](450) NULL,
 CONSTRAINT [PK_UserVendors] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vendors]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vendors](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VendorCode] [nvarchar](max) NULL,
	[VendorName] [nvarchar](max) NULL,
	[VendorCompanyName] [nvarchar](max) NULL,
	[HeadOfficeAddress] [nvarchar](max) NULL,
	[HeadOfficePhone] [nvarchar](max) NULL,
	[HeadOfficeFax] [nvarchar](max) NULL,
	[BillingAddress] [nvarchar](max) NULL,
	[BillingContactName] [nvarchar](max) NULL,
	[BillingPhone] [nvarchar](max) NULL,
	[BillingFax] [nvarchar](max) NULL,
	[BillingMail] [nvarchar](max) NULL,
	[BankName] [nvarchar](max) NULL,
	[BankBranch] [nvarchar](max) NULL,
	[BankAccountType] [nvarchar](max) NULL,
	[BankAccountNumber] [nvarchar](max) NULL,
	[BankAccountHolder] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
	[JamCharge] [float] NULL,
	[BillingDate] [int] NULL,
	[DataKey] [nvarchar](450) NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Vendors] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[AspNetRoleClaims]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[AspNetRoles]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[AspNetUserClaims]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[AspNetUserLogins]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[AspNetUserRoles]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [wms].[AspNetUsers]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[FullName] [nvarchar](max) NULL,
	[Localtion] [nvarchar](max) NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[AspNetUserTokens]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[Bins]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[Bins](
	[Id] [uniqueidentifier] NOT NULL,
	[LocationId] [uniqueidentifier] NOT NULL,
	[LocationCD] [nvarchar](max) NULL,
	[LocationName] [nvarchar](max) NULL,
	[BinCode] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[SortOrderNum] [int] NULL,
 CONSTRAINT [PK_Bins] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[Devices]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[Devices](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Type] [nvarchar](max) NULL,
	[Model] [nvarchar](max) NULL,
	[ActiveUser] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[OS] [nvarchar](max) NULL,
	[CPU] [nvarchar](max) NULL,
	[Memory] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Devices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[DHLPickupLogs]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[DHLPickupLogs](
	[Id] [uniqueidentifier] NOT NULL,
	[ShipmentNo] [nvarchar](max) NULL,
	[RequestPayload] [nvarchar](max) NOT NULL,
	[ResponsePayload] [nvarchar](max) NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[ErrorMessage] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[ImageStorage]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[ImageStorage](
	[Id] [uniqueidentifier] NOT NULL,
	[ResourcesId] [nvarchar](max) NULL,
	[Type] [int] NOT NULL,
	[FileName] [nvarchar](max) NULL,
	[ImageBase64] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ImageStorage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[InventAdjustmentLines]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[InventAdjustmentLines](
	[Id] [uniqueidentifier] NOT NULL,
	[AdjustmentNo] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NULL,
	[ParentProductCode] [nvarchar](max) NULL,
	[LotNo] [nvarchar](max) NULL,
	[Qty] [float] NULL,
	[UnitId] [int] NULL,
	[Status] [int] NOT NULL,
	[Remark] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Reasons] [nvarchar](1024) NULL,
	[IsBundle] [bit] NULL,
	[ExpirationDate] [date] NULL,
 CONSTRAINT [PK_InventAdjustmentLines] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[InventAdjustments]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[InventAdjustments](
	[Id] [uniqueidentifier] NOT NULL,
	[AdjustmentNo] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Bin] [nvarchar](max) NULL,
	[AdjustmentDate] [datetime2](7) NULL,
	[Status] [int] NOT NULL,
	[PersonInCharge] [nvarchar](max) NULL,
	[TenantId] [int] NOT NULL,
	[Location] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_InventAdjustments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[InventBundleLines]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[InventBundleLines](
	[Id] [uniqueidentifier] NOT NULL,
	[TransNo] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NULL,
	[Bin] [nvarchar](max) NULL,
	[LotNo] [nvarchar](max) NULL,
	[DemandQty] [float] NULL,
	[ActualQty] [float] NULL,
	[UnitId] [int] NULL,
	[Status] [int] NOT NULL,
	[ExpirationDate] [datetime2](7) NULL,
	[CreateAt] [datetime2](7) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[IsDeleted] [bit] NULL,
	[UpdateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[Location] [varchar](36) NULL,
 CONSTRAINT [PK_InventBundleLines] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[InventBundles]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[InventBundles](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductBundleCode] [nvarchar](max) NULL,
	[TransNo] [nvarchar](max) NULL,
	[TransDate] [datetime2](7) NULL,
	[Description] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[Bin] [nvarchar](max) NULL,
	[LotNo] [nvarchar](max) NULL,
	[Qty] [float] NULL,
	[ExpirationDate] [datetime2](7) NULL,
	[Status] [int] NOT NULL,
	[PersonInCharge] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[TenantId] [int] NOT NULL,
 CONSTRAINT [PK_InventBundles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[InventStockTakeLines]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[InventStockTakeLines](
	[Id] [uniqueidentifier] NOT NULL,
	[StockTakeNo] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NULL,
	[ExpectedQty] [float] NULL,
	[ActualQty] [float] NULL,
	[UnitId] [int] NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_InventStockTakeLines] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[InventStockTakeRecording]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[InventStockTakeRecording](
	[Id] [uniqueidentifier] NOT NULL,
	[StockTakeNo] [nvarchar](max) NULL,
	[RecordNo] [int] NOT NULL,
	[Location] [nvarchar](max) NULL,
	[PersonInCharge] [nvarchar](max) NULL,
	[TransactionDate] [date] NOT NULL,
	[Status] [int] NOT NULL,
	[Remarks] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[HHTInfo] [nvarchar](max) NULL,
	[HHTStatus] [int] NOT NULL,
	[TenantId] [int] NOT NULL,
 CONSTRAINT [PK_InventStockTakeRecording] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[InventStockTakeRecordingLines]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[InventStockTakeRecordingLines](
	[Id] [uniqueidentifier] NOT NULL,
	[StockTakeRecordingId] [uniqueidentifier] NOT NULL,
	[StockTakeNo] [nvarchar](max) NULL,
	[RecordNo] [int] NOT NULL,
	[ProductCode] [nvarchar](max) NULL,
	[Bin] [nvarchar](max) NULL,
	[LotNo] [nvarchar](max) NULL,
	[ExpectedQty] [float] NULL,
	[ActualQty] [float] NULL,
	[UnitId] [int] NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[TenantId] [int] NOT NULL,
 CONSTRAINT [PK_InventStockTakeRecordingLines] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[InventStockTakes]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[InventStockTakes](
	[Id] [uniqueidentifier] NOT NULL,
	[StockTakeNo] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[TransactionDate] [datetime2](7) NULL,
	[Status] [int] NOT NULL,
	[PersonInCharge] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[HHTInfo] [nvarchar](max) NULL,
	[HHTStatus] [int] NOT NULL,
	[TenantId] [int] NOT NULL,
 CONSTRAINT [PK_InventStockTakes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[InventTransfer]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[InventTransfer](
	[Id] [uniqueidentifier] NOT NULL,
	[TransferNo] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[TransferDate] [date] NULL,
	[Status] [int] NOT NULL,
	[PersonInCharge] [nvarchar](max) NULL,
	[TenantId] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Location] [nvarchar](100) NOT NULL,
	[HHTInfo] [nvarchar](max) NULL,
	[HHTStatus] [int] NOT NULL,
 CONSTRAINT [PK_InventTransfer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[InventTransferLines]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[InventTransferLines](
	[Id] [uniqueidentifier] NOT NULL,
	[InventTransferId] [uniqueidentifier] NOT NULL,
	[TransferNo] [nvarchar](max) NULL,
	[Qty] [float] NULL,
	[UnitId] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[FromBin] [nvarchar](max) NULL,
	[ToBin] [nvarchar](max) NULL,
	[FromLotNo] [nvarchar](max) NULL,
	[ToLotNo] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[ProductCode] [nvarchar](100) NULL,
 CONSTRAINT [PK_InventTransferLines] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[LogTime]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[LogTime](
	[Id] [uniqueidentifier] NOT NULL,
	[LogName] [nvarchar](max) NULL,
	[EslapseTime] [float] NULL,
	[CreatedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_LogTime] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[MstUserSettings]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[MstUserSettings](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [nvarchar](max) NULL,
	[Currency] [nvarchar](max) NULL,
 CONSTRAINT [PK_MstUserSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[NumberSequences]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[NumberSequences](
	[Id] [uniqueidentifier] NOT NULL,
	[JournalType] [nvarchar](max) NULL,
	[Prefix] [nvarchar](max) NULL,
	[SequenceLength] [int] NULL,
	[CurrentSequenceNo] [int] NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_NumberSequences] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[Permissions]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[Permissions](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[PermissionsTenant]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[PermissionsTenant](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_PermissionsTenant] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[RefreshTokens]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[RefreshTokens](
	[RefreshToken] [nvarchar](450) NOT NULL,
	[UserId] [nvarchar](max) NULL,
	[Token] [nvarchar](max) NULL,
	[ExpirationTime] [datetime2](7) NOT NULL,
	[Activated] [bit] NULL,
 CONSTRAINT [PK_RefreshTokens] PRIMARY KEY CLUSTERED 
(
	[RefreshToken] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[ReturnOrderLines]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[ReturnOrderLines](
	[Id] [uniqueidentifier] NOT NULL,
	[ReturnOrderNo] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[Qty] [float] NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[ProductCode] [nvarchar](max) NULL,
	[UnitId] [int] NOT NULL,
 CONSTRAINT [PK_ReturnOrderLines] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[ReturnOrders]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[ReturnOrders](
	[Id] [uniqueidentifier] NOT NULL,
	[ReturnOrderNo] [nvarchar](max) NULL,
	[ShipmentNo] [nvarchar](max) NULL,
	[ReturnDate] [date] NULL,
	[Reason] [nvarchar](max) NULL,
	[PersonInCharge] [nvarchar](max) NULL,
	[ShipDate] [date] NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ReturnOrders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[RoleToPermission]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[RoleToPermission](
	[Id] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](max) NULL,
	[PermissionId] [uniqueidentifier] NOT NULL,
	[PermisionName] [nvarchar](max) NULL,
	[PermisionDescription] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_RoleToPermission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[RoleToPermissionTenant]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[RoleToPermissionTenant](
	[Id] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](max) NULL,
	[PermissionTenantId] [uniqueidentifier] NOT NULL,
	[PermissionTenantName] [nvarchar](max) NULL,
	[PermissionTenantDesciption] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_RoleToPermissionTenant] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[ShippingBoxes]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[ShippingBoxes](
	[Id] [uniqueidentifier] NOT NULL,
	[BoxName] [nvarchar](max) NULL,
	[BoxType] [nvarchar](max) NULL,
	[Length] [float] NULL,
	[Width] [float] NULL,
	[Height] [float] NULL,
	[MaxWeight] [float] NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[ShippingCarrierCode] [nvarchar](500) NULL,
	[ShippingCarrierId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ShippingBoxes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[ShippingCarriers]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[ShippingCarriers](
	[Id] [uniqueidentifier] NOT NULL,
	[ShippingCarrierCode] [nvarchar](max) NULL,
	[ShippingCarrierName] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[PrinterName] [nvarchar](max) NULL,
 CONSTRAINT [PK_ShippingCarriers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[Units]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[Units](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UnitName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Units] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[UserToTenant]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[UserToTenant](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [nvarchar](max) NULL,
	[TenantId] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_UserToTenant] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehouseParameters]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehouseParameters](
	[Id] [uniqueidentifier] NOT NULL,
	[CompanyName] [nvarchar](max) NULL,
	[CompanyAddress] [nvarchar](max) NULL,
	[CompanyPhone] [nvarchar](max) NULL,
	[CompanyEmail] [nvarchar](max) NULL,
	[CompanyFax] [nvarchar](max) NULL,
	[CompanyWebsite] [nvarchar](max) NULL,
	[InvoiceAddress] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
	[PutawayLocation] [nvarchar](max) NULL,
	[ReceivingLocation] [nvarchar](max) NULL,
	[ShipmentLocation] [nvarchar](max) NULL,
 CONSTRAINT [PK_WarehouseParameters] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehousePickingLines]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehousePickingLines](
	[Id] [uniqueidentifier] NOT NULL,
	[PickNo] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NULL,
	[LotNo] [nvarchar](max) NULL,
	[PickQty] [float] NULL,
	[ActualQty] [float] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
	[UnitId] [int] NOT NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[Bin] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[ShipmentLineId] [uniqueidentifier] NOT NULL,
	[ShipmentNo] [nvarchar](100) NULL,
	[ExpirationDate] [date] NOT NULL,
 CONSTRAINT [PK_WarehousePickingLines] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehousePickingList]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehousePickingList](
	[Id] [uniqueidentifier] NOT NULL,
	[PickNo] [nvarchar](max) NULL,
	[TenantId] [int] NULL,
	[PersonInCharge] [nvarchar](max) NULL,
	[PickedDate] [date] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Status] [int] NULL,
	[EstimatedShipDate] [date] NULL,
	[Location] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
	[HHTInfo] [nvarchar](max) NULL,
	[HHTStatus] [int] NOT NULL,
 CONSTRAINT [PK_WarehousePickingList] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehousePickingStaging]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehousePickingStaging](
	[Id] [uniqueidentifier] NOT NULL,
	[PickNo] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NULL,
	[Unit] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[Bin] [nvarchar](max) NULL,
	[LotNo] [nvarchar](max) NULL,
	[PickQty] [float] NULL,
	[ActualQty] [float] NULL,
	[UnitId] [int] NULL,
	[Status] [int] NOT NULL,
	[ShipmentLineId] [uniqueidentifier] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[ProductQRCode] [nvarchar](500) NULL,
	[ShipmentNo] [nvarchar](100) NULL,
	[PickingBoxCode] [nvarchar](100) NULL,
 CONSTRAINT [PK_WarehousePickingStaging] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehousePutAwayLines]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehousePutAwayLines](
	[Id] [uniqueidentifier] NOT NULL,
	[PutAwayNo] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NULL,
	[UnitId] [int] NULL,
	[JournalQty] [float] NULL,
	[TransQty] [float] NULL,
	[Bin] [nvarchar](max) NULL,
	[LotNo] [nvarchar](max) NULL,
	[ExpirationDate] [date] NULL,
	[TenantId] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[ReceiptLineId] [uniqueidentifier] NULL,
	[HHTInfo] [nvarchar](max) NULL,
	[HHTStatus] [int] NOT NULL,
 CONSTRAINT [PK_WarehousePutAwayLines] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehousePutAways]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehousePutAways](
	[Id] [uniqueidentifier] NOT NULL,
	[PutAwayNo] [nvarchar](max) NULL,
	[ReceiptNo] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[TenantId] [int] NOT NULL,
	[TransDate] [date] NULL,
	[DocumentDate] [date] NULL,
	[DocumentNo] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[PostedDate] [date] NULL,
	[PostedBy] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[HHTInfo] [nvarchar](max) NULL,
	[HHTStatus] [int] NOT NULL,
	[ReferenceNo] [nvarchar](max) NULL,
	[ReferenceType] [int] NOT NULL,
 CONSTRAINT [PK_WarehousePutAways] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehousePutAwayStaging]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehousePutAwayStaging](
	[Id] [uniqueidentifier] NOT NULL,
	[PutAwayNo] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NULL,
	[Unit] [nvarchar](max) NULL,
	[JournalQty] [float] NULL,
	[TransQty] [float] NULL,
	[Bin] [nvarchar](max) NULL,
	[LotNo] [nvarchar](max) NULL,
	[ReceiptLineId] [uniqueidentifier] NULL,
	[PutAwayLineId] [uniqueidentifier] NOT NULL,
	[Status] [int] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[ExpiryDate] [date] NULL,
 CONSTRAINT [PK_WarehousePutAwayStaging] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehouseReceiptOrder]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehouseReceiptOrder](
	[Id] [uniqueidentifier] NOT NULL,
	[ReceiptNo] [nvarchar](max) NULL,
	[VendorCode] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[ExpectedDate] [date] NULL,
	[ArrivalType] [nvarchar](max) NULL,
	[TenantId] [int] NOT NULL,
	[ScheduledArrivalNumber] [float] NULL,
	[DocumentNo] [nvarchar](max) NULL,
	[SupplierId] [int] NOT NULL,
	[PersonInCharge] [nvarchar](max) NULL,
	[ConfirmedBy] [nvarchar](max) NULL,
	[ConfirmedDate] [date] NULL,
	[Status] [int] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[PostedBy] [nvarchar](max) NULL,
	[PostedDate] [date] NULL,
	[SupplierCode] [nvarchar](max) NULL,
	[ReferenceNo] [nvarchar](max) NULL,
	[ReferenceType] [int] NOT NULL,
	[HHTInfo] [nvarchar](max) NULL,
	[HHTStatus] [int] NOT NULL,
 CONSTRAINT [PK_WarehouseReceiptOrder] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehouseReceiptOrderLine]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehouseReceiptOrderLine](
	[Id] [uniqueidentifier] NOT NULL,
	[ReceiptNo] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NOT NULL,
	[UnitName] [nvarchar](max) NULL,
	[OrderQty] [float] NULL,
	[TransQty] [float] NULL,
	[Bin] [nvarchar](max) NULL,
	[LotNo] [nvarchar](max) NULL,
	[ExpirationDate] [date] NULL,
	[Putaway] [bit] NULL,
	[UnitId] [int] NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[ArrivalNo] [float] NULL,
	[ErrorImages] [nvarchar](max) NULL,
	[ReceiptLineIdParent] [uniqueidentifier] NULL,
 CONSTRAINT [PK_WarehouseReceiptOrderLine] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehouseReceiptStaging]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehouseReceiptStaging](
	[Id] [uniqueidentifier] NOT NULL,
	[ReceiptNo] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NULL,
	[UnitId] [int] NOT NULL,
	[OrderQty] [float] NULL,
	[TransQty] [float] NULL,
	[Bin] [nvarchar](max) NULL,
	[LotNo] [nvarchar](max) NULL,
	[ExpirationDate] [date] NULL,
	[ReceiptLineId] [uniqueidentifier] NOT NULL,
	[Status] [int] NOT NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[ErrorImages] [nvarchar](max) NULL,
	[StatusError] [int] NULL,
 CONSTRAINT [PK_WarehouseReceiptStaging] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehouseShipmentLines]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehouseShipmentLines](
	[Id] [uniqueidentifier] NOT NULL,
	[ShipmentNo] [nvarchar](max) NULL,
	[ProductCode] [nvarchar](max) NULL,
	[UnitId] [int] NULL,
	[ShipmentQty] [float] NULL,
	[Location] [nvarchar](max) NULL,
	[Status] [int] NULL,
	[PackedQty] [float] NULL,
	[PackedDate] [date] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[Bin] [nvarchar](max) NULL,
	[LotNo] [nvarchar](100) NULL,
	[ExpirationDate] [date] NOT NULL,
 CONSTRAINT [PK_WarehouseShipmentLines] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehouseShipments]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehouseShipments](
	[Id] [uniqueidentifier] NOT NULL,
	[ShipmentNo] [nvarchar](max) NULL,
	[SalesNo] [nvarchar](max) NULL,
	[TenantId] [int] NOT NULL,
	[TenantName] [nvarchar](max) NULL,
	[PlanShipDate] [date] NULL,
	[PersonInCharge] [nvarchar](max) NULL,
	[ShippingCarrierCode] [nvarchar](max) NULL,
	[ShippingAddress] [nvarchar](max) NULL,
	[Telephone] [nvarchar](max) NULL,
	[TrackingNo] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Status] [int] NULL,
	[PickingNo] [nvarchar](max) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
	[LocationName] [nvarchar](max) NULL,
	[PersonInChargeName] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[BinId] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[ShipmentType] [int] NOT NULL,
	[DHLPickup] [int] NULL,
	[DHLPickupDatetime] [datetime2](7) NULL,
	[OrderDispatchId] [int] NULL,
	[ShippingBoxesId] [uniqueidentifier] NULL,
	[ShippingBoxesName] [nvarchar](100) NULL,
 CONSTRAINT [PK_WarehouseShipments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [wms].[WarehouseTransBackup]    Script Date: 24/02/2025 6:41:36 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [wms].[WarehouseTransBackup](
	[Id] [uniqueidentifier] NOT NULL,
	[ProductCode] [nvarchar](max) NULL,
	[Qty] [float] NOT NULL,
	[DatePhysical] [date] NULL,
	[TransType] [int] NOT NULL,
	[TransNumber] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[Bin] [nvarchar](max) NULL,
	[LotNo] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
	[StatusIssue] [int] NULL,
	[StatusReceipt] [int] NULL,
	[TransId] [uniqueidentifier] NULL,
	[TransLineId] [uniqueidentifier] NULL,
	[PutAwayNo] [nvarchar](max) NULL,
	[PackingNo] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[TenantId] [int] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AuthUsers_Email]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_AuthUsers_Email] ON [authp].[AuthUsers]
(
	[Email] ASC
)
WHERE ([Email] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AuthUsers_TenantId]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_AuthUsers_TenantId] ON [authp].[AuthUsers]
(
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AuthUsers_UserName]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_AuthUsers_UserName] ON [authp].[AuthUsers]
(
	[UserName] ASC
)
WHERE ([UserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RefreshTokens_AddedDateUtc]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_RefreshTokens_AddedDateUtc] ON [authp].[RefreshTokens]
(
	[AddedDateUtc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RoleToPermissions_RoleType]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_RoleToPermissions_RoleType] ON [authp].[RoleToPermissions]
(
	[RoleType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RoleToPermissionsTenant_TenantsTenantId]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_RoleToPermissionsTenant_TenantsTenantId] ON [authp].[RoleToPermissionsTenant]
(
	[TenantsTenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Tenants_ParentDataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_Tenants_ParentDataKey] ON [authp].[Tenants]
(
	[ParentDataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Tenants_ParentTenantId]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_Tenants_ParentTenantId] ON [authp].[Tenants]
(
	[ParentTenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Tenants_TenantFullName]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Tenants_TenantFullName] ON [authp].[Tenants]
(
	[TenantFullName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserToRoles_RoleName]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_UserToRoles_RoleName] ON [authp].[UserToRoles]
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ApiCodes_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_ApiCodes_DataKey] ON [dbo].[ApiCodes]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AttachedItemCategories_AttachedItemId]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_AttachedItemCategories_AttachedItemId] ON [dbo].[AttachedItemCategories]
(
	[AttachedItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AttachedItemCategories_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_AttachedItemCategories_DataKey] ON [dbo].[AttachedItemCategories]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AttachedItemChannels_AttachedItemId]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_AttachedItemChannels_AttachedItemId] ON [dbo].[AttachedItemChannels]
(
	[AttachedItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AttachedItemChannels_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_AttachedItemChannels_DataKey] ON [dbo].[AttachedItemChannels]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AttachedItemDetails_AttachedItemId]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_AttachedItemDetails_AttachedItemId] ON [dbo].[AttachedItemDetails]
(
	[AttachedItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AttachedItemDetails_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_AttachedItemDetails_DataKey] ON [dbo].[AttachedItemDetails]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AttachedItemProductTypes_AttachedItemId]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_AttachedItemProductTypes_AttachedItemId] ON [dbo].[AttachedItemProductTypes]
(
	[AttachedItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AttachedItemProductTypes_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_AttachedItemProductTypes_DataKey] ON [dbo].[AttachedItemProductTypes]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AttachedItems_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_AttachedItems_DataKey] ON [dbo].[AttachedItems]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ChannelMasters_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_ChannelMasters_DataKey] ON [dbo].[ChannelMasters]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Channels_ChannelMasterCode]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_Channels_ChannelMasterCode] ON [dbo].[Channels]
(
	[ChannelMasterCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Channels_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_Channels_DataKey] ON [dbo].[Channels]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Companies_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_Companies_DataKey] ON [dbo].[Companies]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_CountryMasters_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_CountryMasters_DataKey] ON [dbo].[CountryMasters]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Currencies_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_Currencies_DataKey] ON [dbo].[Currencies]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_CurrencyPairSettings_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_CurrencyPairSettings_DataKey] ON [dbo].[CurrencyPairSettings]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ExchangeRates_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_ExchangeRates_DataKey] ON [dbo].[ExchangeRates]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ForecastedSalesDatas_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_ForecastedSalesDatas_DataKey] ON [dbo].[ForecastedSalesDatas]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_OrderDispatches_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_OrderDispatches_DataKey] ON [dbo].[OrderDispatches]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_OrderDispatchProducts_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_OrderDispatchProducts_DataKey] ON [dbo].[OrderDispatchProducts]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderDispatchProducts_DispatchHeaderId]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_OrderDispatchProducts_DispatchHeaderId] ON [dbo].[OrderDispatchProducts]
(
	[DispatchHeaderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_OrderItems_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_DataKey] ON [dbo].[OrderItems]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderItems_OrderHeaderId]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_OrderHeaderId] ON [dbo].[OrderItems]
(
	[OrderHeaderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Orders_ChannelCode]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_Orders_ChannelCode] ON [dbo].[Orders]
(
	[ChannelCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Orders_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_Orders_DataKey] ON [dbo].[Orders]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_OrderStatuses_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_OrderStatuses_DataKey] ON [dbo].[OrderStatuses]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ProductBundles_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_ProductBundles_DataKey] ON [dbo].[ProductBundles]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ProductCategories_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_ProductCategories_DataKey] ON [dbo].[ProductCategories]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ProductJanCodes_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_ProductJanCodes_DataKey] ON [dbo].[ProductJanCodes]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Products_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_Products_DataKey] ON [dbo].[Products]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UC_Products_Filtered]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE UNIQUE NONCLUSTERED INDEX [UC_Products_Filtered] ON [dbo].[Products]
(
	[CompanyId] ASC,
	[ProductCode] ASC
)
WHERE ([isDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ProductStatuses_DataKey]    Script Date: 24/02/2025 6:41:36 am ******/
CREATE NONCLUSTERED INDEX [IX_ProductStatuses_DataKey] ON [dbo].[ProductStatuses]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ProductStocks_DataKey]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE NONCLUSTERED INDEX [IX_ProductStocks_DataKey] ON [dbo].[ProductStocks]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SalesData_DataKey]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE NONCLUSTERED INDEX [IX_SalesData_DataKey] ON [dbo].[SalesData]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ShippingCountries_DataKey]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE NONCLUSTERED INDEX [IX_ShippingCountries_DataKey] ON [dbo].[ShippingCountries]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Suppliers_DataKey]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE NONCLUSTERED INDEX [IX_Suppliers_DataKey] ON [dbo].[Suppliers]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SystemClassCompanies_DataKey]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE NONCLUSTERED INDEX [IX_SystemClassCompanies_DataKey] ON [dbo].[SystemClassCompanies]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_TaskChatHistories_DataKey]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE NONCLUSTERED INDEX [IX_TaskChatHistories_DataKey] ON [dbo].[TaskChatHistories]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_TaskChatHistories_TaskId]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE NONCLUSTERED INDEX [IX_TaskChatHistories_TaskId] ON [dbo].[TaskChatHistories]
(
	[TaskId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Tasks_DataKey]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE NONCLUSTERED INDEX [IX_Tasks_DataKey] ON [dbo].[Tasks]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserSettings_DataKey]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE NONCLUSTERED INDEX [IX_UserSettings_DataKey] ON [dbo].[UserSettings]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserVendors_DataKey]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE NONCLUSTERED INDEX [IX_UserVendors_DataKey] ON [dbo].[UserVendors]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Vendors_DataKey]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE NONCLUSTERED INDEX [IX_Vendors_DataKey] ON [dbo].[Vendors]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [wms].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [wms].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [wms].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [wms].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [wms].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [wms].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 24/02/2025 6:41:37 am ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [wms].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [authp].[AuthUsers] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsDisabled]
GO
ALTER TABLE [authp].[RoleToPermissions] ADD  DEFAULT (CONVERT([tinyint],(0))) FOR [RoleType]
GO
ALTER TABLE [authp].[Tenants] ADD  DEFAULT (CONVERT([bit],(0))) FOR [HasOwnDb]
GO
ALTER TABLE [dbo].[BatchSchedules] ADD  CONSTRAINT [DF__Batches__creat__1AA9E072]  DEFAULT (getdate()) FOR [CreateAt]
GO
ALTER TABLE [dbo].[BatchSchedules] ADD  CONSTRAINT [DF_BatchSchedules_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[BatchSchedulesNew] ADD  CONSTRAINT [DF__BatchesNew__creat__1AA9E072]  DEFAULT (getdate()) FOR [CreateAt]
GO
ALTER TABLE [dbo].[BatchSchedulesNew] ADD  CONSTRAINT [DF_BatchSchedulesNew_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[DeliveryCompanies] ADD  CONSTRAINT [DF_DeliveryCompanies_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[DeliverySplitPriceRestriction] ADD  CONSTRAINT [DF_DeliverySplitPriceRestriction_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[DeliverySplitQtyRestriction] ADD  CONSTRAINT [DF_DeliverySplitQtyRestriction_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[DeliverySplitWeightRestriction] ADD  CONSTRAINT [DF_DeliverySplitWeightRestriction_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[OrderDispatches] ADD  CONSTRAINT [DF_OrderDispatches_IsCourierAssigned]  DEFAULT ((0)) FOR [IsCourierAssigned]
GO
ALTER TABLE [dbo].[OrderDispatches] ADD  CONSTRAINT [DF__OrderDisp__FdaRe__16644E42]  DEFAULT ((0)) FOR [FdaRegistrationStatus]
GO
ALTER TABLE [dbo].[OrderDispatches] ADD  CONSTRAINT [DF__OrderDisp__Stock__1758727B]  DEFAULT ((0)) FOR [StockUpStatus]
GO
ALTER TABLE [dbo].[OrderDispatchProducts] ADD  DEFAULT (CONVERT([bit],(0))) FOR [isAttachedItem]
GO
ALTER TABLE [dbo].[OrderDispatchProducts] ADD  DEFAULT ((0)) FOR [StockUpStatus]
GO
ALTER TABLE [dbo].[OrderItems] ADD  DEFAULT (CONVERT([bit],(0))) FOR [isAttachedItem]
GO
ALTER TABLE [dbo].[Orders] ADD  CONSTRAINT [DF__Orders__StockUpS__0F2D40CE]  DEFAULT ((0)) FOR [StockUpStatus]
GO
ALTER TABLE [dbo].[PreNoticeShippingLimits] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[PreSetProductSettings] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsConvertPrice]
GO
ALTER TABLE [dbo].[PreSetProductSettings] ADD  DEFAULT ((0)) FOR [NumberOfTimes]
GO
ALTER TABLE [dbo].[ProductJanCodes] ADD  DEFAULT ((0)) FOR [ProductId]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [CompanyId]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [ProductStatus]
GO
ALTER TABLE [dbo].[SalesData] ADD  DEFAULT ((0)) FOR [OrderMonth]
GO
ALTER TABLE [dbo].[Suppliers] ADD  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Tasks] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [ReceiptOrderLineId]
GO
ALTER TABLE [dbo].[Tasks] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsTaskCreatedByBatch]
GO
ALTER TABLE [wms].[DHLPickupLogs] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [wms].[DHLPickupLogs] ADD  DEFAULT (getutcdate()) FOR [Timestamp]
GO
ALTER TABLE [wms].[InventAdjustments] ADD  DEFAULT ((0)) FOR [TenantId]
GO
ALTER TABLE [wms].[InventBundles] ADD  DEFAULT ((0)) FOR [TenantId]
GO
ALTER TABLE [wms].[InventStockTakeRecording] ADD  DEFAULT ((0)) FOR [HHTStatus]
GO
ALTER TABLE [wms].[InventStockTakeRecording] ADD  DEFAULT ((0)) FOR [TenantId]
GO
ALTER TABLE [wms].[InventStockTakeRecordingLines] ADD  DEFAULT ((0)) FOR [TenantId]
GO
ALTER TABLE [wms].[InventStockTakes] ADD  DEFAULT ((0)) FOR [HHTStatus]
GO
ALTER TABLE [wms].[InventStockTakes] ADD  DEFAULT ((0)) FOR [TenantId]
GO
ALTER TABLE [wms].[InventTransfer] ADD  DEFAULT ((0)) FOR [HHTStatus]
GO
ALTER TABLE [wms].[ReturnOrderLines] ADD  DEFAULT ((0)) FOR [UnitId]
GO
ALTER TABLE [wms].[ShippingBoxes] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [ShippingCarrierId]
GO
ALTER TABLE [wms].[WarehouseParameters] ADD  CONSTRAINT [DF_WarehouseParameters_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [wms].[WarehousePickingLines] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [ShipmentLineId]
GO
ALTER TABLE [wms].[WarehousePickingLines] ADD  DEFAULT ('0001-01-01') FOR [ExpirationDate]
GO
ALTER TABLE [wms].[WarehousePickingList] ADD  DEFAULT ((0)) FOR [HHTStatus]
GO
ALTER TABLE [wms].[WarehousePickingStaging] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [ShipmentLineId]
GO
ALTER TABLE [wms].[WarehousePutAwayLines] ADD  DEFAULT ((0)) FOR [HHTStatus]
GO
ALTER TABLE [wms].[WarehousePutAways] ADD  DEFAULT ((0)) FOR [HHTStatus]
GO
ALTER TABLE [wms].[WarehousePutAways] ADD  DEFAULT ((0)) FOR [ReferenceType]
GO
ALTER TABLE [wms].[WarehousePutAwayStaging] ADD  DEFAULT ('00000000-0000-0000-0000-000000000000') FOR [PutAwayLineId]
GO
ALTER TABLE [wms].[WarehouseReceiptOrder] ADD  DEFAULT ((0)) FOR [ReferenceType]
GO
ALTER TABLE [wms].[WarehouseReceiptOrder] ADD  DEFAULT ((0)) FOR [HHTStatus]
GO
ALTER TABLE [wms].[WarehouseShipmentLines] ADD  DEFAULT ('0001-01-01') FOR [ExpirationDate]
GO
ALTER TABLE [wms].[WarehouseShipments] ADD  DEFAULT ((0)) FOR [ShipmentType]
GO
ALTER TABLE [wms].[WarehouseShipments] ADD  CONSTRAINT [DF_WarehouseShipments_DHLPickup]  DEFAULT ((0)) FOR [DHLPickup]
GO
ALTER TABLE [authp].[AuthUsers]  WITH CHECK ADD  CONSTRAINT [FK_AuthUsers_Tenants_TenantId] FOREIGN KEY([TenantId])
REFERENCES [authp].[Tenants] ([TenantId])
GO
ALTER TABLE [authp].[AuthUsers] CHECK CONSTRAINT [FK_AuthUsers_Tenants_TenantId]
GO
ALTER TABLE [authp].[RoleToPermissionsTenant]  WITH CHECK ADD  CONSTRAINT [FK_RoleToPermissionsTenant_RoleToPermissions_TenantRolesRoleName] FOREIGN KEY([TenantRolesRoleName])
REFERENCES [authp].[RoleToPermissions] ([RoleName])
ON DELETE CASCADE
GO
ALTER TABLE [authp].[RoleToPermissionsTenant] CHECK CONSTRAINT [FK_RoleToPermissionsTenant_RoleToPermissions_TenantRolesRoleName]
GO
ALTER TABLE [authp].[RoleToPermissionsTenant]  WITH CHECK ADD  CONSTRAINT [FK_RoleToPermissionsTenant_Tenants_TenantsTenantId] FOREIGN KEY([TenantsTenantId])
REFERENCES [authp].[Tenants] ([TenantId])
ON DELETE CASCADE
GO
ALTER TABLE [authp].[RoleToPermissionsTenant] CHECK CONSTRAINT [FK_RoleToPermissionsTenant_Tenants_TenantsTenantId]
GO
ALTER TABLE [authp].[Tenants]  WITH CHECK ADD  CONSTRAINT [FK_Tenants_Tenants_ParentTenantId] FOREIGN KEY([ParentTenantId])
REFERENCES [authp].[Tenants] ([TenantId])
GO
ALTER TABLE [authp].[Tenants] CHECK CONSTRAINT [FK_Tenants_Tenants_ParentTenantId]
GO
ALTER TABLE [authp].[UserToRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserToRoles_AuthUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [authp].[AuthUsers] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [authp].[UserToRoles] CHECK CONSTRAINT [FK_UserToRoles_AuthUsers_UserId]
GO
ALTER TABLE [authp].[UserToRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserToRoles_RoleToPermissions_RoleName] FOREIGN KEY([RoleName])
REFERENCES [authp].[RoleToPermissions] ([RoleName])
ON DELETE CASCADE
GO
ALTER TABLE [authp].[UserToRoles] CHECK CONSTRAINT [FK_UserToRoles_RoleToPermissions_RoleName]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AttachedItemCategories]  WITH CHECK ADD  CONSTRAINT [FK_AttachedItemCategories_AttachedItems_AttachedItemId] FOREIGN KEY([AttachedItemId])
REFERENCES [dbo].[AttachedItems] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AttachedItemCategories] CHECK CONSTRAINT [FK_AttachedItemCategories_AttachedItems_AttachedItemId]
GO
ALTER TABLE [dbo].[AttachedItemChannels]  WITH CHECK ADD  CONSTRAINT [FK_AttachedItemChannels_AttachedItems_AttachedItemId] FOREIGN KEY([AttachedItemId])
REFERENCES [dbo].[AttachedItems] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AttachedItemChannels] CHECK CONSTRAINT [FK_AttachedItemChannels_AttachedItems_AttachedItemId]
GO
ALTER TABLE [dbo].[AttachedItemDetails]  WITH CHECK ADD  CONSTRAINT [FK_AttachedItemDetails_AttachedItems_AttachedItemId] FOREIGN KEY([AttachedItemId])
REFERENCES [dbo].[AttachedItems] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AttachedItemDetails] CHECK CONSTRAINT [FK_AttachedItemDetails_AttachedItems_AttachedItemId]
GO
ALTER TABLE [dbo].[AttachedItemProductTypes]  WITH CHECK ADD  CONSTRAINT [FK_AttachedItemProductTypes_AttachedItems_AttachedItemId] FOREIGN KEY([AttachedItemId])
REFERENCES [dbo].[AttachedItems] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AttachedItemProductTypes] CHECK CONSTRAINT [FK_AttachedItemProductTypes_AttachedItems_AttachedItemId]
GO
ALTER TABLE [dbo].[Channels]  WITH CHECK ADD  CONSTRAINT [FK_Channels_ChannelMasters_ChannelMasterCode] FOREIGN KEY([ChannelMasterCode])
REFERENCES [dbo].[ChannelMasters] ([ChannelMasterCode])
GO
ALTER TABLE [dbo].[Channels] CHECK CONSTRAINT [FK_Channels_ChannelMasters_ChannelMasterCode]
GO
ALTER TABLE [dbo].[OrderDispatchProducts]  WITH CHECK ADD  CONSTRAINT [FK_OrderDispatchProducts_OrderDispatches_DispatchHeaderId] FOREIGN KEY([DispatchHeaderId])
REFERENCES [dbo].[OrderDispatches] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderDispatchProducts] CHECK CONSTRAINT [FK_OrderDispatchProducts_OrderDispatches_DispatchHeaderId]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderItems_Orders_OrderHeaderId] FOREIGN KEY([OrderHeaderId])
REFERENCES [dbo].[Orders] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK_OrderItems_Orders_OrderHeaderId]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Channels_ChannelCode] FOREIGN KEY([ChannelCode])
REFERENCES [dbo].[Channels] ([ChannelCode])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Channels_ChannelCode]
GO
ALTER TABLE [dbo].[TaskChatHistories]  WITH CHECK ADD  CONSTRAINT [FK_TaskChatHistories_Tasks_TaskId] FOREIGN KEY([TaskId])
REFERENCES [dbo].[Tasks] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TaskChatHistories] CHECK CONSTRAINT [FK_TaskChatHistories_Tasks_TaskId]
GO
ALTER TABLE [wms].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [wms].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [wms].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [wms].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [wms].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [wms].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [wms].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [wms].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [wms].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [wms].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [wms].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [wms].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [wms].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [wms].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [wms].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [wms].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [wms].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [wms].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
/****** Object:  StoredProcedure [dbo].[ProcCreateSchedule]    Script Date: 24/02/2025 6:41:37 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO







/* ***************************************************************
 Batchスケジュール登録 ScheduleDivision "00": 1回のみ "01":繰り返し登録
*************************************************************** */

CREATE PROCEDURE [dbo].[ProcCreateSchedule]
	@CompanyId As varchar(50)
AS
BEGIN

SET NOCOUNT ON;

	DECLARE @BatchId			VARCHAR(50)
	DECLARE @ScheduleDivision	VARCHAR(50)
	DECLARE @ScheduleTime		TIME
	DECLARE @StartTime			TIME
	DECLARE @EndTime			TIME
	DECLARE @EndDate			datetime
	DECLARE @StartDate			datetime
	DECLARE @SetDateTime		datetime
	DECLARE @TargetDayPlus	int
	DECLARE @TargetDate		varchar(50)
	DECLARE @ScheduleType		varchar(10)
	DECLARE @TargetDayOfWeek	VARCHAR(1)
	DECLARE @TargetDayOfMonth	VARCHAR(2)

	set @TargetDayPlus = 2
	set @TargetDate = CONVERT(varchar, dateadd(day,+@TargetDayPlus,getdate()), 111);
	set @ScheduleType = ISNULL((SELECT top 1 ScheduleType FROM BatchCalendars where date=@TargetDate),'0');
	set @TargetDayOfWeek = DATEPART(WEEKDAY, dateadd(day,+@TargetDayPlus,getdate()));
	set @TargetDayOfMonth = DATEPART(day, dateadd(day,+@TargetDayPlus,getdate()));
	

	DECLARE CurBatchScheduleDT CURSOR FOR
	select 
		 A.CompanyId
		,A.BatchId
		,A.StartTime
		,IsNULL(A.EndTime,convert(time,'23:59:59:999')) as EndTime
		,A.ScheduleDivision
		,A.ScheduleTime
	from BatchSchedules A
		 inner join Batches B
			on B.BatchId	= A.BatchId
			and B.IsDeleted = '0'
			and A.IsDeleted = '0'
			and
			(
				(ISNULL(DayOfWeek,'0') IN ('','0') and ISNULL(ScheduleType,'0') = @ScheduleType)
				or
				(ISNULL(DayOfWeek,'0') = @TargetDayOfWeek)
			)
			and
			(
				(ISNULL(DayOfMonth,'0') IN ('','0') and ISNULL(ScheduleType,'0') = @ScheduleType)
				or
				(ISNULL(DayOfMonth,'0') = @TargetDayOfMonth)
			)
			
	where a.CompanyId = @CompanyId

	BEGIN TRY
		BEGIN TRANSACTION;
			

			OPEN CurBatchScheduleDT
			FETCH NEXT 
			FROM 
			CurBatchScheduleDT
			INTO @CompanyId,@BatchId,@StartTime,@EndTime,@ScheduleDivision,@ScheduleTime

	
			WHILE @@FETCH_STATUS = 0
			BEGIN 
				
				/*当日のスケジュール作成*/
				--select	 @StartDate = convert(datetime,convert(varchar,convert(date,getDate() )) + ' ' + Left(convert(varchar,@StartTime),8))
				--		,@EndDate  = convert(datetime,convert(varchar,convert(date,getDate() )) + ' ' + Left(convert(varchar,@EndTime),8))
				/*翌日のスケジュール作成*/
				--select	 @StartDate = convert(datetime,convert(varchar,convert(date,getDate() + 1)) + ' ' + Left(convert(varchar,@StartTime),8))
				--		,@EndDate  = convert(datetime,convert(varchar,convert(date,getDate() + 1)) + ' ' + Left(convert(varchar,@EndTime),8))
				
				-- 2020/1/14 add start
				select	 @StartDate = convert(datetime,convert(varchar,convert(date,getDate() + @TargetDayPlus)) + ' ' + Left(convert(varchar,@StartTime),8))
						,@EndDate  = convert(datetime,convert(varchar,convert(date,getDate() + @TargetDayPlus)) + ' ' + Left(convert(varchar,@EndTime),8))
				-- 2020/1/14 add end

				set @SetDateTime = @StartDate;
				--set @SetDateTime = dateadd(mi,datediff(mi,convert(time,'00:00:00'),@ScheduleTime), @SetDateTime)

				WHILE @SetDateTime < @EndDate
				BEGIN

					--don't insert past Date
					IF @SetDateTime > getDate()
					BEGIN

						-- MERGE INTO
						--   [trn_task_schedule] AS XX
						-- USING
						--   (
						--   SELECT
						-- 	  @CompanyId AS CompanyId
						-- 	  ,@BatchId AS BatchId
						-- 	  ,@SetDateTime AS SetDateTime
              			-- 
						--   ) AS YY ON
						--   (
						-- 		XX.CompanyId = YY.CompanyId 
						-- 	AND XX.BatchId = YY.BatchId
						-- 	AND XX.schedule_datetime = YY.SetDateTime
						-- 	)
						-- WHEN NOT MATCHED THEN

							INSERT into TaskSchedules ([ScheduleDatetime]
									   ,[CompanyId]
									   ,[BatchId]
									   ,[Priority]
									   ,[IsFailed]
									   ,[IsStopped]
									   ,[IsBatchInRequest]
									   ,[CreateOperatorId]
									   ,[CreateAt]
									   ,[UpdateOperatorId]
									   ,[UpdateAt]
									   ,[IsDeleted])
								 VALUES
									   (@SetDateTime
									   ,@CompanyId
									   ,@BatchId
									   ,'0'
									   ,'0'
									   ,'0'
									   ,'0'
									   ,'9099999'
									   ,getDate()
									   ,'9099999'
									   ,getDate()
									   ,'0'
									   );
					END

					IF @ScheduleDivision = '00'
					BEGIN
						SET @SetDateTime = DATEADD(year, 1, @SetDateTime);
					END
					Else
					BEGIN
						set @SetDateTime = dateadd(mi,datediff(mi,convert(time,'00:00:00'),@ScheduleTime), @SetDateTime)
					END
				END


				FETCH NEXT FROM CurBatchScheduleDT 
				INTO @CompanyId,@BatchId,@StartTime,@EndTime,@ScheduleDivision,@ScheduleTime
				;
			END
		 COMMIT TRANSACTION
		 
		 CLOSE CurBatchScheduleDT;
		 DEALLOCATE CurBatchScheduleDT;

	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION
		SELECT  
         ERROR_NUMBER() AS ErrorNumber  
        ,ERROR_SEVERITY() AS ErrorSeverity  
        ,ERROR_STATE() AS ErrorState  
        ,ERROR_PROCEDURE() AS ErrorProcedure  
        ,ERROR_LINE() AS ErrorLine  
        ,ERROR_MESSAGE() AS ErrorMessage;  
		
		IF CURSOR_STATUS('global','CurBatchScheduleDT') > 0
			BEGIN
				CLOSE CurBatchScheduleDT;
				DEALLOCATE CurBatchScheduleDT;
			END


		RETURN 1

	END CATCH

	RETURN 0

END
GO
/****** Object:  StoredProcedure [dbo].[sp_dashboardGetPutawayData]    Script Date: 24/02/2025 6:41:37 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		cong.nguyen
-- Create date: 2025-02-04
-- Description:	Get data putaway for dashboard.
-- =============================================
CREATE PROCEDURE [dbo].[sp_dashboardGetPutawayData]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    with cteGetData as (
			SELECT 
				wpa.TenantId, 
				wpa.TransDate, 
				--SUM(wpl.JournalQty) AS TotalJournalQty
				COALESCE(SUM(wpl.JournalQty), 0) AS TotalJournalQty,
				COALESCE(SUM(wpl.TransQty), 0)  AS TotalTransQty,
				COALESCE(SUM(wpl.JournalQty) - SUM(wpl.TransQty), 0) AS TotalRemainQty
			FROM wms.WarehousePutAways wpa
			JOIN wms.WarehousePutAwayLines wpl 
				ON wpa.PutAwayNo = wpl.PutAwayNo
			GROUP BY wpa.TenantId, wpa.TransDate
			),
			cteGetDataOpen as (
				SELECT 
					wpa.TenantId, 
					wpa.TransDate, 
					--SUM(wpl.JournalQty) AS TotalJournalQty
					COALESCE(SUM(wpl.JournalQty), 0) AS TotalJournalQty,
					COALESCE(SUM(wpl.TransQty), 0)   AS TotalTransQty,
					COALESCE(SUM(wpl.JournalQty) - SUM(wpl.TransQty), 0) AS TotalRemainQty
				FROM wms.WarehousePutAways wpa
				JOIN wms.WarehousePutAwayLines wpl 
					ON wpa.PutAwayNo = wpl.PutAwayNo
				where wpa.Status = 0
				GROUP BY wpa.TenantId, wpa.TransDate
				),
			cteGetDataComplete as (
				SELECT 
					wpa.TenantId, 
					wpa.TransDate, 
					--SUM(wpl.JournalQty) AS TotalJournalQty
					COALESCE(SUM(wpl.JournalQty), 0) AS TotalJournalQty,
					COALESCE(SUM(wpl.TransQty), 0) AS TotalTransQty,
					COALESCE(SUM(wpl.JournalQty) - SUM(wpl.TransQty), 0) AS TotalRemainQty
				FROM wms.WarehousePutAways wpa
				JOIN wms.WarehousePutAwayLines wpl 
					ON wpa.PutAwayNo = wpl.PutAwayNo
				where wpa.Status = 1
				GROUP BY wpa.TenantId, wpa.TransDate
				)

		select _cte.TransDate [Period]
			,_com.FullName Tenant
			,CONCAT(_cte.TotalTransQty,'/',_cte.TotalJournalQty) ExpectedStock

			,case
				when _cteOpen.TotalTransQty is not null then CONCAT(_cteOpen.TotalRemainQty,'/',_cteOpen.TotalJournalQty)
				else null
			end RemainingNumber

			--,CONCAT(ROUND(_cte.TotalTransQty / NULLIF(_cte.TotalJournalQty, 0) * 100, 2),'%') ProgressRateString
			,CAST(ROUND(_cte.TotalTransQty / NULLIF(_cte.TotalJournalQty, 0) * 100, 2) as float) ProgressRate --float
			,CAST(COALESCE(_cte.TotalJournalQty, 0) as int) TotalJournalQty --int
			,CAST(_cte.TotalTransQty as int) TotalTransQty --int
			,CAST(_cte.TotalRemainQty as int) TotalRemainingNumber --int
			,CAST(ROUND(NULLIF(_cteComplete.TotalTransQty,0)/8, 1) as float) Productivity --just only putaway completed
			,NULLIF(_cteOpen.TotalJournalQty, 0 ) TotalJournalQtyOpen --int
			,NULLIF(_cteOpen.TotalTransQty, 0) TotalTransQtyOpen --int
			,NULLIF(_cteOpen.TotalRemainQty, 0) TotalRemainingNumberOpen --int
			,NULLIF(_cteComplete.TotalJournalQty, 0) TotalJournalQtyComplete --int
			,NULLIF(_cteComplete.TotalTransQty, 0) TotalTransQtyComplete --int
			,NULLIF(_cteComplete.TotalRemainQty, 0) TotalRemainingNumberComplete --int
		from cteGetData _cte
			left join dbo.Companies _com on _com.AuthPTenantId = _cte.TenantId
			left join cteGetDataOpen _cteOpen on _cteOpen.TenantId = _cte.TenantId and _cteOpen.TransDate = _cte.TransDate
			left join cteGetDataComplete _cteComplete on _cteComplete.TenantId = _cte.TenantId and _cteComplete.TransDate = _cte.TransDate
		order by _cte.TenantId, _cte.TransDate desc
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getImageStorageByLiatResources]    Script Date: 24/02/2025 6:41:37 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<WMS, cong.nguyen>
-- Create date: <C2024-11-06>
-- Description:	<Get Inventory information flow product code>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getImageStorageByLiatResources] 
	-- Add the parameters for the stored procedure here
	@resourceId nvarchar(max) = null--list product code.
	,@type int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @resourceIdList TABLE (ResourceId nvarchar(100));
	INSERT INTO @resourceIdList
	SELECT value 
	FROM STRING_SPLIT(@resourceId, ',')
	WHERE value IS NOT NULL;
 
	select * 
	from wms.ImageStorage
	where [Type] = @type
		and ResourcesId IN (SELECT ResourceId FROM @resourceIdList)

END
GO
/****** Object:  StoredProcedure [dbo].[sp_getInventoryInfo]    Script Date: 24/02/2025 6:41:37 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<WMS, cong.nguyen>
-- Create date: <C2024-11-06>
-- Description:	<Get Inventory information flow product code>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getInventoryInfo] 
	-- Add the parameters for the stored procedure here
	@productCode nvarchar(100) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(@productCode is null) -- get all
		begin
			 select * from view_wmsGetInventoryInfo
		end
	else --Get 1 productCode
		begin
			select * from view_wmsGetInventoryInfo where productcode = @productCode
		end
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getInventoryInfoFlowBinLot]    Script Date: 24/02/2025 6:41:37 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<WMS, cong.nguyen>
-- Create date: <C2024-11-06>
-- Description:	<Get Inventory information flow product code>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getInventoryInfoFlowBinLot] 
	-- Add the parameters for the stored procedure here
	@productCode nvarchar(max) = null--list product code.
	,@companyId int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(@productCode is null) -- get all
		begin
			 select * 
			 from view_wmsGetInventoryInfoFlowBinLotTenant
			 where TenantId = @companyId or @companyId = 0
		end
	else --Get 1 productCode
		begin
			DECLARE @ProductCodeList TABLE (ProductCodeItem nvarchar(100));
			INSERT INTO @ProductCodeList
			SELECT value 
			FROM STRING_SPLIT(@productCode, ',')
			WHERE value IS NOT NULL;
 
			select * 
			from view_wmsGetInventoryInfoFlowBinLotTenant
			where (TenantId = @companyId or @companyId = 0)
				and ProductCode IN (SELECT ProductCodeItem FROM @ProductCodeList)
		end
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getInventoryInfoFlowBinLotOld]    Script Date: 24/02/2025 6:41:37 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<WMS, cong.nguyen>
-- Create date: <C2024-11-06>
-- Description:	<Get Inventory information flow product code>
-- =============================================
CREATE PROCEDURE [dbo].[sp_getInventoryInfoFlowBinLotOld] 
	-- Add the parameters for the stored procedure here
	@productCode nvarchar(max) = null--list product code.
	,@companyId int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(@productCode is null) -- get all
		begin
			 select * 
			 from view_wmsGetInventoryInfoFlowBinLot
			 where TenantId = @companyId or @companyId = 0
		end
	else --Get 1 productCode
		begin
			DECLARE @ProductCodeList TABLE (ProductCodeItem nvarchar(100));
			INSERT INTO @ProductCodeList
			SELECT value 
			FROM STRING_SPLIT(@productCode, ',')
			WHERE value IS NOT NULL;
 
			select * 
			from view_wmsGetInventoryInfoFlowBinLot 
			where (TenantId = @companyId or @companyId = 0)
				and ProductCode IN (SELECT ProductCodeItem FROM @ProductCodeList)
		end
END
GO
/****** Object:  StoredProcedure [dbo].[sp_getWarehouseShipments]    Script Date: 24/02/2025 6:41:37 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[sp_getWarehouseShipments]
AS
BEGIN
select 
	ws.Id
	,ws.ShipmentNo
	,ws.ShipmentType
	,ws.ShippingAddress
	,ws.TenantId
	,ws.TenantName
	,wsl.PackedQty
	,wsl.ProductCode
from wms.WarehouseShipments ws 
left join wms.WarehouseShipmentLines wsl 
on ws.ShipmentNo = wsl.ShipmentNo and wsl.IsDeleted = 0
where ws.IsDeleted = 0
END
GO
/****** Object:  StoredProcedure [dbo].[sp_packingGetShipmentByShipmentNo]    Script Date: 24/02/2025 6:41:37 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<WMS, cong.nguyen>
-- Create date: <C2024-11-15>
-- Description:	<Get data master for packing list>
-- =============================================
CREATE PROCEDURE [dbo].[sp_packingGetShipmentByShipmentNo] 
	-- Add the parameters for the stored procedure here
	@shipmentNo nvarchar(100) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	select  _WarehouseShipmentLines.*
		,_WarehouseShipments.Id IdShipment
		,_WarehouseShipments.SalesNo
		,_WarehouseShipments.TenantId

		--,_location.LocationName LocationOfShipment
		--,_WarehouseShipments.[Location]

		,_WarehouseShipments.PlanShipDate
		,_WarehouseShipments.[Status] StatusOfShipment
		,_WarehouseShipments.CreateAt CreateAtOfShipment
		,_WarehouseShipments.CreateOperatorId CreateOperatorIdOfShipment
		,_WarehouseShipments.UpdateAt UpdateAtOfShipment
		,_WarehouseShipments.UpdateOperatorId UpdateOperatorIdOfShipment
				
		--,_user.UserName PersonInCharge
		,_WarehouseShipments.PersonInCharge 

		,_WarehouseShipments.ShippingCarrierCode
		,_WarehouseShipments.ShippingBoxesId
		,_WarehouseShipments.ShippingBoxesName
		,_WarehouseShipments.ShippingAddress
		,_WarehouseShipments.Telephone
		,_WarehouseShipments.TrackingNo
		,_WarehouseShipments.Email
		,_WarehouseShipments.BinId 
		,_WarehouseShipments.[Address] 
		,_WarehouseShipments.PickingNo 
		,_WarehouseShipments.TenantName
		,_WarehouseShipments.LocationName
		,_WarehouseShipments.PersonInChargeName
		,_WarehousePickingList.CreateAt PickedDate
		,_Products.ProductName
		,_Units.UnitName
		,_pickingStaging.ProductQRCode
		,_carrier.PrinterName
		,_orderDispatches.LabelFilePath
		,_orderDispatches.LabelFileExtension
	from wms.WarehouseShipmentLines _WarehouseShipmentLines
		left join wms.WarehouseShipments _WarehouseShipments on _WarehouseShipments.ShipmentNo = _WarehouseShipmentLines.ShipmentNo
		--left join wms.Locations _location on _location.Id = _WarehouseShipmentLines.[Location]
		--left join wms.AspNetUsers _user on _user.Id = _WarehouseShipments.PersonInCharge
		left join wms.WarehousePickingList _WarehousePickingList on _WarehousePickingList.PickNo = _WarehouseShipments.PickingNo
		left join dbo.Products _Products on _Products.ProductCode = _WarehouseShipmentLines.ProductCode
		left join wms.Units _Units on _Units.Id = _WarehouseShipmentLines.UnitId
		left join wms.WarehousePickingStaging _pickingStaging on _pickingStaging.ShipmentLineId = _WarehouseShipmentLines.Id
		left join wms.ShippingCarriers _carrier on _carrier.ShippingCarrierCode = _WarehouseShipments.ShippingCarrierCode
		left join dbo.OrderDispatches _orderDispatches on _orderDispatches.TrackingNo = _WarehouseShipments.TrackingNo
	where  _WarehouseShipments.ShipmentNo = @shipmentNo
		and (_WarehouseShipments.[Status] in (3,4,5))
END
GO
/****** Object:  StoredProcedure [dbo].[sp_packingGetShipmentByTrackingNo]    Script Date: 24/02/2025 6:41:37 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<WMS, cong.nguyen>
-- Create date: <C2024-11-15>
-- Description:	<Get data master for packing list>
-- =============================================
CREATE PROCEDURE [dbo].[sp_packingGetShipmentByTrackingNo] 
	-- Add the parameters for the stored procedure here
	@TrackingNo nvarchar(100) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	select  _WarehouseShipmentLines.*
		,_WarehouseShipments.Id IdShipment
		,_WarehouseShipments.SalesNo
		,_WarehouseShipments.TenantId

		--,_location.LocationName LocationOfShipment
		--,_WarehouseShipments.[Location]

		,_WarehouseShipments.PlanShipDate
		,_WarehouseShipments.[Status] StatusOfShipment
		,_WarehouseShipments.CreateAt CreateAtOfShipment
		,_WarehouseShipments.CreateOperatorId CreateOperatorIdOfShipment
		,_WarehouseShipments.UpdateAt UpdateAtOfShipment
		,_WarehouseShipments.UpdateOperatorId UpdateOperatorIdOfShipment
				
		--,_user.UserName PersonInCharge
		,_WarehouseShipments.PersonInCharge 

		,_WarehouseShipments.ShippingCarrierCode
		,_WarehouseShipments.ShippingBoxesId
		,_WarehouseShipments.ShippingBoxesName
		,_WarehouseShipments.ShippingAddress
		,_WarehouseShipments.Telephone
		,_WarehouseShipments.TrackingNo
		,_WarehouseShipments.Email
		,_WarehouseShipments.BinId 
		,_WarehouseShipments.[Address] 
		,_WarehouseShipments.PickingNo 
		,_WarehouseShipments.TenantName
		,_WarehouseShipments.LocationName
		,_WarehouseShipments.PersonInChargeName
		,_WarehousePickingList.CreateAt PickedDate
		,_Products.ProductName
		,_Units.UnitName
		,_pickingStaging.ProductQRCode
		,_carrier.PrinterName
		,_orderDispatches.LabelFilePath
		,_orderDispatches.LabelFileExtension
	from wms.WarehouseShipmentLines _WarehouseShipmentLines
		left join wms.WarehouseShipments _WarehouseShipments on _WarehouseShipments.ShipmentNo = _WarehouseShipmentLines.ShipmentNo
		--left join wms.Locations _location on _location.Id = _WarehouseShipmentLines.[Location]
		--left join wms.AspNetUsers _user on _user.Id = _WarehouseShipments.PersonInCharge
		left join wms.WarehousePickingList _WarehousePickingList on _WarehousePickingList.PickNo = _WarehouseShipments.PickingNo
		left join dbo.Products _Products on _Products.ProductCode = _WarehouseShipmentLines.ProductCode
		left join wms.Units _Units on _Units.Id = _WarehouseShipmentLines.UnitId
		left join wms.WarehousePickingStaging _pickingStaging on _pickingStaging.ShipmentLineId = _WarehouseShipmentLines.Id
		left join wms.ShippingCarriers _carrier on _carrier.ShippingCarrierCode = _WarehouseShipments.ShippingCarrierCode
		left join dbo.OrderDispatches _orderDispatches on _orderDispatches.TrackingNo = _WarehouseShipments.TrackingNo
	where  _WarehouseShipments.TrackingNo = @TrackingNo
		and (_WarehouseShipments.[Status] = 3 or _WarehouseShipments.[Status] = 4)
END
GO
/****** Object:  StoredProcedure [dbo].[sp_packingListGetDataMaster]    Script Date: 24/02/2025 6:41:37 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<WMS, cong.nguyen>
-- Create date: <C2024-10-14>
-- Description:	<Get data master for packing list>
-- =============================================
CREATE PROCEDURE [dbo].[sp_packingListGetDataMaster] 
	-- Add the parameters for the stored procedure here
	@ShipmentNo nvarchar(50) = null
	,@PlanShipDateFrom nvarchar(50) = null
	,@PlanShipDateTo nvarchar(50) =null
	,@DeliveryLocation nvarchar(500) = null
	,@Bin nvarchar(10) = null
	,@Status int = null
	,@TrackingNo nvarchar(100) = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(@Status < 8) -- get shipment base on status
		begin
			select  _WarehouseShipmentLines.*

				,_WarehouseShipments.Id IdShipment
				,_WarehouseShipments.SalesNo
				,_WarehouseShipments.TenantId

				--,_location.LocationName LocationOfShipment
				--,_WarehouseShipments.[Location]

				,_WarehouseShipments.PlanShipDate
				,_WarehouseShipments.[Status] StatusOfShipment
				,_WarehouseShipments.CreateAt CreateAtOfShipment
				,_WarehouseShipments.CreateOperatorId CreateOperatorIdOfShipment
				,_WarehouseShipments.UpdateAt UpdateAtOfShipment
				,_WarehouseShipments.UpdateOperatorId UpdateOperatorIdOfShipment
				
				--,_user.UserName PersonInCharge
				--,_WarehouseShipments.PersonInCharge 

				,_WarehouseShipments.ShippingCarrierCode
				,_WarehouseShipments.ShippingAddress
				,_WarehouseShipments.Telephone
				,_WarehouseShipments.TrackingNo
				,_WarehouseShipments.Email
				,_WarehouseShipments.BinId 
				,_WarehouseShipments.[Address] 
				,_WarehouseShipments.PickingNo 
				,_WarehouseShipments.TenantName
				,_WarehouseShipments.LocationName
				,_WarehouseShipments.PersonInChargeName
				,_WarehousePickingList.PickedDate
				,_Products.ProductName
				,_Units.UnitName
			from wms.WarehouseShipmentLines _WarehouseShipmentLines
				left join wms.WarehouseShipments _WarehouseShipments on _WarehouseShipments.ShipmentNo = _WarehouseShipmentLines.ShipmentNo
				--left join wms.Locations _location on _location.Id = _WarehouseShipmentLines.[Location]
				--left join wms.AspNetUsers _user on _user.Id = _WarehouseShipments.PersonInCharge
				left join wms.WarehousePickingList _WarehousePickingList on _WarehousePickingList.PickNo = _WarehouseShipments.PickingNo
				left join dbo.Products _Products on _Products.ProductCode = _WarehouseShipmentLines.ProductCode
				left join wms.Units _Units on _Units.Id = _WarehouseShipmentLines.UnitId
			where  (_WarehouseShipmentLines.ShipmentNo = @ShipmentNo OR @ShipmentNo IS NULL)
				and (_WarehouseShipments.PlanShipDate >= @PlanShipDateFrom OR @PlanShipDateFrom IS NULL)
				and (_WarehouseShipments.PlanShipDate <= @PlanShipDateTo OR @PlanShipDateTo IS NULL)
				AND (_WarehouseShipments.[Location] = @DeliveryLocation OR @DeliveryLocation IS NULL)
				AND (_WarehouseShipmentLines.Bin = @Bin OR @Bin IS NULL)
				and (_WarehouseShipments.TrackingNo = @TrackingNo or @TrackingNo is null)
				AND (_WarehouseShipments.[Status] = 3 or _WarehouseShipments.[Status] = 4 or _WarehouseShipments.[Status] = 5)
				AND (_WarehouseShipments.[Status] = @Status OR @Status IS NULL)
		end
	else --Get all status shipment
		begin
			select  _WarehouseShipmentLines.*

				,_WarehouseShipments.Id IdShipment
				,_WarehouseShipments.SalesNo
				,_WarehouseShipments.TenantId

				--,_location.LocationName LocationOfShipment
				--,_WarehouseShipments.[Location]

				,_WarehouseShipments.PlanShipDate
				,_WarehouseShipments.[Status] StatusOfShipment
				,_WarehouseShipments.CreateAt CreateAtOfShipment
				,_WarehouseShipments.CreateOperatorId CreateOperatorIdOfShipment
				,_WarehouseShipments.UpdateAt UpdateAtOfShipment
				,_WarehouseShipments.UpdateOperatorId UpdateOperatorIdOfShipment
				
				--,_user.UserName PersonInCharge
				,_WarehouseShipments.PersonInCharge 

				,_WarehouseShipments.ShippingCarrierCode
				,_WarehouseShipments.ShippingAddress
				,_WarehouseShipments.Telephone
				,_WarehouseShipments.TrackingNo
				,_WarehouseShipments.Email
				,_WarehouseShipments.BinId 
				,_WarehouseShipments.[Address] 
				,_WarehouseShipments.PickingNo 
				,_WarehouseShipments.TenantName
				,_WarehouseShipments.LocationName
				,_WarehouseShipments.PersonInChargeName
				,_WarehousePickingList.PickedDate
				,_Products.ProductName
				,_Units.UnitName
			from wms.WarehouseShipmentLines _WarehouseShipmentLines
				left join wms.WarehouseShipments _WarehouseShipments on _WarehouseShipments.ShipmentNo = _WarehouseShipmentLines.ShipmentNo
				--left join wms.Locations _location on _location.Id = _WarehouseShipmentLines.[Location]
				--left join wms.AspNetUsers _user on _user.Id = _WarehouseShipments.PersonInCharge
				left join wms.WarehousePickingList _WarehousePickingList on _WarehousePickingList.PickNo = _WarehouseShipments.PickingNo
				left join dbo.Products _Products on _Products.ProductCode = _WarehouseShipmentLines.ProductCode
				left join wms.Units _Units on _Units.Id = _WarehouseShipmentLines.UnitId
			where  (_WarehouseShipmentLines.ShipmentNo = @ShipmentNo OR @ShipmentNo IS NULL)
				and (_WarehouseShipments.PlanShipDate >= @PlanShipDateFrom OR @PlanShipDateFrom IS NULL)
				and (_WarehouseShipments.PlanShipDate <= @PlanShipDateTo OR @PlanShipDateTo IS NULL)
				AND (_WarehouseShipments.[Location] = @DeliveryLocation OR @DeliveryLocation IS NULL)
				AND (_WarehouseShipmentLines.Bin = @Bin OR @Bin IS NULL)
				and (_WarehouseShipments.TrackingNo = @TrackingNo or @TrackingNo is null)
				AND (_WarehouseShipments.[Status] = 3 or _WarehouseShipments.[Status] = 4 or _WarehouseShipments.[Status] = 5)
		end
END
GO
/****** Object:  StoredProcedure [dbo].[sp_productCheckExits]    Script Date: 24/02/2025 6:41:37 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<WMS, cong.nguyen>
-- Create date: <C2024-10-14>
-- Description:	<Get data master for packing list>
-- =============================================
CREATE PROCEDURE [dbo].[sp_productCheckExits] 
	-- Add the parameters for the stored procedure here
	@Unit int = null
	,@CategoryId int = null
	,@SupplierId int = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select  _product.*
	from dbo.products _product
	where  (_product.UnitId = @Unit OR @Unit is null)
		and (_product.CategoryId = @categoryId OR @categoryId = 0)
		and (_product.SupplierId = @SupplierId OR @SupplierId = 0)
END
GO
/****** Object:  StoredProcedure [dbo].[sp_productGetDataMaster]    Script Date: 24/02/2025 6:41:37 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<WMS, cong.nguyen>
-- Create date: <C2024-10-14>
-- Description:	<Get data master for packing list>
-- =============================================
CREATE PROCEDURE [dbo].[sp_productGetDataMaster] 
	-- Add the parameters for the stored procedure here
	@id int = 0
	,@ProductCode nvarchar(100) = null
	,@CategoryId int = 0
	,@ProductStatus int = 3--All
	,@TenantId int = 0 --All
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(@ProductStatus >= 3) -- get shipment base on status
		begin
			select  _product.*
				,_unit.UnitName
				,_supplier.SupplierName
				,_category.CategoryName
				,_category.SlipDeliveryPrinting
				,_comp.FullName TenantName
				,CAST(_v.InventoryStock as int) InventoryStock
				,CAST(_v.OnOrder as int) OnOrder
				,CAST(_v.InventoryAvailable as int) InventoryAvailable
			from dbo.products _product
				left join wms.Units _unit on _unit.Id = _product.UnitId
				left join dbo.Suppliers _supplier on _supplier.Id = _product.SupplierId
				left join dbo.ProductCategories _category on _category.Id = _product.CategoryId
				left join dbo.Companies _comp on _comp.AuthPTenantId = _product.CompanyId
				left join dbo.view_wmsGetInventoryInfo _v on _v.ProductCode = _product.ProductCode and _v.CompanyId = _product.CompanyId
			where  (_product.ProductCode like '%'+ @ProductCode +'%' OR @ProductCode is null)
				and (_product.CategoryId = @categoryId OR @categoryId = 0)
				and (_product.Id = @id OR @id = 0)
				and (_product.CompanyId = @TenantId OR @TenantId = 0)
				and _product.IsDeleted = 0
		end
	else --Get all status shipment
		begin
			select  _product.*
				,_unit.UnitName
				,_supplier.SupplierName
				,_category.CategoryName
				,_category.SlipDeliveryPrinting
				,_comp.FullName TenantName
				,CAST(_v.InventoryStock as int) InventoryStock
				,CAST(_v.OnOrder as int) OnOrder
				,CAST(_v.InventoryAvailable as int) InventoryAvailable
			from dbo.products _product
				left join wms.Units _unit on _unit.Id = _product.UnitId
				left join dbo.Suppliers _supplier on _supplier.Id = _product.SupplierId
				left join dbo.ProductCategories _category on _category.Id = _product.CategoryId
				left join dbo.Companies _comp on _comp.AuthPTenantId = _product.CompanyId
				left join dbo.view_wmsGetInventoryInfo _v on _v.ProductCode = _product.ProductCode and _v.CompanyId = _product.CompanyId
			where  (_product.ProductCode like '%'+ @ProductCode +'%' OR @ProductCode is null)
				and (_product.CategoryId = @categoryId OR @categoryId = 0)
				and (_product.Id = @id OR @id = 0)
				and (_product.ProductStatus = @ProductStatus)
				and (_product.CompanyId = @TenantId OR @TenantId = 0)
				and _product.IsDeleted = 0
		end
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateRepeatPurchaseType]    Script Date: 24/02/2025 6:41:37 am ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateRepeatPurchaseType]
    @CompanyId NVARCHAR(MAX)
AS
BEGIN
    WITH CustomerOrderCount AS (
        SELECT CustomerId,
               COUNT(*) AS OrderCount
        FROM [FBT_DEV2].[dbo].[SalesData]
        WHERE CompanyId = @CompanyId
        GROUP BY CustomerId
    )
    UPDATE sd
    SET sd.RepeatPurchase = CASE
                                WHEN coc.OrderCount > 1 THEN '1'
                                ELSE '0'
                            END
    FROM [FBT_DEV2].[dbo].[SalesData] sd
    INNER JOIN CustomerOrderCount coc
        ON sd.CustomerId = coc.CustomerId
    WHERE sd.CompanyId = @CompanyId AND sd.Type = 'order_header';
END
GO
USE [master]
GO
ALTER DATABASE [FBT_DEV2] SET  READ_WRITE 
GO
