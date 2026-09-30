//// Licensed to the .NET Foundation under one or more agreements.
//// The .NET Foundation licenses this file to you under the MIT license.
//#nullable disable

//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Text.Encodings.Web;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI.Services;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.AspNetCore.WebUtilities;
//using Microsoft.Extensions.Logging;
//using roomit2.Models;

//namespace roomit2.Areas.Identity.Pages.Account
//{
//    public class RegisterModel : PageModel
//    {
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly IUserStore<ApplicationUser> _userStore;
//        private readonly IUserEmailStore<ApplicationUser> _emailStore;
//        private readonly ILogger<RegisterModel> _logger;
//        private readonly IEmailSender _emailSender;
//        private readonly RoomitContext _context;

//        public RegisterModel(
//            UserManager<ApplicationUser> userManager,
//            IUserStore<ApplicationUser> userStore,
//            SignInManager<ApplicationUser> signInManager,
//            ILogger<RegisterModel> logger,
//            IEmailSender emailSender,
//            RoomitContext context)
//        {
//            _userManager = userManager;
//            _userStore = userStore;
//            _emailStore = GetEmailStore();
//            _signInManager = signInManager;
//            _logger = logger;
//            _emailSender = emailSender;
//            _context = context;
//        }

//        /// <summary>
//        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//        ///     directly from your code. This API may change or be removed in future releases.
//        /// </summary>
//        [BindProperty]
//        public InputModel Input { get; set; }

//        /// <summary>
//        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//        ///     directly from your code. This API may change or be removed in future releases.
//        /// </summary>
//        public string ReturnUrl { get; set; }

//        public string serverMessage { get; set; }

//        /// <summary>
//        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//        ///     directly from your code. This API may change or be removed in future releases.
//        /// </summary>
//        public IList<AuthenticationScheme> ExternalLogins { get; set; }

//        /// <summary>
//        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//        ///     directly from your code. This API may change or be removed in future releases.
//        /// </summary>
//        public class InputModel
//        {
//            /// <summary>
//            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//            ///     directly from your code. This API may change or be removed in future releases.
//            /// </summary>
//            [Required(ErrorMessage = "E-mail ist ein  Pflichtfeld")]
//            [EmailAddress(ErrorMessage = "Email Format wird nicht erkannt")]
//            [Display(Name = "E-Mail")]
//            public string Email { get; set; }

//            /// <summary>
//            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//            ///     directly from your code. This API may change or be removed in future releases.
//            /// </summary>
//            [Required(ErrorMessage = "Passwort ist ein  Pflichtfeld")]
//            [StringLength(100, ErrorMessage = "Die {0} muss mindestens {2} und maximal {1} ​​Zeichen lang sein.", MinimumLength = 8)]
//            [DataType(DataType.Password)]
//            [Display(Name = "Passwort")]
//            public string Password { get; set; }

//            /// <summary>
//            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
//            ///     directly from your code. This API may change or be removed in future releases.
//            /// </summary>
//            [DataType(DataType.Password)]
//            [Display(Name = "Bestätige das Passwort")]
//            [Compare("Password", ErrorMessage = "Das Passwort und das Bestätigungspasswort stimmen nicht überein.")]
//            public string ConfirmPassword { get; set; }

//            public string? KundenType { get; set; }
//            public string? Anrede { get; set; }

//            [Required(ErrorMessage = "Vorname ist ein  Pflichtfeld")]
//            [DataType(DataType.Text, ErrorMessage = "Text Format wird nicht erkannt")]
//            public string Vorname { get; set; }

//            [Required(ErrorMessage = "Nachname ist ein  Pflichtfeld ")]
//            [DataType(DataType.Text, ErrorMessage = "Text Format wird nicht erkannt")]
//            public string Nachname { get; set; }

//            [CheckBoxRequired(ErrorMessage = "akzeptieren Sie bitte die Geschäftsbedingungen")]
//            public bool GeschäftsbedingungenAkzeptiert { get; set; }

//            [Required(ErrorMessage = "Telefon ist ein  Pflichtfeld ")]
//            [DataType(DataType.PhoneNumber)]
//            public string Telefon { get; set; }   

//            public bool HasTowAdresse { get; set; }
//            public Adresse RechnungAdresse { get; set; }
//            public Adresse LieferAdresse { get; set; }

//        }


//        public async Task OnGetAsync(string returnUrl = null)
//        {
//            ReturnUrl = returnUrl;
//            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
//        }

//        //public async Task<IActionResult> OnPostAsync(string returnUrl = null)
//        //{
//        //    returnUrl ??= Url.Content("~/");
//        //    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
//        //    if (ModelState.IsValid)
//        //    {
//        //        var user = CreateUser();

//        //        if (Input.HasTowAdresse)
//        //        {
//        //            if (Input?.LieferAdresse?.Straße != null || Input?.LieferAdresse?.Ort != null || Input?.LieferAdresse?.PLZ != null)
//        //            {
//        //                Adresse Lieferadresse = new Adresse()
//        //                {
//        //                    AdresseTyp = "LieferAdresse",
//        //                    Straße = Input.LieferAdresse.Straße,
//        //                    Ort = Input.LieferAdresse.Ort,
//        //                    PLZ = Input.LieferAdresse.PLZ,
//        //                    ApplicationUserID = user.Id,
//        //                };

