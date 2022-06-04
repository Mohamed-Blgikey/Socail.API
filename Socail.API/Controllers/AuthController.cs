﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Socail.BL.Authentcation;
using Socail.BL.Dtos;
using Socail.BL.Helper;
using Socail.DAL.Database;
using Socail.DAL.Extend;

namespace Socail.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices auth;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        #region fields

        #endregion

        #region Ctor
        public AuthController(IAuthServices auth,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            this.auth = auth;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        #endregion

        #region Register
        [HttpPost]
        [Route("~/Register")]
        public async Task<IActionResult> Register(RegisterDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await auth.Register(dTO);

            if (!result.IsAuthencated)
                return Ok(new { message = result.Message });

            return Ok(new { message = result.Message, token = result.Token, expiresOn = result.ExpiresOn });
        }
        #endregion

        //#region Register
        //[HttpGet]
        //[Route("~/seed")]
        //public async Task<IActionResult> seed()
        //{

        //    try
        //    {
        //        var userData = System.IO.File.ReadAllText("Controllers/userData.json");
        //        var users = JsonConvert.DeserializeObject<List<ApplicationUser>>(userData);
        //        foreach (var user in users)
        //        {
        //            await userManager.CreateAsync(user, user.PasswordHash);
        //            var RoleExsit = await roleManager.RoleExistsAsync("Admin");
        //            if (!RoleExsit)
        //            {
        //                await roleManager.CreateAsync(new IdentityRole("Admin"));
        //                await userManager.AddToRoleAsync(user, "Admin");
        //            }
        //            else
        //            {
        //                await roleManager.CreateAsync(new IdentityRole("User"));
        //                await userManager.AddToRoleAsync(user, "User");
        //            }
        //        }
        //        return Ok(new { message = "Done" });
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        //#endregion

        #region Login
        [HttpPost]
        [Route("~/Login")]
        public async Task<IActionResult> Login(LoginDTO dTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await auth.Login(dTO);

            if (!result.IsAuthencated)
                return Ok(new { message = result.Message });

            return Ok(new { message = result.Message, token = result.Token, expiresOn = result.ExpiresOn });
        }
        #endregion
    }
}
