namespace Fake.Detection.Telegram.Client.Bll.Configure;

public class TelegramBotOptions
{
    public string Token { get; init; } = default!;

    public string GetFilePathUrl(string fileId) => $"https://api.telegram.org/bot{Token}/getFile?file_id={fileId}";
    public string GetFileUrl(string filePath) => $"https://api.telegram.org/file/bot{Token}/{filePath}";
}