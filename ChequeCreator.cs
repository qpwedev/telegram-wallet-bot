class ChequeCreator
{
    // Creates hash, write it into database and return cheque object
    public static Cheque Create(long id, double amount)
    {
        var chequeHash = HashGenerator.GenerateHash(12);
        Database.InsertCheque(id, chequeHash, amount);
        return new Cheque(chequeHash, id, amount);
    }
}
