using Telegram.BotAPI;
using Telegram.BotAPI.InlineMode;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;

class InlineProcessor
{
    public static void Process(ref BotClient bot, InlineQuery inlineQuery)
    {
        if (Double.TryParse(inlineQuery.Query, out double amount) && TransactionVerifier.IsValidAmount(amount))
        {
            // Invoice
            var createInvoiceResult = new InlineQueryResultArticle();
            createInvoiceResult.Description = $"Create an invoice to receive {amount}$";
            createInvoiceResult.Title = "Invoice";
            createInvoiceResult.Id = "1";
            createInvoiceResult.InputMessageContent = new InputTextMessageContent(TextGenerator.OnCreateInvoiceInlineText(inlineQuery.From.Id, amount), ParseMode.HTML);

            // Cheque
            var sender = Database.SelectUser(inlineQuery.From.Id);
            var createChequeResult = new InlineQueryResultArticle();

            var cheque = ChequeCreator.Create(inlineQuery.From.Id, amount);
            var chequeLink = TextGenerator.GenerateChequeLink(cheque);
            createChequeResult.Description = $"Create a cheque to send {amount}$";
            createChequeResult.Title = "Send Money";
            createChequeResult.Id = "2";
            createChequeResult.InputMessageContent = new InputTextMessageContent(TextGenerator.OnCreateChequeInlineText(chequeLink, amount), ParseMode.HTML);

            InlineModeExtensions.AnswerInlineQuery(bot, inlineQuery.Id, new InlineQueryResult[] { createChequeResult, createInvoiceResult },
             cacheTime: 0);
        }

        // Inline document request
        else if (inlineQuery.Query == "docs")
        {
            var documents = Database.SelectUserDocuments(inlineQuery.From.Id);

            if (documents.Count == 0)
            {
                return;
            }

            var inlineQueryResults = new List<InlineQueryResult>();
            var queryId = 1;

            foreach (var document in documents)
            {
                var documentQuery = new InlineQueryResultPhoto();
                documentQuery.PhotoUrl = document.FileId;
                documentQuery.ThumbUrl = document.FileId;
                documentQuery.Caption = $"{document.Caption}";
                documentQuery.Title = $"Document {document.Hash}";
                documentQuery.Id = $"{queryId++}";
                inlineQueryResults.Add(documentQuery);
            }

            InlineModeExtensions.AnswerInlineQuery(bot, inlineQuery.Id, inlineQueryResults.ToArray(), cacheTime: 0);
        }
    }
}