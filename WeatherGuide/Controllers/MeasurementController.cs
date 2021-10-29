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
    public class MeasurementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MeasurementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Measurement
        [Route("Administration/[controller]/[action]/")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Measurements.Include(m => m.Country).Include(m => m.State);
            return View("~/Views/Administration/Measurement/index.cshtml", await applicationDbContext.ToListAsync());
        }

        // GET: Measurement/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var measurement = await _context.Measurements
                .Include(m => m.Country)
                .Include(m => m.State)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (measurement == null)
            {
                return NotFound();
            }

            return View("~/Views/Administration/Measurement/Details.cshtml", measurement);
        }

        // GET: Measurement/Create
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id");
            ViewData["StateId"] = new SelectList(_context.States, "Id", "Id");
            return View("~/Views/Administration/Measurement/Create.cshtml");
        }

        // POST: Measurement/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Temperature,Humidity,WindSpeed,DateTime,CountryId,StateId")] Measurement measurement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(measurement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id", measurement.CountryId);
            ViewData["StateId"] = new SelectList(_context.States, "Id", "Id", measurement.StateId);
            return View("~/Views/Administration/Measurement/Create.cshtml", measurement);
        }

        // GET: Measurement/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var measurement = await _context.Measurements.FindAsync(id);
            if (measurement == null)
            {
                return NotFound();
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id", measurement.CountryId);
            ViewData["StateId"] = new SelectList(_context.States, "Id", "Id", measurement.StateId);
            return View("~/Views/Administration/Measurement/Edit.cshtml", measurement);
        }

        // POST: Measurement/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Temperature,Humidity,WindSpeed,DateTime,CountryId,StateId")] Measurement measurement)
        {
            if (id != measurement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(measurement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeasurementExists(measurement.Id))
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
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id", measurement.CountryId);
            ViewData["StateId"] = new SelectList(_context.States, "Id", "Id", measurement.StateId);
            return View("~/Views/Administration/Measurement/Edit.cshtml", measurement);
        }

        // GET: Measurement/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var measurement = await _context.Measurements
                .Include(m => m.Country)
                .Include(m => m.State)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (measurement == null)
            {
                return NotFound();
            }

            return View("~/Views/Administration/Measurement/Delete.cshtml", measurement);
        }

        // POST: Measurement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var measurement = await _context.Measurements.FindAsync(id);
            _context.Measurements.Remove(measurement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MeasurementExists(int id)
        {
            return _context.Measurements.Any(e => e.Id == id);
        }
    }
}
