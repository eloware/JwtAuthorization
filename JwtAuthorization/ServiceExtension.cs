#nullable enable

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Authorization {
    public static class ServiceExtension {
        public static void AddJwtAuthentication(this IServiceCollection services,
            string jwtTokenSecret, TimeSpan? validSpan = null) {
            services.AddTransient<ILogonService>(_ => new LogonService(
                jwtTokenSecret, validSpan ?? new TimeSpan(7, 0, 0, 0)));

            services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x => {
                x.Events = new JwtBearerEvents {
                    OnMessageReceived = context => {
                        context.Token = context.Request.Headers["Authorization"];
                        return Task.CompletedTask;
                    },
#pragma warning disable 1998
                    // To serve interface keep async
                    OnAuthenticationFailed = async context => {
#pragma warning restore 1998
                        // Console.WriteLine("Authentication failed");
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenSecret)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }
    }
}
