using System.Collections.Generic;

namespace Login.Core.Interfaces {
    
    public interface IUser
    {
        User Authenticate(string username, string password);
        User Create(User user, string password);
        IEnumerable<User> GetAll();
    }
}