using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Login.Core.Helpers;
using Login.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace Login.UseCases
{
    public class ActiveDirectoryUserUseCase : IActiveDirectoryUser
    {
        private readonly AppSettings _appSettings;
        private readonly IRepository _repository;

        public ActiveDirectoryUserUseCase(IOptions<AppSettings> appSettings, IRepository repository)
        {
            _appSettings = appSettings.Value;
            _repository = repository;
        }

        public ActiveDirectoryUser GetUser(string userName)
        {
            var adUser = new ActiveDirectoryUser(); 
            
            using (var context = new PrincipalContext(ContextType.Domain, _appSettings.ActiveDirectoryDomain))
            {
                UserPrincipal user = UserPrincipal.FindByIdentity(context, userName);
                if (user != null)
                {
                    DirectoryEntry dsUser = user.GetUnderlyingObject() as DirectoryEntry;
                    adUser.Manager = dsUser.Properties["manager"][0].ToString();
                    adUser.Email = dsUser.Properties["mail"].Value.ToString();
                    adUser.Username = dsUser.Properties["samaccountname"].Value.ToString();
                    adUser.FirstName = dsUser.Properties["givenname"].Value.ToString();
                    adUser.LastName = dsUser.Properties["sn"].Value.ToString();
                }
            }
            
            return adUser;
        }
    }
}