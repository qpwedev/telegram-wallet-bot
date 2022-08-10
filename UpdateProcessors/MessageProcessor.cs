using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using System.Text.RegularExpressions;

class MessageProcessor
{
    public static void Process(ref BotClient bot, Message message)
    {
        if (message.Text == null)
        {
            return;
        }

        var tokens = message.Text.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);


        // Check first part of the command and do the appropriate action
        switch (tokens[0])
        {
            case "/start":
                OnStartCommand(ref bot, tokens, message);
                break;
            case "/topup":
                OnTopupCommand(ref bot, tokens, message);
                break;
            case "/send":
                OnSendCommand(ref bot, tokens, message);
                break;
            case "/createinvoice":
                OnCreateInvoiceCommand(ref bot, tokens, message);
                break;
            case "/createcheque":
                OnCreateChequeCommand(ref bot, tokens, message);
                break;
            case "/dailylimit":
                OnDailyLimitCommand(ref bot, tokens, message);
                break;
            case "/monthlylimit":
                OnMonthlyLimitCommand(ref bot, tokens, message);
                break;
            case "/showdocument":
                OnShowDocumentCommand(ref bot, tokens, message);
                break;
            case "/deletedocument":
                OnDeleteDocumentCommand(ref bot, tokens, message);
                break;
            case "/deletealldocuments":
                OnDeleteAllDocumentsCommand(ref bot, tokens, message);
                break;
            default:
                MessageSender.OnUnsuccessfulCommand(ref bot, message.Chat.Id);
                break;
        }
    }


    static void OnInvoice(ref BotClient bot, string[] tokens, Message message)
    {
        var commandData = tokens[1].Split(new string[] { "send" }, StringSplitOptions.RemoveEmptyEntries);

        if (Double.TryParse(commandData[1], out double amount) && TransactionVerifier.IsValidAmount(amount) && long.TryParse(commandData[0], out long receiverId))
        {
            var sender = Database.SelectUser(message.Chat.Id);
            var receiver = Database.SelectUser(receiverId);

            // Telegram start links does not support dots. To avoid it the amount in the link should be increased by 100
            amount = amount * 0.01;

            TransactionSender.Send(ref bot, sender, receiver, amount);
        }
        else
        {
            MessageSender.OnUnsuccessfulCommand(ref bot, message.Chat.Id);
        }
    }

    // If cheque exists use it
    static void OnChequeReceivement(ref BotClient bot, string[] tokens, Message message)
    {
        var chequeHash = tokens[1].Replace("receive", "");
        var cheque = Database.SelectCheque(chequeHash);

        if (cheque.Hash == null)
        {
            MessageSender.OnInvalidCheque(ref bot, message.Chat.Id);
            return;
        }

        var sender = Database.SelectUser(cheque.SenderId);
        var receiver = Database.SelectUser(message.Chat.Id);

        TransactionSender.ActivateCheque(ref bot, sender, receiver, cheque);
    }

    // Retrieve document by ID and send it to user

    static void OnStartDocumentRequest(ref BotClient bot, string[] tokens, Message message)
    {
        var documentHash = tokens[1].Replace("doc", "");
        var document = Database.SelectDocument(documentHash);

        if (document.Hash == null)
        {
            MessageSender.OnInvalidDocument(ref bot, message.Chat.Id, documentHash);
            return;
        }
        MessageSender.SendDocument(ref bot, message.Chat.Id, document);
    }

    static void OnStartCommand(ref BotClient bot, string[] tokens, Message message)
    {
        Database.AddUser(message.Chat.Id, message.Chat.FirstName, message.Chat.Username);

        // Simple /start command
        if (tokens.Length == 1)
        {
            MessageSender.OnStartCommand(ref bot, message.Chat.Id);
        }
        // User entered into the bot via a link t.me/[username]?start= and passed an additional argument
        else if (tokens.Length == 2 && Regex.Match(tokens[1], "[0-9]+send[0-9]+").Success)
        {
            OnInvoice(ref bot, tokens, message);
        }
        else if (tokens.Length == 2 && Regex.Match(tokens[1], "[0-9A-Za-z]+receive").Success)
        {
            OnChequeReceivement(ref bot, tokens, message);
        }
        else if (tokens.Length == 2 && Regex.Match(tokens[1], "[0-9A-Za-z]+doc").Success)
        {
            OnStartDocumentRequest(ref bot, tokens, message);
        }
    }

    static void OnTopupCommand(ref BotClient bot, string[] tokens, Message message)
    {
        if (tokens.Length == 2 && Double.TryParse(tokens[1], out double amount) && TransactionVerifier.IsValidAmount(amount))
        {
            Database.TopUp(message.Chat.Id, amount);
            MessageSender.OnSuccessfulTopup(ref bot, message.Chat.Id, amount);
        }
        else
        {
            MessageSender.OnUnsuccessfulCommand(ref bot, message.Chat.Id);
        }
    }
    static void OnSendCommand(ref BotClient bot, string[] tokens, Message message)
    {
        if (tokens.Length == 3 && Double.TryParse(tokens[1], out double amount) && TransactionVerifier.IsValidAmount(amount) && long.TryParse(tokens[2], out long receiverId))
        {
            var sender = Database.SelectUser(message.Chat.Id);
            var receiver = Database.SelectUser(receiverId);

            TransactionSender.Send(ref bot, sender, receiver, amount);
        }
        else
        {
            MessageSender.OnUnsuccessfulCommand(ref bot, message.Chat.Id);
        }
    }
    static void OnCreateInvoiceCommand(ref BotClient bot, string[] tokens, Message message)
    {
        if (tokens.Length == 2 && Double.TryParse(tokens[1], out double amount) && TransactionVerifier.IsValidAmount(amount))
        {
            MessageSender.OnCreateInvoice(ref bot, message.Chat.Id, amount);
        }
        else
        {
            MessageSender.OnUnsuccessfulCommand(ref bot, message.Chat.Id);
        }
    }
    static void OnCreateChequeCommand(ref BotClient bot, string[] tokens, Message message)
    {
        if (tokens.Length == 2 && Double.TryParse(tokens[1], out double amount) && TransactionVerifier.IsValidAmount(amount))
        {
            var cheque = ChequeCreator.Create(message.Chat.Id, amount);
            var chequeLink = TextGenerator.GenerateChequeLink(cheque);
            MessageSender.OnCreateCheque(ref bot, message.Chat.Id, chequeLink, amount);
        }
        else
        {
            MessageSender.OnUnsuccessfulCommand(ref bot, message.Chat.Id);
        }
    }

    static void OnShowDocumentCommand(ref BotClient bot, string[] tokens, Message message)
    {
        if (tokens.Length == 2)
        {
            var documentHash = tokens[1];
            var document = Database.SelectDocument(documentHash);

            if (document.Hash == null)
            {
                MessageSender.OnInvalidDocument(ref bot, message.Chat.Id, documentHash);
                return;
            }

            MessageSender.SendDocument(ref bot, message.Chat.Id, document);
        }
        else
        {
            MessageSender.OnUnsuccessfulCommand(ref bot, message.Chat.Id);
        }
    }

    static void OnMonthlyLimitCommand(ref BotClient bot, string[] tokens, Message message)
    {
        if (tokens.Length == 1)
        {
            Database.UpdateMonthlyLimit(message.Chat.Id, -1);
            MessageSender.OnUpdateMonthlyLimit(ref bot, message.Chat.Id, -1);
        }
        else if (tokens.Length == 2 && Double.TryParse(tokens[1], out double monthlyLimit) && (TransactionVerifier.IsValidAmount(monthlyLimit) || monthlyLimit == -1))
        {
            Database.UpdateMonthlyLimit(message.Chat.Id, monthlyLimit);
            MessageSender.OnUpdateMonthlyLimit(ref bot, message.Chat.Id, monthlyLimit);
        }
        else
        {
            MessageSender.OnUnsuccessfulCommand(ref bot, message.Chat.Id);
        }
    }

    static void OnDailyLimitCommand(ref BotClient bot, string[] tokens, Message message)
    {
        if (tokens.Length == 1)
        {
            Database.UpdateDailyLimit(message.Chat.Id, -1);
            MessageSender.OnUpdateDailyLimit(ref bot, message.Chat.Id, -1);
        }
        else if (tokens.Length == 2 && Double.TryParse(tokens[1], out double dailyLimit) && (TransactionVerifier.IsValidAmount(dailyLimit)))
        {
            Database.UpdateDailyLimit(message.Chat.Id, dailyLimit);
            MessageSender.OnUpdateDailyLimit(ref bot, message.Chat.Id, dailyLimit);
        }
        else
        {
            MessageSender.OnUnsuccessfulCommand(ref bot, message.Chat.Id);
        }
    }

    static void OnDeleteAllDocumentsCommand(ref BotClient bot, string[] tokens, Message message)
    {
        if (tokens.Length == 1)
        {
            Database.DeleteAllDocuments(message.Chat.Id);
            MessageSender.OnAllDocumentDeletion(ref bot, message.Chat.Id);
        }
        else
        {
            MessageSender.OnUnsuccessfulCommand(ref bot, message.Chat.Id);
        }
    }

    static void OnDeleteDocumentCommand(ref BotClient bot, string[] tokens, Message message)
    {
        if (tokens.Length == 2)
        {
            var documentHash = tokens[1];
            var document = Database.SelectDocument(documentHash);

            if (document.Hash == null)
            {
                MessageSender.OnInvalidDocument(ref bot, message.Chat.Id, documentHash);
                return;
            }
            else if (document.UserId != message.Chat.Id)
            {
                MessageSender.OnDocumentDeletionDeny(ref bot, message.Chat.Id);
                return;
            }

            Database.DeleteDocument(documentHash);

            MessageSender.OnDocumentDeletion(ref bot, message.Chat.Id, documentHash);
        }
        else
        {
            MessageSender.OnUnsuccessfulCommand(ref bot, message.Chat.Id);
        }
    }
}