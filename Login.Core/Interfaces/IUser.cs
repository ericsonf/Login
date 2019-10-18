using System.Collections.Generic;

namespace Login.Core.Interfaces
{
    public interface IUser
    {
        IEnumerable<User> List();

        User GetById(int id);

        string GenerateToken(User user);
    }
}
