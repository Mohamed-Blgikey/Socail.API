using Socail.BL.Dtos;
using Socail.BL.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socail.BL.Authentcation
{
    public interface IAuthServices
    {
        Task<AuthModel> Register(RegisterDTO registerDTO);
        Task<AuthModel> Login(LoginDTO loginDTO);

        Task<AuthModel> RefreshTokenAsync(string token);
        Task<bool> ReVokeTokenAsync(string token);
    }
}
