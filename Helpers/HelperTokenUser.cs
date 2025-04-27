using Newtonsoft.Json;
using System.Security.Claims;
using ApiEshop.Models;
using ApiEshop.Models.DTOs;
using ApiEshop.Helpers;

namespace ApiEshop.Helpers
{
    public class HelperTokenUser
    {

        private IHttpContextAccessor contextAccessor;

        public HelperTokenUser(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public User GetUser()
        {
            Claim claim = contextAccessor.HttpContext.User.FindFirst(x => x.Type == "UserData");
            if(claim == null)
            {
                throw new UnauthorizedAccessException("'UserData' claim not found.");
            }

            string json = claim.Value;
            string jsonUsuario = HelperCryptography.DecryptString(json);
            User user = JsonConvert.DeserializeObject<User>(json);
            return user;
        }
    }

}
