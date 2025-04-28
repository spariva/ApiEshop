using ApiEshop.Helpers;
using ApiEshop.Models;
using ApiEshop.Models.DTOs;
using ApiEshop.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiEshop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        private RepositoryUsers repo;
        private HelperOAuth helper;

        public AuthController(RepositoryUsers repo, HelperOAuth helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Login(LoginDto model)
        {
            User u = await this.repo.LoginAsync(model.Email, model.Password);
            if(u == null)
            {
                return Unauthorized();
            }

            //here a possible dto to avoid sending all the user data in the token... if you dont want that
            string json = JsonConvert.SerializeObject(u);
            //string jsonCifrado = HelperCryptography.EncryptString(json);
            Claim[] informacion = new[]
            {
                new Claim("UserData", json)
            };

            SigningCredentials credentials = new SigningCredentials(this.helper.GetKeyToken(), SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                claims: informacion,
                issuer: this.helper.Issuer,
                audience: this.helper.Audience,
                signingCredentials: credentials,
                expires: DateTime.UtcNow.AddMinutes(30),
                notBefore: DateTime.UtcNow
            );

            return Ok(new { response = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            try
            {
                await this.repo.InsertUserAsync(model.Name, model.Email, model.Password, model.Telephone, model.Address);
                return Ok(new { success = true, message = "New user created." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
