namespace Fake.Detection.Telegram.Client.Bll.Services.interfaces;

public interface IBridgeService
{
    Task<long> CreatePost(string authorId, string externalId, string token, CancellationToken cancellationToken);
    Task<long?> GetPost(string externalId, string token, CancellationToken cancellationToken);
    Task<string?> SendPostItem(long postId, string itemType, byte[] bytes, string token, CancellationToken cancellationToken);
    Task ProcessItem(long postId, string itemId, string token, CancellationToken cancellationToken);
}