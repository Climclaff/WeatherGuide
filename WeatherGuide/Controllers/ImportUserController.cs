using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace WeatherGuide.Controllers
{
    [Authorize("IsAdminPolicy")]
    public class ImportUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public ImportUserController(ApplicationDbContext context, UserManager<Models.AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View("~/Views/Administration/ImportUser/Index.cshtml");
        }
        public async Task<IActionResult> Import(IFormFile file)
        {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var list = new List<AppUser>();
                using (var stream = new MemoryStream()) 
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowcount = worksheet.Dimension.Rows;
                        for (int row = 2; row <= rowcount; row++)
                        {
                                list.Add(new AppUser
                                {
                                    CountryId = Convert.ToInt32(worksheet.Cells[row, 1].Value),
                                    StateId = Convert.ToInt32(worksheet.Cells[row, 3].Value),
                                    UserName = worksheet.Cells[row, 7].Value.ToString().Trim(),
                                    NormalizedUserName = worksheet.Cells[row, 8].Value.ToString().Trim(),
                                    Email = worksheet.Cells[row, 9].Value.ToString().Trim(),
                                    NormalizedEmail = worksheet.Cells[row, 10].Value.ToString().Trim(),
                                    EmailConfirmed = Convert.ToBoolean(worksheet.Cells[row, 11].Value),
                                    PasswordHash = worksheet.Cells[row, 12].Value.ToString().Trim(),
                                    SecurityStamp = worksheet.Cells[row, 13].Value.ToString().Trim(),
                                    ConcurrencyStamp = worksheet.Cells[row, 14].Value.ToString().Trim(),
                                    PhoneNumber = null,
                                    PhoneNumberConfirmed = Convert.ToBoolean(worksheet.Cells[row, 16].Value),
                                    TwoFactorEnabled = Convert.ToBoolean(worksheet.Cells[row, 17].Value),
                                    LockoutEnd = null,
                                    LockoutEnabled = Convert.ToBoolean(worksheet.Cells[row, 19].Value),
                                    AccessFailedCount = Convert.ToInt32(worksheet.Cells[row, 20].Value),
                                });                            
                        }
                    }
                }
                List<Claim> claims = new List<Claim> {
                        new Claim("Name", "name"),
                        new Claim("Surname", "surname"),
                        new Claim("IsAdmin", "false"),
                        new Claim("IsDesigner", "false")
                        };
                for (int i = 0; i < list.Count; i++)
                {
                    _context.Add(list[i]);
                    int Idt = _context.Users.Max(u => u.Id);
                    var user = await _userManager.FindByIdAsync(Convert.ToString(Idt));
                    for (int p = 0; p < claims.Count; ++p)
                    {
                        await _userManager.AddClaimAsync(user, claims[p]);
                    }
                    await _context.SaveChangesAsync();
                }
                
            
            return RedirectToAction("Index");
        }
    }
}
