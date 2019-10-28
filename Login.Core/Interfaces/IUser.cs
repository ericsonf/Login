using System.Collections.Generic;

namespace Login.Core.Interfaces
{
    public interface IUser
    {
        IEnumerable<User> List();
        User GetById(int id);
        void Create(User user, string password);
        void Update(User user);
        void Delete(User user);
        User Authenticate(string userName, string password);
        string GenerateToken(User user);
    }
}
