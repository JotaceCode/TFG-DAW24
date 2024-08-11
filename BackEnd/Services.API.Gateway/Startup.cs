using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

namespace Services.API.Gateway
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();

            services.AddSingleton<NotificationHub>();

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsRule", rule =>
                {
                    rule.WithOrigins("https://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddOcelot();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("lSQOzerZ0NiZ5Ot8SQhlM1rCAUjRnl6cKibUHcgVrn+0DL8y0bUaFZeOOlHQ5zBeSn6tKObZFYfX52iLtuNcCQ=="));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateAudience = false, //all ip request
                    ValidateIssuer = false, //all domain requests
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsRule");
            app.UseStaticFiles();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseRouting();
            //app.UseHttpsRedirection();
            app.UseAuthentication(); // for signal
            app.UseAuthorization(); // for jwt

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("/notificationHub/notificationHub");
                endpoints.MapControllers();
            });

            app.UseOcelot();
        }
    }
}
