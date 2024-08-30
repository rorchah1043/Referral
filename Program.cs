using Referral.Models;
using ReferralSystem;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions;
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

        Console.WriteLine("Бот работает. Нажмите Enter для завершения.");
        Console.ReadLine();

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

                    if (!HasReferrer(userId))
                    {
                        int? referrer = null;

                        string[] messageParts = message.Text.Split(' ');
                        if (messageParts.Length > 1)
                        {
                            if (int.TryParse(messageParts[1], out int referrerCandidate))
                            {
                                if (userId != referrerCandidate && GetAllUsers().Contains(referrerCandidate))
                                {
                                    referrer = referrerCandidate;
                                }
                            }
                        }
                        
                        RegisterUser(userId, referrer);
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"Привет, {message.From.FirstName}!");
                    }
                }

        }

    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Произошла ошибка: {exception.Message}");
        return Task.CompletedTask;
    }

    private static void ProcessReferral(ReferralContext context, string currentUserId, string referrerId)
    {
        var existingUser = context.Users.SingleOrDefault(u => u.AuthKey == currentUserId);
        if (existingUser != null)
        {
            Console.WriteLine("Deadend");
            return;
        }

        var existingReferralData = context.ReferralData.SingleOrDefault(rd => rd.AuthKey == currentUserId && !string.IsNullOrEmpty(rd.Referrer));
        if (existingReferralData != null)
        {
            Console.WriteLine("Deadend");
            return;
        }

        var newReferralData = context.ReferralData.SingleOrDefault(rd => rd.AuthKey == currentUserId);
        if (newReferralData == null)
        {
            newReferralData = new ReferralData { AuthKey = currentUserId, Referrer = referrerId };
            context.ReferralData.Add(newReferralData);
        }
        else
        {
            newReferralData.Referrer = referrerId;
        }
        context.SaveChanges();

        var referrerData = context.ReferralData.SingleOrDefault(rd => rd.AuthKey == referrerId);
        if (referrerData == null)
        {
            Console.WriteLine("Deadend");
            return;
        }

        var referrals = referrerData.ReferralList;
        if (!referrals.Contains(currentUserId))
        {
            referrals.Add(currentUserId);
            referrerData.ReferralList = referrals;
            context.SaveChanges();
        }
    }
    static void RegisterUser(long userId, int? referrer)
    {
        
        Console.WriteLine($"Зарегистрирован пользователь {userId} с реферером {referrer}");
    }

    static bool HasReferrer(long userId)
    {
       
        return false; 
    }

    static int[] GetAllUsers()
    {
        // Логика получения всех пользователей. Например, запрос в базу данных
        return new int[] { 123456, 654321 }; // В данном примере возвращаем фиксированный набор идентификаторов пользователей
    }


}