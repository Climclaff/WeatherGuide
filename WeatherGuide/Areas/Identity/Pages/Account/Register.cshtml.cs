using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using WeatherGuide.Data;
using WeatherGuide.Models;
namespace WeatherGuide.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<Models.AppUser> _signInManager;
        private readonly UserManager<Models.AppUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        public RegisterModel(
            UserManager<Models.AppUser> userManager,
            SignInManager<Models.AppUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _sharedLocalizer = sharedLocalizer;
        }
        [TempData]
        public string StatusMessage { get; set; }
        [BindProperty(SupportsGet = true)]
        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Country")]
        public int CountryId { get; set; }
        [BindProperty(SupportsGet = true)]
        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "State")]
        public int StateId { get; set; }
        public SelectList Countries { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public class InputModel
        {
            [Required(ErrorMessage = "{0} is required")]
            [EmailAddress(ErrorMessage = "{0} is not valid")]
            [Display(Name = "EmailAddress")]
            public string Email { get; set; }

            [Required(ErrorMessage = "{0} is required")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "{0} is required")]          
            [RegularExpression(@"^(([A-za-zÀ-ÙÜÞß¥ª²¯à-ùüþÿ´º³¿]+[\s]{1}[A-za-zÀ-ÙÜÞß¥ª²¯à-ùüþÿ´º³¿]+)|([A-Za-zÀ-ÙÜÞß¥ª²¯à-ùüþÿ´º³¿]+))$", 
                ErrorMessage = "The field must contain alphabetical characters")]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required(ErrorMessage = "{0} is required")]
            [RegularExpression(@"^(([A-za-zÀ-ÙÜÞß¥ª²¯à-ùüþÿ´º³¿]+[\s]{1}[A-za-zÀ-ÙÜÞß¥ª²¯à-ùüþÿ´º³¿]+)|([A-Za-zÀ-ÙÜÞß¥ª²¯à-ùüþÿ´º³¿]+))$",
                ErrorMessage = "The field must contain alphabetical characters")]
            [Display(Name = "Surname")]
            public string Surname { get; set; }

        }
        public async Task<IEnumerable<State>> GetStatesAsync(int countryId)
        {
            return await _context.States.Where(s => s.CountryId == countryId).ToListAsync();
        }
        public async Task OnGetStateSelectAsync(string stateName, string countryId)
        {
            var state = await _context.Set<State>().Where(x => x.CountryId == Convert.ToInt32(countryId)).Where(x => x.Name == stateName).FirstOrDefaultAsync();
            if (state != null)
            {
                TempData["StateId"] = Convert.ToString(state.Id);
                TempData.Keep("StateId");
            }
        }
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            Countries = new SelectList(_context.Countries, nameof(Country.Id), nameof(Country.Name));
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }
        public async Task<JsonResult> OnGetStatesAsync()
        {
            return new JsonResult(await GetStatesAsync(CountryId));
        }
        public async Task<IActionResult> OnPostRegisterAsync(string returnUrl = null)
        {          
            returnUrl = returnUrl ?? Url.Content("~/"); 
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();           
            StateId = Convert.ToInt32(TempData.Peek("StateId"));
            TempData.Remove("StateId");
            if (CountryId != 0 && StateId > 0)
            {
                ModelState.Remove(nameof(StateId));
            }
            if (ModelState.IsValid)
            {
                var user = new Models.AppUser { UserName = Input.Email, Email = Input.Email };
                    user.CountryId = CountryId;
                    user.StateId = StateId;
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {

                    List<Claim> claims = new List<Claim> { 
                        new Claim("Name", Input.Name), 
                        new Claim("Surname", Input.Surname),
                        new Claim("IsAdmin", "false"),
                        new Claim("IsDesigner", "false")
                        };
                    for (int i = 0; i<claims.Count; ++i)
                    await _userManager.AddClaimAsync(user, claims[i]);        
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            StatusMessage = _sharedLocalizer["Error during registration, maybe user with such name already exists"];
            return RedirectToPage();        
        }
    }
}
