using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<MemberDTO>> GetMemberAsync()
        {
            return await this.context.Users
                .ProjectTo<MemberDTO>(this.mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<MemberDTO> GetMemberAsync(string username)
        {
            return await this.context
                .Users
                .Where(e => e.UserName == username.ToLower())
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await this.context
                .Users
                .FindAsync(id);
        }

        public async Task<AppUser> GetUserBynameAsync(string name)
        {
            return await this.context
                .Users
                .Include(e => e.Photos)
                .FirstOrDefaultAsync(e => e.UserName == name.ToLower());
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await this.context
                .Users
                .Include(e => e.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllChangeAsync()
        {
            return await this.context.SaveChangesAsync() > 0;
        }

        public void update(AppUser user)
        {
            this.context.Entry(user).State = EntityState.Modified;
        }
    }
}