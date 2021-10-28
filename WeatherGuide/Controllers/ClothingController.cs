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
using System.IO;

namespace WeatherGuide.Controllers
{
    [Authorize("IsAdminPolicy")]
    [Route("Administration/[controller]/[action]/{id?}")]
    public class ClothingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClothingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clothing
        public async Task<IActionResult> Index()
        {          
            var applicationDbContext = _context.Clothings.Include(c => c.Category);
            return View("~/Views/Administration/Clothing/index.cshtml", await applicationDbContext.ToListAsync());
        }

        // GET: Clothing/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clothing = await _context.Clothings
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clothing == null)
            {
                return NotFound();
            }

            return View("~/Views/Administration/Clothing/Details.cshtml", clothing);
        }

        // GET: Clothing/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id");
            return View("~/Views/Administration/Clothing/Create.cshtml");
        }

        // POST: Clothing/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameEN,NameUA,CategoryId,ImageData")] Clothing clothing)
        {
            if (ModelState.IsValid && Request.Form.Files.Count > 0)
            {
                clothing.ImageData = UploadFile();
                _context.Add(clothing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", clothing.CategoryId);
            return View("~/Views/Administration/Clothing/Create.cshtml", clothing);
        }

        // GET: Clothing/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clothing = await _context.Clothings.FindAsync(id);
            if (clothing == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", clothing.CategoryId);
            return View("~/Views/Administration/Clothing/Edit.cshtml", clothing);
        }

        // POST: Clothing/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameEN,NameUA,CategoryId,ImageData")] Clothing clothing)
        {
            if (id != clothing.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid && Request.Form.Files.Count > 0)
            {
                try
                {
                    clothing.ImageData = UploadFile();
                    _context.Update(clothing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClothingExists(clothing.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", clothing.CategoryId);
            return View("~/Views/Administration/Clothing/Edit.cshtml", clothing);
        }

        // GET: Clothing/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clothing = await _context.Clothings
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clothing == null)
            {
                return NotFound();
            }

            return View("~/Views/Administration/Clothing/Delete.cshtml", clothing);
        }

        // POST: Clothing/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clothing = await _context.Clothings.FindAsync(id);
            _context.Clothings.Remove(clothing);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClothingExists(int id)
        {
            return _context.Clothings.Any(e => e.Id == id);
        }
        private byte[] UploadFile()
        {
                var file = Request.Form.Files[0];
                MemoryStream ms = new MemoryStream();

                file.CopyTo(ms);
                byte[] imgData = ms.ToArray();
                ms.Close();
                ms.Dispose();

            return imgData;
        }
    }
}
