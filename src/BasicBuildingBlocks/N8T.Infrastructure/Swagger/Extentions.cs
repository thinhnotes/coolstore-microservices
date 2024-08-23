using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;

namespace N8T.Infrastructure.Swagger
{
    public static class Extentions
    {
        public static IServiceCollection AddOpenApi(this IServiceCollection services, IConfiguration config, Dictionary<string, string> scope)
        {
            //services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(options =>
            {
                JwtBearerOptions authSetting = new JwtBearerOptions();
                config.Bind("Authn", authSetting);
                options.OperationFilter<AuthorizeCheckOperationFilter>();

                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Protected API", Version = "v1" });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{authSetting.Authority}/connect/authorize"),
                            TokenUrl = new Uri($"{authSetting.Authority}/connect/token"),
                            Scopes = scope
                        }
                    }
                });
            });
            return services;
        }

        public static IApplicationBuilder UseOpenApi(this IApplicationBuilder app)
        {
            app.UseSwagger();
            return app;
        }
    }
}
