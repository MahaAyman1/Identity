using cmspro1.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Data;
using System.Reflection.Metadata.Ecma335;

namespace cmspro1.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region ConfigrationCode
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        #endregion

        #region Uesercong
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.Mobile
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
                foreach (var i in result.Errors)
                {
                    ModelState.AddModelError(i.Code, i.Description);
                }
                return View(model);

            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,  false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }



        public IActionResult WelcomePage()
        {
            return View();
        }
        #endregion

        #region Roles
        [Authorize(Roles = "Admin")]

        public IActionResult RolesList()
        {
            return View(_roleManager.Roles);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = model.RoleName
                };
                var r = await _roleManager.CreateAsync(role);
                if (r.Succeeded)
                {
                    return RedirectToAction("RolesList");
                }
                foreach (var err in r.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return View(model);

            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> EditRole(String id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            EditRoleViewModel model = new EditRoleViewModel
            {
                RoleName = role.Name!,

                RoleID = role.Id
            };
            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name!))
                {
                    model.Users.Add(user.UserName!);
                }
            }

                return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.RoleID);
                if (role == null)
                {
                    return RedirectToAction("NotFound");
                }

                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("RolesList");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }

            return View(model);
        }
       

        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

         

            return View(role);
        }

       

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteRolee(String id) {

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return RedirectToAction("NotFound");
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("RolesList");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return View(role);
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> EditUserInRole( string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if(role == null)
            {
                return RedirectToAction("NotFound");
            }
            var model = new List<UserRoleViewModel>();
            foreach (var user in _userManager.Users)
            {
                UserRoleViewModel userRoleViewModel = new UserRoleViewModel
                {
                    UserName = user.UserName,
                    UserId = user.Id,   
                };
                if(await _userManager.IsInRoleAsync(user, role.Name!))
                {
                    userRoleViewModel.IsSelected=true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            }
            return View(model); 
        }
       
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> EditUserInRole(List<UserRoleViewModel> model, string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return RedirectToAction("NotFound");
            }
            for (int i = 0; i < model.Count; i++) {
                var user = await _userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user!, role.Name!)))
                {
                    result = await _userManager.AddToRoleAsync(user!, role.Name!);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user!, role.Name!))
                {
                    result = await _userManager.RemoveFromRoleAsync(user!, role.Name!);
                }
                else
                {
                    continue;
                }
                return RedirectToAction("RolesList");


            }
            return RedirectToAction("RolesList");
        }

        public IActionResult NotFound()
        {
            return View();  
        }

        public IActionResult ss()
        {
            return View();  
        }
        #endregion


    }
}