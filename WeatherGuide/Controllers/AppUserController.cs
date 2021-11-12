using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WeatherGuide.Data;
using WeatherGuide.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WeatherGuide.Controllers
{
    [Authorize("IsAdminPolicy")]
    public class AppUserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppUserController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Route("Administration/[controller]/[action]/")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Users.Include(a => a.Country).Include(a => a.State);
            return View("~/Views/Administration/AppUser/index.cshtml", await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _context.Users
                .Include(a => a.Country)
                .Include(a => a.State)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View("~/Views/Administration/AppUser/Details.cshtml", appUser);
        }

        // GET: AppUser/Create
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id");
            ViewData["StateId"] = new SelectList(_context.States, "Id", "Id");
            return View("~/Views/Administration/AppUser/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CountryId,StateId,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id", appUser.CountryId);
            ViewData["StateId"] = new SelectList(_context.States, "Id", "Id", appUser.StateId);
            return View("~/Views/Administration/AppUser/Create.cshtml", appUser);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            Claims claims = new Claims();
            claims.ClaimsList = await _context.UserClaims.Where(x => x.UserId == id).ToListAsync();           
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _context.Users.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id", appUser.CountryId);
            ViewData["StateId"] = new SelectList(_context.States, "Id", "Id", appUser.StateId);
            ViewData["Claims"] = claims;
            return View("~/Views/Administration/AppUser/Edit.cshtml", appUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CountryId,StateId,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AppUser appUser)
        {
            if (id != appUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(appUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id", appUser.CountryId);
            ViewData["StateId"] = new SelectList(_context.States, "Id", "Id", appUser.StateId);
            return View("~/Views/Administration/AppUser/Edit.cshtml", appUser);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _context.Users
                .Include(a => a.Country)
                .Include(a => a.State)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View("~/Views/Administration/AppUser/Delete.cshtml", appUser);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recommendations = await _context.Set<Recommendation>().Where(x => x.AppUserId == id).ToListAsync();
            if (recommendations.Count != 0)
            {
                for (int i = 0; i < recommendations.Count; ++i)
                {
                    _context.Recommendations.Remove(recommendations[i]);
                }
                await _context.SaveChangesAsync();
            }
            var appUser = await _context.Users.FindAsync(id);
            _context.Users.Remove(appUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Route("[controller]/[action]/{id?}")]
        public async Task<IActionResult> ClaimChange(int claimId, string value)
        {
           var claim = await _context.UserClaims.FindAsync(claimId);
            claim.ClaimValue = value;
            _context.UserClaims.Update(claim);
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", new { id = claim.UserId });
        }
        private bool AppUserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
