using Fake.Detection.Telegram.Client.Bll.Configure;
using Fake.Detection.Telegram.Client.Bll.Services.interfaces;
using MediatR;
using Microsoft.Extensions.Options;

namespace Fake.Detection.Telegram.Client.Bll.Commands;

public class UpdateHandler :
    IRequestHandler<MessageCommand, List<MessageCommandResponse>>
{
    private readonly IBotTextHandler _botTextHandler;
    private readonly IBotPhotoHandler _botPhotoHandler;
    private readonly IBotVideoHandler _botVideoHandler;
    private readonly IBotAudioHandler _botAudioHandler;
    private readonly IOptionsMonitor<PolicyOptions> _policyOptions;

    public UpdateHandler(
        IBotTextHandler botTextHandler,
        IBotPhotoHandler botPhotoHandler,
        IBotVideoHandler botVideoHandler,
        IBotAudioHandler botAudioHandler,
        IOptionsMonitor<PolicyOptions> policyOptions)
    {
        _botTextHandler = botTextHandler;
        _botPhotoHandler = botPhotoHandler;
        _botVideoHandler = botVideoHandler;
        _botAudioHandler = botAudioHandler;
        _policyOptions = policyOptions;
    }

    public async Task<List<MessageCommandResponse>> Handle(MessageCommand request, CancellationToken cancellationToken)
    {
        if (request.Message is not { } message)
            return new List<MessageCommandResponse> { new() };

        if (message.From == null)
            throw new ArgumentException($"Cannot identify user");

        if (!_policyOptions.CurrentValue.IsAvailableUser(message.From.Id))
            throw new ArgumentException($"User with ID: {message.From.Id} is not available");

        var chatId = message.Chat.Id;
        var messageId = message.MessageId;
        var telegramId = message.From.Id;

        var textResponse = await _botTextHandler.Handle(chatId, messageId, telegramId, message, cancellationToken);
        var photoResponse = await _botPhotoHandler.Handle(chatId, messageId, telegramId, message, cancellationToken);
        var videoResponse = await _botVideoHandler.Handle(chatId, messageId, telegramId, message, cancellationToken);
        var audioResponse = await _botAudioHandler.Handle(chatId, messageId, telegramId, message, cancellationToken);

        return new List<MessageCommandResponse> { textResponse, photoResponse, videoResponse, audioResponse };
    }
}