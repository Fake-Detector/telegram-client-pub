namespace Fake.Detection.Telegram.Client.Bll.Services.interfaces;

public interface IApiTelegram
{
    Task<string?> GetFilePath(string fileId, CancellationToken cancellationToken);
}