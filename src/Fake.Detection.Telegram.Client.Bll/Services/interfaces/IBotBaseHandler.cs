using Fake.Detection.Telegram.Client.Bll.Commands;
using Telegram.Bot.Types;

namespace Fake.Detection.Telegram.Client.Bll.Services.interfaces;

public interface IBotBaseHandler
{
    public Task<MessageCommandResponse> Handle(long chatId, int messageId, long telegramId, Message message, CancellationToken token);
}