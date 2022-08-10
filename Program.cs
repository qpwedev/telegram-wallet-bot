using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace TelegramWalletBot
{
    class Program
    {
        static void Main()
        {
            Database.Initialization();
            var bot = new BotClient(Constants.BotToken);
            Polling(ref bot);
        }

        static void Polling(ref BotClient bot)
        {
            var updates = bot.GetUpdates();

            // Polling Loop
            while (true)
            {
                if (updates.Length > 0)
                {
                    foreach (var update in updates)
                    {

                        if (update is null)
                        {
                            break;
                        }

                        switch (update.Type)
                        {
                            // Inline Queries
                            case UpdateType.InlineQuery:
                                InlineProcessor.Process(ref bot, update.InlineQuery);
                                break;

                            // Messages and docs
                            case UpdateType.Message:
                                if (update.Message.Photo != null)
                                {
                                    DocumentProcessor.Process(ref bot, update.Message);
                                }
                                else
                                {
                                    MessageProcessor.Process(ref bot, update.Message);
                                }

                                break;

                            // Callbacks
                            case UpdateType.CallbackQuery:
                                CallbackProcessor.Process(ref bot, update.CallbackQuery);
                                break;
                        }
                    }

                    // Get updates from the next update
                    updates = bot.GetUpdates(offset: updates.Max(u => u.UpdateId) + 1);
                }
                else
                {
                    updates = bot.GetUpdates();
                }
            }
        }
    }
}