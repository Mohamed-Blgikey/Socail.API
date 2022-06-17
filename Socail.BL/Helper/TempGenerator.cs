using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Socail.BL.Dtos;
using Socail.BL.Interface;
using Socail.DAL.Entity;
using Socail.DAL.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socail.BL.Helper
{
    public class TempGenerator
    {
        private readonly IMapper _mapper;
        private readonly ISocailRep<Photo> _repo;
        private readonly UserManager<ApplicationUser> userManager;

        public TempGenerator(ISocailRep<Photo> repo,UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _repo = repo;
            this.userManager = userManager;
            _mapper = mapper;

        }

        public string GetHTMLStringForUser(string userId)
        {
			//exception of Global Query Filter we use false
            var user = userManager.FindByIdAsync(userId).Result;
            var userToReturn = _mapper.Map<UserForDetailsDto>(user);

            var likers = _repo.GetLikersAndLikees(userId, "likers").Result;
            var likees = _repo.GetLikersAndLikees(userId, "likees").Result;
            var likersCount=likers.Count(a=>true);
            var likeesCount=likees.Count(a => true);


            var sb = new StringBuilder();

            sb.Append(@"
                        <html dir='rtl'>
                            <head>
                            </head>
                            <body>
                                <div class='page-header'><h2 class='header-container'>بطاقة " + userToReturn.FullName + @"</h2></div>
                                                             
                                <div class='card-data'>
                                 <img src='" + userToReturn.PhotoName + @"'>
                                <table style='display:inline;width: 50%;height: 300px;'>
                                <div>
                                <tr>
                                <td>الإسم</td>
                                    <td>" + userToReturn.FullName + @"</td>
                                </tr>
                                <tr>
                                    <td>العمر</td>
                                    <td>" + userToReturn.DateOfBirth + @"</td>
                                </tr>    
                                <tr>
                                    <td>البلد</td>
                                    <td>" + userToReturn.Country + @"</td>
                                </tr>    
                                <tr>
                                    <td>تاريخ الإشتراك</td>
                                    <td>" + userToReturn.Created.ToShortDateString() + @"</td>
                                </tr> 
                                </div>   
                              </table>
                                </div>
                                <div class='page-header'><h2 class='header-container'>المعجبين &nbsp;&nbsp;["+likersCount+@"]</h2></div>
                                <table align='center'>
                                    <tr>
                                        <th>الإسم</th>
                                        <th>تاريخ الإشتراك</th>
                                        <th>العمر</th>
                                        <th>البلد</th>
                                    </tr>");

            foreach (var liker in likers)
            {
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                  </tr>", liker.FullName, liker.Created.ToShortDateString(), liker.DateOfBirth, liker.Country);
            }

            sb.Append(@"
                                </table>
                                <div class='page-header'><h2 class='header-container'>المعجب بهم  &nbsp;&nbsp;["+likeesCount+@"] </h2></div>
                                <table align='center'>
                                <tr>
                                 <th>الإسم</th>
                                        <th>تاريخ الإشتراك</th>
                                        <th>العمر</th>
                                        <th>البلد</th>
                                </tr>");
            foreach (var likee in likees)
            {
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                  </tr>", likee.FullName, likee.Created.ToShortDateString(), likee.DateOfBirth, likee.Country);
            }

            sb.Append(@"     </table>                   
                            </body>
                        </html>");

            return sb.ToString();
        }
    }
}
