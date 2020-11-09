using MongoDB.Driver;

namespace FIWAREHub.SynchronizerDaemon
{
    public class OrionContext
    {
        private static readonly MongoCredential Credentials = MongoCredential.CreateCredential("admin", "admin", "epu_ntua_2020");

        private static MongoServerAddress MongoServerAddress()
        {
            string connectionString;

#if DEBUG
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Mongo"]
                .ConnectionString;
#endif
#if !DEBUG
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MongoRelease"].ConnectionString;
#endif

            return new MongoServerAddress(connectionString, 27017);
        } 
        private static readonly MongoClientSettings MongoClientSettings = new MongoClientSettings
        {
            Credential = Credentials,
            Server = MongoServerAddress(),
            //ReplicaSetName = "rs0",
            //WriteConcern = WriteConcern.W1,
            //ReadPreference = ReadPreference.Primary
        };
        public const string DatabaseName = "orion-epu_ntua";
        public const string EntitiesCollectionName = "entities";

        // This is ok... Normally, they would be put into
        // an IoC container.
        private static readonly IMongoClient _client;
        private static readonly IMongoDatabase _database;

        static OrionContext()
        {
            _client = new MongoClient(MongoClientSettings);
            _database = _client.GetDatabase(DatabaseName);
        }

        public IMongoClient Client => _client;

        public IMongoCollection<dynamic> Entities => _database.GetCollection<dynamic>(EntitiesCollectionName);

    }
}
