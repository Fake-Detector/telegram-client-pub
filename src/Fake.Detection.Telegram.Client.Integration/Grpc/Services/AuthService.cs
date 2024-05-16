using Fake.Detection.Auth;
using Fake.Detection.Telegram.Client.Bll.Models;
using Fake.Detection.Telegram.Client.Bll.Services.interfaces;

namespace Fake.Detection.Telegram.Client.Integration.Grpc.Services;

public class AuthService : IAuthService
{
    private readonly UserService.UserServiceClient _client;

    public AuthService(UserService.UserServiceClient client) => _client = client;

    public async Task<AuthInfo> TgLink(long tgId, string token, CancellationToken cancellationToken)
    {
        var request = new TGLinkRequest
        {
            TgId = tgId,
            Token = token
        };

        var response = await _client.TGLinkAsync(request, cancellationToken: cancellationToken);

        return new AuthInfo(response.Result, response.User.Name, response.User.Token,
            response.ErrorStatus is ErrorResponse.None ? null : response.ErrorStatus.ToString());
    }

    public async Task<AuthInfo> TgLogin(long tgId, CancellationToken cancellationToken)
    {
        var request = new TGLoginRequest
        {
            TgId = tgId
        };

        var response = await _client.TGLoginAsync(request, cancellationToken: cancellationToken);
        
        return new AuthInfo(response.Result, response.User.Name, response.User.Token,
            response.ErrorStatus is ErrorResponse.None ? null : response.ErrorStatus.ToString());
    }

    public async Task<bool> TgSignOut(long tgId, CancellationToken cancellationToken)
    {
        var request = new TGSignOutRequest
        {
            TgId = tgId
        };

        var response = await _client.TGSignOutAsync(request, cancellationToken: cancellationToken);

        return response.Result;
    }
}