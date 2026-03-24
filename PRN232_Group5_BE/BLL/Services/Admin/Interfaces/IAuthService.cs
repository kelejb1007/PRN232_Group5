using DAL.DTOs.UserAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DAL.DTOs.UserAccount.AuthDTO;

namespace BLL.Services.Admin.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO?> LoginAsync(LoginRequestDTO request);
        Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request);
    }
}
