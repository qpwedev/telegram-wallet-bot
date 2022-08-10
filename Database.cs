
using Microsoft.Data.Sqlite;
public static class Database
{
    public static void Initialization()
    {
        Database.CreateChequesTable();
        Database.CreateDocumentsTable();
        Database.CreateTransactionsTable();
        Database.CreateUserTable();
    }
    public static bool CreateUserTable()
    {
        try
        {
            using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                        @"CREATE TABLE users (id INTEGER PRIMARY KEY, name TEXT, username TEXT, balance REAL, dailyLimit REAL, monthlyLimit REAL);";
                command.ExecuteNonQuery();
            }
        }
        catch (Microsoft.Data.Sqlite.SqliteException)
        {
            return false;
        }

        return true;
    }

    public static bool CreateTransactionsTable()
    {
        try
        {
            using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                        @"CREATE TABLE transactions (id INTEGER PRIMARY KEY AUTOINCREMENT, senderId INTEGER, receiverId INTEGER, amount REAL, datetime TEXT);";
                command.ExecuteNonQuery();
            }
        }
        catch (Microsoft.Data.Sqlite.SqliteException)
        {
            return false;
        }

        return true;
    }

    public static bool CreateDocumentsTable()
    {
        try
        {
            using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                        @"CREATE TABLE documents (id INTEGER PRIMARY KEY AUTOINCREMENT, hash TEXT, userId INTEGER, fileId INTEGER, caption TEXT);";
                command.ExecuteNonQuery();
            }
        }
        catch (Microsoft.Data.Sqlite.SqliteException)
        {
            return false;
        }

        return true;
    }

    public static bool CreateChequesTable()
    {
        try
        {
            using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                        @"CREATE TABLE cheques (id INTEGER PRIMARY KEY AUTOINCREMENT, senderId INTEGER, hash TEXT, amount REAL);";
                command.ExecuteNonQuery();
            }
        }
        catch (Microsoft.Data.Sqlite.SqliteException)
        {
            return false;
        }

        return true;
    }

    public static bool AddUser(long id, string name, string username)
    {
        try
        {
            using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"INSERT INTO users (id, name, username, balance, dailyLimit, monthlyLimit) VALUES ({id},'{name}','{username}',0,-1,-1);";

                command.ExecuteNonQuery();
            }
        }
        catch (Microsoft.Data.Sqlite.SqliteException)
        {
            return false;
        }

        return true;
    }

    public static User SelectUser(long id)
    {
        try
        {
            using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM users WHERE id = {id}";
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();

                    var name = reader.GetString(1);
                    var username = reader.GetString(2);
                    var balance = reader.GetDouble(3);
                    var dailyLimit = reader.GetDouble(4);
                    var monthlyLimit = reader.GetDouble(5);

                    return new User(id, name, username, balance, dailyLimit, monthlyLimit);
                }
            }
        }
        catch (System.InvalidOperationException)
        {
            // In case of non existent user
            return new User(-1);
        }
    }

    public static void TopUp(long id, double amount)
    {
        var user = SelectUser(id);
        using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $"UPDATE users SET balance = {user.Balance + amount} WHERE id = {id};";
            command.ExecuteNonQuery();
        }
    }

    public static void SendTransaction(User sender, User receiver, double amount)
    {
        using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"INSERT INTO transactions (senderId,receiverId, amount, datetime) VALUES({sender.Id}, {receiver.Id},{amount},'{DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss")}');
                                           UPDATE users SET balance = {sender.Balance - amount} WHERE id = {sender.Id}; 
                                           UPDATE users SET balance = {receiver.Balance + amount} WHERE id = {receiver.Id};";
            command.ExecuteNonQuery();
        }
    }


    public static void InsertCheque(long id, string hash, double amount)
    {
        using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"INSERT INTO cheques (senderId,hash, amount) VALUES({id}, '{hash}', {amount})";
            command.ExecuteNonQuery();
        }
    }

    public static void InsertDocument(long userId, string hash, string fileId, string caption = "")
    {
        using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"INSERT INTO documents (hash,userId, fileId, caption) VALUES(@hash,@userId, @fileId, @caption)";

            command.Parameters.AddWithValue("hash", hash);
            command.Parameters.AddWithValue("userId", userId);
            command.Parameters.AddWithValue("fileId", fileId);
            command.Parameters.AddWithValue("caption", caption);

            command.ExecuteNonQuery();
        }
    }

    public static List<Transaction> SelectTransactionHistory(long id)
    {
        try
        {
            using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM transactions WHERE senderId={id} OR receiverId={id} ORDER BY datetime(datetime) ASC";
                using (var reader = command.ExecuteReader())
                {
                    var transactions = new List<Transaction>();
                    while (reader.Read())
                    {
                        var senderId = reader.GetInt64(1);
                        var receiverId = reader.GetInt64(2);
                        var amount = reader.GetDouble(3);
                        var datetime = reader.GetString(4);

                        transactions.Add(new Transaction(senderId, receiverId, amount, datetime));
                    }

                    return transactions;
                }
            }
        }
        catch (System.InvalidOperationException)
        {
            return new List<Transaction>();
        }
    }

    public static List<Document> SelectUserDocuments(long id)
    {
        try
        {
            using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM documents WHERE userId={id}";
                using (var reader = command.ExecuteReader())
                {
                    var transactions = new List<Document>();
                    while (reader.Read())
                    {
                        var hash = reader.GetString(1);
                        var userId = reader.GetInt64(2);
                        var fileId = reader.GetString(3);
                        var caption = reader.GetString(4);

                        transactions.Add(new Document(hash, fileId, caption, userId));
                    }

                    return transactions;
                }
            }
        }
        catch (System.InvalidOperationException)
        {
            return new List<Document>();
        }
    }

    public static double SelectSendedAmountForPeriod(long id, string datetimeFrom, string datetimeTo)
    {
        try
        {
            using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"SELECT sum(amount) FROM transactions WHERE datetime BETWEEN '{datetimeFrom}' AND '{datetimeTo}'";
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();

                    var sum = reader.GetDouble(0);

                    return sum;
                }
            }
        }
        catch (System.InvalidOperationException)
        {
            return 0;
        }
    }

    public static double SelectMonthlyLimit(long id)
    {
        using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"SELECT monthlyLimit FROM users WHERE id = {id}";
            using (var reader = command.ExecuteReader())
            {
                reader.Read();

                var monthlyLimit = reader.GetDouble(0);

                return monthlyLimit;
            }
        }
    }


    public static double SelectDailyLimit(long id)
    {
        using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"SELECT dailyLimit FROM users WHERE id = {id}";
            using (var reader = command.ExecuteReader())
            {
                reader.Read();

                var dailyLimit = reader.GetDouble(0);

                return dailyLimit;
            }
        }
    }


    public static void UpdateDailyLimit(long id, double dailyLimit)
    {
        using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"UPDATE users SET dailyLimit = {dailyLimit} WHERE id = {id};";
            command.ExecuteNonQuery();
        }
    }

    public static void UpdateMonthlyLimit(long id, double monthlyLimit)
    {
        using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"UPDATE users SET monthlyLimit = {monthlyLimit} WHERE id = {id};";
            command.ExecuteNonQuery();
        }
    }

    public static Cheque SelectCheque(string hash)
    {
        try
        {
            using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM cheques WHERE hash = '{hash}'";
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();

                    var amount = reader.GetDouble(3);

                    var senderId = reader.GetInt64(1);
                    return new Cheque(hash, senderId, amount);
                }
            }
        }
        catch (System.InvalidOperationException)
        {
            return new Cheque();
        }
    }

    public static Document SelectDocument(string hash)
    {
        try
        {
            using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM documents WHERE hash = '{hash}'";
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();

                    var userId = reader.GetInt64(2);
                    var fileId = reader.GetString(3);
                    var caption = reader.GetString(4);

                    return new Document(hash, fileId, caption, userId);
                }
            }
        }
        catch (System.InvalidOperationException)
        {
            return new Document();
        }
    }

    public static void DeleteDocument(string hash)
    {
        using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"DELETE FROM documents WHERE hash='{hash}';";
            command.ExecuteNonQuery();
        }
    }


    public static void DeleteAllDocuments(long id)
    {
        using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"DELETE FROM documents WHERE userId='{id}';";
            command.ExecuteNonQuery();
        }
    }

    public static void DeleteCheque(string hash)
    {
        using (var connection = new SqliteConnection($"Data Source={Constants.DBName}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"DELETE FROM cheques WHERE hash='{hash}';";
            command.ExecuteNonQuery();
        }
    }

}
