using Fake.Detection.Telegram.Client.Bll.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace Fake.Detection.Telegram.Client.Bll.Commands;

public record MessageCommandResponse(
    bool Reply = false, 
    long? ChatId = null, 
    int? MessageId = null,
    string? TextReply = null,
    IReplyMarkup? ReplyMarkup = null,
    ItemInfo? ItemInfo = null);