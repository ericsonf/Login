public class CommonUser : User
{
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
}