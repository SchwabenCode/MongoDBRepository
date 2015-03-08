using SchwabenCode.MongoDBRepository;

namespace AddressManager.Models
{
    /// <summary>
    /// This interface is used for all address models.
    /// This main interface may have held no implementation of the properties, since otherwise no part models can be created.
    /// </summary>
    public interface IAddress : IMongoEntity
    {
        // Leave this empty
    }
}