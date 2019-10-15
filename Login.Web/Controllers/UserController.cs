using System.Linq;
using System.Threading.Tasks;
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

        public UserController(ILogger<UserController> logger, IUser user)
        {
            _logger = logger;
            _user = user;
        }

        [HttpPost("new-account")]
        public IActionResult Register(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors));
            
            var user = new User {
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                Username = registerUser.UserName,
                Email = registerUser.Email
            };

            var createdUser = _user.Create(user, registerUser.Password);

            return Ok(createdUser);
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors));
            
            var user = _user.Authenticate(loginUser.UserName, loginUser.Password);
            if (user == null) return BadRequest();

            return Ok(user);
        }

        public IActionResult GetAll()
        {
            var users = _user.GetAll();
            return Ok(users);
        }
    }
}
