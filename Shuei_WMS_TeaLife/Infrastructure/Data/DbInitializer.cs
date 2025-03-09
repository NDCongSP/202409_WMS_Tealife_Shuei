using Application.Extentions;


using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata;

namespace Infrastructure.Data
{
    /// <summary>
    /// Seeding data ban đầu.
    /// </summary>
    public class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            #region Initial DB if not exist DB or any change entity
            // Ensure the database is created (or already exists), chỉ chạy 1 lần, sau đó nếu có sự thay đổi Entity thì nó ko cập nhật DB, nên nếu bật thì chỉ bật khi chạy runTime
            //context.Database.EnsureCreated();

            //chạy migration theo các migration đc tạo trong lúc build code.
            //if (context.Database.GetPendingMigrations().Any())
            //    context.Database.Migrate();
            //context.SaveChanges();
            #endregion

            #region Sedding data if null
            //string[] categoryName = { "Categorys 1", "Categorys 2", "Categorys 3", "Categorys 4", "Categorys 5", "Categorys 6" };
            //string[] productName = { "Product 1", "Product 2", "Product 3", "Product 4", "Product 5", "Product 6" };

            //var units = new List<Unit>();
            //units.Add(new Unit()
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "Box",
            //    CreatedDate = DateTime.Now,
            //    CreatedBy = "SeedingData",
            //    IsActived = true,
            //});
            //units.Add(new Unit()
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "Pcs",
            //    CreatedDate = DateTime.Now,
            //    CreatedBy = "SeedingData",
            //    IsActived = true,
            //});

            //var categorys = new List<Category>();
            //for (int i = 0; i < 6; i++)
            //{
            //    categorys.Add(new Category()
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = categoryName[i],
            //        CreatedDate = DateTime.Now,
            //        CreatedBy = "SeedingData",
            //        IsActived = true,
            //    });
            //}

            //var products = new List<Product>();
            //for (int i = 0; i < 6; i++)
            //{
            //    products.Add(new Product()
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = productName[i],
            //        IdUnit = units[0].Id,
            //        UnitName = units[0].Name,
            //        IdCategory = categorys[i].Id,
            //        CategoryName = categorys[i].Name,
            //        CreatedDate = DateTime.Now,
            //        CreatedBy = "SeedingData",
            //        IsActived = true,
            //    });
            //}

            // Check if there are any products already present
            if (!context.Roles.Any())
            {
                await context.Roles.AddAsync(new Microsoft.AspNetCore.Identity.IdentityRole()
                {
                    Name = ConstantExtention.Roles.WarehouseAdmin,
                    NormalizedName = ConstantExtention.Roles.WarehouseAdmin.ToUpper()
                });
                await context.Roles.AddAsync(new Microsoft.AspNetCore.Identity.IdentityRole()
                {
                    Name = ConstantExtention.Roles.WarehouseStaff,
                    NormalizedName = ConstantExtention.Roles.WarehouseStaff.ToUpper()
                });
                await context.Roles.AddAsync(new Microsoft.AspNetCore.Identity.IdentityRole()
                {
                    Name = ConstantExtention.Roles.WarehouseSystem,
                    NormalizedName = ConstantExtention.Roles.WarehouseSystem.ToUpper()
                });
            }

            //if (!context.Permissions.Any())
            //{
            //    await context.Permissions.AddAsync(new FBT.ShareModels.WMS.Authentication.Permissions()
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Insert",
            //        Description = "Allow add new data",
            //        CreateAt = DateTime.Now,
            //    });

            //    await context.Permissions.AddAsync(new FBT.ShareModels.WMS.Authentication.Permissions()
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Update",
            //        Description = "Allow update data",
            //        CreateAt = DateTime.Now,
            //    });

            //    await context.Permissions.AddAsync(new FBT.ShareModels.WMS.Authentication.Permissions()
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Delete",
            //        Description = "Allow delete new data",
            //        CreateAt = DateTime.Now,
            //    });
            //}

            //if (!context.Permissions.Any())
            //{
            //    await context.AddAsync(new Permissions()
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Add",
            //        Description = "Can add data"
            //    });
            //    await context.AddAsync(new Permissions()
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Update",
            //        Description = "Can update data"
            //    });
            //    await context.AddAsync(new Permissions()
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Delete",
            //        Description = "Can delete data"
            //    });
            //}

            //if (!context.Units.Any())
            //{
            //    await context.Units.AddRangeAsync(units);
            //}

            //if (!context.Categorys.Any())
            //{
            //    await context.Categorys.AddRangeAsync(categorys);
            //}

            //if (!context.Products.Any())
            //{
            //    await context.Products.AddRangeAsync(products);
            //}

            if (!context.Units.Any())
            {
                await context.Units.AddAsync(new FBT.ShareModels.WMS.Unit()
                {
                    UnitName = "Box",
                    Description = "BOX",
                    Status = EnumStatus.Activated,
                    CreateAt = DateTime.Now,
                    IsDeleted = false,
                });
                await context.Units.AddAsync(new FBT.ShareModels.WMS.Unit()
                {
                    UnitName = "Pcs",
                    Description = "PCS",
                    Status = EnumStatus.Activated,
                    CreateAt = DateTime.Now,
                    IsDeleted = false,
                });
                await context.Units.AddAsync(new FBT.ShareModels.WMS.Unit()
                {
                    UnitName = "Pallet",
                    Description = "PALLET",
                    Status = EnumStatus.Activated,
                    CreateAt = DateTime.Now,
                    IsDeleted = false,
                });
            }

            if (!context.ProductCategories.Any())
            {
                await context.ProductCategories.AddAsync(new FBT.ShareModels.Entities.ProductCategory()
                {
                    CategoryName = "Category 1",
                    Description = "Test",
                    Status = EnumStatus.Activated,
                    CreateAt = DateTime.Now,
                    IsDeleted = false,
                });
                await context.ProductCategories.AddAsync(new FBT.ShareModels.Entities.ProductCategory()
                {
                    CategoryName = "Category 2",
                    Description = "Test",
                    Status = EnumStatus.Activated,
                    CreateAt = DateTime.Now,
                    IsDeleted = false,
                });
            }

            if (!context.Currencies.Any())
            {
                await context.Currencies.AddAsync(new FBT.ShareModels.Entities.Currency()
                {
                    CurrencyCode = "JPY",
                    Country = "Japan",
                    Description = "Japan Yen",
                    CreateAt = DateTime.Now,
                    IsDeleted = false,
                });
                await context.Currencies.AddAsync(new FBT.ShareModels.Entities.Currency()
                {
                    CurrencyCode = "USD",
                    Country = "United States",
                    Description = "United States Dollar",
                    CreateAt = DateTime.Now,
                    IsDeleted = false,
                });
            }

            if (!context.TenantAuth.Any())
            {
                await context.TenantAuth.AddAsync(new FBT.ShareModels.TenantAuth()
                {
                    //TenantId=1,
                    ParentTenantId = 1,
                    ParentDataKey = "1",
                    TenantFullName = "Tealine",
                    IsHierarchical = true,
                    HasOwnDb = false,
                });
                await context.TenantAuth.AddAsync(new FBT.ShareModels.TenantAuth()
                {
                    //TenantId = 2,
                    ParentTenantId = 1,
                    ParentDataKey = "1",
                    TenantFullName = "Tealine | KOKORO",
                    IsHierarchical = true,
                    HasOwnDb = false,
                });
            }

            // save the changes to the database
            await context.SaveChangesAsync();
            #endregion
        }
    }
}
