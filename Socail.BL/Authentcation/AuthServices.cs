using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Socail.BL.Dtos;
using Socail.BL.Helper;
using Socail.DAL.Database;
using Socail.DAL.Extend;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Socail.BL.Authentcation
{
    public class AuthServices : IAuthServices
    {
        #region fileds

        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IOptions<JWT> jwt;
        private readonly AppDbContext context;

        #endregion

        #region Ctor
        public AuthServices(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt,AppDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.jwt = jwt;
            this.context = context;
        }
        #endregion
        #region Login
        public async Task<AuthModel> Login(LoginDTO loginDTO)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(loginDTO.Email);

                if (user == null || !await userManager.CheckPasswordAsync(user, loginDTO.Password))
                    return new AuthModel { Message = "Inavlid Email Or Password" };

                var jwtSecurityToken = await CreateJwtToken(user);

                return new AuthModel
                {
                    Message = "Success",
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    ExpiresOn = jwtSecurityToken.ValidTo,
                    IsAuthencated = true
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion


        #region Register
        public async Task<AuthModel> Register(RegisterDTO registerDTO)
        {
            try
            {
                if (await userManager.FindByEmailAsync(registerDTO.Email) != null)
                    return new AuthModel { Message = "Email Is Already Token" };
                var user = new ApplicationUser
                {
                    Email = registerDTO.Email,
                    UserName = registerDTO.Email,
                    FirstName = registerDTO.FirstName,
                    LastName = registerDTO.LastName,
                    PhotoName = "https://res.cloudinary.com/dz0g6ou0i/image/upload/v1654960873/defualt_w4v99c.png",
                    Gender = registerDTO.Gender,
                    City = registerDTO.City,
                    Country = registerDTO.Country,
                    DateOfBirth = registerDTO.DateOfBirth,
                    LastActive =registerDTO.LastActive,
                    Interests = "..",
                    LookingFor="..",
                    Introduction="..",
                };

                var result = await userManager.CreateAsync(user, registerDTO.Password);

                if (!result.Succeeded)
                {
                    var error = string.Empty;
                    foreach (var item in result.Errors)
                    {
                        error += $"{item.Description},";
                    }
                    return new AuthModel { Message = error };
                }

                var RoleExsit = await roleManager.RoleExistsAsync("Admin");
                if (!RoleExsit)
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                    await userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                    await userManager.AddToRoleAsync(user, "User");
                }

                var jwtSecurityToken = await CreateJwtToken(user);


                return new AuthModel
                {
                    Message = "Success",
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    ExpiresOn = jwtSecurityToken.ValidTo,
                    IsAuthencated = true
                };
            }
            catch (Exception)
            {
                throw;
            }


        }
        #endregion




        #region Create Token
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("photoName", user.PhotoName),
                new Claim("fullName", user.FullName),
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Value.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: jwt.Value.Issuer,
                audience: jwt.Value.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(jwt.Value.DurationInDays),
                signingCredentials: signingCredentials);



            return jwtSecurityToken;


        }
        #endregion
    }
}
