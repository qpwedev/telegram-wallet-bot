using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;

class MessageSender
{
    public static void OnStartCommand(ref BotClient bot, long chatId)
    {
        bot.SendMessage(chatId, TextGenerator.OnStartText(chatId), ParseMode.HTML, replyMarkup: ReplyMarkup.OnStartMarkup());
    }

    public static void OnSameRecipient(ref BotClient bot, long chatId)
    {
        bot.SendMessage(chatId, Constants.OnSameRecipientText, ParseMode.HTML, replyMarkup: ReplyMarkup.BackMarkup());
    }

    public static void OnAllDocumentDeletion(ref BotClient bot, long chatId)
    {
        bot.SendMessage(chatId, TextGenerator.OnAllDocumentDeletionText(chatId), ParseMode.HTML);
    }

    public static void OnInvalidDocument(ref BotClient bot, long chatId, string documentHash)
    {
        bot.SendMessage(chatId, TextGenerator.OnInvalidDocumentText(documentHash), ParseMode.HTML, replyMarkup: ReplyMarkup.BackMarkup());
    }

    public static void OnDocumentDeletion(ref BotClient bot, long chatId, string documentHash)
    {
        bot.SendMessage(chatId, TextGenerator.OnDocumentDeletionText(documentHash), ParseMode.HTML);
    }

    public static void SendDocument(ref BotClient bot, long chatId, Document document)
    {
        bot.SendPhoto(chatId, document.FileId, document.Caption);
    }

    public static void OnSuccessfulTopup(ref BotClient bot, long chatId, double amount)
    {
        bot.SendMessage(chatId, TextGenerator.OnSuccessfulTopupText(amount), ParseMode.HTML);
    }

    public static void OnSuccessfulSend(ref BotClient bot, long senderId, long receiverId, double amount)
    {
        bot.SendMessage(senderId, TextGenerator.OnSuccessfulSendSenderText(amount, receiverId), ParseMode.HTML);
        bot.SendMessage(receiverId, TextGenerator.OnSuccessfulSendReceiverText(amount, senderId), ParseMode.HTML);
    }

    public static void OnUnsuccessfulCommand(ref BotClient bot, long chatId)
    {
        bot.SendMessage(chatId, Constants.UnsuccessfulCommandText, ParseMode.HTML);
    }

    public static void OnInsufficientFunds(ref BotClient bot, long chatId, double amount)
    {
        bot.SendMessage(chatId, TextGenerator.OnInsufficientFundsText(amount), ParseMode.HTML, replyMarkup: ReplyMarkup.BackMarkup());
    }

    public static void OnInabilityChequeSending(ref BotClient bot, long senderId, long receiverId, double amount)
    {
        bot.SendMessage(receiverId, TextGenerator.OnInabilityChequeSendingText(senderId), ParseMode.HTML, replyMarkup: ReplyMarkup.BackMarkup());
    }

    public static void OnInvalidCheque(ref BotClient bot, long id)
    {
        bot.SendMessage(id, Constants.InvalidChequeText, ParseMode.HTML, replyMarkup: ReplyMarkup.BackMarkup());
    }

    public static void OnNonExistentUser(ref BotClient bot, long senderId, long receiverId)
    {
        bot.SendMessage(senderId, TextGenerator.OnInsufficientFundsText(receiverId), ParseMode.HTML, replyMarkup: ReplyMarkup.BackMarkup());
    }

    public static void OnCreateInvoice(ref BotClient bot, long chatId, double amount)
    {
        bot.SendMessage(chatId, TextGenerator.OnCreateInvoiceText(chatId, amount), ParseMode.HTML, disableWebPagePreview: true);
    }

    public static void OnExceedsDailyLimit(ref BotClient bot, long chatId)
    {
        bot.SendMessage(chatId, Constants.ExceedsDailyLimitText, ParseMode.HTML);
    }

    public static void OnExceedsMonthlyLimit(ref BotClient bot, long chatId)
    {
        bot.SendMessage(chatId, Constants.ExceedsMonthlyLimitText, ParseMode.HTML);
    }

    public static void OnUpdateDailyLimit(ref BotClient bot, long chatId, double dailyLimit)
    {
        bot.SendMessage(chatId, TextGenerator.OnUpdateDailyLimitText(dailyLimit), ParseMode.HTML);
    }

