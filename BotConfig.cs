using Telegram.Bot;
using Telegram.Bot.Types;
namespace Bot;


public class BotConfig
{
    private static readonly string Token = "7824801699:AAFvy9FfRWwPjWVkLfsOeeergOxONes1R7g";

    public static async Task<User> GetMe()
    {
        var bot = new TelegramBotClient(Token);
        return await bot.GetMe();
    }
}