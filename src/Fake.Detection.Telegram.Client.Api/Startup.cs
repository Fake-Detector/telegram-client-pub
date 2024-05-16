using Common.Library.Kafka.Common.Extensions;
using Fake.Detection.Post.Monitoring.Client.Extensions;
using Fake.Detection.Telegram.Client.Api.Services;
using Fake.Detection.Telegram.Client.Bll.Configure;
using Fake.Detection.Telegram.Client.Bll.Extensions;
using Fake.Detection.Telegram.Client.Integration.Extensions;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Fake.Detection.Telegram.Client.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<TelegramBotOptions>(_configuration.GetSection(nameof(TelegramBotOptions)));

        services.AddCommonKafka(_configuration);
        services.AddMonitoring(_configuration);
        
        services.AddSingleton<ITelegramBotClient, TelegramBotClient>(x =>
        {
            var telegramBotOptions = x.GetRequiredService<IOptions<TelegramBotOptions>>();

            return new TelegramBotClient(telegramBotOptions.Value.Token);
        });

        services.AddBll(_configuration);
        services.AddIntegration(_configuration);

        services.AddHostedService<BotHandler>();
    }

    public void Configure()
    {

    }
}