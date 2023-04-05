using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void update(AppUser user);
        Task<bool> SaveAllChangeAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserBynameAsync(string name);
        Task<IEnumerable<MemberDTO>> GetMemberAsync();
        Task<MemberDTO> GetMemberAsync(string username);
    }
}