using Fake.Detection.Auth;
using Fake.Detection.Telegram.Client.Bll.Services.interfaces;
using Fake.Detection.Telegram.Client.Integration.Configure;
using Fake.Detection.Telegram.Client.Integration.Grpc.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PostBridgeServiceClient = Fake.Detection.Post.Bridge.Api.PostBridgeService.PostBridgeServiceClient;

namespace Fake.Detection.Telegram.Client.Integration.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIntegration(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<GrpcOptions>(config.GetSection(nameof(GrpcOptions)));

        services.AddGrpcClient<PostBridgeServiceClient>((provider, options) =>
        {
            options.Address = new Uri(provider.GetRequiredService<IOptions<GrpcOptions>>().Value.BridgeUrl);
        }).ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            return handler;
        });

        services.AddGrpcClient<UserService.UserServiceClient>((provider, options) =>
        {
            options.Address = new Uri(provider.GetRequiredService<IOptions<GrpcOptions>>().Value.AuthUrl);
        }).ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            return handler;
        });

        services.AddSingleton<IApiTelegram, ApiTelegram>();
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IBridgeService, PostBridgeService>();

        return services;
    }
}