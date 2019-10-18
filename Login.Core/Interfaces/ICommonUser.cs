namespace Login.Core.Interfaces {
    
    public interface ICommonUser
    {
        void Create(CommonUser user, string password);

        void Update(CommonUser user);

        void Delete(CommonUser user);

        CommonUser Authenticate(string userName, string password);
    }
}