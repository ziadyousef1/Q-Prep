
using Core.AuthenticationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Core.Interfaces
{
    public interface IAuthentication
    {
        Task<AuthenticateDTO> RegisterAsync(RegisterDTO dto ,  string photo);
        Task<AuthenticateDTO> LoginAsync(LogInDTo dto );
        Task<AuthenticateDTO> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);


    }
}
