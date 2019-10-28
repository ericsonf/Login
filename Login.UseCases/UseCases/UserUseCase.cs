using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
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

        public void Create(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _repository.Add<User>(user);
            _repository.Save();
        }

        public void Update(User user)
        {
            _repository.Update<User>(user);
            _repository.Save();
        }

        public void Delete(User user)
        {
            _repository.Delete<User>(user);
            _repository.Save();
        }

        public User Authenticate(string userName, string password)
        {
            var user = _repository.Filter<User>(x => x.Username.Equals(userName)).FirstOrDefault();
            if (user == null) return null;
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) return null;

            return user;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        public string GenerateToken(User user)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Role, "view-user")
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
