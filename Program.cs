using Telegram.Bot;
using Bot;
using System;

var me = await BotConfig.GetMe();
Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");