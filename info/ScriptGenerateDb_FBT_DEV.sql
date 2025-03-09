
ALTER DATABASE [FBT_DEV] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [FBT_DEV].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [FBT_DEV] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [FBT_DEV] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [FBT_DEV] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [FBT_DEV] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [FBT_DEV] SET ARITHABORT OFF 
GO
ALTER DATABASE [FBT_DEV] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [FBT_DEV] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [FBT_DEV] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [FBT_DEV] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [FBT_DEV] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [FBT_DEV] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [FBT_DEV] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [FBT_DEV] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [FBT_DEV] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [FBT_DEV] SET  ENABLE_BROKER 
GO
ALTER DATABASE [FBT_DEV] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [FBT_DEV] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [FBT_DEV] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [FBT_DEV] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [FBT_DEV] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [FBT_DEV] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [FBT_DEV] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [FBT_DEV] SET RECOVERY FULL 
GO
ALTER DATABASE [FBT_DEV] SET  MULTI_USER 
GO
ALTER DATABASE [FBT_DEV] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [FBT_DEV] SET DB_CHAINING OFF 
GO
ALTER DATABASE [FBT_DEV] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [FBT_DEV] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [FBT_DEV] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [FBT_DEV] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [FBT_DEV] SET QUERY_STORE = OFF
GO
USE [FBT_DEV]
GO
/****** Object:  Schema [authp]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE SCHEMA [authp]
GO
/****** Object:  Table [authp].[AuthUsers]    Script Date: 9/9/2024 2:37:22 AM ******/
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
/****** Object:  Table [authp].[RefreshTokens]    Script Date: 9/9/2024 2:37:22 AM ******/
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
/****** Object:  Table [authp].[RoleToPermissions]    Script Date: 9/9/2024 2:37:22 AM ******/
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
/****** Object:  Table [authp].[RoleToPermissionsTenant]    Script Date: 9/9/2024 2:37:22 AM ******/
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
/****** Object:  Table [authp].[Tenants]    Script Date: 9/9/2024 2:37:22 AM ******/
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
/****** Object:  Table [authp].[UserToRoles]    Script Date: 9/9/2024 2:37:22 AM ******/
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
/****** Object:  Table [dbo].[__AppDbContextMigrationHistory]    Script Date: 9/9/2024 2:37:22 AM ******/
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
/****** Object:  Table [dbo].[__AuthPermissionsMigrationHistory]    Script Date: 9/9/2024 2:37:22 AM ******/
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
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 9/9/2024 2:37:22 AM ******/
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
/****** Object:  Table [dbo].[ApiCodes]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApiCodes](
	[Code] [nvarchar](450) NOT NULL,
	[Message] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
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
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 9/9/2024 2:37:22 AM ******/
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
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 9/9/2024 2:37:22 AM ******/
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
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 9/9/2024 2:37:22 AM ******/
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
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 9/9/2024 2:37:22 AM ******/
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
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 9/9/2024 2:37:22 AM ******/
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
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 9/9/2024 2:37:22 AM ******/
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
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 9/9/2024 2:37:22 AM ******/
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
/****** Object:  Table [dbo].[ChannelMasters]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChannelMasters](
	[ChannelMasterCode] [nvarchar](450) NOT NULL,
	[ChannelMasterName] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
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
/****** Object:  Table [dbo].[Channels]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Channels](
	[ChannelCode] [nvarchar](450) NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[ChannelName] [nvarchar](max) NULL,
	[ChannelMasterCode] [nvarchar](450) NULL,
	[ChannelMasterName] [nvarchar](max) NULL,
	[PaymentMethod] [nvarchar](max) NULL,
	[BillCalcType] [nvarchar](max) NULL,
	[Currency] [nvarchar](max) NULL,
	[LanguageCode] [nvarchar](max) NULL,
	[IsActive] [bit] NULL,
	[DataKey] [varchar](250) NULL,
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
/****** Object:  Table [dbo].[Companies]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Companies](
	[CompanyTenantId] [uniqueidentifier] NOT NULL,
	[FullName] [nvarchar](max) NULL,
	[ShortName] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[AuthPTenantId] [int] NOT NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED 
(
	[CompanyTenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CountryMasters]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CountryMasters](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CountryIsoNumeric] [nvarchar](max) NULL,
	[CountryIso2] [nvarchar](max) NULL,
	[CountryIso3] [nvarchar](max) NULL,
	[CountryNameEn] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
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
/****** Object:  Table [dbo].[Currencies]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Currencies](
	[CurrencyCode] [nvarchar](450) NOT NULL,
	[Country] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[IsDisplayCurrency] [bit] NOT NULL,
	[DataKey] [varchar](250) NULL,
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
/****** Object:  Table [dbo].[CurrencyPairSettings]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CurrencyPairSettings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CurrencyCodeFrom] [nvarchar](max) NULL,
	[CurrencyCodeTo] [nvarchar](max) NULL,
	[RateDecimalPlaces] [int] NOT NULL,
	[DataKey] [varchar](250) NULL,
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
/****** Object:  Table [dbo].[ExchangeRates]    Script Date: 9/9/2024 2:37:22 AM ******/
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
	[DataKey] [varchar](250) NULL,
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
/****** Object:  Table [dbo].[OrderItems]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderHeaderId] [int] NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[OrderId] [nvarchar](max) NULL,
	[LineNo] [int] NULL,
	[SalesProductCode] [nvarchar](max) NULL,
	[SalesProductName] [nvarchar](max) NULL,
	[Quantity] [int] NULL,
	[PurchaseUnitPrice] [float] NULL,
	[DeclaredValue] [float] NULL,
	[Currency] [nvarchar](max) NULL,
	[ReturnQuantity] [int] NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_OrderItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderReturnItems]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderReturnItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderHeaderId] [int] NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[OrderId] [nvarchar](max) NULL,
	[SalesProductCode] [nvarchar](max) NULL,
	[SalesProductName] [nvarchar](max) NULL,
	[ReturnQuantity] [int] NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_OrderReturnItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[WarehouseCode] [nvarchar](max) NULL,
	[ChannelCode] [nvarchar](450) NULL,
	[OrderId] [nvarchar](max) NULL,
	[OrderStatus] [nvarchar](max) NULL,
	[OrderDate] [datetime2](7) NULL,
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
	[BillCity] [nvarchar](max) NULL,
	[BillState] [nvarchar](max) NULL,
	[BillCountry] [nvarchar](max) NULL,
	[BillMail] [nvarchar](max) NULL,
	[OrderDeliveryCompany] [nvarchar](max) NULL,
	[Remarks] [nvarchar](max) NULL,
	[InternalRemarks] [nvarchar](max) NULL,
	[ReturnStatus] [nvarchar](max) NULL,
	[ShippedDate] [datetime2](7) NULL,
	[IsDeltaData] [bit] NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderStatuses]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderStatuses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StatusOrder] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [int] NOT NULL,
	[DataKey] [varchar](250) NULL,
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
/****** Object:  Table [dbo].[ProductBundles]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductBundles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentProductId] [int] NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[WarehouseCode] [nvarchar](max) NULL,
	[BundleCode] [nvarchar](max) NULL,
	[SalesProductCode] [nvarchar](max) NULL,
	[Quantity] [int] NOT NULL,
	[PriceRate] [float] NULL,
	[DataKey] [varchar](250) NULL,
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
/****** Object:  Table [dbo].[Products]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[WarehouseCode] [nvarchar](max) NULL,
	[SalesProductCode] [nvarchar](max) NULL,
	[SalesName] [nvarchar](max) NULL,
	[CustomerProductCode] [nvarchar](max) NULL,
	[ThirdplSystemSku] [nvarchar](max) NULL,
	[JanCode] [nvarchar](max) NULL,
	[HsCode] [nvarchar](max) NULL,
	[Weight] [float] NULL,
	[VendorCode] [nvarchar](max) NULL,
	[ProductType] [nvarchar](max) NULL,
	[RegularPrice] [float] NULL,
	[MaxDiscount] [int] NULL,
	[SalesPrice] [float] NULL,
	[Currency] [nvarchar](max) NULL,
	[CategoryId] [nvarchar](max) NULL,
	[CategoryName] [nvarchar](max) NULL,
	[ProductStatus] [nvarchar](max) NULL,
	[CountryOfOrigin] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[IsBundle] [bit] NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductStatuses]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductStatuses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StatusProduct] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
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
/****** Object:  Table [dbo].[ProductStocks]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductStocks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
	[WarehouseCode] [nvarchar](max) NULL,
	[SalesProductCode] [nvarchar](max) NULL,
	[StockQuantity] [int] NULL,
	[StockThreshold] [int] NULL,
	[DataKey] [varchar](250) NULL,
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
/****** Object:  Table [dbo].[SalesData]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalesData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](max) NULL,
	[company_id] [nvarchar](max) NULL,
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
	[DataKey] [varchar](250) NULL,
 CONSTRAINT [PK_SalesData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ShippingCountries]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShippingCountries](
	[CountryCode] [nvarchar](450) NOT NULL,
	[CountryName] [nvarchar](max) NULL,
	[DisplayName] [nvarchar](max) NULL,
	[DisplayOrder] [int] NULL,
	[DataKey] [varchar](250) NULL,
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
/****** Object:  Table [dbo].[SystemClassCompanies]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemClassCompanies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [nvarchar](max) NULL,
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
	[DataKey] [varchar](250) NULL,
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
/****** Object:  Table [dbo].[UserSettings]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSettings](
	[UserId] [nvarchar](450) NOT NULL,
	[CurrencyCode] [nvarchar](max) NULL,
	[PageLength] [int] NOT NULL,
	[DataKey] [varchar](250) NULL,
 CONSTRAINT [PK_UserSettings] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserVendors]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserVendors](
	[UserId] [nvarchar](450) NOT NULL,
	[VendorCode] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
 CONSTRAINT [PK_UserVendors] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vendors]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vendors](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VendorCode] [nvarchar](max) NULL,
	[VendorName] [nvarchar](max) NULL,
	[VendorCompanyName] [nvarchar](max) NULL,
	[DeadOfficeAddress] [nvarchar](max) NULL,
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
	[DataKey] [varchar](250) NULL,
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
/****** Object:  Table [dbo].[WarehouseUserSettings]    Script Date: 9/9/2024 2:37:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WarehouseUserSettings](
	[WarehouseUserId] [nvarchar](450) NOT NULL,
	[WarehouseCode] [nvarchar](max) NULL,
	[CompanyId] [nvarchar](max) NULL,
	[DataKey] [varchar](250) NULL,
	[CreateOperatorId] [nvarchar](max) NULL,
	[CreateAt] [datetime2](7) NULL,
	[UpdateOperatorId] [nvarchar](max) NULL,
	[UpdateAt] [datetime2](7) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_WarehouseUserSettings] PRIMARY KEY CLUSTERED 
(
	[WarehouseUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AuthUsers_Email]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_AuthUsers_Email] ON [authp].[AuthUsers]
(
	[Email] ASC
)
WHERE ([Email] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_AuthUsers_TenantId]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_AuthUsers_TenantId] ON [authp].[AuthUsers]
(
	[TenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AuthUsers_UserName]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_AuthUsers_UserName] ON [authp].[AuthUsers]
(
	[UserName] ASC
)
WHERE ([UserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RefreshTokens_AddedDateUtc]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_RefreshTokens_AddedDateUtc] ON [authp].[RefreshTokens]
(
	[AddedDateUtc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RoleToPermissions_RoleType]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_RoleToPermissions_RoleType] ON [authp].[RoleToPermissions]
(
	[RoleType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_RoleToPermissionsTenant_TenantsTenantId]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_RoleToPermissionsTenant_TenantsTenantId] ON [authp].[RoleToPermissionsTenant]
(
	[TenantsTenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Tenants_ParentDataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_Tenants_ParentDataKey] ON [authp].[Tenants]
(
	[ParentDataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Tenants_ParentTenantId]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_Tenants_ParentTenantId] ON [authp].[Tenants]
(
	[ParentTenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Tenants_TenantFullName]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Tenants_TenantFullName] ON [authp].[Tenants]
(
	[TenantFullName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserToRoles_RoleName]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserToRoles_RoleName] ON [authp].[UserToRoles]
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ApiCodes_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_ApiCodes_DataKey] ON [dbo].[ApiCodes]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ChannelMasters_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_ChannelMasters_DataKey] ON [dbo].[ChannelMasters]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Channels_ChannelMasterCode]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_Channels_ChannelMasterCode] ON [dbo].[Channels]
(
	[ChannelMasterCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Channels_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_Channels_DataKey] ON [dbo].[Channels]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Companies_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_Companies_DataKey] ON [dbo].[Companies]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_CountryMasters_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_CountryMasters_DataKey] ON [dbo].[CountryMasters]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Currencies_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_Currencies_DataKey] ON [dbo].[Currencies]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_CurrencyPairSettings_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_CurrencyPairSettings_DataKey] ON [dbo].[CurrencyPairSettings]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ExchangeRates_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_ExchangeRates_DataKey] ON [dbo].[ExchangeRates]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_OrderItems_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_DataKey] ON [dbo].[OrderItems]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderItems_OrderHeaderId]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderItems_OrderHeaderId] ON [dbo].[OrderItems]
(
	[OrderHeaderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_OrderReturnItems_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderReturnItems_DataKey] ON [dbo].[OrderReturnItems]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderReturnItems_OrderHeaderId]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderReturnItems_OrderHeaderId] ON [dbo].[OrderReturnItems]
(
	[OrderHeaderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Orders_ChannelCode]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_Orders_ChannelCode] ON [dbo].[Orders]
(
	[ChannelCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Orders_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_Orders_DataKey] ON [dbo].[Orders]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_OrderStatuses_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderStatuses_DataKey] ON [dbo].[OrderStatuses]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ProductBundles_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_ProductBundles_DataKey] ON [dbo].[ProductBundles]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ProductBundles_ParentProductId]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_ProductBundles_ParentProductId] ON [dbo].[ProductBundles]
(
	[ParentProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Products_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_Products_DataKey] ON [dbo].[Products]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ProductStatuses_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_ProductStatuses_DataKey] ON [dbo].[ProductStatuses]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ProductStocks_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_ProductStocks_DataKey] ON [dbo].[ProductStocks]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SalesData_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_SalesData_DataKey] ON [dbo].[SalesData]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_ShippingCountries_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_ShippingCountries_DataKey] ON [dbo].[ShippingCountries]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_SystemClassCompanies_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_SystemClassCompanies_DataKey] ON [dbo].[SystemClassCompanies]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserSettings_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserSettings_DataKey] ON [dbo].[UserSettings]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_UserVendors_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_UserVendors_DataKey] ON [dbo].[UserVendors]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Vendors_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_Vendors_DataKey] ON [dbo].[Vendors]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_WarehouseUserSettings_DataKey]    Script Date: 9/9/2024 2:37:22 AM ******/
