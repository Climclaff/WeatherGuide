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
    public class RecommendationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecommendationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Recommendation
        [Route("Administration/[controller]/[action]/")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Recommendations.Include(r => r.FirstClothing).Include(r => r.SecondClothing).Include(r => r.ThirdClothing).Include(r => r.User);
            return View("~/Views/Administration/Recommendation/index.cshtml", await applicationDbContext.ToListAsync());
        }

        // GET: Recommendation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recommendation = await _context.Recommendations
                .Include(r => r.FirstClothing)
                .Include(r => r.SecondClothing)
                .Include(r => r.ThirdClothing)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recommendation == null)
            {
                return NotFound();
            }

            return View("~/Views/Administration/Recommendation/Details.cshtml", recommendation);
        }

        // GET: Recommendation/Create
        public IActionResult Create()
        {
            ViewData["FirstClothingId"] = new SelectList(_context.Clothings, "Id", "Id");
            ViewData["SecondClothingId"] = new SelectList(_context.Clothings, "Id", "Id");
            ViewData["ThirdClothingId"] = new SelectList(_context.Clothings, "Id", "Id");
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View("~/Views/Administration/Recommendation/Create.cshtml");
        }

        // POST: Recommendation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AppUserId,FirstClothingId,SecondClothingId,ThirdClothingId,DateTime")] Recommendation recommendation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(recommendation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FirstClothingId"] = new SelectList(_context.Clothings, "Id", "Id", recommendation.FirstClothingId);
            ViewData["SecondClothingId"] = new SelectList(_context.Clothings, "Id", "Id", recommendation.SecondClothingId);
            ViewData["ThirdClothingId"] = new SelectList(_context.Clothings, "Id", "Id", recommendation.ThirdClothingId);
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", recommendation.AppUserId);
            return View("~/Views/Administration/Recommendation/Create.cshtml", recommendation);
        }

        // GET: Recommendation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recommendation = await _context.Recommendations.FindAsync(id);
            if (recommendation == null)
            {
                return NotFound();
            }
            ViewData["FirstClothingId"] = new SelectList(_context.Clothings, "Id", "Id", recommendation.FirstClothingId);
            ViewData["SecondClothingId"] = new SelectList(_context.Clothings, "Id", "Id", recommendation.SecondClothingId);
            ViewData["ThirdClothingId"] = new SelectList(_context.Clothings, "Id", "Id", recommendation.ThirdClothingId);
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", recommendation.AppUserId);
            return View("~/Views/Administration/Recommendation/Edit.cshtml", recommendation);
        }

        // POST: Recommendation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUserId,FirstClothingId,SecondClothingId,ThirdClothingId,DateTime")] Recommendation recommendation)
        {
            if (id != recommendation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recommendation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecommendationExists(recommendation.Id))
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
            ViewData["FirstClothingId"] = new SelectList(_context.Clothings, "Id", "Id", recommendation.FirstClothingId);
            ViewData["SecondClothingId"] = new SelectList(_context.Clothings, "Id", "Id", recommendation.SecondClothingId);
            ViewData["ThirdClothingId"] = new SelectList(_context.Clothings, "Id", "Id", recommendation.ThirdClothingId);
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", recommendation.AppUserId);
            return View("~/Views/Administration/Recommendation/Edit.cshtml", recommendation);
        }

        // GET: Recommendation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recommendation = await _context.Recommendations
                .Include(r => r.FirstClothing)
                .Include(r => r.SecondClothing)
                .Include(r => r.ThirdClothing)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recommendation == null)
            {
                return NotFound();
            }

            return View("~/Views/Administration/Recommendation/Delete.cshtml", recommendation);
        }

        // POST: Recommendation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recommendation = await _context.Recommendations.FindAsync(id);
            _context.Recommendations.Remove(recommendation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecommendationExists(int id)
        {
            return _context.Recommendations.Any(e => e.Id == id);
        }
    }
}
