using Application.DTOs.Request;
using Application.Extentions;
using Application.Services;
using Newtonsoft.Json;
using RestEase;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;

using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repos
{
    public class RepositoryReportsServices(RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager, IConfiguration config,
        SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext dbContext)
    {
        public Task<Result<string>> GetReportBase64([Body] ReportRequestDTO model)
        {
            throw new NotImplementedException();
        }

        //     [HttpGet("GetReportBase64/{InjectionLotId}")]
        //public async Task<string> GetReportBase64(int InjectionLotId)
        //{
        //    var bytes = await GetReportBytes(InjectionLotId);
        //    return Convert.ToBase64String(bytes);
        //}


        //private async Task<byte[]> GetReportBytes(int InjectionLotId)
        //{
        //    ReportParameters p = GetReportParameters(InjectionLotId);
        //    string filePath = Path.Combine(this.Environment.ContentRootPath, "LableTemplate/template.xlsx");

        //    // Load a document from a file.
        //    Workbook workbook = new Workbook();
        //    workbook.LoadDocument(filePath, DocumentFormat.OpenXml);
        //    FillToTemplate("General_Template", p, workbook);

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        await workbook.ExportToPdfAsync(ms);
        //        return ms.ToArray();
        //    }

        //    return null;
        //}
    }
}
