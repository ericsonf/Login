using Login.Core.Shared;

public class ActiveDirectoryUser : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Title { get; set; }
    public string Department { get; set; }
}