using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Socail.BL.Dtos;
using Socail.BL.Helper;
using Socail.BL.Interface;
using Socail.DAL.Entity;
using Socail.DAL.Extend;
using System.Security.Claims;

namespace Socail.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly ISocailRep<Message> messageRep;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;

        public MessagesController(ISocailRep<Message> messageRep,IMapper mapper,UserManager<ApplicationUser> userManager)
        {
            this.messageRep = messageRep;
            this.mapper = mapper;
            this.userManager = userManager;
        }


        [HttpGet]
        [Route("~/GetMessage/{userId}/{id}")]
        public async Task<IActionResult> GetMessage(int id,string userId)
        {
            try
            {
                if (User.FindFirst(ClaimTypes.NameIdentifier).Value != userId)
                    return Unauthorized();

                var message = messageRep.GetById(id);
                if (message == null) 
                    return Ok(new {message = "لا يوجد"});

                return Ok(message);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("~/CreateMessage")]
        public async Task<IActionResult> CreateMessage(MessageForCreationDto dto)
        {
            if (User.FindFirst(ClaimTypes.NameIdentifier).Value != dto.SenderId)
                return Unauthorized();

            var recipient = await userManager.FindByIdAsync(dto.ResipientId);
            
            if (recipient == null) 
                return Ok(new {message = "لا يوجد مرسل اليه"});

            var message = mapper.Map<Message>(dto);

            var result = await messageRep.Add(message);
            if (result != null && await messageRep.SaveAll())
            {
                var x = mapper.Map<MessageToReturnDto>(result);
               return Ok(x);
            }
            return Ok("sadasd");
        }


        [HttpGet]
        [Route("~/GetMessages/{userId}")]
        public async Task<IActionResult> GetAllmessages(string userId,[FromQuery]MessageParams messageParams)
        {
            if (userId != (User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            messageParams.UserId = userId;
            var MessagesFromRepo = await messageRep.GetMessagesForUser(messageParams);
           
            var messages = mapper.Map<IEnumerable<MessageToReturnDto>>(MessagesFromRepo);
            return Ok(new {
                currentPage = MessagesFromRepo.CurrentPage,
                itemPerPage = MessagesFromRepo.PageSize,
                totalItems = MessagesFromRepo.TotalCount,
                totalPages = MessagesFromRepo.TotalPage,
                data = messages,
            });
        }

        [HttpGet]
        [Route("~/GetConversation/{userId}/{recipientId}")]
        public async Task<IActionResult> GetConversation(string userId, string recipientId)
        {
            if (userId != (User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var MessagesFromRepo = await messageRep.GetConversation(userId,recipientId);
            var messages = mapper.Map<IEnumerable<MessageToReturnDto>>(MessagesFromRepo);
            return Ok(messages);
        }

    }
}
