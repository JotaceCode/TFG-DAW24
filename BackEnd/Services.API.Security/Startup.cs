using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Services.API.Security.Core.Application;
using Services.API.Security.Core.Entities;
using Services.API.Security.Core.JwtLogic;
using System.Text;
using Services.API.Security.Core.Persistence;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Services.API.Security.Core;
using Microsoft.Extensions.FileProviders;

namespace Services.API.Security
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var sqlServerConnectionString = Environment.GetEnvironmentVariable("APP_CONNECTION_STRING");
            if (string.IsNullOrEmpty(sqlServerConnectionString))
            {
                sqlServerConnectionString = Configuration.GetSection("SQLServer:ConnectionString").Value;
            }
            // SQL Driver - Conexion
            services.AddDbContext<SQLServerContext>(options =>
            {
                options.UseSqlServer(sqlServerConnectionString);
            });

            // Add methods to UserEntity for IdentityCore (SQL DB methods)
            var identity = services.AddIdentityCore<UserEntity>();
            // Entity Framework Core (for map the data grom IdentityCore)
            var identityBuilder = new IdentityBuilder(identity.UserType, identity.Services);
            // Core Identity - Entity Framework Core (join both services)
            identityBuilder.AddEntityFrameworkStores<SQLServerContext>();
            // Login Manager for UserEntity
            identityBuilder.AddSignInManager<SignInManager<UserEntity>>();
            // For set DateTime auto when creates a new User in the DB
            services.TryAddSingleton(TimeProvider.System);

            // Inject UserRegisterCommand class for controllers
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(Register.UserRegisterCommand)));

            // Inject IMapper
            services.AddAutoMapper(typeof(Register.UserRegisterHandler));

            // Inject JWT
            services.AddScoped<IJwtGenerator, JwtGenerator>();

            // Inject LoggedUser 
            services.AddScoped<IUserLogged, UserLogged>();

            // Authentication with JWT for API request
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

            // reset password if is admin
            services.AddSingleton<PasswordHasher<IdentityUser>>();

            services.AddControllers();

            // Rule for Access, open for all
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsRule", rule =>
                {
                    rule.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Middleware for centralize manage errors
            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseCors("CorsRule");
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            CreateDefaultUser(app).GetAwaiter().GetResult();

        }
        private async Task CreateDefaultUser(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<UserEntity>>();
                    var contextEF = services.GetRequiredService<SQLServerContext>();

                    await SQLServerData.InsertUser(contextEF, userManager);
                }
                catch (Exception e)
                {
                    var logger = services.GetRequiredService<ILogger<Startup>>();
                    logger.LogError(e, "Error al registrar usuario");
                }
            }
        }
    }
}