namespace Fake.Detection.Telegram.Client.Bll.Models;

public record ItemInfo(string MessageId, string Value, ItemTypeEnum Type, string? ExternalId = null);