using System;
using SchwabenCode.MongoDBRepository;

namespace AddressManager.Models
{
    /// <summary>
    /// This class represents a cutout from the object 'Address'. For example, we only want the address labels.
    /// It must implement the interface 'IAddress' because our AddressRepository takes only IAddress objects.
    /// Also, it must implement the interface 'IMongoDiscoverable' so the repository know, it should not load the full entity.
    /// With 'IMongoDiscoverable' the repository is discovering all available properties and is using 'SetFields', provider by the MongoDB C# Driver.
    /// Take care: only properties of your main entity (here Address) can be used!
    /// </summary>
    public class AddressLabel : IMongoDiscoverable, IAddress
    {
        public String Label { get; set; }
    }
}