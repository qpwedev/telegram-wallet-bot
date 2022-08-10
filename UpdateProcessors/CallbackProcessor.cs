using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.UpdatingMessages;

class CallbackProcessor
{
    public static void Process(ref BotClient bot, CallbackQuery query)
    {

        try { bot.DeleteMessage(query.Message.Chat.Id, query.Message.MessageId); }
        catch (Telegram.BotAPI.BotRequestException) { }

        if (query.Data == "back")
        {
            MessageSender.OnStartCommand(ref bot, query.Message.Chat.Id);
        }
        else if (query.Data == "send")
        {
            MessageSender.OnSendCallback(ref bot, query.Message.Chat.Id);
        }
        else if (query.Data == "topup")
        {
            MessageSender.OnTopupCallback(ref bot, query.Message.Chat.Id);
        }
        else if (query.Data == "cheques")
        {
            MessageSender.OnChequeReceivementsCallback(ref bot, query.Message.Chat.Id);
        }
        else if (query.Data == "documents")
        {
            var documents = Database.SelectUserDocuments(query.Message.Chat.Id);
            MessageSender.OnDocumentsCallback(ref bot, query.Message.Chat.Id, documents);
        }
        else if (query.Data == "limits")
        {
            MessageSender.OnLimitsCallback(ref bot, query.Message.Chat.Id);
        }
        else if (query.Data == "history")
        {
            var transactions = Database.SelectTransactionHistory(query.Message.Chat.Id);
            MessageSender.OnTransactionHistoryCallback(ref bot, query.Message.Chat.Id, transactions);
        }
    }
}