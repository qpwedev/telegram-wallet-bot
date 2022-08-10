public struct Transaction
{
    public long SenderId { get; set; }
    public long ReceiverId { get; set; }
    public double Amount { get; set; }
    public string Datetime { get; set; }

    public Transaction(long senderId, long receiverId, double amount, string datetime)
    {

        SenderId = senderId;
        ReceiverId = receiverId;
        Amount = amount;
        Datetime = datetime;
    }
}