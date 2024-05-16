using MediatR;
using Telegram.Bot.Types;

namespace Fake.Detection.Telegram.Client.Bll.Commands;

public record MessageCommand(Message? Message) : IRequest<List<MessageCommandResponse>>;