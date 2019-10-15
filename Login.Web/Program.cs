using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Login.Web
{
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
                    webBuilder.UseStartup<Startup>()
                        .UseUrls(new string[] { "http://localhost:5000", "https://localhost:5001"});
                });
    }
}
