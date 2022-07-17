namespace Play.Common.Settings
{
    /// <summary>
    /// Represenation of the MongoDbSettings section in appsettings
    /// </summary>
    public class MongoDbSettings
    {
        public string Host { get; init; }
        public int Port { get; init; }
        public string ConnectionString => $"mongodb://{ Host }:{ Port }";
    }
}
