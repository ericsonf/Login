using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Login.Core.Helpers;
using Login.Core.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Login.UseCases.UseCase
{
    public class UserUseCase : IUser
    {
        private readonly AppSettings _appSettings;

        private readonly IRepository _repository;

        public UserUseCase(IOptions<AppSettings> appSettings, IRepository repository)
        {
            _appSettings = appSettings.Value;
            _repository = repository;
        }

        public IEnumerable<User> List()
        {
            return _repository.List<User>();
        }

        public User GetById(int id)
        {
            return _repository.GetById<User>(id);
        }

        public string GenerateToken(User user)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Role, "view-user"),
                    new Claim(ClaimTypes.Role, "create-user")
                }),
                Expires = DateTime.UtcNow.AddMilliseconds(_appSettings.Expiration),
                Issuer = _appSettings.Issuer,
                Audience = _appSettings.Audience.Split(',').First(),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret)), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
