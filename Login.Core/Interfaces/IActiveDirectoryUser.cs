namespace Login.Core.Interfaces {
    
    public interface IActiveDirectoryUser
    {
        ActiveDirectoryUser GetUser(string userName);
        string GenerateToken(ActiveDirectoryUser user);
    }
}