using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Login.Core.Helpers;
using Login.Core.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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
                var user = UserPrincipal.FindByIdentity(context, userName);
                if (user != null)
                {
                    DirectoryEntry dsUser = user.GetUnderlyingObject() as DirectoryEntry;
                    adUser.Title = dsUser.Properties["title"].Value.ToString();
                    adUser.Department = dsUser.Properties["department"].Value.ToString();
                    adUser.Email = dsUser.Properties["mail"].Value.ToString();
                    adUser.Username = dsUser.Properties["samaccountname"].Value.ToString();
                    adUser.FirstName = dsUser.Properties["givenname"].Value.ToString();
                    adUser.LastName = dsUser.Properties["sn"].Value.ToString();
                }
            }
            
            return adUser;
        }

        public string GenerateToken(ActiveDirectoryUser user)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Role, "view-user"),
                    new Claim(ClaimTypes.Role, "create-user")
                }),
                Expires = DateTime.UtcNow.AddMilliseconds(_appSettings.Expiration),
                Issuer = _appSettings.Issuer,
                Audience = _appSettings.Audience.Split(',').First(),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret)), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}