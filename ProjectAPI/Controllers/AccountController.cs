using Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.DTO;
using System.Net;

namespace ProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("UserRole")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment hosting;

        public AccountController(UserManager<AppUser> userManager , Microsoft.AspNetCore.Hosting.IHostingEnvironment hosting)
        {
            this.userManager = userManager;
            this.hosting = hosting;
        }


        [HttpGet("GetUsers")]
        [Authorize("AdminRole")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await userManager.Users.ToListAsync();
            if (users == null)
                return NotFound("Not Found Any Users");

            var user = users.Select(x => new GetUsersDTO
            {
                Address = x.Address,
                BirthDay = x.BirthDay,
                Email = x.Email,
                FirstName = x.Name.Split(" ").First(),
                LastName = x.Name.Split(" ").Last(),
                Location = x.Location,
                PhoneNamber = x.PhoneNumber,
                UrlPhoto = x.Photo,
            }).ToList();

            return Ok(user);
        }


        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("This User Not Registed");

            var mapUser = new GetUsersDTO
            {
                Address = user.Address,
                BirthDay = user.BirthDay,
                Email = user.Email,
                FirstName = user.Name.Split(" ").First(),
                LastName = user.Name.Split(" ").Last(),
                Location = user.Location,
                PhoneNamber = user.PhoneNumber,
                UrlPhoto = user.Photo,

            };

            return Ok(mapUser);


        }

        [HttpPut("EditUser")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDTO dto)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("This User Not Registed");

            if (dto.Photo != null)
            {
                string uploads = Path.Combine(hosting.WebRootPath, @"ProfilePhoto\");
                string fullPath = Path.Combine(uploads, dto.Photo.FileName);
                dto.Photo.CopyTo(new FileStream(fullPath, FileMode.Create));
                user.Photo = dto.Photo.FileName;
            }
            user.Address = dto.Address ?? user.Address;
            user.BirthDay = dto.BirthDay ?? user.BirthDay;
            user.Email = dto.Email ?? user.Email;
            user.Name = dto.FirstName + " " + dto.LastName ?? user.Name ;
            user.Location = dto.Location ?? user.Location;
            user.PhoneNumber = dto.PhoneNamber ?? user.PhoneNumber;

            await userManager.UpdateAsync(user);
            var mapUser = new GetUsersDTO
            {
                Address = user.Address,
                BirthDay = user.BirthDay,
                Email = user.Email,
                FirstName = user.Name.Split(" ").First(),
                LastName = user.Name.Split(" ").Last(),
                Location = user.Location,
                PhoneNamber = user.PhoneNumber,
                UrlPhoto = user.Photo,

            };

            return Ok(mapUser);
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) 
                return NotFound("This User Not Registed");

            await userManager.DeleteAsync(user);
            return Ok("User Is Deleted !");



        }
    }
}
