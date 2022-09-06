using MathBot.BusinessLogic.Implementations;
using MathBot.BusinessLogic.Interfaces;
using MathBot.Common.DTO;
using MathBot.Model.DatabaseModels;
using MathBot.Model.Enum;
using System.Collections.Generic;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

namespace MathBot.Api.Controllers
{
    public class BotController
    {
        private readonly IUsersService _usersService;
        private readonly ITestsService _testsService;
        private readonly IExercisesService _exercisesService;
        private readonly INumbersService _numbersService;

        public BotController(IUsersService usersService, ITestsService testsService, IExercisesService exercisesService, INumbersService numbersService)
        {
            _usersService = usersService;
            _testsService = testsService;
            _exercisesService = exercisesService;
            _numbersService = numbersService;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandleMessage(bot, update.Message);
                return;
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallbackQuery(bot, update.CallbackQuery);
            }
        }

        public async Task HandleMessage(ITelegramBotClient bot, Message message)
        {
            Console.WriteLine($"{message.Chat.Id} {message.Chat.FirstName} Message type: {message.Type}");
            if (message.Text is not { } messageText)
                return;

            if (message.Text.ToLower() == "/start")
            {
                await Register(bot, message);
            }

            if (message.Text.ToLower() == "/help")
            {
                await Help(bot, message);
            }

            if (message.Text.ToLower() == "/register")
            {
                await Register(bot, message);
            }

            if (message.Text.ToLower() == "/start_test" || message.Text == "Запустить тест")
            {
                await ChooseType(bot, message);
            }

            if (message.Text == "Пробный тест 🎲")
            {
                await StartBetaTest(message, bot);
            }

            if (message.Text == "Начать тест с подсчетом очков⚔")
            {
                await StartProductionTest(message, bot);
            }

            if (message.Text == "Предварительно закончить тест 🤚")
            {
                await StopTest(message, bot);
            }

            if (message.Type == MessageType.Contact && message.Contact != null)
            {
                //save the number
                Console.WriteLine($"Phone number: {message.Contact.PhoneNumber}");
            }

            //await Help(bot, message);
        }

        static async Task<Message> Hello(ITelegramBotClient bot, Message message)
        {
           return await bot.SendTextMessageAsync(message.Chat.Id, $"Приветствую, {message.Chat.FirstName} 💜");
        }

