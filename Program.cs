using Referral;
using Referral.Models;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class Program
{
    private static ITelegramBotClient _botClient;
    private static ReceiverOptions _receiverOptions;

    private static async Task Main()
    {
        _botClient = new TelegramBotClient("7528880932:AAHgPXrDuwvu5wJkBQ974RWJoadCA25kOrk");


        try
        {
            var me = await _botClient.GetMeAsync();
            Console.WriteLine($"Бот запущен. Имя бота: {me.Username}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Не удалось подключиться к боту: {ex.Message}");
            return;
        }

        using var cts = new CancellationTokenSource();

        _receiverOptions = new ReceiverOptions 
        {
            AllowedUpdates = new[] 
            {
                UpdateType.Message, 
            },
            
            ThrowPendingUpdates = true,
        };

        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            _receiverOptions,
            cancellationToken: cts.Token
        );

        Console.WriteLine("Бот работает.");

        //cts.Cancel();
        await Task.Delay(-1);
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
         if (update.Type == UpdateType.Message && update.Message.Type == MessageType.Text)
            {
                var message = update.Message;

            if (message.Text.StartsWith("/start"))
            {
                long userId = message.From.Id;
                Console.WriteLine("Пользователь с Id" + userId);
                int? referrer = null;

                string[] messageParts = message.Text.Split(' ');
                if (messageParts.Length > 1 && int.TryParse(messageParts[1], out int referrerCandidate))
                {
                    referrer = referrerCandidate;
                }

                using var context = new ReferralContext();
                context.Database.EnsureCreated();
                ReferralProcessor rp = new(context);

               
                    if (rp.UserExists(userId.ToString()) || rp.ReferralDataExists(userId.ToString()))
                    {
                        HandleDeadend();
                    }

                    rp.GetOrCreateReferralData(userId.ToString(), referrer.ToString());

                    var referrerData = context.ReferralData.SingleOrDefault(rd => rd.AuthKey == referrer.ToString());
                    if (referrerData == null)
                    {
                        HandleDeadend();
                    }

                    rp.AddReferralIfNotExists(referrerData, userId.ToString());
                    context.SaveChanges();
                

                try
                {
                     await botClient.SendTextMessageAsync(message.Chat.Id, $"Привет, {message.From.FirstName}!", cancellationToken: cancellationToken);

                }
                catch (AggregateException ex)
                {
                    foreach (Exception e in ex.InnerExceptions)
                        Console.WriteLine(e.Message);
                }
            }

        }

    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Произошла ошибка: {exception.Message}");
        return Task.CompletedTask;
    }


    private static void HandleDeadend()
    {
        Console.WriteLine("Deadend");
    }


}