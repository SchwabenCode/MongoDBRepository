using MongoDB.Driver;
using SchwabenCode.MongoDBRepository;

namespace AddressManager.Database
{
    public class DbContext : IMongoUnitOfWork
    {

        public DbContext( MongoDatabase dbContext )
        {
            Context = dbContext;
        }


        /// <summary>
        /// This is required
        /// </summary>
        public MongoDatabase Context { get; private set; }
    }
}