        static async Task<Message> Help(ITelegramBotClient botClient, Message message)
        {
            const string functionsString = "/start_test - начать тест 👨‍🎓\n/register - зарегистрироваться 🕵️‍♀️";

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: functionsString);
        }

        public async Task<Message> StartTestAgain(ITelegramBotClient bot, Message message)
        {
            ReplyKeyboardMarkup TestKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Запустить тест" },
            })
            {
                ResizeKeyboard = true
            };

            return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                    text: "Запустить тест еще раз?",
                                                                    replyMarkup: TestKeyboardMarkup);
        }

        public async Task<Message> ChooseType(ITelegramBotClient bot, Message message)
        {
            ReplyKeyboardMarkup ChooseTypeKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Пробный тест 🎲", "Начать тест с подсчетом очков⚔" },
            })
            {
                ResizeKeyboard = true
            };

            if (_usersService.GetByTelegramId(message.Chat.Id) != null)
            {
                if (!_testsService.IsRunningTest(message.Chat.Id) && !_testsService.IsRunningProduction(message.Chat.Id) || _testsService.TestIsEnd(message.Chat.Id))
                {
                    await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Необходимо выбрать второе по величине число.\nПример: у нас есть числа 1, 3, 8, 4.\nСамое большое среди чисел 8. Перед ним идет 4. Ответ: 4.\nТеперь попробуй сам. Удачи 😎");
                    //await Task.Delay(2000);
                    return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                        text: "Солнышко, выбирай режим 📚",
                                                                        replyMarkup: ChooseTypeKeyboardMarkup);
                }
                else return await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Тест уже запущен");
            }
            else return await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: "Сначала зарегистрируйтесь /register");
        }


        public async Task<Message> StopTestKeyboard(ITelegramBotClient bot, Message message)
        {
            ReplyKeyboardMarkup StopTestKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Предварительно закончить тест 🤚" },
            })
            {
                ResizeKeyboard = true
            };
            
            return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                   text: "Если желаете раньше закончить тест, нажмите на кнопку",
                                                                   replyMarkup: StopTestKeyboardMarkup);
        }

        public async Task StartBetaTest(Message message, ITelegramBotClient bot)
        {
            //if (!_testsService.IsRunningTest(message.Chat.Id))
            if (_usersService.GetByTelegramId(message.Chat.Id) != null)
            {
                if (!_testsService.IsRunningTest(message.Chat.Id) && !_testsService.IsRunningProduction(message.Chat.Id) || _testsService.TestIsEnd(message.Chat.Id))
                {
                    DateTime startTime = DateTime.UtcNow;
                    TestType type = TestType.Test;
                    Test test = _testsService.Create(message.Chat.Id, startTime, type); //Create(int testId, DateTime startTime, TestType type)//chatId need???
                    _usersService.AddTest(message.Chat.Id, test);

                    await StopTestKeyboard(bot, message);

                   // await CheckTimeForTest(message, bot);


                    await CallBetaTest(message, bot);
                }
                else bot.SendTextMessageAsync(chatId: message.Chat.Id, text: "Солнышко, тест уже запущен 😑");
            }

            return;
        }

        public async Task StopTest(Message message, ITelegramBotClient bot)
        {
            _testsService.StopTest(message.Chat.Id);
            await StartTestAgain(bot, message);

        }

        public async Task CallBetaTest(Message message, ITelegramBotClient bot)
        {
            //await bot.DeleteMessageAsync(chatId: message.Chat.Id, messageId: (message.MessageId - 1)); //////&&&&&&&&&&&&&&&

            var test = _testsService.GetLastTest(message.Chat.Id);
            if (DateTime.UtcNow <= test.StartTime.AddSeconds(20) || _testsService.TestIsEnd(message.Chat.Id))
            {
                _testsService.AddCounts(test);

                //Console.WriteLine("!!!!!!!!!!!!New iteration " + test.StartTime.ToString() + " end at " + test.StartTime.AddSeconds(20) + " now: " + DateTime.UtcNow);

                var exercise = _exercisesService.Create(test.Id);
                _testsService.AddExercise(test, exercise);
                List<Number> numbers = exercise.Numbers;

                List<int> numbersValue = new();
                foreach (var number in numbers)
                {
                    numbersValue.Add(number.Value);
                }

                if(test.Count > 2) await bot.DeleteMessageAsync(chatId: message.Chat.Id, messageId: (message.MessageId-1)); //////

                await GetTestInlineKeyboard(message, bot, numbersValue);
            }
            else
            {
                await bot.DeleteMessageAsync(chatId: message.Chat.Id, messageId: (message.MessageId - 1)); ///

                await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Пробный тест закончен");
               
                await StartTestAgain(bot, message);
            } 
            //await bot.DeleteMessageAsync(chatId: message.Chat.Id, messageId: (message.MessageId-1)); //////

            return;
        }

        public async Task StartProductionTest(Message message, ITelegramBotClient bot)
        {
            
            if (_usersService.GetByTelegramId(message.Chat.Id) != null)
            {
                if (!_testsService.IsRunningTest(message.Chat.Id) && !_testsService.IsRunningProduction(message.Chat.Id) || _testsService.TestIsEnd(message.Chat.Id))
                {
                    DateTime startTime = DateTime.UtcNow;
                    TestType type = TestType.Production;
                    Test test = _testsService.Create(message.Chat.Id, startTime, type); //Create(int testId, DateTime startTime, TestType type)//chatId need???
                    _usersService.AddTest(message.Chat.Id, test);

                    await StopTestKeyboard(bot, message);

                    await CallProductionTest(message, bot);
                }
                else bot.SendTextMessageAsync(chatId: message.Chat.Id, text: "Солнце, тест уже запущен 😑");
            }
            
            return;
        }

        
        public async Task CallProductionTest(Message message, ITelegramBotClient bot)
        {
            var test = _testsService.GetLastTest(message.Chat.Id);
            if (DateTime.UtcNow <= test.StartTime.AddMinutes(1) || _testsService.TestIsEnd(message.Chat.Id))
            {
                _testsService.AddCounts(test);

                var exercise = _exercisesService.Create(test.Id);
                _testsService.AddExercise(test, exercise);
                List<Number> numbers = exercise.Numbers;

                List<int> numbersValue = new();
                foreach (var number in numbers)
                {
                    numbersValue.Add(number.Value);
                }

                if (test.Count > 2) await bot.DeleteMessageAsync(chatId: message.Chat.Id, messageId: (message.MessageId - 1)); //////

                await GetTestInlineKeyboard(message, bot, numbersValue);
            }
            else
            {
                await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Результаты теста: {test.RightAnswers} из {test.Count}");
                await bot.DeleteMessageAsync(chatId: message.Chat.Id, messageId: (message.MessageId - 1)); //////

                await StartTestAgain(bot, message);
            }
            return;
            //await Help(bot, message);
        }

        public async Task GetTestInlineKeyboard(Message message, ITelegramBotClient bot, List<int> numbers)
        {
            InlineKeyboardMarkup inlineKeyboardNumbers = new(new[]
            {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: numbers[0].ToString(), callbackData: "number0"),
                            InlineKeyboardButton.WithCallbackData(text: numbers[1].ToString(), callbackData: "number1"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: numbers[2].ToString(), callbackData: "number2"),
                            InlineKeyboardButton.WithCallbackData(text: numbers[3].ToString(), callbackData: "number3"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: numbers[4].ToString(), callbackData: "number4"),
                            InlineKeyboardButton.WithCallbackData(text: numbers[5].ToString(), callbackData: "number5"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: numbers[6].ToString(), callbackData: "number6"),
                            InlineKeyboardButton.WithCallbackData(text: numbers[7].ToString(), callbackData: "number7"),
                        },
            });

            await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                text: "Выберите второе наибольшее число",
                                                                replyMarkup: inlineKeyboardNumbers);
        }

        public async Task CheckTimeForTest(Message message, ITelegramBotClient bot)
        {
            var test = _testsService.GetLastTest(message.Chat.Id);
            while (true)
            { 

            if(test.Type == TestType.Test)
            {
                if (!(DateTime.UtcNow <= test.StartTime.AddSeconds(20) || !_testsService.TestIsEnd(message.Chat.Id))) _testsService.StopTest(message.Chat.Id);
            }

            if (test.Type == TestType.Test)
            {
                if (!(DateTime.UtcNow <= test.StartTime.AddMinutes(1) || !_testsService.TestIsEnd(message.Chat.Id))) _testsService.StopTest(message.Chat.Id);
            }}
        }


        public async Task CheckAnswer(ITelegramBotClient bot, long id) //, int index
        {

            var test = _testsService.GetLastTest(id);
            Exercise exercise = _exercisesService.GetLastExercise(test.Id);
            int rightAnswer = _exercisesService.Result(exercise.Numbers);
            //int userAnswer = _exercisesService.NumberById(exercise.Numbers, index);
            int userAnswer = _exercisesService.GetUserAnswer(exercise);

            if (userAnswer == rightAnswer)
            {
                _testsService.AddRightAnswer(test);
                await bot.SendTextMessageAsync(id, $"Верно! Правильный ответ: {rightAnswer}");

            }
            else await bot.SendTextMessageAsync(id, $"Вы ошиблись. Правильный ответ: {rightAnswer}");

        }

        public async Task Register(ITelegramBotClient bot, Message message)
        {
            await Hello(bot, message);
            if (_usersService.GetByTelegramId(message.Chat.Id) == null)
            {
                try
                {
                    RegisterUserDTO newUser = new RegisterUserDTO
                    {
                        Username = message.Chat.FirstName,
                        ChatId = message.Chat.Id,
                        DateRegistration = DateTime.UtcNow,
                    };

                    _usersService.Create(newUser);

                    await bot.SendTextMessageAsync(message.Chat.Id, "Пользователь добавлен успешно.");
                }
                catch
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "Произошла ошибка.");
                }
            }
            await Help(bot, message);
            return;
        }

        async Task HandleCallbackQuery(ITelegramBotClient bot, CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data.StartsWith("number0"))
            {
                int i = 0;
                await GetTestCallbackQuery(bot, callbackQuery.Message, i);

                return;
            }
            if (callbackQuery.Data.StartsWith("number1"))
            {
                int i = 1;
                await GetTestCallbackQuery(bot, callbackQuery.Message, i);
                return;
            }
            if (callbackQuery.Data.StartsWith("number2"))
            {
                int i = 2;
                await GetTestCallbackQuery(bot, callbackQuery.Message, i);
                return;
            }
            if (callbackQuery.Data.StartsWith("number3"))
            {
                int i = 3;
                await GetTestCallbackQuery(bot, callbackQuery.Message, i);
                return;
            }
            if (callbackQuery.Data.StartsWith("number4"))
            {
                int i = 4;
                await GetTestCallbackQuery(bot, callbackQuery.Message, i);
                return;
            }
            if (callbackQuery.Data.StartsWith("number5"))
            {
                int i = 5;
                await GetTestCallbackQuery(bot, callbackQuery.Message, i);
                return;
            }
            if (callbackQuery.Data.StartsWith("number6"))
            {
                int i = 6;
                await GetTestCallbackQuery(bot, callbackQuery.Message, i);
                return;
            }
            if (callbackQuery.Data.StartsWith("number7"))
            {
                int i = 7;
                await GetTestCallbackQuery(bot, callbackQuery.Message, i);
                return;
            }
        }
        
        public async Task GetTestCallbackQuery(ITelegramBotClient bot, Message message, int i)
        {
            var test = _testsService.GetLastTest(message.Chat.Id);
            Exercise exercise = _exercisesService.GetLastExercise(test.Id);
            _exercisesService.PutUserAnswer(exercise, i);
            await CheckAnswer(bot, message.Chat.Id);

            await bot.DeleteMessageAsync(chatId: message.Chat.Id, messageId: message.MessageId); /////
            //await bot.DeleteMessageAsync(chatId: message.Chat.Id, messageId: message.MessageId-1);

            if (test.Type == TestType.Test) await CallBetaTest(message, bot);
            if (test.Type == TestType.Production) await CallProductionTest(message, bot);

            return;
        }
        async Task ResultReaction(ITelegramBotClient bot, Message message, int Score, int Count)
        {
            await Task.Delay(2000);
            double Percents = Math.Round((Convert.ToDouble(Score) / Convert.ToDouble(Count)), 2) * 100;
            Console.WriteLine($"{message.Chat.Id} {message.Chat.FirstName} {DateTime.Now} Tест закончен. \nРезультаты: {Score} из {Count}\nПравильность ответов: {Percents}%");
            await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                text: $"Tест закончен. \nВаши результаты: {Score} из {Count}\nПравильность ответов: {Percents}%");

            if (Percents == 100)
                await bot.SendStickerAsync(chatId: message.Chat.Id, sticker: "https://cdn.tlgrm.app/stickers/ccd/a8d/ccda8d5d-d492-4393-8bb7-e33f77c24907/192/1.webp");
            if (Percents < 100 && Percents >= 70)
                await bot.SendStickerAsync(chatId: message.Chat.Id, sticker: "https://tlgrm.ru/_/stickers/ccd/a8d/ccda8d5d-d492-4393-8bb7-e33f77c24907/192/22.webp");
            if (Percents < 70 && Percents >= 50)
                await bot.SendStickerAsync(chatId: message.Chat.Id, sticker: "https://tlgrm.ru/_/stickers/ccd/a8d/ccda8d5d-d492-4393-8bb7-e33f77c24907/192/21.webp");
            if (Percents < 50 && Percents >= 25)
                await bot.SendStickerAsync(chatId: message.Chat.Id, sticker: "https://tlgrm.ru/_/stickers/ccd/a8d/ccda8d5d-d492-4393-8bb7-e33f77c24907/192/19.webp");
            if (Percents < 25 && Percents >= 0)
                await bot.SendStickerAsync(chatId: message.Chat.Id, sticker: "https://tlgrm.ru/_/stickers/ccd/a8d/ccda8d5d-d492-4393-8bb7-e33f77c24907/192/23.webp");
        }

        public Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Ошибка Telegram API:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        /*static async Task<Message> RequestContact(ITelegramBotClient bot, Message message)
        {
            ReplyKeyboardMarkup requestReplyKeyboard = new(
                new[]
                {
                    KeyboardButton.WithRequestContact("Поделиться моим номером телефона"),
                });
            return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "Твой номер?💙",
                                                        replyMarkup: requestReplyKeyboard);
        }*/
    }
}
