using Fake.Detection.Telegram.Client.Bll.Commands;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Fake.Detection.Telegram.Client.Api.Services;

public class BotHandler : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IMediator _mediator;
    private readonly ILogger<BotHandler> _logger;

    public BotHandler(
        ITelegramBotClient botClient,
        IMediator mediator,
        ILogger<BotHandler> logger)
    {
        _botClient = botClient;
        _mediator = mediator;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("START RECEIVING...");

        _botClient.StartReceiving(
            updateHandler: OnMessage,
            pollingErrorHandler: OnError,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task OnMessage(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        try
        {
            var messageResponses = await _mediator.Send(new MessageCommand(update.Message), cancellationToken);

            foreach (var response in messageResponses.Where(it => it.Reply))
            {
                try
                {
                    if (response is { TextReply: not null, ChatId: not null })
                        await bot.SendTextMessageAsync(
                            chatId: response.ChatId.Value,
                            text: response.TextReply,
                            replyToMessageId: response.MessageId,
                            replyMarkup: response.ReplyMarkup,
                            cancellationToken: cancellationToken);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error while sending: {Message}", exception.Message);
                }
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error: {Message}", exception.Message);
        }
    }

    private async Task OnError(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Error: {Message}", exception.Message);
    }
}