using System.Collections.Generic;

namespace Login.Core.Interfaces {
    
    public interface ICommonUser
    {
        IEnumerable<CommonUser> List();
        CommonUser GetById(int id);
        CommonUser Authenticate(string username, string password);
        string GenerateToken(CommonUser user);
        void Create(CommonUser user, string password);
        void Update(CommonUser user);
        void Delete(CommonUser user);
    }
}