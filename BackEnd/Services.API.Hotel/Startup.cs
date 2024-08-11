using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Services.API.Hotel.Core;
using Services.API.Hotel.Repository;

namespace Services.API.Hotel
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
            var sqlServerConnectionDatabase = Environment.GetEnvironmentVariable("APP_DATABASE");

            if (string.IsNullOrEmpty(sqlServerConnectionString))
            {
                sqlServerConnectionString = Configuration.GetSection("MongoDB:ConnectionString").Value;
            }
            if (string.IsNullOrEmpty(sqlServerConnectionDatabase))
            {
                sqlServerConnectionDatabase = Configuration.GetSection("MongoDB:Database").Value;
            }

            // MongoDB Driver - Conexion
            services.Configure<MongoSettings>(options =>
            {
                options.ConnectionString = sqlServerConnectionString;
                options.Database = sqlServerConnectionDatabase;
            });



            // SQL Driver - Conexion
            services.AddSignalR();

            // Singleton for Inject ONLY 1 instance for Connect with DB --> MongoDBDriver
            services.AddSingleton<MongoSettings>();

            // Scoped for MORE than 1 instance for each API request and auto delete it when end --> Repository
            services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));

            services.AddControllers();

            // Rule for Access, open for all
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsRule", rule =>
                {
                    rule.WithOrigins("http://192.168.1.226:4200")
                                   .AllowAnyHeader()
                                   .AllowAnyMethod()
                                   .AllowCredentials();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            //app.UseHttpsRedirection();

            app.UseCors("CorsRule");
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Middleware for centralize manage errors
            app.UseMiddleware<ErrorHandlerMiddleware>();

            CreateIndexs(app).GetAwaiter().GetResult();

        }
        private async Task CreateIndexs(IApplicationBuilder app)
        {
            var sqlServerConnectionString = Environment.GetEnvironmentVariable("APP_CONNECTION_STRING");
            var sqlServerConnectionDatabase = Environment.GetEnvironmentVariable("APP_DATABASE");
            // Create indexs for MongoDB
            if (string.IsNullOrEmpty(sqlServerConnectionString))
            {
                sqlServerConnectionString = Configuration.GetSection("MongoDB:ConnectionString").Value;
            }
            if (string.IsNullOrEmpty(sqlServerConnectionDatabase))
            {
                sqlServerConnectionDatabase = Configuration.GetSection("MongoDB:Database").Value;
            }

            using (var context = app.ApplicationServices.CreateScope())
            {
                var services = context.ServiceProvider;
                try
                {
                    var client = new MongoClient(sqlServerConnectionString);

                    var cmdStr = "{ createIndexes: 'Room', indexes: [ { key: { RoomNumber: 1 }, name: 'RoomNumber-1', unique: true } ] }";
                    var cmd = BsonDocument.Parse(cmdStr);
                    var resultRoom = client.GetDatabase(sqlServerConnectionDatabase).RunCommand<BsonDocument>(cmd);
                    Console.WriteLine(resultRoom);

                    cmdStr = "{ createIndexes: 'Task', indexes: [ { key: { 'Room.RoomNumber': 1 }, name: 'TaskRoomNumber-1', unique: true } ] }";
                    cmd = BsonDocument.Parse(cmdStr);
                    var resultTask = client.GetDatabase(sqlServerConnectionDatabase).RunCommand<BsonDocument>(cmd);
                    Console.WriteLine(resultTask);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}