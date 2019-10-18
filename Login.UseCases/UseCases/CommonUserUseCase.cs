using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Login.Core.Interfaces;

namespace Login.UseCases
{
    public class CommonUserUseCase : ICommonUser
    {
        private readonly IRepository _repository;

        public CommonUserUseCase(IRepository repository)
        {
            _repository = repository;
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

        public CommonUser Authenticate(string userName, string password)
        {
            var user = _repository.Filter<CommonUser>(x => x.Username.Equals(userName)).FirstOrDefault();
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
    }
}