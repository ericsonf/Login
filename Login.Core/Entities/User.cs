using Login.Core.Shared;

public abstract class User : BaseEntity
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }
}