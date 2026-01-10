using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
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

    var replyKeyboard = new ReplyKeyboardMarkup(new[] {
       new[]
       {
          new KeyboardButton("Курс Валют"),
          new KeyboardButton("Мій Гаманець")
       }
    })
    {
       ResizeKeyboard = true
    };

    string responseText = null;

    switch (messageText) {
       case "/start":
            responseText = "Цей бот допоможе користуватись монобанком зручніше та мобільніше.";
            break;
       case "Курс Валют":
            responseText = await GetMonoCurrency(); 
            break;
       default:
            responseText = "Я не знаю таку команду 🤷‍♂️";
            break;
    }

    if (responseText != null)
    {
        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: responseText,
            replyMarkup: replyKeyboard,
            cancellationToken: cancellationToken
        );
    }
}

async Task<string> GetMonoCurrency()
{
    string url = "https://api.monobank.ua/bank/currency";

    using var httpClient = new HttpClient();

    try {
       var currencies = await httpClient.GetFromJsonAsync<List<MonoCurrencyInfo>>(url) ?? new List<MonoCurrencyInfo>();
       
       var usd = currencies.FirstOrDefault(c => c.CurrencyCodeA == 840 && c.CurrencyCodeB == 980);
       var eur = currencies.FirstOrDefault(c => c.CurrencyCodeA == 978 && c.CurrencyCodeB == 980);
       
       return $"курс в Monobank:\n\n" +
               $"🇺🇸 USD: Покупка {usd?.RateBuy} / Продажа {usd?.RateSell}\n" +
               $"🇪🇺 EUR: Покупка {eur?.RateBuy} / Продажа {eur?.RateSell}";
    }
    catch (Exception)
    {
        return "Не удалось получить курс валют. Возможно, слишком частые запросы.";
    }
}

Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    Console.WriteLine($"Error: {exception.Message}");
    return Task.CompletedTask;
}

public class MonoCurrencyInfo
{
    [JsonPropertyName("currencyCodeA")]
    public int CurrencyCodeA { get; set; }

    [JsonPropertyName("currencyCodeB")]
    public int CurrencyCodeB { get; set; }

    [JsonPropertyName("rateBuy")]
    public float RateBuy { get; set; }

    [JsonPropertyName("rateSell")]
    public float RateSell { get; set; }
}