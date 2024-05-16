using Fake.Detection.Telegram.Client.Bll.Models;

namespace Fake.Detection.Telegram.Client.Bll.Services.interfaces;

public interface IAuthService
{
    Task<AuthInfo> TgLink(long tgId, string token, CancellationToken cancellationToken);
    Task<AuthInfo> TgLogin(long tgId, CancellationToken cancellationToken);
    Task<bool> TgSignOut(long tgId, CancellationToken cancellationToken);
}