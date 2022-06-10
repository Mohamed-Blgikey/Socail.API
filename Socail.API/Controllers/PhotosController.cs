using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Socail.BL.Dtos;
using Socail.BL.Helper;
using Socail.BL.Interface;
using Socail.DAL.Entity;
using Socail.DAL.Extend;
using System.Security.Claims;

namespace Socail.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        #region fields
        private readonly IOptions<CloudinarySettings> cloudinaryCongig;
        private readonly IMapper mapper;
        private readonly ISocailRep<Photo> photoRep;
        private Cloudinary _cloudinary;

        #endregion

        public PhotosController(UserManager<ApplicationUser> userManager, IOptions<CloudinarySettings> cloudinaryCongig,IMapper mapper,ISocailRep<Photo> PhotoRep  )
        {
            this.userManager = userManager;
            this.cloudinaryCongig = cloudinaryCongig;
            this.mapper = mapper;
            photoRep = PhotoRep;

            Account account = new Account(
                cloudinaryCongig.Value.CloudName,
                cloudinaryCongig.Value.ApiKey,
                cloudinaryCongig.Value.ApiSecret
                );

            _cloudinary = new Cloudinary(account);
        }

        #region GetPhoto
        [HttpGet("{id}",Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photo = photoRep.GetById(id);
            var photoForReturn = mapper.Map<PhotoForReturnDto>(photo); 
            return Ok(photoForReturn);
        }
        #endregion


        #region add
        [HttpPost]
        [Route("~/AddPhoto")]
        public async Task<IActionResult> AddPhoto([FromForm] PhotoForCreateDto dto)
        {
            try
            {
                if (User.FindFirst(ClaimTypes.NameIdentifier).Value != dto.UserId)
                {
                    return Unauthorized();
                }
                var user = await userManager.FindByIdAsync(dto.UserId);

                var file = dto.File;
                var uploadResult = new ImageUploadResult();

                if (file != null && file.Length > 0)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.Name, stream),
                            Transformation = new Transformation()
                            .Width(500).Height(500).Crop("fill").Gravity("face")
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);
                    }
                }
                dto.Url = uploadResult.Url.ToString();
                dto.PublicId = uploadResult.PublicId;
                var photo = mapper.Map<Photo>(dto);

                var userPhotos = await photoRep.GetAllAsync(p=>p.UserId == dto.UserId);
                if (!userPhotos.Any(p=>p.IsMain))
                {
                    photo.IsMain = true;
                    user.PhotoName = photo.Url;
                    await userManager.UpdateAsync(user);
                }
                photoRep.Add(photo);
                if (await photoRep.SaveAll())
                { 
                    var pp = mapper.Map<PhotoForReturnDto>(photo);
                    return CreatedAtRoute("GetPhoto", new {id = photo.Id},pp);
                }
                return BadRequest();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region edit
        [HttpPost]
        [Route("~/SetMain")]
        public async Task<IActionResult> SetMain(EditMainDto dto)
        {
            if (User.FindFirst(ClaimTypes.NameIdentifier).Value != dto.UserId)
            {
                return Unauthorized();
            }


            var user = await userManager.FindByIdAsync(dto.UserId);
            var oldPhoto = await photoRep.GetByIdAsync(p => p.IsMain == true);
            var newPhoto = photoRep.GetById(dto.NewPhotoId);

            if (oldPhoto.Id == dto.NewPhotoId)
            {
                return Ok(new {message = "this is main photo " });
            }

            oldPhoto.IsMain = false;
            newPhoto.IsMain = true;
            user.PhotoName = newPhoto.Url;
            var x = await photoRep.SaveAll();
            return Ok(x);
        }
        #endregion

        #region delete
        [HttpPost]
        [Route("~/DeletePhoto/{id}/{userId}")]
        public async Task<IActionResult> DeletePhoto(int id,string userId)
        {
            if (User.FindFirst(ClaimTypes.NameIdentifier).Value != userId)
            {
                return Unauthorized();
            }
            
            var photo = photoRep.GetById(id);
            if (photo.IsMain)
            {
                return Ok(new {message = "Stopped"});
            }

            if (photo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photo.PublicId);
                var resule = _cloudinary.Destroy(deleteParams);
                if (resule.Result == "ok")
                {
                    photoRep.Delete(photo);
                }
            }
            if (photo.PublicId == null)
            {
                photoRep.Delete(photo);
            }
            await photoRep.SaveAll();

            return Ok();
        }

        #endregion
    }
}
