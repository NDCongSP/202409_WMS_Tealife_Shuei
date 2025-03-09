using Application.DTOs;
using Application.Extentions;
using Application.Services;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Vml;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class RepositoryProductBundlesServicess(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IProductBundles
    {
        public Task<Result<List<ProductBundle>>> AddRangeAsync([Body] List<ProductBundle> model)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ProductBundle>> DeleteAsync([Body] ProductBundle model)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ProductBundle>> DeleteRangeAsync([Body] List<ProductBundle> model)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<ProductBundle>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Result<List<ProductBundle>>> GetAllDistinctAsync()
        {
            try
            {
                var productBundles = await dbContext.ProductBundles
                     .ToListAsync();
                var retunData = productBundles.DistinctBy(x => x.ProductBundleCode).ToList();
                return await Result<List<ProductBundle>>.SuccessAsync(retunData);
            }
            catch (Exception ex)
            {
                return await Result<List<ProductBundle>>.FailAsync(ex.Message);
            }
        }

        public async Task<Result<List<String>>> GetProductCodesByBundleCodeAsync([Path] string BundleCode)
        {
            try
            {
                var productCodes = await dbContext.ProductBundles.Where(x => x.ProductBundleCode == BundleCode)
                    .Select(x => x.ProductCode)
                     .Distinct()
                     .ToListAsync();
                return await Result<List<String>>.SuccessAsync(productCodes);
            }
            catch (Exception ex)
            {
                return await Result<List<string>>.FailAsync(ex.Message);
            }
        }

        public Task<Result<ProductBundle>> GetByIdAsync([Path] int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<IEnumerable<ProductBundleDto>>> GetPlannedShipmentBundlesAsync()
        {
            var productBundlesGroupBy = from bundle in dbContext.ProductBundles
                                        group bundle by new { bundle.ProductBundleCode, bundle.ProductCode } into g
                                        select new { ProductBundleCode = g.Key.ProductBundleCode, ProductCode = g.Key.ProductCode, TotalQuantity = g.Sum(x => x.Quantity) };

            var OrderStatusQty = from orderItem in dbContext.OrderItems.Where(_ => productBundlesGroupBy.Select(_ => _.ProductBundleCode).Contains(_.ProductCode))
                                 join order in dbContext.Orders on orderItem.OrderId equals order.OrderId
                                 where order.OrderStatus == "10" || order.OrderStatus == "20"
                                 group orderItem by orderItem.ProductCode into g
                                 select new { ProductCode = g.Key, TotalQuantity = g.Sum(x => x.Quantity) };
            var OrderStatusQtySet =  OrderStatusQty.AsEnumerable().ToHashSet();

            var StockUpStatusQty = from orderItem in dbContext.OrderItems.Where(_ => productBundlesGroupBy.Select(_ => _.ProductBundleCode).Contains(_.ProductCode))
                                   join order in dbContext.Orders on orderItem.OrderId equals order.OrderId
                                 where order.StockUpStatus == 0
                                 group orderItem by orderItem.ProductCode into g
                                 select new { ProductCode = g.Key, TotalQuantity = g.Sum(x => x.Quantity) };
            var StockUpStatusQtySet = StockUpStatusQty.AsEnumerable().ToHashSet();

           

         

            var openShipmentSet = (from shipmentLine in dbContext.WarehouseShipmentLines.Where( _ => productBundlesGroupBy.Select(_ => _.ProductBundleCode).Contains(_.ProductCode))
                               join shipment in dbContext.WarehouseShipments on shipmentLine.ShipmentNo equals shipment.ShipmentNo
                               where shipment.Status != EnumShipmentOrderStatus.Completed
                               group shipmentLine by shipmentLine.ProductCode into g
                               select new { ProductCode = g.Key, TotalQuantity = g.Sum(x => x.ShipmentQty) }).AsEnumerable().ToHashSet();

            var productBundles = from productBundle in productBundlesGroupBy.Where(p => p.ProductBundleCode != null).OrderBy(x => x.ProductBundleCode)
                                 join productBunle in dbContext.Products.Where(p => p.IsDeleted != true) on productBundle.ProductBundleCode equals productBunle.ProductCode
                                 join productItem in dbContext.Products.Where( _ => _.IsDeleted != true)on productBundle.ProductCode equals productItem.ProductCode
                                 select new ProductBundleDto
                                 {
                                     SaleProductBundleCode = productBunle.SaleProductCode,
                                     ProductBundleSetName = productBundle.ProductBundleCode,
                                     ProductBundleCode = productBundle.ProductBundleCode,
                                     ProductCode = productBundle.ProductCode,
                                     ProductName = productItem.ProductName,
                                     Quantity = productBundle.TotalQuantity
                                 };
            int sequenceNo = 0;
            string runningProductBundleCode = "";
            List<ProductBundleDto> ret = new();
            foreach (var bundle in productBundles)
            {
                if (runningProductBundleCode != bundle.ProductBundleCode)
                {
                    sequenceNo = 1;
                    runningProductBundleCode = bundle.ProductBundleCode;
                }
                else
                {
                    sequenceNo++;
                }
                bundle.SequenceNo = sequenceNo;
                bundle.OrderStatusQty = OrderStatusQtySet.Where(_ => _.ProductCode == bundle.ProductBundleCode).FirstOrDefault()?.TotalQuantity;
                bundle.StockUpStatusQty = StockUpStatusQtySet.Where(_ => _.ProductCode == bundle.ProductBundleCode).FirstOrDefault()?.TotalQuantity;
                bundle.DemandQty = bundle.StockUpStatusQty * bundle.Quantity;
                bundle.OpenShipmentQty = (openShipmentSet.Where(_ => _.ProductCode == bundle.ProductBundleCode).FirstOrDefault()?.TotalQuantity);
                ret.Add(bundle);
            }

            //dbContext.ProductBundles.ToList().OrderBy(x => x.ProductBundleCode);
            return await Result<IEnumerable<ProductBundleDto>>.SuccessAsync(ret.OrderBy(x => x.ProductBundleCode).AsEnumerable());
        }

        public Task<Result<ProductBundle>> InsertAsync([Body] ProductBundle model)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ProductBundle>> UpdateAsync([Body] ProductBundle model)
        {
            throw new NotImplementedException();
        }
    }
}
