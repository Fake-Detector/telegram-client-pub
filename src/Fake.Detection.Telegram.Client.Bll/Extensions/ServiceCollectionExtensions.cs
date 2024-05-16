using Fake.Detection.Telegram.Client.Bll.Configure;
using Fake.Detection.Telegram.Client.Bll.Services;
using Fake.Detection.Telegram.Client.Bll.Services.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fake.Detection.Telegram.Client.Bll.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBll(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<PolicyOptions>(config.GetSection(nameof(PolicyOptions)));
        services.AddServices();
        services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IBotTextHandler, BotTextHandler>();
        services.AddSingleton<IBotPhotoHandler, BotPhotoHandler>();
        services.AddSingleton<IBotVideoHandler, BotVideoHandler>();
        services.AddSingleton<IBotAudioHandler, BotAudioHandler>();
        services.AddSingleton<IItemHandler, ItemHandler>();

        return services;
    }
}