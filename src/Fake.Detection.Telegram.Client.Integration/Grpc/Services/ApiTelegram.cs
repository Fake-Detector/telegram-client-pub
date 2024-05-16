using Fake.Detection.Telegram.Client.Bll.Configure;
using Fake.Detection.Telegram.Client.Bll.Services.interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Fake.Detection.Telegram.Client.Integration.Grpc.Services;

public class ApiTelegram : IApiTelegram
{
    private readonly IOptionsMonitor<TelegramBotOptions> _options;

    public ApiTelegram(IOptionsMonitor<TelegramBotOptions> options) => _options = options;
    
    public async Task<string?> GetFilePath(string fileId, CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, _options.CurrentValue.GetFilePathUrl(fileId));
        using var response = await httpClient.SendAsync(request, cancellationToken: cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonConvert.DeserializeObject<GetFilePathResponse>(content);

        return result?.Result?.FilePath is null ? null : _options.CurrentValue.GetFileUrl(result.Result.FilePath);
    }

    private record GetFilePathResponse([property: JsonProperty("result")] FilePathResult? Result);

    private record FilePathResult([property: JsonProperty("file_path")] string? FilePath);
}