//        //                await _context.TblAdressen.AddAsync(Lieferadresse);
//        //                await _context.SaveChangesAsync();
//        //            }
//        //            else 
//        //            {
//        //                serverMessage = "Rechnungs- und Lieferadresse weichen voneinander ab, daher ist die Lieferadresse ist ein Pflichtfeld";
//        //                return Page();
//        //            }
                    
//        //        }

//        //        if (Input.RechnungAdresse.Straße != null)
//        //        {
//        //            Adresse rechnungadresse = new Adresse()
//        //            {
//        //                AdresseTyp = "RechnungAdresse",
//        //                Straße = Input.RechnungAdresse.Straße,
//        //                Ort = Input.RechnungAdresse.Ort,
//        //                PLZ = Input.RechnungAdresse.PLZ,
//        //                ApplicationUserID = user.Id,
//        //            };

//        //           await _context.TblAdressen.AddAsync(rechnungadresse);
//        //           await _context.SaveChangesAsync();
//        //        }
//        //        IdentityResult result = new IdentityResult();
//        //        var userAddressen = _context.TblAdressen.Where(a => a.ApplicationUserID == user.Id).ToList();

//        //        if (userAddressen.Count > 0)
//        //        {
//        //            user.Anrede = Input.Anrede;
//        //            user.KundenType = Input.KundenType;
//        //            user.Vorname = Input.Vorname;
//        //            user.Nachname = Input.Nachname;
//        //            user.GeschäftsbedingungenAkzeptiert = Input.GeschäftsbedingungenAkzeptiert;
//        //            user.PhoneNumber = Input.Telefon;
//        //            user.hasTowAdresse = Input.HasTowAdresse;
//        //            user.RechnungAdresseID = userAddressen.FirstOrDefault(a => a.AdresseTyp == "RechnungAdresse") != null ? userAddressen.FirstOrDefault(a => a.AdresseTyp == "RechnungAdresse").ID : 0 ;
//        //            user.LieferAdresseID = userAddressen.FirstOrDefault(a => a.AdresseTyp == "LieferAdresse") != null ? userAddressen.FirstOrDefault(a => a.AdresseTyp == "LieferAdresse").ID : 0 ;

//        //            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
//        //            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
//        //            result = await _userManager.CreateAsync(user, Input.Password);

//        //        }

//        //        if (result.Succeeded)
//        //        {
//        //            _logger.LogInformation("User created a new account with password.");

//        //            var userId = await _userManager.GetUserIdAsync(user);
//        //            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
//        //            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
//        //            var callbackUrl = Url.Page(
//        //                "/Account/ConfirmEmail",
//        //                pageHandler: null,
//        //                values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
//        //                protocol: Request.Scheme);

//        //            await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
//        //                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

//        //            if (_userManager.Options.SignIn.RequireConfirmedAccount)
//        //            {
//        //                //return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
//        //                return RedirectToAction("Index", "Shop");
//        //            }
//        //            else
//        //            {
//        //                await _signInManager.SignInAsync(user, isPersistent: false);
//        //                return LocalRedirect(returnUrl);
//        //            }
//        //        }
//        //        // falls result.Succeeded = False, dann soll die neuen Adressen entfernen
//        //        _context.TblAdressen.RemoveRange(userAddressen);
//        //        await _context.SaveChangesAsync();

//        //        // Username 'Sale441@hotmail.de' is already taken.
//        //        //Email 'Sale441@hotmail.de' is already taken
//        //        foreach (var error in result.Errors)
//        //        {
//        //            if (error.Description.Contains("is already taken."))
//        //            {
//        //                error.Description = error.Description.Replace("is already taken.", "ist schon vergeben.");
//        //            }
//        //            if (!error.Description.Contains("Username"))
//        //            {
//        //                ModelState.AddModelError(string.Empty, error.Description);
//        //            }
//        //        }
//        //    }

//        //    // If we got this far, something failed, redisplay form
//        //    return Page();
//        //}

//        private ApplicationUser CreateUser()
//        {
//            try
//            {
//                return Activator.CreateInstance<ApplicationUser>();
//            }
//            catch
//            {
//                throw new InvalidOperationException($"Es kann keine Instanz von „{nameof(ApplicationUser)}“ erstellt werden. " +
//                    $"Stellen Sie sicher, dass '{nameof(ApplicationUser)}' keine abstrakte Klasse ist und über einen Konstruktor ohne Parameter verfügt, oder alternativ " +
//                    $"überschreibt die Registerseite in /Areas/Identity/Pages/Account/Register.cshtml");
//            }
//        }

//        private IUserEmailStore<ApplicationUser> GetEmailStore()
//        {
//            if (!_userManager.SupportsUserEmail)
//            {
//                throw new NotSupportedException("Die Standardbenutzeroberfläche erfordert einen Benutzerspeicher mit E-Mail-Unterstützung.");
//            }
//            return (IUserEmailStore<ApplicationUser>)_userStore;
//        }
//    }
//}
