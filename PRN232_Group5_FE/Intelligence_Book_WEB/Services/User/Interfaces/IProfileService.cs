using static Models.UserAccount.AuthDTO;

namespace Intelligence_Book_WEB.Services.User.Interfaces
{
    public interface IProfileService
    {
        Task<UserProfileDTO?> GetProfileAsync(string? accessToken);
        Task<bool> UpdateProfileAsync(string? accessToken, UpdateProfileRequestDTO updateRequest);
    }
}
