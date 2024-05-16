using System.Text;
using System.Text.RegularExpressions;
using Fake.Detection.Telegram.Client.Bll.Extensions;
using Fake.Detection.Telegram.Client.Bll.Models;
using Fake.Detection.Telegram.Client.Bll.Services.interfaces;

namespace Fake.Detection.Telegram.Client.Bll.Services;

public class ItemHandler : IItemHandler
{
    private readonly IAuthService _authService;
    private readonly IBridgeService _bridgeService;
    private readonly Regex _linkRegex = new Regex(@"^\/link [^\s-]+");

    public ItemHandler(
        IAuthService authService,
        IBridgeService bridgeService)
    {
        _authService = authService;
        _bridgeService = bridgeService;
    }

    public async Task<(bool result, long? postId)> HandleInfo(long telegramId, ItemInfo info,
        CancellationToken cancellationToken)
    {
        try
        {
            switch (info.Type)
            {
                case ItemTypeEnum.Text:
                    return await TextHandle(telegramId, info, cancellationToken);
                default:
                    return await CommonHandle(telegramId, info, cancellationToken);
            }
        }
        catch (Exception)
        {
            return (false, null);
        }
    }

    private async Task<(bool result, long? postId)> TextHandle(long telegramId, ItemInfo info,
        CancellationToken cancellationToken)
    {
        if (_linkRegex.IsMatch(info.Value.Trim()))
        {
            var token = info.Value.Trim().Split()[1];

            await _authService.TgLink(telegramId, token, cancellationToken);
            return (true, null);
        }

        if (info.Value.Trim().Equals("/signout", StringComparison.InvariantCultureIgnoreCase))
        {
            await _authService.TgSignOut(telegramId, cancellationToken);
            return (true, null);
        }

        return await CommonHandle(telegramId, info, cancellationToken);
    }


    private async Task<(bool result, long? postId)> CommonHandle(long telegramId, ItemInfo info,
        CancellationToken cancellationToken)
    {
        var authInfo = await _authService.TgLogin(telegramId, cancellationToken);

        if (!authInfo.Result)
            return (false, null);

        var postId = await _bridgeService.GetPost(info.ExternalId ?? info.MessageId, authInfo.Token, cancellationToken);

        var login = authInfo.Token.GetLoginFromToken();
        if (login is null)
            return (false, null);

        postId ??= await _bridgeService.CreatePost(login, info.ExternalId ?? info.MessageId, authInfo.Token,
            cancellationToken);

        var itemId = await _bridgeService.SendPostItem(
            postId.Value,
            info.Type.ToString(),
            Encoding.UTF8.GetBytes(info.Value),
            authInfo.Token, cancellationToken);

        if (itemId is null)
            return (false, null);

        await _bridgeService.ProcessItem(postId.Value, itemId, authInfo.Token, cancellationToken);

        return (true, postId);
    }
}