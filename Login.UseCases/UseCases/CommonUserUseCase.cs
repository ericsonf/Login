using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using Login.Core.Helpers;
using Login.Core.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Login.UseCases
{
    public class CommonUserUseCase : ICommonUser
    {
        private readonly AppSettings _appSettings;
        private readonly IRepository _repository;

        public CommonUserUseCase(IOptions<AppSettings> appSettings, IRepository repository)
        {
            _appSettings = appSettings.Value;
            _repository = repository;
        }

        public IEnumerable<CommonUser> List()
        {
            return _repository.List<CommonUser>();
        }

        public CommonUser GetById(int id)
        {
            return _repository.GetById<CommonUser>(id);
        }

        public CommonUser Authenticate(string userName, string password)
        {
            var user = _repository.Filter<CommonUser>(x => x.Username.Equals(userName)).FirstOrDefault();
            if (user == null) return null;
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) return null;

            return user;
        }

        public string GenerateToken(CommonUser user)
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

        public void Create(CommonUser user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _repository.Add<CommonUser>(user);
            _repository.Save();
        }

        public void Update(CommonUser user)
        {
            _repository.Update<User>(user);
            _repository.Save();
        }

        public void Delete(CommonUser user)
        {
            _repository.Delete<User>(user);
            _repository.Save();
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
    }
}