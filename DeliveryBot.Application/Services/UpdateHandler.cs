using DeliveryBot.Application.ResponseModel;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DeliveryBot.Application.Services;
#pragma warning disable
public class UpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandler> _logger;
    public UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        var handler = update switch
        {
            { Message: { } message } => BotOnMessageReceived(message, cancellationToken),  // Asosiy qismi logikasi shu methodda
            { EditedMessage: { } message } => BotOnMessageReceived(message, cancellationToken),
            { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(callbackQuery, cancellationToken),  // ok
           // { InlineQuery: { } inlineQuery } => BotOnInlineQueryReceived(inlineQuery, cancellationToken),
            { ChosenInlineResult: { } chosenInlineResult } => BotOnChosenInlineResultReceived(chosenInlineResult, cancellationToken),  // ok
            _ => UnknownUpdateHandlerAsync(update, cancellationToken)
        };

        await handler;
    }

    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receive message type: {MessageType}", message.Type);
        if (message.Text is not { } messageText)
            return;

        var action = messageText.Split(' ')[0] switch
        {
            _ => StartBot(_botClient, message, cancellationToken)
        };
        Message sentMessage = await action;
        _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);

        // Send inline keyboard
        // You can process responses in BotOnCallbackQueryReceived handler


        //// chatting here1
        //static async Task<Message> SendInlineKeyboard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        //{
        //    await botClient.SendChatActionAsync(
        //        chatId: message.Chat.Id,
        //        chatAction: ChatAction.Typing,
        //        cancellationToken: cancellationToken);

        //    // Simulate longer running task
        //    await Task.Delay(500, cancellationToken);

        //    InlineKeyboardMarkup inlineKeyboard = new(
        //        new[]
        //        {
        //            // first row
        //            new []
        //            {
        //                InlineKeyboardButton.WithCallbackData("1.1", "11"),
        //                InlineKeyboardButton.WithCallbackData("1.2", "12"),
        //            },
        //            // second row
        //            new []
        //            {
        //                InlineKeyboardButton.WithCallbackData("2.1", "21"),
        //                InlineKeyboardButton.WithCallbackData("2.2", "22"),
        //            },
        //        });

        //    return await botClient.SendTextMessageAsync(
        //        chatId: message.Chat.Id,
        //        text: "Choose",
        //        replyMarkup: inlineKeyboard,
        //        cancellationToken: cancellationToken);
        //}


        //static async Task<Message> StartBot(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        //{
        //    await botClient.SendChatActionAsync(
        //        chatId: message.Chat.Id,
        //        chatAction: ChatAction.Typing,
        //        cancellationToken: cancellationToken
        //        );
        //    await Task.Delay(500, cancellationToken);
        //    InlineKeyboardMarkup inlineKeyboard = new(
        //        new[]
        //        {
        //            new[]
        //            {
        //                InlineKeyboardButton.WithCallbackData("Buyurtma berish", "Language")
        //            },
        //        });

        //    return await botClient.SendTextMessageAsync(
        //        chatId: message.Chat.Id,
        //        text: "Choose",
        //        replyMarkup: inlineKeyboard,
        //        cancellationToken: cancellationToken);
        //}

        

         static async Task<Message> StartBot(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            var messageHelper = (message.Text.ToString().ToLower());
            var defaultLanguage = message.From.LanguageCode.ToLower().ToString();

            ReplyKeyboardMarkup replyMarkupBot = new(
            new[]
            {
                    new KeyboardButton[] { "🇺🇿 Uzbek" },
                    new KeyboardButton[] { "🇱🇷 English" },
                    new KeyboardButton[] { "🇷🇺 Russian" },
            })
            {
                ResizeKeyboard = true,
            };


            // Uzbek 

            if (messageHelper == "back")
            {
                return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "sukest",
                    replyMarkup: replyMarkupBot,
                    cancellationToken: cancellationToken);
            }


            if (messageHelper == "🇺🇿 uzbek")
            {
                var languageUz = LanguageResponse("uz uzbek");

                ReplyKeyboardMarkup replyMarkupOrder = new(
                    new[]
                    {
                        new KeyboardButton[] {($"{languageUz.Order}") },
                        new KeyboardButton[] {($"Biz haqimizda") },
                        new KeyboardButton[] {($"{languageUz.Change_language}") },
                        new KeyboardButton[] {($"{languageUz.BackDefault}") },
                    })
                {
                    ResizeKeyboard = true
                };
                return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: languageUz.Hello,
                    replyMarkup: replyMarkupOrder,
                    cancellationToken: cancellationToken);
            }

            // 1
            else if (messageHelper == "buyurtma berish")
            {
                var languageUz = LanguageResponse("uz uzbek");

                string url = "https://ubiquitous-biscotti-83cee4.netlify.app/";


                WebAppInfo webAppInfo = new WebAppInfo();
                webAppInfo.Url = url;

                ReplyKeyboardMarkup replyKeyboardMarkup = new(
                    new[]
                    {
                        KeyboardButton.WithWebApp("Bosh sahifa", webAppInfo),
                        KeyboardButton.WithRequestContact("Telefon raqam"),
                        KeyboardButton.WithRequestLocation("Manzil"),
                    })
                {
                    ResizeKeyboard = true,
                };

                return await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Tugmani bosing!",
                        replyMarkup: replyKeyboardMarkup,
                        cancellationToken: cancellationToken);
            }


            // 2
            else if (messageHelper == "biz haqimizda")
            {
                var languageUz = LanguageResponse("uz uzbek");

                return await botClient.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: languageUz.About_us,
                   cancellationToken: cancellationToken);
            }


            // 3
            else if (messageHelper == "tilni o'zgartirish")
            {
                var languageUz = LanguageResponse("uz uzbek");

                return await botClient.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: languageUz.Choose_language,
                   replyMarkup: replyMarkupBot,
                   cancellationToken: cancellationToken);
            }

            // 4
            else if (messageHelper == "orqaga")
            {
                var languageUz = LanguageResponse("uz uzbek");

                return await botClient.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: languageUz.BackDefault,
                   replyMarkup: replyMarkupBot,
                   cancellationToken: cancellationToken);
            }



            // English 

            else if (messageHelper == "🇱🇷 english")
            {
                var languageEn = LanguageResponse("🇱🇷 english");

                ReplyKeyboardMarkup replyMarkupOrder = new(
                    new[]
                    {
                        new KeyboardButton[] {($"{languageEn.Order}") },
                        new KeyboardButton[] {($"About us") },
                        new KeyboardButton[] {($"{languageEn.Change_language}") },
                        new KeyboardButton[] {($"{languageEn.BackDefault}") },
                    })
                {
                    ResizeKeyboard = true
                };
                return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: languageEn.Hello,
                    replyMarkup: replyMarkupOrder,
                    cancellationToken: cancellationToken);
            }


            // 1
            else if (messageHelper == "order")
            {
                var languageEn = LanguageResponse("🇱🇷 english");

                string url = "https://ubiquitous-biscotti-83cee4.netlify.app/";


                WebAppInfo webAppInfo = new WebAppInfo();
                webAppInfo.Url = url;

                ReplyKeyboardMarkup replyKeyboardMarkup = new(
                    new[]
                    {
                        KeyboardButton.WithWebApp("Menu", webAppInfo),
                        KeyboardButton.WithRequestContact("Contact"),
                        KeyboardButton.WithRequestLocation("Location"),
                    })
                {
                    ResizeKeyboard = true,
                };

                return await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: languageEn.Choose_Button,
                        replyMarkup: replyKeyboardMarkup,
                        cancellationToken: cancellationToken);
            }


            // 2
            else if (messageHelper == "about us")
            {
                var languageEn = LanguageResponse("🇱🇷 english");

                return await botClient.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: languageEn.About_us,
                   cancellationToken: cancellationToken);
            }


            // 3
            else if (messageHelper == "change language")
            {
                var languageEn = LanguageResponse("🇱🇷 english");

                return await botClient.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: languageEn.Choose_language,
                   replyMarkup: replyMarkupBot,
                   cancellationToken: cancellationToken);
            }


            // 4
            else if (messageHelper == "back")
            {
                var languageEn = LanguageResponse("🇱🇷 english");

                return await botClient.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: languageEn.BackDefault,
                   replyMarkup: replyMarkupBot,
                   cancellationToken: cancellationToken);
            }





            // Russian 

            else if (messageHelper == "🇷🇺 russian")
            {
                var languageRu = LanguageResponse("🇷🇺 russian");

                ReplyKeyboardMarkup replyMarkupOrder = new(
                    new[]
                    {
                        new KeyboardButton[] {($"{languageRu.Order}") },
                        new KeyboardButton[] { ($"О нас") },
                        new KeyboardButton[] {($"{languageRu.Change_language}") },
                        new KeyboardButton[] {($"{languageRu.BackDefault}") },
                    })
                {
                    ResizeKeyboard = true
                };

                return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: languageRu.Choose_Button,
                    replyMarkup: replyMarkupOrder,
                    cancellationToken: cancellationToken);
            }


            // 1
            else if (messageHelper == "заказ")
            {
                var languageRu = LanguageResponse("🇷🇺 russian");

                string url = "https://ubiquitous-biscotti-83cee4.netlify.app/";


                WebAppInfo webAppInfo = new WebAppInfo();
                webAppInfo.Url = url;

                ReplyKeyboardMarkup replyKeyboardMarkup1 = new(
                    new[]
                    {
                        KeyboardButton.WithWebApp("Меню", webAppInfo),
                        KeyboardButton.WithRequestContact("Контакт"),
                        KeyboardButton.WithRequestLocation("Расположение"),
                    })
                {
                    ResizeKeyboard = true,
                };

                return await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: languageRu.Choose_Button,
                        replyMarkup: replyKeyboardMarkup1,
                        cancellationToken: cancellationToken);
            }



            // 2
            else if (messageHelper == "о нас")
            {
                var languageRu = LanguageResponse("🇷🇺 russian");

                return await botClient.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: languageRu.About_us,
                   cancellationToken: cancellationToken);
            }



            // 3
            else if (messageHelper == "изменить язык")
            {
                var languageRu = LanguageResponse("🇷🇺 russian");

                return await botClient.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: languageRu.Choose_language,
                   replyMarkup: replyMarkupBot,
                   cancellationToken: cancellationToken);
            }



            // 4
            else if (messageHelper == "назад")
            {
                var languageRu = LanguageResponse("🇷🇺 russian");

                return await botClient.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: languageRu.BackDefault,
                   replyMarkup: replyMarkupBot,
                   cancellationToken: cancellationToken);
            }


            // Default language
            else if (messageHelper == "/start")
            {
                string a = messageHelper + " " + defaultLanguage;
                var languageDef = LanguageResponse(a);

                return await botClient.SendTextMessageAsync(
                              chatId: message.Chat.Id,
                              text: languageDef.Hello,
                              replyMarkup: replyMarkupBot,
                              cancellationToken: cancellationToken);
            }


            else
            {
                return await botClient.SendTextMessageAsync(
                               chatId: message.Chat.Id,
                               text: "Cho'ta nimadir shubxali",
                               replyMarkup: replyMarkupBot,
                               cancellationToken: cancellationToken);
            }


            static LanguageResponseModel LanguageResponse(string languageCode)
            {
                var model = new LanguageResponseModel();

                var langCode = languageCode.ToString();

                if (langCode == "uz uzbek")
                {
                    model.Hello = "Assalomu alaykum";
                    model.Order = "Buyurtma berish";
                    model.Uz = "O'zbek tili";
                    model.En = "Ingliz tili";
                    model.Ru = "Rus tili";
                    model.Change_language = "Tilni o'zgartirish";
                    model.Choose_language = "Tilni tanlamoq";
                    model.Back = "Orqaga";
                    model.LanguageChangeSuccess = "Muffaqiyatli o'zgardi";
                    model.Successfully = "Muffaqiyatli o'zgardi";
                    model.Choose_Button = "Tugmani tanlang";
                    model.BackDefault = "Orqaga";
                    model.About_us = "Menyu\n" +
                                     "Salom\n" +
                                     "Qandaysiz\n" +
                                     "Nima qilyapsan\n";
                    return model;
                }  

                else if (langCode == "🇱🇷 english")
                {
                    model.Hello = "Hello";
                    model.Order = "Order";
                    model.Uz = "Uzbek language";
                    model.En = "English language";
                    model.Ru = "Russian language";
                    model.Change_language = "Change language";
                    model.Choose_language = "Choose language";
                    model.Back = "Back";
                    model.LanguageChangeSuccess = "Successfully changed";
                    model.Successfully = "Successfully changed";
                    model.Choose_Button = "Choose the button";
                    model.BackDefault = "Back";
                    model.About_us = "Menu\n" +
                                     "Hello\n" +
                                     "How are you\n" +
                                     "What are you doing\n";
                    return model;
                }
                else if (langCode == "/start en")
                {
                    model.Hello = "Hello";
                    model.Order = "Order";
                    model.About_us = "About us";
                    model.Uz = "Uzbek language";
                    model.En = "English language";
                    model.Ru = "Russian language";
                    model.Change_language = "Change language";
                    model.Choose_language = "Choose language";
                    model.Back = "Back";
                    model.LanguageChangeSuccess = "Successfully changed";
                    model.BackDefault = "Back";
                    return model;
                }

                else if (langCode == "🇷🇺 russian")
                {
                    model.Hello = "Здравствуйте!";
                    model.Order = "Заказ";
                    model.Uz = "Узбекский язык";
                    model.En = "Aнглийский язык";
                    model.Ru = "Русский язык";
                    model.Change_language = "Изменить язык";
                    model.Choose_language = "Выберите язык";
                    model.Back = "Назад";
                    model.LanguageChangeSuccess = "Успешно изменено";
                    model.Successfully = "Успешно изменено";
                    model.Choose_Button = "Кнопка выбора";
                    model.BackDefault = "Назад";
                    model.About_us = "Меню\n" +
                                     "Привет\n" +
                                     "Как дела\n" +
                                     "Что ты делаешь\n";
                    return model;
                }


                else if (langCode == "/start ru")
                {
                    model.Hello = "Здравствуйте!";
                    model.Order = "Заказ";
                    model.About_us = "О нас";
                    model.Uz = "Узбекский язык";
                    model.En = "Aнглийский язык";
                    model.Ru = "Русский язык";
                    model.Change_language = "Изменить язык";
                    model.Choose_language = "Выберите язык";
                    model.Back = "Назад";
                    model.LanguageChangeSuccess = "Успешно изменено";
                    model.BackDefault = "Назад";
                    return model;
                }
                else
                {
                    return model;
                }
            }
        }

     

       
    }

    // Process Inline Keyboard callback data
    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);
        

        await _botClient.AnswerCallbackQueryAsync(
            callbackQueryId: callbackQuery.Id,
            text: $"Received {callbackQuery.Data}",
            cancellationToken: cancellationToken);


        await _botClient.SendTextMessageAsync(
            chatId: callbackQuery.Message!.Chat.Id,
            text: $"Received {callbackQuery.Data}",
            cancellationToken: cancellationToken);
    }
    
    #region Inline Mode

    //private async Task BotOnInlineQueryReceived(InlineQuery inlineQuery, CancellationToken cancellationToken)
    //{
    //    _logger.LogInformation("Received inline query from: {InlineQueryFromId}", inlineQuery.From.Id);

    //    InlineQueryResult[] results = {
    //        // displayed result
    //        new InlineQueryResultArticle(
    //            id: "1",
    //            title: "TgBots",
    //            inputMessageContent: new InputTextMessageContent("hello"))
    //    };

    //    await _botClient.AnswerInlineQueryAsync(
    //        inlineQueryId: inlineQuery.Id,
    //        results: results,
    //        cacheTime: 0,
    //        isPersonal: true,
    //        cancellationToken: cancellationToken);
    //}

    private async Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received inline result: {ChosenInlineResultId}", chosenInlineResult.ResultId);

        await _botClient.SendTextMessageAsync(
            chatId: chosenInlineResult.From.Id,
            text: $"You chose result with Id: {chosenInlineResult.ResultId}",
            cancellationToken: cancellationToken);
    }

    #endregion

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable RCS1163 // Unused parameter.
    private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
#pragma warning restore RCS1163 // Unused parameter.
#pragma warning restore IDE0060 // Remove unused parameter
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);

        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
}