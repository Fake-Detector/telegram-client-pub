using Fake.Detection.Telegram.Client.Bll.Commands;
using Fake.Detection.Telegram.Client.Bll.Consts;
using Fake.Detection.Telegram.Client.Bll.Models;
using Fake.Detection.Telegram.Client.Bll.Services.interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Fake.Detection.Telegram.Client.Bll.Services;

public class BotAudioHandler : IBotAudioHandler
{
    private readonly IApiTelegram _apiTelegram;
    private readonly IItemHandler _itemHandler;

    public BotAudioHandler(
        IApiTelegram apiTelegram,
        IItemHandler itemHandler)
    {
        _apiTelegram = apiTelegram;
        _itemHandler = itemHandler;
    }

    public async Task<MessageCommandResponse> Handle(long chatId, int messageId, long telegramId, Message message,
        CancellationToken token)
    {
        var audioResponse = new MessageCommandResponse(
            ChatId: chatId,
            MessageId: messageId,
            TextReply: PostTextReply.ErrorTextReply);

        if (message.Audio == null && message.Voice == null) return audioResponse;

        var audio = message.Audio;
        var voice = message.Voice;
        var fileId = audio?.FileId ?? voice?.FileId ?? "";

        var filePath = await _apiTelegram.GetFilePath(fileId, token);

        if (filePath is null) return audioResponse;

        var itemInfo = new ItemInfo(message.MessageId.ToString(), filePath, ItemTypeEnum.AudioUrl,
            message.MediaGroupId);

        audioResponse = audioResponse with { Reply = true, ItemInfo = itemInfo };

        var (result, postId) = await _itemHandler.HandleInfo(telegramId, itemInfo, token);

        audioResponse = audioResponse with
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

        return audioResponse;
    }
}