using LiteDB.Async;

namespace DarkBot;

public static class Database
{
    public static LiteDatabaseAsync LiteDb { get; private set; } = new LiteDatabaseAsync(@"Filename=Data.db");
}