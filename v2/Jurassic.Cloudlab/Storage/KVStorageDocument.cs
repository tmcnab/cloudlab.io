namespace Jurassic.Cloudlab
{
    using MongoDB.Bson;

    public class KVStorageDocument
    {
        public ObjectId Id { get; set; }

        public string Username { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
