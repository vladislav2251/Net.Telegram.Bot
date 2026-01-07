using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Bot;

using var cts = new CancellationTokenSource();

var bot = new TelegramBotClient(BotConfig.Token);

var me = await bot.GetMe(); 

Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>() 
};

bot.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    receiverOptions,
    cts.Token
);

Console.ReadLine();
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message)
        return;
    if (message.Text is not { } messageText)
        return;

    Console.WriteLine($"Received '{messageText}' in chat {message.Chat.Id}");

    await botClient.SendMessage(
        chatId: message.Chat.Id,
        text: messageText,
        cancellationToken: cancellationToken
    );
}

Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    Console.WriteLine($"Error: {exception.Message}");
    return Task.CompletedTask;
}