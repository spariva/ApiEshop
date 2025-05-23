﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Azure.Security.KeyVault.Secrets;

namespace ApiEshop.Helpers
{
    public class HelperOAuth
    {
        private SecretClient secretCli;
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }

        public HelperOAuth
            (IConfiguration configuration, SecretClient secretCli)
        {
            this.secretCli = secretCli;
            this.Issuer = GetSecretFromKeyVaultAsync("IssuerLocal").GetAwaiter().GetResult(); //constructor cant process normal await
            this.Audience = GetSecretFromKeyVaultAsync("Audience").GetAwaiter().GetResult();
            this.SecretKey = GetSecretFromKeyVaultAsync("SecretKey").GetAwaiter().GetResult();
        }

        private async Task<string> GetSecretFromKeyVaultAsync(string secretName)
        {
            KeyVaultSecret secret = await secretCli.GetSecretAsync(secretName);
            return secret.Value;
        }

        //necesitamos un metodo para generar el token que se basa en el secret key
        public SymmetricSecurityKey GetKeyToken()
        {
            //convertimos el secret key a bytes[]
            byte[] data = Encoding.UTF8.GetBytes(this.SecretKey);
            //devolvemos la key 
            return new SymmetricSecurityKey(data);
        }


        public Action<JwtBearerOptions> GetJwtBearerOptions()
        {
            Action<JwtBearerOptions> options =
                new Action<JwtBearerOptions>(options =>
                {
                    //indicamos que debemos validar de nuestro
                    //token: issuer, audience, time...
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = this.Issuer,
                        ValidAudience = this.Audience,
                        IssuerSigningKey = this.GetKeyToken()
                    };
                });
            return options;
        }


        public Action<AuthenticationOptions> GetAuthenticationSchema()
        {
            Action<AuthenticationOptions> options =
                new Action<AuthenticationOptions>(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                });
            return options;
        }
    }
}
