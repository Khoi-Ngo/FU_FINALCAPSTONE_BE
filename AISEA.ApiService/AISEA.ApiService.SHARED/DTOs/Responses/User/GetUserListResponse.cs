using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Responses.User
{
    public class GetUserListResponse
    {
        public long Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTimeOffset? DateOfBirth { get; set; }
        public string? AvatarUrl { get; set; }
        public string RoleName { get; set; }
        public EUserStatus Status { get; set; }

    }
}