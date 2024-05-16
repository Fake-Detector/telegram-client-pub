namespace Fake.Detection.Telegram.Client.Bll.Models;

public record AuthInfo(
    bool Result, 
    string Name, 
    string Token, 
    string? ErrorMessage = null);