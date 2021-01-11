using Microsoft.Extensions.Configuration;
using Telegram.Bot;

namespace TGGoogleBot.Services
{
    public class TelegramBorClientProvider
    {
        public TelegramBotClient TelegramBotClient { get; set; }
        
        public TelegramBorClientProvider(IConfiguration configuration)
        {
            TelegramBotClient = new TelegramBotClient(configuration.GetSection("Telegram")["BotToken"]);
        }
    }
}