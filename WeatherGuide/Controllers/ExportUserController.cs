using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WeatherGuide.Data;
using WeatherGuide.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.IO;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace WeatherGuide.Controllers
{
    [Authorize("IsAdminPolicy")]
    public class ExportUserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExportUserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(_context.Users.ToListAsync().Result.ToDataTable());
            ViewBag.details = ds.Tables[0];
            return View("~/Views/Administration/ExportUser/Index.cshtml");
        }
        public IActionResult ExportToExcel()
        {
            DataSet userSet= new DataSet();
            DataSet userRightSet = new DataSet();   
            userSet.Tables.Add(_context.Users.ToListAsync().Result.ToDataTable());
            userRightSet.Tables.Add(_context.UserClaims.ToListAsync().Result.ToDataTable());
            var stream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(stream))
            {
                var userWorksheet = package.Workbook.Worksheets.Add("Sheet1");
                var rightsWorksheet = package.Workbook.Worksheets.Add("Sheet2");
                userWorksheet.Cells.LoadFromDataTable(userSet.Tables[0], true);
                rightsWorksheet.Cells.LoadFromDataTable(userRightSet.Tables[0], true);
                package.Save();
            }
            stream.Position = 0;
            string excelname = $"UserList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelname);
        }

    }
    public static class DataTableExtensions
    {
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
