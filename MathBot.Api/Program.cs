#region configurationBuider
using AutoMapper;
using MathBot.Api.Controllers;
using MathBot.BusinessLogic.Implementations;
using MathBot.BusinessLogic.Interfaces;
using MathBot.Common.Heplers;
using MathBot.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;

var builder = new ConfigurationBuilder();
BuildConfig(builder);
builder.SetBasePath(Directory.GetCurrentDirectory());
builder.AddJsonFile("appsettings.json");
var config = builder.Build();
string connection = config.GetConnectionString("DefaultConnection");

var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
IMapper mapper = mappingConfig.CreateMapper();

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services.AddTransient<IUsersService, UsersService>();
        services.AddTransient<ITestsService, TestsService>();
        services.AddTransient<IExercisesService, ExercisesService>();
        services.AddTransient<INumbersService, NumbersService>();
        services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
        services.AddSingleton(mapper);
    })
    .Build();

var _usersService = ActivatorUtilities.CreateInstance<UsersService>(host.Services);
var _exercisesService = ActivatorUtilities.CreateInstance<ExercisesService>(host.Services);
var _numbersService = ActivatorUtilities.CreateInstance<NumbersService>(host.Services);
var _testsService = ActivatorUtilities.CreateInstance<TestsService>(host.Services);
#endregion

static void BuildConfig(IConfigurationBuilder builder)
{
    builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsetings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIROMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables();
}

var botController = new BotController(_usersService, _testsService, _exercisesService, _numbersService);

var bot = new TelegramBotClient(token: "5563379378:AAGzAMFdQKLO9m0zxI16QDnkp_nOceQju9Q");
using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { },
};

bot.StartReceiving(
    botController.HandleUpdateAsync,
    botController.HandleErrorAsync,
    receiverOptions,
    cancellationToken: cts.Token);

var me = await bot.GetMeAsync();
Console.WriteLine($"Start @{me.Username}");
Console.ReadLine();

cts.Cancel();