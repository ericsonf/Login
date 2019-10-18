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

        private readonly ICommonUser _commonUser;

        private readonly IActiveDirectoryUser _adUser;

        public UserController(ILogger<UserController> logger, IUser user, ICommonUser commonUser, IActiveDirectoryUser adUser)
        {
            _logger = logger;
            _user = user;
            _commonUser = commonUser;
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
            if (user == null) return NotFound();
            
            return Ok(user);
        }

        [Authorize(Roles = "create-user")]
        [HttpPost("create-user")]
        public IActionResult Post(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors));
            if (registerUser == null) return NoContent();
            
            var user = new CommonUser {
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                Username = registerUser.UserName,
                Email = registerUser.Email
            };

            _commonUser.Create(user, registerUser.Password);

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

            _commonUser.Update((CommonUser)userToUpdate);

            return Ok();
        }

        [Authorize(Roles = "delete-user")]
        [HttpPut("delete-user")]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors));

            var userToDelete = _user.GetById(id);
            if (userToDelete == null) return NotFound();

            _commonUser.Delete((CommonUser)userToDelete);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("authenticate-user")]
        public IActionResult Authenticate([FromBody]LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors));
            
            var user = _commonUser.Authenticate(loginUser.UserName, loginUser.Password);
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
    }
}
