using Telegram.BotAPI;

class TransactionSender
{

    // If transaction is valid send it and notify users else send an error to user
    public static void Send(ref BotClient bot, User sender, User receiver, double amount)
    {
        switch (TransactionVerifier.Verify(sender, receiver, amount))
        {
            case TransactionStatusCode.SameRecipient:
                MessageSender.OnSameRecipient(ref bot, sender.Id);
                break;
            case TransactionStatusCode.InsufficientFunds:
                MessageSender.OnInsufficientFunds(ref bot, sender.Id, amount);
                break;
            case TransactionStatusCode.NonExistentUser:
                MessageSender.OnNonExistentUser(ref bot, sender.Id, receiver.Id);
                break;
            case TransactionStatusCode.ExceedsMonthlyLimit:
                MessageSender.OnExceedsMonthlyLimit(ref bot, sender.Id);
                break;
            case TransactionStatusCode.ExceedsDailyLimit:
                MessageSender.OnExceedsDailyLimit(ref bot, sender.Id);
                break;
            case TransactionStatusCode.Success:
                Database.SendTransaction(sender, receiver, amount);
                MessageSender.OnSuccessfulSend(ref bot, sender.Id, receiver.Id, amount);
                break;
        }
    }

    // Send money from the cheque owner if it is possible
    public static void ActivateCheque(ref BotClient bot, User sender, User receiver, Cheque cheque)
    {
        switch (TransactionVerifier.Verify(sender, receiver, cheque.Amount))
        {
            case TransactionStatusCode.SameRecipient:
                MessageSender.OnSameRecipient(ref bot, sender.Id);
                break;
            case TransactionStatusCode.InsufficientFunds:
            case TransactionStatusCode.ExceedsMonthlyLimit:
            case TransactionStatusCode.ExceedsDailyLimit:
                MessageSender.OnInabilityChequeSending(ref bot, sender.Id, receiver.Id, cheque.Amount);
                break;
            case TransactionStatusCode.Success:
                Database.SendTransaction(sender, receiver, cheque.Amount);
                Database.DeleteCheque(cheque.Hash);
                MessageSender.OnSuccessfulSend(ref bot, sender.Id, receiver.Id, cheque.Amount);
                break;
        }
    }
}