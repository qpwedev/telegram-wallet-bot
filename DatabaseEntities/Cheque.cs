public struct Cheque
{
    public string Hash { get; set; }
    public double Amount { get; set; }
    public long SenderId { get; set; }

    public Cheque(string hash, long senderId, double amount)
    {
        Hash = hash;
        Amount = amount;
        SenderId = senderId;
    }
}