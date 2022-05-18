using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WeatherGuide.Attributes;
using WeatherGuide.Data;
using WeatherGuide.Models;
using WeatherGuide.Models.ViewModels;

namespace WeatherGuide.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<ReviewController> _localizer;
        public ReviewController(UserManager<AppUser> userManager,
            ApplicationDbContext context,
            IStringLocalizer<ReviewController> localizer)
        {
            _userManager = userManager; 
            _context = context;
            _localizer = localizer;
        }
        [WebRequestLimit(Name = "Limit Review Get", Seconds = 5, MaxRequestCount = 3)]
        public async Task<ActionResult> Reviews()
        {
            var applicationDbContext = _context.Reviews.Include(m => m.User);
            ReviewViewModel model = new ReviewViewModel();
            model.Reviews = await applicationDbContext.ToListAsync();
            model.Review = new Review();
            
            if (model.Reviews != null)
            {
                int i = 0;
                bool[] commentOwnership = new bool[applicationDbContext.ToListAsync().Result.Count()];
                var user = await _userManager.GetUserAsync(HttpContext.User);
                foreach (var review in model.Reviews)
                {
                    bool isCommentOwner = IsCommentOwner(user, review);
                    if (isCommentOwner)
                    {
                        commentOwnership[i] = true;                       
                    }
                    i++;
                }
                TempData["commentOwnership"] = commentOwnership;
            }

            return View(model);
        }

        [WebRequestLimit(Name = "Limit Review Post", Seconds = 5, MaxRequestCount = 2)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddReview([Bind("Id,Content,AppUserId,DateTime")] Review review)
        {
            if (review.Content == null)
            {
                TempData["ErrorMessage"] = _localizer["Empty review"].ToString();
                return RedirectToAction(nameof(Reviews));
            }
            if (review.Content.Count() > 500)
            {
                TempData["ErrorMessage"] = _localizer["Message too big"].ToString();
                return RedirectToAction(nameof(Reviews));
            }
            review.DateTime = DateTime.UtcNow;
            var user = await _userManager.GetUserAsync(HttpContext.User);
            review.User = user;
            review.AppUserId = user.Id;
            if (ModelState.IsValid)
            {               
                _context.Add(review);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = _localizer["Review shared"].ToString();
                return RedirectToAction(nameof(Reviews));
            }
            TempData["ErrorMessage"] = _localizer["Something went wrong"].ToString();
            return RedirectToAction(nameof(Reviews));           
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(IFormCollection collection)
        {            
            try
            {
                string val = collection["commentId"];
                if (val != null)
                {
                   int id = Convert.ToInt32(val);
                   var user = await _userManager.GetUserAsync(HttpContext.User);
                    if (user != null)
                    {
                        IList<Claim> claims = await _userManager.GetClaimsAsync(user);                      
                        Review review = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id);

                        bool isCommentOwner = IsCommentOwner(user, review);
                        if (isCommentOwner)
                        {
                            _context.Reviews.Remove(review);
                            await _context.SaveChangesAsync();
                            TempData["SuccessMessage"] = _localizer["Review deleted"].ToString();
                            return RedirectToAction(nameof(Reviews));
                        }
                        for (int i = 0; i<claims.Count; i++)
                        {
                            if (claims[i].Type.ToString() == "IsAdmin" && claims[i].Value.ToString() == "true")
                            {
                                _context.Reviews.Remove(review);
                                await _context.SaveChangesAsync();
                                TempData["SuccessMessage"] = _localizer["Review deleted"].ToString();
                                return RedirectToAction(nameof(Reviews));
                            }
                        }
                    }
                }
                TempData["ErrorMessage"] = _localizer["Cant delete other reviews"].ToString();
                return RedirectToAction(nameof(Reviews));
            }
            catch
            {
                TempData["ErrorMessage"] = _localizer["Something went wrong"].ToString();
                return View();
            }
        }
        private bool IsCommentOwner(AppUser user, Review review)
        {
            if (review != null)
            {
                if (user.Id == review.AppUserId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
