class HashGenerator
{
    private static Random random = new Random();

    // Generates unique hash for use in the database
    public static string GenerateHash(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return @$"{new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray())}{DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
    }
}