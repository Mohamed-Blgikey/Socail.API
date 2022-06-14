using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Socail.DAL.Extend;
using Socail.BL.Dtos;
using Socail.DAL.Entity;

namespace Socail.BL.Helper
{
    public class DomainProfile:Profile
    {
        public DomainProfile()
        {
            CreateMap<ApplicationUser, UserForReturnDto>();
            CreateMap<ApplicationUser, UserForDetailsDto>();


            CreateMap<Photo, PhotoForDetailsDto>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<UserForUpdateDto, ApplicationUser>();
            CreateMap<PhotoForCreateDto, Photo>();

            CreateMap<MessageForCreationDto, Message>();
            CreateMap<Message,MessageToReturnDto>();
        }
    }
}
