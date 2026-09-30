// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using roomit2.Models;

namespace roomit2.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "E-Mail")]
            public string Email { get; set; }
            public string? KundenType { get; set; }
            public string? Anrede { get; set; }

            [Required(ErrorMessage = "Vorname ist ein  Pflichtfeld")]
            [DataType(DataType.Text)]
            public string? Vorname { get; set; }

            [Required(ErrorMessage = "Nachname ist ein  Pflichtfeld ")]
            [DataType(DataType.Text)]
            public string? Nachname { get; set; }
            public bool GeschäftsbedingungenAkzeptiert { get; set; }

            [Required(ErrorMessage = "Telefon ist ein  Pflichtfeld ")]
            [DataType(DataType.PhoneNumber)]
            public string? PhoneNumber { get; set; }

            public bool IsTowAdresse { get; set; }

            public Adresse RechnungAdresse { get; set; }
            public Adresse LieferAdresse { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Email = user.Email,
                KundenType = user.KundenType,
                Anrede = user.Anrede,
                Vorname = user.Vorname,
                Nachname = user.Nachname,
                GeschäftsbedingungenAkzeptiert = user.GeschäftsbedingungenAkzeptiert,
                IsTowAdresse = user.hasTowAdresse,

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

        public async Task<IActionResult> OnPostAsync()
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

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (Input.Anrede != user.Anrede)
            {
                user.Anrede = Input.Anrede;
            }
            if (Input.Nachname != user.Nachname)
            {
                user.Nachname = Input.Nachname;
            }
            if (Input.IsTowAdresse != user.hasTowAdresse)
            {
                user.hasTowAdresse = Input.IsTowAdresse;
            }
            if (Input.Email != user.Email)
            {
                user.Email = Input.Email;
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
