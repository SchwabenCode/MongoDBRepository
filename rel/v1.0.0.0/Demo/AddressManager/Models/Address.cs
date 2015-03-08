using System;
using MongoDB.Bson;

namespace AddressManager.Models
{
    /// <summary>
    /// This class represents the full entity for 'Address'. Here, all properties are defined.
    /// It must implement the interface 'IAddress' because our AddressRepository takes only IAddress objects.
    /// </summary>
    public class Address : IAddress
    {
        public ObjectId ID { get; set; }

        public String Label { get; set; }
        public String Street { get; set; }
        public String ZipCode { get; set; }
        public String City { get; set; }
        public String Country { get; set; }
    }
}
