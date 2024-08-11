using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Services.API.Gateway;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
                });
                webBuilder.UseStartup<Startup>();
            });
}
