using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Socail.BL.Dtos;
using Socail.BL.Helper;
using Socail.DAL.Database;
using Socail.DAL.Entity;
using Socail.DAL.Extend;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
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
                var authModel = new AuthModel();
                var user = await userManager.FindByEmailAsync(loginDTO.Email);

                if (user == null || !await userManager.CheckPasswordAsync(user, loginDTO.Password))
                    return new AuthModel { Message = "Inavlid Email Or Password" };

                var jwtSecurityToken = await CreateJwtToken(user);

                authModel.Message = "Success";
                authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                authModel.IsAuthencated = true;
                if (user.RefreshTokens.Any(t=>t.IsActive))
                {
                    var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                    authModel.RefrshToken = activeRefreshToken.Token;
                    authModel.RefrshTokenExpiration = activeRefreshToken.ExpiresOn;
                }
                else
                {
                    var refreshToken = GenerateRefreshToken();
                    authModel.RefrshToken = refreshToken.Token;
                    authModel.RefrshTokenExpiration = refreshToken.ExpiresOn;
                    user.RefreshTokens.Add(refreshToken);
                    await userManager.UpdateAsync(user);
                }

                return authModel;
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
                    //ExpiresOn = jwtSecurityToken.ValidTo,
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
                new Claim("gender", user.Gender.ToString())
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Value.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: jwt.Value.Issuer,
                audience: jwt.Value.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(jwt.Value.DurationInDayes),
                signingCredentials: signingCredentials);



            return jwtSecurityToken;


        }
        #endregion


        public async Task<AuthModel> RefreshToken(string token)
        {

            var authModel = new AuthModel();

            var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                authModel.Message = "Invalid token";
                return authModel;
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                authModel.Message = "Inactive token";
                return authModel;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);
            authModel.IsAuthencated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            var roles = await userManager.GetRolesAsync(user);
            authModel.RefrshToken = newRefreshToken.Token;
            authModel.RefrshTokenExpiration = newRefreshToken.ExpiresOn;

            return authModel;
        }


        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(60),
                CreatedOn = DateTime.UtcNow,

            };
            
        }

        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            var authModel = new AuthModel();
            var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                authModel.Message = "Invalid Token";
                return authModel;
            }

            var refreshToken = user.RefreshTokens.Single(t=>t.Token ==token);
            if (!refreshToken.IsActive)
            {
                authModel.Message = "InActive Token";
                return authModel;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);

            authModel.IsAuthencated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.RefrshToken = newRefreshToken.Token;
            authModel.RefrshTokenExpiration = newRefreshToken.ExpiresOn;

            return authModel;
        }


        public async Task<bool> ReVokeTokenAsync(string token)
        {
            var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;
            await userManager.UpdateAsync(user);


            return true;
        }

    }
}
