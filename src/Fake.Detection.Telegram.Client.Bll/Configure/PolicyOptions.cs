namespace Fake.Detection.Telegram.Client.Bll.Configure;

public class PolicyOptions
{
    public long[] AvailableUsers { get; init; } = default!;
    public bool IsEnabled { get; init; }
    
    public bool IsAvailableUser(long userId) => !IsEnabled || IsEnabled && AvailableUsers.Contains(userId);
}