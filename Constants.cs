public static class Constants
{

    public static string BotToken => "5449619987:AAE_q8FQXdyacRQVb9t9Xvo1irlw4D6B5i4";
    public static string DBName => "walletBot.db";
    public static string BotUsername => "TeleWalleterBot";
    public static string TopUpText => @"<b>Top Up</b> 

<code>/topup</code> [AMOUNT] 

Minimum: 0.01$
Maximum: 1,000,000,000$";
    public static string SendText => @"<b>Send</b> 

<code>/send</code> [AMOUNT] [USERID]

Minimum: 0.01$
Maximum: 1,000,000,000$";
    public static string ChequesText => @"<b>Create cheques and invoices</b> 

<code>/createcheque</code> [AMOUNT]
<code>/createinvoice</code> [AMOUNT]

Minimum: 0.01$
Maximum: 1,000,000,000$";
    public static string LimitsText => @"<b>Change limits</b> 

<code>/dailylimit</code> [AMOUNT]
<code>/monthlylimit</code> [AMOUNT]

Minimum: 0.01$
Maximum: 1,000,000,000$
To reset omit the amout";
    public static string DocumentsText => @"<b>Documents</b>
        
<code>/uploaddocument</code> [CAPTION] + Attached photo
<code>/showdocument</code> [DOCUMENTID]
<code>/deletedocument</code> [DOCUMENTID]
<code>/deletealldocuments</code>";
    public static string UnsuccessfulCommandText => $"Try again 🚫";
    public static string ExceedsMonthlyLimitText => $@"<b>Monthly limit has been exceeded 🚫 </b>";
    public static string ExceedsDailyLimitText => $@"<b>Daily limit has been exceeded 🚫 </b>";
    public static string InvalidChequeText => $"Invalid or used cheque 🚫";
    public static string DocumentDeletionDenyText => $"You do not have rights to delete this document 🚫";
    public static string OnSameRecipientText => $"Sender and receiver must be different 🚫";
    public static string ExceedsCaptionSymbolsText => $"Caption is too long (max. 200) 🚫";
}