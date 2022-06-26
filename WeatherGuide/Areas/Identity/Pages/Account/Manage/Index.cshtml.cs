using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using WeatherGuide.Attributes;
using WeatherGuide.Data;
using WeatherGuide.Models;
namespace WeatherGuide.Areas.Identity.Pages.Account.Manage
{
    [WebRequestLimitRazor(Name = "Limit Profile Web", Seconds = 10, MaxRequestCount = 30)]
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<Models.AppUser> _userManager;
        private readonly SignInManager<Models.AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IDistributedCache _cache;
        public IndexModel(
            UserManager<Models.AppUser> userManager,
            SignInManager<Models.AppUser> signInManager,
            ApplicationDbContext context,
            IStringLocalizer<SharedResource> sharedLocalizer,
            IDistributedCache cache)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _sharedLocalizer = sharedLocalizer;
            _cache = cache;
        }

        public string Username { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CountryId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int StateId { get; set; }
        public SelectList Countries { get; set; }
        public SelectList States { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

        }

        private async Task LoadAsync(Models.AppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var countryId = user.CountryId;
            var stateId = user.StateId;
            Username = userName;
            CountryId = (int)countryId;
            StateId = (int)stateId;
            Input = new InputModel
            {

            };
        }
        public async Task<IEnumerable<State>> GetStatesAsync(int countryId)
        {
            return await _context.States.Where(s => s.CountryId == countryId).ToListAsync();
        }
        public async Task<JsonResult> OnGetStatesAsync()
        {
            return new JsonResult(await GetStatesAsync(CountryId));
        }
        public async Task OnGetStateSelectAsync(string stateName, string countryId)
        {
            var state = await _context.Set<State>().Where(x => x.CountryId == Convert.ToInt32(countryId)).Where(x => x.Name == stateName).FirstOrDefaultAsync();
            TempData["StateId"] = Convert.ToString(state.Id);
            TempData.Keep("StateId");
        }
        public async Task OnGetCountrySelectAsync(string id)
        {
            var state = await _context.Set<State>().Where(x => x.CountryId == Convert.ToInt32(id)).FirstOrDefaultAsync();
            TempData["StateId"] = Convert.ToString(state.Id);
            TempData.Keep("StateId");
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }           
            await LoadAsync(user);
            Countries = new SelectList(_context.Countries, nameof(Country.Id), nameof(Country.Name));
            States = new SelectList(_context.States.Where(x => x.CountryId == CountryId), nameof(State.Id), nameof(State.Name));
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var cacheEntry = _cache.GetAsync(user.Id.ToString() + "recommendation");
            if(cacheEntry.Result != null)
            {
                StatusMessage = _sharedLocalizer["Recommendation cooldown"];
                return RedirectToPage();
            }

            StateId = Convert.ToInt32(TempData.Peek("StateId"));
            if (StateId == 0)
            {
                var state = await _context.Set<State>().Where(x => x.CountryId == CountryId).FirstOrDefaultAsync();
                StateId = state.Id;
            }
            if (StateId != user.StateId && StateId != 0)
            {
                ModelState.Remove(nameof(StateId));               
            }
            TempData.Remove("StateId");
            if (!ModelState.IsValid || StateId == 0)
            {
                await LoadAsync(user);
                return RedirectToPage();
            }
            ModelState.SetModelValue(nameof(StateId), new Microsoft.AspNetCore.Mvc.ModelBinding.ValueProviderResult());
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (user.CountryId == CountryId && user.StateId == StateId)
            {
                return RedirectToPage();
            }
            if (CountryId != user.CountryId || StateId != user.StateId)
            {               
                    user.CountryId = CountryId;
                    user.StateId = StateId;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
            }

                await _signInManager.RefreshSignInAsync(user);
            StatusMessage = _sharedLocalizer["Your profile has been updated"];
            return RedirectToPage();
        }
    }
}
