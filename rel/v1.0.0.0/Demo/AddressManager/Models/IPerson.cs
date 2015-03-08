using SchwabenCode.MongoDBRepository;

namespace AddressManager.Models
{
    /// <summary>
    /// This interface is used for all person models and supports validation.
    /// This main interface may have held no implementation of the properties, since otherwise no part models can be created.
    /// </summary>
    public interface IPerson : IMongoEntityValidatable
    {
        // Leave this empty
    }
}