    public static void OnUpdateMonthlyLimit(ref BotClient bot, long chatId, double monthlyLimit)
    {
        bot.SendMessage(chatId, TextGenerator.OnUpdateMonthlyLimitText(monthlyLimit), ParseMode.HTML);
    }

    public static void OnCreateCheque(ref BotClient bot, long chatId, string link, double amount)
    {
        bot.SendMessage(chatId, TextGenerator.OnCreateChequeText(link, amount), ParseMode.HTML, disableWebPagePreview: true);
    }

    public static void OnDocumentUpload(ref BotClient bot, long chatId, string documentHash, string caption)
    {
        bot.SendMessage(chatId, TextGenerator.OnDocumentUploadText(documentHash, caption), ParseMode.HTML, disableWebPagePreview: true);
    }

    public static void OnSendCallback(ref BotClient bot, long chatId)
    {
        bot.SendMessage(chatId, Constants.SendText, ParseMode.HTML, replyMarkup: ReplyMarkup.BackMarkup());
    }
    public static void OnTopupCallback(ref BotClient bot, long chatId)
    {
        bot.SendMessage(chatId, Constants.TopUpText, ParseMode.HTML, replyMarkup: ReplyMarkup.BackMarkup());

    }
    public static void OnChequeReceivementsCallback(ref BotClient bot, long chatId)
    {
        bot.SendMessage(chatId, Constants.ChequesText, ParseMode.HTML, replyMarkup: ReplyMarkup.BackMarkup());

    }
    public static void OnLimitsCallback(ref BotClient bot, long chatId)
    {
        bot.SendMessage(chatId, Constants.LimitsText, ParseMode.HTML, replyMarkup: ReplyMarkup.BackMarkup());
    }
    public static void OnExceedsCaptionSymbols(ref BotClient bot, long chatId)
    {
        bot.SendMessage(chatId, Constants.ExceedsCaptionSymbolsText, ParseMode.HTML);
    }
    public static void OnDocumentsCallback(ref BotClient bot, long chatId, List<Document> documents)
    {
        if (documents.Count == 0)
        {
            bot.SendMessage(chatId, TextGenerator.NoDocumentsText(documents), ParseMode.HTML, replyMarkup: ReplyMarkup.BackMarkup());
            return;
        }
        else
        {
            bot.SendMessage(chatId, TextGenerator.DocumentsTopText(documents), ParseMode.HTML);
        }

        for (int i = 0; i < documents.Count; i += 10)
        {
            var items = documents.Skip(i).Take(10).ToList();
            if (i + 10 >= documents.Count)
            {
                bot.SendMessage(chatId, TextGenerator.DocumentsItemsText(items), ParseMode.HTML, replyMarkup: ReplyMarkup.BackMarkup(), disableWebPagePreview: true);
            }
            else
            {
                bot.SendMessage(chatId, TextGenerator.DocumentsItemsText(items), ParseMode.HTML, disableWebPagePreview: true);
            }
        }
    }
    public static void OnTransactionHistoryCallback(ref BotClient bot, long chatId, List<Transaction> transactions)
    {
        if (transactions.Count == 0)
        {
            bot.SendMessage(chatId, TextGenerator.NoTransactionHistoryText(transactions), ParseMode.HTML, replyMarkup: ReplyMarkup.BackMarkup());
            return;
        }
        else
        {
            bot.SendMessage(chatId, TextGenerator.TransactionHistoryTopText(transactions), ParseMode.HTML);
        }

        for (int i = 0; i < transactions.Count; i += 10)
        {
            var items = transactions.Skip(i).Take(10).ToList();
            if (i + 10 >= transactions.Count)
            {
                bot.SendMessage(chatId, TextGenerator.TransactionHistoryItemsText(items), ParseMode.HTML, replyMarkup: ReplyMarkup.BackMarkup(), disableWebPagePreview: true);
            }
            else
            {
                bot.SendMessage(chatId, TextGenerator.TransactionHistoryItemsText(items), ParseMode.HTML, disableWebPagePreview: true);
            }
        }
    }
    public static void OnDocumentDeletionDeny(ref BotClient bot, long chatId)
    {
        bot.SendMessage(chatId, Constants.DocumentDeletionDenyText, ParseMode.HTML, replyMarkup: ReplyMarkup.BackMarkup());
    }
}