using System.Text;
using Newtonsoft.Json;

namespace Fake.Detection.Telegram.Client.Bll.Extensions;

public static class JwtExtensions
{
    private record LoginInfo([JsonProperty("login")] string Login);

    public static string? GetLoginFromToken(this string token)
    {
        var split = token.Split(".", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return split.Length <= 2
            ? null
            : JsonConvert.DeserializeObject<LoginInfo>(
                Encoding.UTF8.GetString(Convert.FromBase64String(split[1]))
            )?.Login;
    }
}