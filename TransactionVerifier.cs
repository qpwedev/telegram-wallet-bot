enum TransactionStatusCode
{
    Success,
    InsufficientFunds,
    NonExistentUser,
    ExceedsMonthlyLimit,
    ExceedsDailyLimit,
    SameRecipient
}

class TransactionVerifier
{
    public static TransactionStatusCode Verify(User sender, User receiver, double amount)
    {
        if (sender.Id == receiver.Id)
        {
            return TransactionStatusCode.SameRecipient;
        }
        else if (sender.Balance < amount)
        {
            return TransactionStatusCode.InsufficientFunds;
        }
        else if (receiver.Id == User.NonExistentUserId)
        {
            return TransactionStatusCode.NonExistentUser;
        }
        else if (!PassedMonthlyLimit(sender, amount))
        {
            return TransactionStatusCode.ExceedsMonthlyLimit;
        }
        else if (!PassedDailyLimit(sender, amount))
        {
            return TransactionStatusCode.ExceedsDailyLimit;
        }

        return TransactionStatusCode.Success;
    }


    // Check if transactions from the beggining of the month do not exeed the monthly limit
    static bool PassedMonthlyLimit(User sender, double amount)
    {
        var monthlyLimit = Database.SelectMonthlyLimit(sender.Id);
        if (monthlyLimit == -1)
        {
            return true;
        }

        var StartOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var sendedAmountForPeriod = Database.SelectSendedAmountForPeriod(sender.Id, StartOfMonth.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"), DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));

        if (sendedAmountForPeriod + amount > monthlyLimit)
        {
            return false;
        }

        return true;
    }

    // Check if transactions from the beggining of the day do not exeed the daily limit
    static bool PassedDailyLimit(User sender, double amount)
    {
        var dailyLimit = Database.SelectDailyLimit(sender.Id);
        if (dailyLimit == -1)
        {
            return true;
        }

        var StartOfDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        var sendedAmountForPeriod = Database.SelectSendedAmountForPeriod(sender.Id, StartOfDay.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"), DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));

        if (sendedAmountForPeriod + amount > dailyLimit)
        {
            return false;
        }

        return true;
    }
    public static bool IsValidAmount(double amount) => amount >= 0.01 && amount <= 1e9;
}