using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;

namespace TelegramBotExperiments
{

    class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("test");
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Привет! Кидай в меня SGTIN-ом, а я кину QR-код! :)");
                    return;
                }
                if (message.Text.ToLower().Length == 27)
                {
                    string gtin = message.Text.Substring(0, 14);
                    string crypt = message.Text.Substring(14, 13);
                    string encodeSgtin = Uri.EscapeDataString(String.Format("01{0}21{1}XD91ffd092UG2+xUuo35f/HF4ZQ5xuz9oq4FCifO6/IAZacvJuUNGQ9ojVo61+HsLvMQJpn8G8Pn0/SshDPfxDLrO71nNzGA==", gtin, crypt));
                    await botClient.SendTextMessageAsync(message.Chat, String.Format("SGTIN: {0}", message.Text));
                    await bot.SendPhotoAsync(message.Chat, photo: String.Format("http://qrcoder.ru/code/?{0}&4&0", encodeSgtin));
                    return;
                }

                await botClient.SendTextMessageAsync(message.Chat, "Жду SGTIN! :)");
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}