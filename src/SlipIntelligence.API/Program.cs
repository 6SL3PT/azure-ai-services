using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SlipIntelligence.API;

public class Program {
    public static void Main(string[] args) {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder.UseStartup<Startup>();
                webBuilder.ConfigureKestrel(options => {
                    options.ListenAnyIP(5202); // HTTP
                    options.ListenAnyIP(7131, listenOptions => {
                        listenOptions.UseHttps(); // HTTPS
                    });
                });
            });
}

