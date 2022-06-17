using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Socail.BL.Dtos;
using Socail.DAL.Database;
using Socail.DAL.Extend;

namespace Socail.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AdminsController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }


        [HttpGet]
        [Route("~/GetUserWithRoles")]
        public async Task<IActionResult> GetUserWithRoles()
        {
            List<object> userRoles = new List<object>();
            var userList = await userManager.Users.ToListAsync();

            foreach (var user in userList)
            {
                var roles = await userManager.GetRolesAsync(user);
                var userwithrole = new
                {
                    Id= user.Id,
                    FullName = user.FullName,
                    Roles = roles
                };
                userRoles.Add(userwithrole);
            }


            return Ok(userRoles);

        }

        [HttpPost]
        [Route("~/EditRoles/{userId}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> EditRoles(string userId , RoleEditDto dto)
        {
            var user = await userManager.FindByIdAsync(userId);
            var userRoles = await userManager.GetRolesAsync(user);
            var selectedRole = dto.RoleNames;
            selectedRole = selectedRole != null ?selectedRole: new string[] { };
            var result = await userManager.AddToRolesAsync(user, selectedRole.Except(userRoles));
            if (!result.Succeeded)
                return Ok(new {message = "حدث خطا"});

            result = await userManager.RemoveFromRolesAsync(user,userRoles.Except(selectedRole));
            if (!result.Succeeded)
                return Ok(new { message = "حدث خطا" });

            return Ok(await userManager.GetRolesAsync(user));
        }
        
    }
}
