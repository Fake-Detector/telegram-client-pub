using Fake.Detection.Telegram.Client.Bll.Commands;
using Fake.Detection.Telegram.Client.Bll.Consts;
using Fake.Detection.Telegram.Client.Bll.Models;
using Fake.Detection.Telegram.Client.Bll.Services.interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Fake.Detection.Telegram.Client.Bll.Services;

public class BotPhotoHandler : IBotPhotoHandler
{
    private readonly IApiTelegram _apiTelegram;
    private readonly IItemHandler _itemHandler;

    public BotPhotoHandler(
        IApiTelegram apiTelegram, 
        IItemHandler itemHandler)
    {
        _apiTelegram = apiTelegram;
        _itemHandler = itemHandler;
    }

    public async Task<MessageCommandResponse> Handle(long chatId, int messageId, long telegramId, Message message,
        CancellationToken token)
    {
        var photoResponse = new MessageCommandResponse(
            ChatId: chatId,
            MessageId: messageId,
            TextReply: PostTextReply.ErrorTextReply);

        if (message.Photo is not { Length: > 0 }) return photoResponse;

        var photo = message.Photo.Last();

        var filePath = await _apiTelegram.GetFilePath(photo.FileId, token);

        if (filePath is null) return photoResponse;

        var itemInfo = new ItemInfo(message.MessageId.ToString(), filePath, ItemTypeEnum.ImageUrl,
            message.MediaGroupId);

        photoResponse = photoResponse with { Reply = true, ItemInfo = itemInfo };
        
        var (result, postId) = await _itemHandler.HandleInfo(telegramId, itemInfo, token);

        photoResponse = photoResponse with
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

        return photoResponse;
    }
}