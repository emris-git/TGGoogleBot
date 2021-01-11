using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Calendar.v3;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;
using TGGoogleBot.Services;

namespace TGGoogleBot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private static CalendarService CalendarService { get; set; }
        private static TelegramBotClient TelegramBotClient { get; set; }

        public Worker( ILogger<Worker> logger, CalendarServiceProvider calendarServiceProvider, TelegramBorClientProvider telegramBorClientProvider)
        {
            _logger = logger;
            CalendarService = calendarServiceProvider.CalendarService;
            TelegramBotClient = telegramBorClientProvider.TelegramBotClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var cts = new CancellationTokenSource();

            TelegramBotClient.OnMessage += Bot_OnMessage;
            TelegramBotClient.StartReceiving();
        }
        
        static async void Bot_OnMessage(object sender, MessageEventArgs e) {
            if (!string.IsNullOrWhiteSpace(e.Message.Text))
            {

                if (e.Message.Text.Contains("Расписание"))
                {
                    await foreach (var task in GetCalendarTasksOnToday())
                    {
                        await TelegramBotClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text:   $"{task}\n"
                        );
                    }
                }
            }
        }
        
        public static async IAsyncEnumerable<string> GetCalendarTasksOnToday()
        {
            var listRequest = CalendarService.Events.List("primary");
            listRequest.TimeMin =
                DateTime.Parse($"{DateTime.Now.Day}/{DateTime.Now.Month}/{DateTime.Now.Year} 00:00:00");
            listRequest.TimeMax = DateTime.Parse($"{DateTime.Now.Day}/{DateTime.Now.Month}/{DateTime.Now.Year} 23:59:59");
            listRequest.ShowDeleted = false;
            listRequest.SingleEvents = true;
            listRequest.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var events = await listRequest.ExecuteAsync();
            Console.WriteLine("Upcoming events:");
            
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    var when = eventItem.Start.DateTime.ToString();
                    if (string.IsNullOrEmpty(when))
                    {
                        when = eventItem.Start.Date;
                    }

                    Console.WriteLine("{0} ({1})", eventItem.Summary, when);
                    yield return $"{eventItem.Summary} ({when})";
                }
            }
            else
            {
                Console.WriteLine("No upcoming events found.");
            }
        }
    }
}