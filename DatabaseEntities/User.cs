public struct User
{
    public static readonly int NonExistentUserId = -1;
    public long Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public double Balance { get; set; }
    public double DailyLimit { get; set; }
    public double MonthlyLimit { get; set; }

    public User(long id = -1, string name = "", string username = "", double balance = 0, double dailyLimit = 0, double monthlyLimit = 0)
    {
        Id = id;
        Name = name;
        Username = username;
        Balance = balance;
        DailyLimit = dailyLimit;
        MonthlyLimit = monthlyLimit;
    }
}