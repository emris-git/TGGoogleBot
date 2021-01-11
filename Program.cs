using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TGGoogleBot.Services;

namespace TGGoogleBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<TelegramBorClientProvider>();
                    services.AddTransient<CalendarServiceProvider>();
                    services.AddHostedService<Worker>();
                });
    }
}