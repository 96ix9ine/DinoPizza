using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DinoPizza.Authorize;
using DinoPizza.BusinessLogic;
using DinoPizza.Models;

namespace DinoPizza.Controllers
{
    [Authorize(Roles = AuthConstants.Roles.Admin)]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ServiceAdmin _serviceAdmin;

        public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, ServiceAdmin serviceAdmin)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _serviceAdmin = serviceAdmin;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ManageUsersView()
        {
            var modelList = await _serviceAdmin.GetUsersModelsAsync();

            return View(modelList);
        }

        public IActionResult ManageRolesView()
        {
            var modelList = _serviceAdmin.GetRoleModels();

            return View(modelList);
        }

        [HttpPost]
        public async Task<IActionResult> EditRolesView(RoleEditModel model)
        {
            if (ModelState.IsValid)
            {
                await _serviceAdmin.UpdateRoleAsync(model);
                string url = Url.Action("Index", "Admin");
                return Redirect(url);
            }

            return View(model);
        }

        public async Task<IActionResult> EditUsersView(string userId)
        {
            var model = await _serviceAdmin.GetUserEditModelAsync(userId);

            return View(model);
        }

        public async Task<IActionResult> EditRolesView(string roleId)
        {
            var model = await _serviceAdmin.GetRoleEditModelAsync(roleId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersView(UserEditModel model)
        {
            if (ModelState.IsValid)
            {
                await _serviceAdmin.UpdateUserAsync(model);

                string url = Url.Action("Index", "Admin");
                return Redirect(url);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUsersView(UserSimpleModel model)
        {
            if (model.UserId == null)
            {
                ModelState.AddModelError("", "Идентификатор пользователя не указан.");
                return RedirectToAction("ManageUsersView");
            }

            var result = await _serviceAdmin.DeleteUserAsync(model.UserId);
            if (!result)
            {
                ModelState.AddModelError("", "Не удалось удалить пользователя.");
                return RedirectToAction("ManageUsersView");
            }

            return RedirectToAction("ManageUsersView");
        }

    }
}
