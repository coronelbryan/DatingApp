using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using System.Security.Cryptography;
using API.Data;
using API.Entities;
using System.Text;
using Microsoft.EntityFrameworkCore;
using API.DTOs;
using API.Interfaces;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext dataContext;
        private readonly ITokenService tokenService;

        public AccountController(
            DataContext dataContext,
            ITokenService tokenService
        )
        {
            this.dataContext = dataContext;
            this.tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.Username))
            {
                return BadRequest("Username is taken");
            }

            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDTO.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PsswordSalt = hmac.Key
            };

            this.dataContext.Add(user);
            await this.dataContext.SaveChangesAsync();

            return new UserDTO
            {
                UserName  = user.UserName,
                Token = tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await this.dataContext
                .Users
                .FirstOrDefaultAsync(e => e.UserName == loginDTO.Username.ToLower());

            if (user is null)
            {
                return Unauthorized("Invalid username");
            }

            using var hmac = new HMACSHA512(user.PsswordSalt);
            var computerHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for (int i = 0; i < computerHash.Length; i++)
            {
                if (computerHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid password");
                }
            }

            return new UserDTO
            {
                UserName = user.UserName,
                Token = tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await this.dataContext
                .Users
                .AnyAsync(e => e.UserName == username.ToLower());

        }
    }
}