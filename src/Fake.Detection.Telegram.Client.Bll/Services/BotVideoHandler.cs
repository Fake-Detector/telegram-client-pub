using Fake.Detection.Telegram.Client.Bll.Commands;
using Fake.Detection.Telegram.Client.Bll.Consts;
using Fake.Detection.Telegram.Client.Bll.Models;
using Fake.Detection.Telegram.Client.Bll.Services.interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Fake.Detection.Telegram.Client.Bll.Services;

public class BotVideoHandler : IBotVideoHandler
{
    private readonly IApiTelegram _apiTelegram;
    private readonly IItemHandler _itemHandler;

    public BotVideoHandler(
        IApiTelegram apiTelegram,
        IItemHandler itemHandler)
    {
        _apiTelegram = apiTelegram;
        _itemHandler = itemHandler;
    }


    public async Task<MessageCommandResponse> Handle(long chatId, int messageId, long telegramId, Message message,
        CancellationToken token)
    {
        var videoResponse = new MessageCommandResponse(
            ChatId: chatId,
            MessageId: messageId,
            TextReply: PostTextReply.ErrorTextReply);

        if (message.Video == null && message.VideoNote == null) return videoResponse;

        var video = message.Video;
        var videoNote = message.VideoNote;

        var fileId = video?.FileId ?? videoNote?.FileId ?? "";

        var filePath = await _apiTelegram.GetFilePath(fileId, token);

        if (filePath is null) return videoResponse;

        var itemInfo = new ItemInfo(message.MessageId.ToString(), filePath, ItemTypeEnum.VideoUrl,
            message.MediaGroupId);

        videoResponse = videoResponse with { Reply = true, ItemInfo = itemInfo };

        var (result, postId) = await _itemHandler.HandleInfo(telegramId, itemInfo, token);

        videoResponse = videoResponse with
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

        return videoResponse;
    }
}