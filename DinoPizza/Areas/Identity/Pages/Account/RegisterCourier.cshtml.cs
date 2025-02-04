using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using DinoPizza.Authorize;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.WebUtilities;
using DinoPizza.Models;
using DinoPizza.DataAccessLayer;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace DinoPizza.Areas.Identity.Pages.Account
{
    public class RegisterCourierModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IUserEmailStore<AppUser> _emailStore;
        private readonly ILogger<RegisterCourierModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly DinoDBContext _context;

        public RegisterCourierModel(
            UserManager<AppUser> userManager,
            IUserStore<AppUser> userStore,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterCourierModel> logger,
            IEmailSender emailSender,
            DinoDBContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Display(Name = "Имя")]
            public string Firstname { get; set; }

            [Required]
            [StringLength(12, MinimumLength = 10, ErrorMessage = "ИНН должен содержать от 10 до 12 цифр.")]
            [RegularExpression(@"^\d+$", ErrorMessage = "ИНН может содержать только цифры.")]
            [Display(Name = "ИНН")]
            public string INN { get; set; }

            [Phone]
            [Required]
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "Номер телефона")]
            public string PhoneNumber { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Пароль должен содержать от {2} до {1} символов.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Подтвердите пароль")]
            [Compare("Password", ErrorMessage = "Пароль и подтверждение не совпадают.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();
                var cleanedPhoneNumber = new string(Input.PhoneNumber.Where(c => Char.IsDigit(c) || c == '+').ToArray());

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                user.FirstName = Input.Firstname;
                user.PhoneNumber = cleanedPhoneNumber;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    result = await _userManager.AddToRoleAsync(user, "courier");
                    if (result == null || !result.Succeeded)
                    {
                        _logger.LogInformation("User not in Role 'courier'.");
                    }

                    
                    var courier = new Courier
                    {
                        Id = Guid.NewGuid(),
                        Name = Input.Firstname,
                        INN = Input.INN,
                        PhoneNumber = cleanedPhoneNumber,
                        IsAvailable = true,
                        UserId = user.Id
                    };

                    _context.Couriers.Add(courier);
                    await _context.SaveChangesAsync();

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Подтвердите ваш email",
                        $"Пожалуйста, подтвердите ваш аккаунт, <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>кликнув здесь</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private AppUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AppUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor.");
            }
        }

        private IUserEmailStore<AppUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AppUser>)_userStore;
        }
    }
}
