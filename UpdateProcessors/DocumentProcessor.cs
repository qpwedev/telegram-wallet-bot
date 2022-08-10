using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;

class DocumentProcessor
{

    // If the user sent photo with command /uploaddocument and possibly caption save it to database and send message with link
    public static void Process(ref BotClient bot, Message message)
    {
        if (message == null || message.Caption == null || message.Photo == null)
        {
            return;
        }

        var tokens = message.Caption.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        if (tokens[0] == "/uploaddocument")
        {
            var fileId = message.Photo[0].FileId;
            var caption = string.Join(" ", tokens.Skip(1));

            if (caption.Length >= 200)
            {
                MessageSender.OnExceedsCaptionSymbols(ref bot, message.Chat.Id);
                return;
            }

            var documentHash = HashGenerator.GenerateHash(4);
            Database.InsertDocument(message.Chat.Id, documentHash, fileId, caption);
            MessageSender.OnDocumentUpload(ref bot, message.Chat.Id, documentHash, caption);
        }
        else
        {
            MessageSender.OnUnsuccessfulCommand(ref bot, message.Chat.Id);
        }
    }
}