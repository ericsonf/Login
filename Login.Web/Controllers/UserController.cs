using System.Linq;
using Login.Core.Interfaces;
using Login.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Login.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUser _user;
        private readonly IActiveDirectoryUser _adUser;

        public UserController(ILogger<UserController> logger, IUser user, IActiveDirectoryUser adUser)
        {
            _logger = logger;
            _user = user;
            _adUser = adUser;
        }

        [Authorize(Roles = "view-user")]
        [HttpGet("get-all-users")]
        public IActionResult Get()
        {
            var users = _user.List();
            return Ok(users);
        }

        [Authorize(Roles = "view-user")]
        [HttpGet("get-user/{id}")]
        public IActionResult Get(int id)
        {
            var user = _user.GetById(id);
            if (user == null) return NotFound();
            
            return Ok(user);
        }

        [Authorize(Roles = "view-user")]
        [HttpGet("get-ad-user/{userName}")]
        public IActionResult Get(string userName)
        {
            var user = _adUser.GetUser(userName);
            if (user.Username == null) return NotFound();
            
            return Ok(user);
        }

        [Authorize(Roles = "create-user")]
        [HttpPost("create-user")]
        public IActionResult Post(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors));
            if (registerUser == null) return NoContent();
            
            var user = new User {
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                Username = registerUser.UserName,
                Email = registerUser.Email
            };

            _user.Create(user, registerUser.Password);

            return Ok();
        }

        [Authorize(Roles = "edit-user")]
        [HttpPut("edit-user")]
        public IActionResult Put(int id, RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors));
            if (registerUser == null) return NoContent();

            var userToUpdate = _user.GetById(id);
            if (userToUpdate == null) return NotFound();
            else 
            {
                userToUpdate.FirstName = registerUser.FirstName;
                userToUpdate.LastName = registerUser.LastName;
                userToUpdate.Username = registerUser.UserName;
                userToUpdate.Email = registerUser.Email;
            }

            _user.Update((User)userToUpdate);

            return Ok();
        }

        [Authorize(Roles = "delete-user")]
        [HttpPut("delete-user")]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors));

            var userToDelete = _user.GetById(id);
            if (userToDelete == null) return NotFound();

            _user.Delete((User)userToDelete);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("authenticate-user")]
        public IActionResult Authenticate([FromBody]LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors));
            
            var user = _user.Authenticate(loginUser.UserName, loginUser.Password);
            if (user == null) return NotFound();

            var token = _user.GenerateToken(user);
            if (token == null) return BadRequest();

            var loggedUser = new LoggedUserViewModel {
                UserName = user.Username,
                Email = user.Email,
                Token = token
            };

            return Ok(loggedUser);
        }

        [AllowAnonymous]
        [HttpPost("authenticate-ad-user/{userName}")]
        public IActionResult Authenticate(string userName)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors));
            
            var user = _adUser.GetUser(userName);
            if (user.Username == null) return NotFound();

            var token = _adUser.GenerateToken(user);
            if (token == null) return BadRequest();

            var loggedUser = new LoggedUserViewModel {
                UserName = user.Username,
                Email = user.Email,
                Token = token
            };

            return Ok(loggedUser);
        }
    }
}
