using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using API.DTOs;
using System.Security.Claims;
using API.Extensions;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;

        public UsersController(
            IUserRepository userRepository,
            IMapper mapper,
            IPhotoService photoService)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users = await this.userRepository.GetMemberAsync();
            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            return await userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
        {
            var username = User.GetUserName();
            var user = await this.userRepository.GetUserBynameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            this.mapper.Map(memberUpdateDTO, user);
            if (await this.userRepository.SaveAllChangeAsync())
            {
                return NoContent();
            }

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var username = User.GetUserName();
            var user = await this.userRepository.GetUserBynameAsync(username);

            if (user == null)
            {
                return BadRequest();
            }

            var result = await this.photoService.AddPhotoAsync(file);

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            photo.IsMain = user.Photos.Count == 0;
            user.Photos.Add(photo);

            if (await this.userRepository.SaveAllChangeAsync())
            {
                return CreatedAtAction(
                    nameof(this.GetUser),
                    new { username = user.UserName },
                    this.mapper.Map<PhotoDTO>(photo));
            }

            return BadRequest();
        }
    }
}