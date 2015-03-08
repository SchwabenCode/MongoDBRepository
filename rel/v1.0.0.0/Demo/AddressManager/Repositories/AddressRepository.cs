using System;
using AddressManager.Models;
using SchwabenCode.MongoDBRepository;

namespace AddressManager.Repositories
{
    public class AddressRepository : MongoRepository<Address, IAddress> // No validation!
    {
        public AddressRepository( IMongoUnitOfWork uow, bool pluralizeTableName = true )
            : base( uow, MongoDiscoverer.GetFieldName<Address>( address => address.ID ), pluralizeTableName )
        {

        }
    }
}
