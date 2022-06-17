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
using Socail.BL.Helper;
using DinkToPdf;
using DinkToPdf.Contracts;

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
        private readonly ISocailRep<ApplicationUser> rep;
        private readonly ISocailRep<Like> likeRep;
        private readonly IConverter converter;

        #endregion
        #region Ctor
        public UsersController(UserManager<ApplicationUser> userManager,IMapper imapper,ISocailRep<Photo> photoRep, ISocailRep<ApplicationUser> rep, ISocailRep<Like> likeRep,IConverter converter)
        {
            this.userManager = userManager;
            this.imapper = imapper;
            this.photoRep = photoRep;
            this.rep = rep;
            this.likeRep = likeRep;
            this.converter = converter;
        }
        #endregion


        #region Actions
        #region GetUsers
        [HttpGet]

        [Route("~/GetUsers")]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userparams)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await userManager.FindByIdAsync(userId);
            userparams.UserId = user.Id;
            if (userparams.Gender == null)
            {
                userparams.Gender = user.Gender == 1?0:1;
            }

            var usersRepo = await rep.GetUsers(userparams);
            var users = imapper.Map<IEnumerable< UserForReturnDto>> (usersRepo);
            return Ok(new
            {
                currentPage = usersRepo.CurrentPage,
                itemPerPage = usersRepo.PageSize,
                totalItems = usersRepo.TotalCount,
                totalPages = usersRepo.TotalPage,
                data = users,
            });
        }
        #endregion

        #region GetUsers
        [HttpGet]
        [Route("~/GetUser/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUser(string id)
        {
            var data =  rep.GetAllAsync(a => a.Id == id, new[] { "Photos" }).Result;
            var user = imapper.Map<IEnumerable<UserForDetailsDto>>(data);
            return Ok(user.FirstOrDefault());
        }
        #endregion


        #region EditUser
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

        #region MakeLike
        [HttpPost]

        [Route("~/MakeLike/{userId}/{recipentId}")]
        public async Task<IActionResult> MakeLike(string userId , string recipentId)
        {
            try
            {
                if (User.FindFirst(ClaimTypes.NameIdentifier).Value != userId)
                {
                    return Unauthorized();
                }
                var like = await likeRep.GetByIdAsync(l=>l.LikerId== userId && l.LikeeId == recipentId);
                
                if (like != null)
                {
                    return Ok(new { message = "تم الاعجاب من قبل" });
                }
                if (await userManager.FindByIdAsync(recipentId) == null)
                {
                    return NotFound();
                }

                var newLike = new Like
                {
                    LikeeId = recipentId,
                    LikerId = userId,
                };

                likeRep.Add(newLike);
                if (await likeRep.SaveAll())
                {
                    return Ok();
                }
                


                return Ok(new {message = " فشل الاعجاب"});
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        [AllowAnonymous]
        //[Authorize(Roles = "Admin")
        [HttpGet("~/UserReport/{userId}")]
        public IActionResult CreatePdfForUser(string userId)
        {
            var templateGenerator = new TempGenerator(photoRep,userManager,imapper);
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 15, Bottom = 20 },
                DocumentTitle = "بطاقة مشترك"

            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = templateGenerator.GetHTMLStringForUser(userId),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                HeaderSettings = { FontName = "Impact", FontSize = 12, Spacing = 5, Line = false },
                FooterSettings = { FontName = "Geneva", FontSize = 15, Spacing = 7, Line = true, Center = "ZwajApp By Eng Muhammad Awadallah", Right = "[page]" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            var file = converter.Convert(pdf);
            return File(file, "application/pdf");
        }
        #endregion

        [AllowAnonymous]
        [HttpGet("~/GetUsersExceptAdmin")]
        public async Task<IActionResult> GetUsersExceptAdmin()
        {
            var users = await rep.GetAllAsync(u => u.UserName != "admin@admin.com");
            var reusers = imapper.Map<IEnumerable<UserForReturnDto>> (users);
            return Ok(reusers);
        }

    }
}
