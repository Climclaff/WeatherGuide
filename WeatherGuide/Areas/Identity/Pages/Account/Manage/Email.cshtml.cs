using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using WeatherGuide.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace WeatherGuide.Areas.Identity.Pages.Account.Manage
{
    public partial class EmailModel : PageModel
    {
        private readonly UserManager<Models.AppUser> _userManager;
        private readonly SignInManager<Models.AppUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        public EmailModel(
            UserManager<Models.AppUser> userManager,
            SignInManager<Models.AppUser> signInManager,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _context = context;
        }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [RegularExpression(@"^(([A-za-zÀ-ÙÜÞß¥ª²¯à-ùüþÿ´º³¿]+[\s]{1}[A-za-zÀ-ÙÜÞß¥ª²¯à-ùüþÿ´º³¿]+)|([A-Za-zÀ-ÙÜÞß¥ª²¯à-ùüþÿ´º³¿]+))$",
                ErrorMessage = "The Name field must contain alphabetical characters")]
            [Display(Name = "New name")]
            public string NewName { get; set; }
            [Required]
            [RegularExpression(@"^(([A-za-zÀ-ÙÜÞß¥ª²¯à-ùüþÿ´º³¿]+[\s]{1}[A-za-zÀ-ÙÜÞß¥ª²¯à-ùüþÿ´º³¿]+)|([A-Za-zÀ-ÙÜÞß¥ª²¯à-ùüþÿ´º³¿]+))$",
                ErrorMessage = "The Surname field must contain alphabetical characters")]
            [Display(Name = "New surname")]
            public string NewSurname { get; set; }
        }

        private async Task LoadAsync(Models.AppUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            Input = new InputModel
            {
                NewName = claims[0].Value,
                NewSurname = claims[1].Value,
            };

        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var claims = await _userManager.GetClaimsAsync(user);
            bool changes = false;
            if (Input.NewName != claims[0].Value)
            {
                var claim = await _context.UserClaims.Where(x => x.ClaimType == "Name").Where(x => x.UserId == user.Id).SingleOrDefaultAsync();
                claim.ClaimValue = Input.NewName;
                _context.UserClaims.Update(claim);
                await _context.SaveChangesAsync();              
                changes = true;
            }
            if (Input.NewSurname != claims[1].Value)
            {
                var claim = await _context.UserClaims.Where(x => x.ClaimType == "Surname").Where(x => x.UserId == user.Id).SingleOrDefaultAsync();
                claim.ClaimValue = Input.NewSurname;
                _context.UserClaims.Update(claim);
                await _context.SaveChangesAsync();
                changes = true;
            }
            if(changes == true)
            {
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, false);
                StatusMessage = "Your info has been changed successfully.";
                return RedirectToPage();
            }
            StatusMessage = "Your info is unchanged.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}
