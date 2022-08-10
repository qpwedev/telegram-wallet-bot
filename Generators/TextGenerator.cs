public static class TextGenerator
{
    public static string OnStartText(long id)
    {
        var user = Database.SelectUser(id);
        return $@"<b>Welcome to the Wallet Bot</b>

User ID (use it to receive money): <code>{id}</code>
Balance: {Math.Round(user.Balance, 2)}$
Daily Limit: {(user.DailyLimit != -1 ? user.DailyLimit + " $" : "No limit")} 
Monthly Limit: {(user.MonthlyLimit != -1 ? user.MonthlyLimit + " $" : "No limit")}";
    }

    public static string OnDocumentDeletionText(string documentHash)
    {
        return $@"<b>Document was deleted 🚫</b>
        
Document ID: <code>{documentHash}</code>";
    }

    public static string OnDocumentUploadText(string documentHash, string caption)
    {
        return $@"<b>Document was saved ✅</b>
        
Document ID: <code>{documentHash}</code>

Use this link to share the document:
t.me/{Constants.BotUsername}?start={documentHash}doc
";
    }

    public static string OnDocumentInlineRequestText(string documentHash, string caption)
    {
        return $@"Document ID: <code>{documentHash}</code>

Use this link to view the document:
t.me/{Constants.BotUsername}?start={documentHash}doc
";
    }

    public static string OnSuccessfulTopupText(double amount)
    {
        return $"+{amount}$ ✅";
    }

    public static string OnSuccessfulSendSenderText(double amount, long receiverId)
    {
        return $"{amount}$ sent to {receiverId} ✅";
    }

    public static string OnSuccessfulSendReceiverText(double amount, long senderId)
    {
        return $"{amount}$ received from {senderId} ✅";
    }

    public static string OnInsufficientFundsText(double amount)
    {
        return $"You don't have enought money to send {amount}$ 🚫";
    }


    public static string OnNonExistentUserText(long receiverId)
    {
        return $"User with ID {receiverId} does not exist 🚫";
    }

    public static string OnCreateInvoiceText(long receiverId, double amount)
    {
        return $@"<b>Share this link to receive {amount}$:</b>

t.me/{Constants.BotUsername}?start={receiverId}send{amount * 100}";
    }

    public static string OnCreateInvoiceInlineText(long receiverId, double amount)
    {
        return $@"<b>Click the link below to send me {amount}$:</b>

t.me/{Constants.BotUsername}?start={receiverId}send{amount * 100}";
    }

    public static string GenerateChequeLink(Cheque cheque)
    {
        return @$"t.me/{Constants.BotUsername}?start={cheque.Hash}receive";
    }

    public static string OnCreateChequeInlineText(string link, double amount)
    {
        return $@"<b>Click the link below to receive {amount}$:</b>

{link}";
    }

    public static string OnCreateChequeText(string link, double amount)
    {
        return $@"<b>Share this link to send {amount}$:</b>

{link}";
    }

    public static string OnUpdateDailyLimitText(double dailyLimit)
    {
        if (dailyLimit == -1)
        {
            return $@"<b>Daily limit has been reset</b>";
        }
        return $@"<b>Daily limit has been changed to: {dailyLimit}$</b>";
    }

    public static string OnUpdateMonthlyLimitText(double monthlyLimit)
    {
        if (monthlyLimit == -1)
        {
            return $@"<b>Monthly limit has been reset</b>";
        }
        return $@"<b>Monthly limit has been changed to: {monthlyLimit}$ </b>";
    }


    public static string NoTransactionHistoryText(List<Transaction> transactions)
    {
        return $@"<b>You don't have any transactions yet</b>";
    }

    public static string TransactionHistoryTopText(List<Transaction> transactions)
    {
        return $@"<b>Transaction history:</b>";
    }

    public static string TransactionHistoryItemsText(List<Transaction> transactions)
    {
        string text = "";

        foreach (var transaction in transactions)
        {
            text += $@"From: {transaction.SenderId} -> To: {transaction.ReceiverId}
{transaction.Amount}$ at {transaction.Datetime}

";
        }
        return text;
    }

    public static string NoDocumentsText(List<Document> documents)
    {
        var text = $@"{Constants.DocumentsText}

";
        text += $@"<b>You don't have any documents yet </b>";

        return text;
    }


    public static string DocumentsTopText(List<Document> documents)
    {
        var text = $@"{Constants.DocumentsText}

";

        text += $@"<b>Documents:</b>";

        return text;
    }

    public static string DocumentsItemsText(List<Document> documents)
    {
        string text = "";

        foreach (var document in documents)
        {
            text += $@"

Document ID: <code>{document.Hash}</code>
Link: t.me/{Constants.BotUsername}?start={document.Hash}doc";

            if (document.Caption != "")
            {
                text += $@"
Caption: <code>{document.Caption}</code>";
            }
        }
        return text;
    }


    public static string OnInabilityChequeSendingText(long senderId)
    {
        return $"Funds cannot be debited from the user's ({senderId}) balance 🚫";
    }

    public static string OnInvalidDocumentText(string documentHash)
    {
        return $@"<b>Document was deleted or does not exist 🚫</b>
        
Document ID: <code>{documentHash}</code>";
    }

    public static string OnAllDocumentDeletionText(long id)
    {
        return $@"<b>All documents of user ({id}) have been deleted 🚫</b>";
    }
}
