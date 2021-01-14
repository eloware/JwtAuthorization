#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthorization {
    public class LogonService : ILogonService {
        private readonly byte[] _secretBytes;
        private readonly TimeSpan _validSpan;

        public LogonService(string jwtTokenSecret, TimeSpan validSpan) {
            _secretBytes = Encoding.ASCII.GetBytes(jwtTokenSecret);
            _validSpan = validSpan;
        }


        private bool ValidateSignature(string token) {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateActor = false,
                IssuerSigningKey = new SymmetricSecurityKey(_secretBytes),
                ValidateLifetime = false,
                ValidateTokenReplay = false,
                //IssuerSigningToken = new BinarySecretSecurityToken(secret)
            };

            SecurityToken validatedToken;
            try {
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            }
            catch (Exception) {
                return false;
            }

            return validatedToken != null;
        }


        public TUser? DecodeToken<TUser>(string token) where TUser : TokenData, new() {
            if (string.IsNullOrWhiteSpace(token))
                return (TUser?) null;

            var secToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var result = new TUser {
                IssuedAt = secToken.IssuedAt.ToLocalTime(),
                ExpirationTime = secToken.ValidTo.ToLocalTime(),
                NotValidBefore = secToken.ValidFrom.ToLocalTime(),
                IsCorrupt = !ValidateSignature(token),
            };

            foreach (var property in typeof(TUser).GetProperties()) {
                var attribute =
                    property.GetCustomAttribute(typeof(JwtTokenProperty)) as JwtTokenProperty;
                if (attribute == null) continue;

                var claimType = attribute.Type ?? property.Name;

                var value = secToken.Claims.FirstOrDefault(c => c.Type == claimType)?.Value ?? "";
                property.SetValue(result, value);
            }

            return result;
        }

        public string EncodeToken<TUser>(TUser user) {
            if (user == null)
                throw new NoNullAllowedException($"{nameof(user)} must not be null");

            var claims = new List<Claim>();
            foreach (var property in user.GetType().GetProperties()) {
                var attribute = property.GetCustomAttribute(typeof(JwtTokenProperty));
                if (attribute == null) continue;

                var type = (attribute as JwtTokenProperty)!.Type ?? property.Name;
                claims.Add(new Claim(type,
                    property.GetValue(user)?.ToString() ?? ""));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(
                    claims.ToArray()
                ),
                Expires = DateTime.Now.Add(_validSpan),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_secretBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}
