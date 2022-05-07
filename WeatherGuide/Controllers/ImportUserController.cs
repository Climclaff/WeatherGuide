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
using Microsoft.Extensions.Localization;

namespace WeatherGuide.Controllers
{
    [Authorize("IsAdminPolicy")]
    public class ImportUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        public ImportUserController(ApplicationDbContext context, UserManager<Models.AppUser> userManager,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _context = context;
            _userManager = userManager;
            _sharedLocalizer = sharedLocalizer; 
        }
        public IActionResult Index()
        {
            return View("~/Views/Administration/ImportUser/Index.cshtml");
        }
        public async Task<IActionResult> Import(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var userList = new List<AppUser>();
            var rightsList = new List<UserRight>();
            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet userWorksheet = package.Workbook.Worksheets[0];
                        ExcelWorksheet userRightsWorksheet = package.Workbook.Worksheets[1];
                        var userRowCount = userWorksheet.Dimension.Rows;
                        var rightsRowCount = userRightsWorksheet.Dimension.Rows;
                        for (int row = 2; row <= userRowCount; row++)
                        {
                            userList.Add(new AppUser
                            {
                                CountryId = Convert.ToInt32(userWorksheet.Cells[row, 1].Value),
                                StateId = Convert.ToInt32(userWorksheet.Cells[row, 3].Value),
                                UserName = userWorksheet.Cells[row, 8].Value.ToString().Trim(),
                                NormalizedUserName = userWorksheet.Cells[row, 9].Value.ToString().Trim(),
                                Email = userWorksheet.Cells[row, 10].Value.ToString().Trim(),
                                NormalizedEmail = userWorksheet.Cells[row, 11].Value.ToString().Trim(),
                                EmailConfirmed = Convert.ToBoolean(userWorksheet.Cells[row, 12].Value),
                                PasswordHash = userWorksheet.Cells[row, 13].Value.ToString().Trim(),
                                SecurityStamp = userWorksheet.Cells[row, 14].Value.ToString().Trim(),
                                ConcurrencyStamp = userWorksheet.Cells[row, 15].Value.ToString().Trim(),
                                PhoneNumber = null,
                                PhoneNumberConfirmed = Convert.ToBoolean(userWorksheet.Cells[row, 17].Value),
                                TwoFactorEnabled = Convert.ToBoolean(userWorksheet.Cells[row, 18].Value),
                                LockoutEnd = null,
                                LockoutEnabled = Convert.ToBoolean(userWorksheet.Cells[row, 20].Value),
                                AccessFailedCount = Convert.ToInt32(userWorksheet.Cells[row, 21].Value),
                            });
                        }
                        for (int row = 2; row <= rightsRowCount; row++)
                        {
                            rightsList.Add(new UserRight
                            {
                                UserId = Convert.ToInt32(userRightsWorksheet.Cells[row, 2].Value),
                                ClaimType = Convert.ToString(userRightsWorksheet.Cells[row, 3].Value),
                                ClaimValue = Convert.ToString(userRightsWorksheet.Cells[row, 4].Value)
                            });

                        }
                    }
                }
                int currentOldUserId = rightsList[0].UserId;
                int counter = 0;
                for (int i = 0; i < userList.Count; i++)
                {
                    _context.Add(userList[i]);
                    await _context.SaveChangesAsync();
                    int rightCount = rightsList.Where(x => x.UserId.Equals(currentOldUserId)).Count();
                    for (int p = 0; p < rightCount; ++p)
                    {
                        await _userManager.AddClaimAsync(userList[i], new Claim(rightsList[counter].ClaimType, rightsList[counter].ClaimValue));
                        ++counter;
                        await _context.SaveChangesAsync();

                    }

                    if (rightsList[i + 1] != null)
                    {
                        currentOldUserId = rightsList[i + rightCount].UserId;
                        continue;
                    }
                    break;
                }


                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Message"] = _sharedLocalizer["File is corrupted"].ToString();
                return RedirectToAction("Index");
            }
        }
    }
    public class UserRight
    {
        public int UserId { get; set; } 
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
