using Telegram.BotAPI.AvailableTypes;

public static class ReplyMarkup
{
    // Markup for the main menu with buttons that will send callback query
    public static InlineKeyboardMarkup OnStartMarkup()
    {
        var replyMarkup = new InlineKeyboardMarkup
        {
            InlineKeyboard = new InlineKeyboardButton[][]{
                                        new InlineKeyboardButton[]{
                                            InlineKeyboardButton.SetCallbackData("Send 🛫", "send"), InlineKeyboardButton.SetCallbackData("Top Up 💰", "topup")
                                            },

                                        new InlineKeyboardButton[]{
                                            InlineKeyboardButton.SetCallbackData("Cheques 💳", "cheques"), InlineKeyboardButton.SetCallbackData("Documents 💰", "documents")
                                            },

                                        new InlineKeyboardButton[]{
                                            InlineKeyboardButton.SetCallbackData("Limits 📊", "limits"),
                                            InlineKeyboardButton.SetCallbackData("History 🗒", "history"),
                                            }
                                        }
        };

        return replyMarkup;
    }

    // Markup that will will send user back to the main menu
    public static InlineKeyboardMarkup BackMarkup()
    {
        var replyMarkup = new InlineKeyboardMarkup
        {
            InlineKeyboard = new InlineKeyboardButton[][]{
                                        new InlineKeyboardButton[]{
                                            InlineKeyboardButton.SetCallbackData("Back 🔙", "back")
                                            }
                                        }
        };

        return replyMarkup;
    }
}
