using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SendGrid;
using System.Threading.Tasks;

namespace MailosaurApiDemo
{
    public class Program
    {
        protected Program() { }

        static async Task Main(string[] args)
        {
            var host = Host
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, configuration) =>
                {
                    configuration.Sources.Clear();
                    configuration
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddUserSecrets<Program>()
                        .AddEnvironmentVariables()
                        .AddCommandLine(args);
                })
                .ConfigureLogging(loggerBuilder =>
                {
                    loggerBuilder.ClearProviders();
                    loggerBuilder.AddConsole(configure => configure.TimestampFormat = "[HH:mm:ss:fff] " /* "[HH:mm:ss:fff MM/dd/yy] " */);
                })
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;
                    services
                        .Configure<MailosaurOptions>(options => configuration.GetSection(nameof(MailosaurOptions)).Bind(options))
                        .Configure<SendgridOptions>(options => configuration.GetSection(nameof(SendgridOptions)).Bind(options));

                    services
                        .AddSingleton<ISendGridClient, SendGridClient>(provider =>
                            new SendGridClient(configuration.GetValue<string>($"{nameof(SendgridOptions)}:ApiKey")))
                        .AddSingleton<IMailosaurClient, MailosaurClientWrapper>(provider =>
                            new MailosaurClientWrapper(configuration.GetValue<string>($"{nameof(MailosaurOptions)}:ApiKey")))
                        .AddSingleton<IMailosaurService, MailosaurService>()
                        .AddSingleton<ISendgridService, SendgridService>()
                        .AddSingleton<IRunnerService, DemoService>();
                })
                .Build();

            var demoService = host.Services.GetRequiredService<IRunnerService>();
            await demoService.RunAsync();

            await Task.Delay(1000);
        }
    }
}
