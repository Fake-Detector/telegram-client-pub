using Fake.Detection.Telegram.Client.Bll.Commands;
using Fake.Detection.Telegram.Client.Bll.Consts;
using Fake.Detection.Telegram.Client.Bll.Models;
using Fake.Detection.Telegram.Client.Bll.Services.interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Fake.Detection.Telegram.Client.Bll.Services;

public class BotTextHandler : IBotTextHandler
{
    private readonly IItemHandler _itemHandler;

    public BotTextHandler(IItemHandler itemHandler) => _itemHandler = itemHandler;

    public async Task<MessageCommandResponse> Handle(long chatId, int messageId, long telegramId, Message message,
        CancellationToken token)
    {
        var textResponse = new MessageCommandResponse(
            ChatId: chatId,
            MessageId: messageId,
            TextReply: PostTextReply.ErrorTextReply);

        var text = message.Text?.Trim() ?? message.Caption?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(text) || text.Equals("/start")) return textResponse;

        var itemInfo = new ItemInfo(message.MessageId.ToString(), text, ItemTypeEnum.Text, message.MediaGroupId);

        textResponse = textResponse with { Reply = true, ItemInfo = itemInfo };

        var (result, postId) = await _itemHandler.HandleInfo(telegramId, itemInfo, token);

        textResponse = textResponse with
        {
            TextReply = result ? PostTextReply.TextReply : PostTextReply.ErrorTextReply,
            ReplyMarkup = result && postId is not null
                ? new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithUrl(
                        text: PostTextReply.AppTextReply,
                        url: $"{PostTextReply.AppUrl}/{postId}/Author")
                })
                : null
        };
        
        return textResponse;
    }
}