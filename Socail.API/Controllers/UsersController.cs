using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Socail.DAL.Extend;
using Socail.BL.Dtos;
using Socail.BL.Interface;
using Socail.DAL.Entity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Socail.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        #region frilds
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper imapper;
        private readonly ISocailRep<Photo> photoRep;

        #endregion
        #region Ctor
        public UsersController(UserManager<ApplicationUser> userManager,IMapper imapper,ISocailRep<Photo> photoRep)
        {
            this.userManager = userManager;
            this.imapper = imapper;
            this.photoRep = photoRep;
        }
        #endregion


        #region Actions
        #region GetUsers
        [HttpGet]
        [Route("~/GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            
            var users = imapper.Map<IEnumerable< UserForReturnDto>> (userManager.Users);
            return Ok(users);
        }
        #endregion

        #region GetUsers
        [HttpGet]
        [Route("~/GetUser/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = imapper.Map<UserForReturnDto>(await userManager.FindByIdAsync(id));
            return Ok(user);
        }
        #endregion

        #region GetUserPhotos
        [HttpGet]
        [Route("~/GetUserPhotos/{id}")]
        public async Task<IActionResult> GetUserPhotos(string id)
        {
            var photos = imapper.Map<IEnumerable<PhotoForDetailsDto>>(await photoRep.GetAllAsync(a=>a.UserId == id));
            return Ok(photos);
        }
        #endregion


        #region GetUserPhotos
        [HttpPut]
        [Route("~/EditUser")]
        public async Task<IActionResult> EditUser(UserForUpdateDto dto)
        {
            try
            {
                if (User.FindFirst(ClaimTypes.NameIdentifier).Value != dto.Id)
                {
                    return Unauthorized();
                }
                var user = await userManager.FindByIdAsync(dto.Id);
                user.City = dto.City;
                user.Country = dto.Country;
                user.LookingFor = dto.LookingFor;
                user.Interests = dto.Interests;
                user.Introduction = dto.Introduction;
                user.DateOfBirth = dto.DateOfBirth;
    
                var result = await userManager.UpdateAsync(user);
                return Ok(result);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        #endregion
    }
}
