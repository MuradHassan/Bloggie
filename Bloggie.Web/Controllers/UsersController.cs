using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICommentRepository _blogPostCommentRepository;
        private readonly ILikeRepository _blogPostLikeRepository;

        public UsersController
            (
            IUserRepository userRepository,
            UserManager<IdentityUser> userManager,
            ICommentRepository blogPostCommentRepository,
            ILikeRepository blogPostLikeRepository
            )
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _blogPostCommentRepository = blogPostCommentRepository;
            _blogPostLikeRepository = blogPostLikeRepository;
        }


        [HttpGet]
        public async Task<IActionResult> List(string? search, string? sort = "Asc", int pageNumber = 1)
        {

            int pageSize = 3;
            var allUsers = await _userManager.Users.ToListAsync();
            var filteredUsers = new List<IdentityUser>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // Admin Role
                if (User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
                {
                    if (roles.Contains("User") &&
                        !roles.Contains("Admin") &&
                        !roles.Contains("SuperAdmin"))
                    {
                        filteredUsers.Add(user);
                    }
                }
                else
                {
                    // SuperAdmin Role
                    filteredUsers.Add(user);
                }
            }

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                filteredUsers = filteredUsers.Where(x =>
                    x.UserName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    x.Email.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Sort
            filteredUsers = sort switch
            {
                "Desc" => filteredUsers.OrderByDescending(x => x.UserName).ToList(),
                _ => filteredUsers.OrderBy(x => x.UserName).ToList()
            };

            var totalUsersCount = filteredUsers.Count;

            var usersList = filteredUsers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new UserViewModel
            {
                Users = new List<User>()
            };

            foreach (var user in usersList)
            {
                var roles = await _userManager.GetRolesAsync(user);

                vm.Users.Add(new User
                {
                    Id = Guid.Parse(user.Id),
                    UserName = user.UserName,
                    EmailAddress = user.Email,
                    RoleName = roles.Contains("SuperAdmin") ? "Super Admin"
                             : roles.Contains("Admin") ? "Admin"
                             : "User"
                });
            }

            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsersCount / pageSize);

            return View(vm);

        }

        [HttpPost]
        public async Task<IActionResult> Add(UserViewModel userViewModel)
        {
            var identityUser = new IdentityUser
            {
                UserName = userViewModel.UserName,
                Email = userViewModel.Email
            };

            var identityResult = await _userManager.CreateAsync(identityUser, userViewModel.Password);

            if (identityResult is not null)
            {
                if (identityResult.Succeeded)
                {
                    // assign roles to this user
                    var roles = new List<string> { "User" };

                    if (User.IsInRole("SuperAdmin") && userViewModel.AdminRoleCheckbox)
                    {
                        roles.Add("Admin");
                    }

                    identityResult = await _userManager.AddToRolesAsync(identityUser, roles);

                    if (identityResult is not null && identityResult.Succeeded)
                    {
                        return RedirectToAction("List", "Users");
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user is null)
                return RedirectToAction("List");

            var targetRoles = await _userManager.GetRolesAsync(user);

            // Delete admin role
            if (User.IsInRole("Admin") &&
                !User.IsInRole("SuperAdmin") &&
                (targetRoles.Contains("Admin") || targetRoles.Contains("SuperAdmin")))
            {
                return Forbid();
            }

            // Remove user comments
            var comments = await _blogPostCommentRepository.GetCommentsByUserIdAsync(id);
            await _blogPostCommentRepository.DeleteRangeAsync(comments);

            // Remove user likes
            var likes = await _blogPostLikeRepository.GetLikesByUserIdAsync(id);
            await _blogPostLikeRepository.DeleteRangeAsync(likes);

            // Remve user
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return RedirectToAction("List");
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel userViewModel)
        {
            var user = await _userManager.FindByIdAsync(userViewModel.Id.ToString());

            if (user is null)
                return RedirectToAction("List");

            var targetRoles = await _userManager.GetRolesAsync(user);

            // Edit admin role
            if (User.IsInRole("Admin") &&
                !User.IsInRole("SuperAdmin") &&
                (targetRoles.Contains("Admin") || targetRoles.Contains("SuperAdmin")))
            {
                return Forbid();
            }

            user.UserName = userViewModel.UserName;
            user.Email = userViewModel.Email;

            await _userManager.UpdateAsync(user);

            // Edit user password
            if (!string.IsNullOrWhiteSpace(userViewModel.Password))
            {
                if (await _userManager.HasPasswordAsync(user))
                {
                    await _userManager.RemovePasswordAsync(user);
                }

                await _userManager.AddPasswordAsync(user, userViewModel.Password);
            }

            // SuperAdmin edit role
            if (User.IsInRole("SuperAdmin"))
            {
                var existingRoles = await _userManager.GetRolesAsync(user);

                await _userManager.RemoveFromRolesAsync(user, existingRoles);

                var newRoles = new List<string> { "User" };

                if (userViewModel.AdminRoleCheckbox)
                {
                    newRoles.Add("Admin");
                }

                await _userManager.AddToRolesAsync(user, newRoles);
            }

            return RedirectToAction("List");
        }
    }
}

