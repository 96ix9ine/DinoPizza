using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using DinoPizza.Authorize;
using DinoPizza.Models;

namespace DinoPizza.BusinessLogic
{
    public class ServiceAdmin
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ILogger<ServiceAdmin> _logger;

        public ServiceAdmin(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, ILogger<ServiceAdmin> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _logger = logger;
        }

        public List<RoleSimpleModel> GetRoleModels()
        {
            return _roleManager.Roles.Select(x => _mapper.Map<RoleSimpleModel>(x)).ToList();
        }

        public async Task<List<UserSimpleModel>> GetUsersModelsAsync()
        {
            var users = _userManager.Users.ToList();

            var modelList = new List<UserSimpleModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var model = _mapper.Map<UserSimpleModel>(user);
                model.RolesList = roles.ToList();
                modelList.Add(model);
            }

            return modelList;
        }

        public async Task<UserEditModel> GetUserEditModelAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var model = _mapper.Map<UserEditModel>(user);

            var roles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            var checkBoxList = new List<CheckBoxItemStringId>();
            foreach (var role in allRoles)
            {
                var checkbox = new CheckBoxItemStringId()
                {
                    Id = role.Id,
                    Name = role.Name,
                    IsChecked = roles.Any(x => x == role.Name)
                };
                checkBoxList.Add(checkbox);
            }
            model.RolesList = checkBoxList;

            return model;
        }

        public async Task<List<CheckBoxItemStringId>> GetUsersWithoutRoles()
        {
            var usersNotInRole = new List<CheckBoxItemStringId>();
            foreach (var u in _userManager.Users.ToList())
            {
                var roles = await _userManager.GetRolesAsync(u);
                if(roles.Count == 0)
                {
                    var checkBox = new CheckBoxItemStringId
                    {
                        Id = u.Id,
                        Name = u.UserName,
                        IsChecked = false
                    };
                    usersNotInRole.Add(checkBox);
                }
            }

            return usersNotInRole;
        }

        public async Task<RoleEditModel> GetRoleEditModelAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                throw new Exception($"Role '{roleId}' not found");

            var allPermissions = ClaimsHelper.GetAllPermissions();

            var currentClaims = await _roleManager.GetClaimsAsync(role);



            var checkBoxList = allPermissions.Select(x => new CheckBoxItemStringId
            {
                Id = $"{x.ClaimType}.{x.ClaimValue}",
                Name = $"{x.ClaimType}.{x.ClaimValue}",
                IsChecked = currentClaims.Any(y => y.Value == $"{x.ClaimType}.{x.ClaimValue}")
            }).ToList();

            var model = new RoleEditModel()
            {
                Id = roleId,
                Name = role.Name
            };
            model.PermissionsList = checkBoxList;

            var allRoles = _roleManager.Roles.Where(x => x.Name != AuthConstants.Roles.User).ToList();
            var usersCheckBoxList = new List<CheckBoxItemStringId>();

            foreach (var r in allRoles)
            {
                var users = await _userManager.GetUsersInRoleAsync(r.Name);

                bool isChecked = r.Name == role.Name;
                var list = users.Select(u=> new CheckBoxItemStringId
                {
                    Id = u.Id,
                    Name = u.UserName,
                    IsChecked = isChecked
                }).ToList();

                //добавляем в общий список, если такого ещё нет
                foreach (var u in list)
                {
                    if (usersCheckBoxList.All(x => x.Name != u.Name))
                        usersCheckBoxList.Add(u);
                }
                
            }

            var usersWithoutRoles = await GetUsersWithoutRoles();
            usersCheckBoxList.AddRange(usersWithoutRoles);

            model.UsersList = usersCheckBoxList;
            return model;
        }
        public async Task UpdateUserAsync(UserEditModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                throw new Exception("User not found");

            user.FirstName=model.FirstName;
            user.PhoneNumber=model.Phone;
            user.Address=model.Address;

            //user = _mapper.Map<AppUser>(model);
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                _logger.Log(LogLevel.Error, "Update user faild");
            }
            var currentRolesList = await _userManager.GetRolesAsync(user);

            //убрать роли, которые не отмечены в списке 
            foreach (var roleName in currentRolesList)
            {
                if(model.RolesList.Any(x => x.Name == roleName && !x.IsChecked))
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                }
            }

            //добавить роли, коорые отмечены
            foreach (var item in model.RolesList.Where(x=>x.IsChecked))
            {
                if (!currentRolesList.Contains(item.Name))
                {
                    await _userManager.AddToRoleAsync(user, item.Name);
                }
            }
        }

        public async Task UpdateRoleAsync(RoleEditModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
                throw new Exception($"Role [{model.Id}] not found");

            var currentClaims = await _roleManager.GetClaimsAsync(role);
            //1 удаляем права которые не выбраны в модели
            foreach (var item in model.PermissionsList.Where(x => !x.IsChecked))
            {
                var claimToRemove = currentClaims.FirstOrDefault(x => x.Value == item.Name);
                if (claimToRemove != null)
                {
                    await _roleManager.RemoveClaimAsync(role, claimToRemove);
                }
            }

            //2 добавляем права которые выбраны
            foreach (var item in model.PermissionsList.Where(x => x.IsChecked))
            {
                string[] parts = item.Name.Split('.');
                var claimToAdd = currentClaims.FirstOrDefault(x => x.Value == parts[1]);
                if (claimToAdd == null)
                {
                    claimToAdd = new Claim(parts[0], item.Name);
                    await _roleManager.AddClaimAsync(role, claimToAdd);
                }
            }

            //Обновить связи Роли и юзеров
            var currentUsers = await _userManager.GetUsersInRoleAsync(role.Name);

            //1 удаляем юзеров, которые не выбраны в модели
            foreach (var checkBox in model.UsersList.Where(x=>!x.IsChecked))
            {
                var user = currentUsers.FirstOrDefault(x => x.UserName == checkBox.Name);
                if (user != null)
                {
                    _userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }

            //2 добавляем тех юзеров, которые выбраны
            foreach (var checkBox in model.UsersList.Where(x => x.IsChecked))
            {
                var user = currentUsers.FirstOrDefault(x => x.UserName == checkBox.Name);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(checkBox.Name);

                    await _userManager.AddToRoleAsync(user, role.Name);
                }
            }
        }
        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID '{userId}' not found.");
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to delete user with ID '{userId}'. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return false;
            }

            _logger.LogInformation($"User with ID '{userId}' deleted successfully.");
            return true;
        }
    }
}
