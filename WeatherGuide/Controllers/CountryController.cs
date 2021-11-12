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

namespace WeatherGuide.Controllers
{
    [Authorize("IsAdminPolicy")]

    public class CountryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CountryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("Administration/[controller]/[action]/")]
        public async Task<IActionResult> Index()
        {
            
            return View("~/Views/Administration/Country/index.cshtml", await _context.Countries.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View("~/Views/Administration/Country/Details.cshtml", country);
        }

        public IActionResult Create()
        {
            return View("~/Views/Administration/Country/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Country country)
        {
            if (ModelState.IsValid)
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Administration/Country/Create.cshtml", country);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return View("~/Views/Administration/Country/Edit.cshtml", country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Country country)
        {
            if (id != country.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.Id))
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
            return View("~/Views/Administration/Country/Edit.cshtml", country);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View("~/Views/Administration/Country/Delete.cshtml", country);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if(id == 1)
            {
                return RedirectToAction(nameof(Index));
            }
            var users = await _context.Set<AppUser>().Where(x => x.CountryId == id).ToListAsync();
            if (users.Count != 0)
            {
                for (int i = 0; i < users.Count; ++i)
                {
                    users[i].CountryId = 1;
                    users[i].StateId = 1;
                    _context.Users.Update(users[i]);
                }             
                await _context.SaveChangesAsync();
            }
            var measurements = await _context.Set<Measurement>().Where(x => x.CountryId == id).ToListAsync();
            if (measurements.Count != 0)
            {
                for (int i = 0; i < measurements.Count; ++i)
                {
                    _context.Measurements.Remove(measurements[i]);
                }
                await _context.SaveChangesAsync();
            }
            var states = await _context.Set<State>().Where(x => x.CountryId == id).ToListAsync();
            if (states.Count != 0)
            {
                for (int i = 0; i < states.Count; ++i)
                {
                    _context.States.Remove(states[i]);
                }
                await _context.SaveChangesAsync();
            }          

            var country = await _context.Countries.FindAsync(id);
            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }
    }
}