CREATE NONCLUSTERED INDEX [IX_WarehouseUserSettings_DataKey] ON [dbo].[WarehouseUserSettings]
(
	[DataKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [authp].[AuthUsers] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsDisabled]
GO
ALTER TABLE [authp].[RoleToPermissions] ADD  DEFAULT (CONVERT([tinyint],(0))) FOR [RoleType]
GO
ALTER TABLE [authp].[Tenants] ADD  DEFAULT (CONVERT([bit],(0))) FOR [HasOwnDb]
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
ALTER TABLE [dbo].[Channels]  WITH CHECK ADD  CONSTRAINT [FK_Channels_ChannelMasters_ChannelMasterCode] FOREIGN KEY([ChannelMasterCode])
REFERENCES [dbo].[ChannelMasters] ([ChannelMasterCode])
GO
ALTER TABLE [dbo].[Channels] CHECK CONSTRAINT [FK_Channels_ChannelMasters_ChannelMasterCode]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderItems_Orders_OrderHeaderId] FOREIGN KEY([OrderHeaderId])
REFERENCES [dbo].[Orders] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK_OrderItems_Orders_OrderHeaderId]
GO
ALTER TABLE [dbo].[OrderReturnItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderReturnItems_Orders_OrderHeaderId] FOREIGN KEY([OrderHeaderId])
REFERENCES [dbo].[Orders] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderReturnItems] CHECK CONSTRAINT [FK_OrderReturnItems_Orders_OrderHeaderId]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Channels_ChannelCode] FOREIGN KEY([ChannelCode])
REFERENCES [dbo].[Channels] ([ChannelCode])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Channels_ChannelCode]
GO
ALTER TABLE [dbo].[ProductBundles]  WITH CHECK ADD  CONSTRAINT [FK_ProductBundles_Products_ParentProductId] FOREIGN KEY([ParentProductId])
REFERENCES [dbo].[Products] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductBundles] CHECK CONSTRAINT [FK_ProductBundles_Products_ParentProductId]
GO
USE [master]
GO
ALTER DATABASE [FBT_DEV] SET  READ_WRITE 
GO
