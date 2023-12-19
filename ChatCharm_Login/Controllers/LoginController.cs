using ChatCharm_Login.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using ChatCharm_Login.Repositories;
using MongoDB.Driver;
using System;
using MongoDB.Bson;

namespace ChatCharm_Login.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginController> _logger;
        private readonly UserRepository _userRepository;

        private readonly ChatCharmDB _context;

        private readonly IUserService _userService;

        public LoginController(ChatCharmDB context, IConfiguration configuration, IUserService userService, UserRepository userRepository)
        {
            _context = context;
            _configuration = configuration;
            _userService = userService;
            _userRepository = userRepository;
        }

        [HttpGet("GetId"), Authorize(Roles = "User")]
        public ActionResult<string> GetId()
        {
            string id = _userService.GetId();
            return Ok(id);
        }

        [HttpGet("GetProfile"), Authorize(Roles = "User")]

        public ActionResult GetUser()
        {

            User user = _userRepository.GetById(_userService.GetId().ToString()).Result;
            return File(user.Image, "image/png");
        }

        [HttpPost("register")]
        public async Task<string> Register(UserDto userDto)
        {
            try
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

          
               User userCheck = _userRepository.GetByName(userDto.Username).Result;

                Console.WriteLine("Test");
                Console.WriteLine(userCheck);

                if(userCheck != null) 
                {
                    return $"Der User mit dem Namen {userDto.Username} existiert bereits.";
                }

                byte[] bytes = new byte[32];

                User user = new User();
                user.Username = userDto.Username;
                user.Password = passwordHash;
                user.Image = bytes;

                await _userRepository.InsertOneAsync(user);

                return "Der User wurde erfolgreich hinzugefügt";
            }catch (Exception ex)
            {
                return ex.Message;
            }

        }

       

        [HttpPost("Login")]
        public ActionResult<User> Login(UserDto data)
        {
            if(data != null)
            {
                try
                {
                    User user = _userRepository.GetByName(data.Username).Result;
                    if (user == null || !BCrypt.Net.BCrypt.Verify(data.Password, user.Password))
                    {
                        return NotFound("Das Passwort oder der Benutzername ist falsch");
                    }
                    Console.WriteLine("Test");
                    var token = CreateToken(user);
                    return Ok(token);
                }catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest("Invalid Data");
        }

        [HttpPost("UploadImage"), Authorize(Roles = "User")]
        public async Task<string> UploadImage(IFormFile image)
        {
            User user = _userRepository.GetById(_userService.GetId().ToString()).Result;

            try
            {
                if (image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                    
                        image.CopyTo(ms);
                        var updateCmd = Builders<User>.Update
                        .Set(b => b.Image, ms.ToArray() );
                        var update = await _context.Users.UpdateOneAsync(b => b.Id == user.Id, updateCmd);
                    }
                
                    
                    return "Bild wurde erfolgreich hochgeladen";
                }
                else
                {
                    return "Kein Bild vorhanden";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPatch("ChangePassword"), Authorize(Roles = "User")]

        public async Task<string> ChangePassword(string password)
        {
            if(password == null)
            {
                return "Es wurde kein Passwort mitgegeben";
            }

            User user = _userRepository.GetById(_userService.GetId().ToString()).Result;

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            user.Password = passwordHash;
            var updateCmd = Builders<User>.Update
               .Set(b => b.Password, passwordHash);
            var update = await _context.Users.UpdateOneAsync(b => b.Id == user.Id, updateCmd);
            return "Das Passwort wurde erfolgreich verändert";
        }

        [HttpDelete("deleteUser"), Authorize(Roles = "User")]

        public async Task<string> DeleteUser()
        {

           await _userRepository.DeleteUser(_userService.GetId().ToString());

            return "User wurde erfolgreich gelöscht";
        }


        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Key").Value!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
