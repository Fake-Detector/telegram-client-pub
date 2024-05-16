using Fake.Detection.Telegram.Client.Bll.Models;

namespace Fake.Detection.Telegram.Client.Bll.Services.interfaces;

public interface IItemHandler
{
    Task<(bool result, long? postId)> HandleInfo(long telegramId, ItemInfo info, CancellationToken cancellationToken);
}