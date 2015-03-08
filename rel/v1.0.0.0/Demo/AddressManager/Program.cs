using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddressManager.Database;
using AddressManager.Models;
using AddressManager.Repositories;
using MongoDB.Driver;
using SchwabenCode.MongoDBRepository;

namespace AddressManager
{
    class Program
    {
        private const String MongoServer = "localhost";
        private const Int32 MongoPort = 27017;
        private const String MongoDbName = "AddressBook";
        private const String MongoUser = "UserName";
        private const String MongoPassword = "UserName";

        static void Main( String[ ] args )
        {
            // Default behavior, see http://docs.mongodb.org/ecosystem/tutorial/authenticate-with-csharp-driver/
            var credential = MongoCredential.CreateMongoCRCredential( MongoDbName, MongoUser, MongoPassword );

            var settings = new MongoClientSettings
            {
                //Credentials = new[ ] { credential },
                Server = new MongoServerAddress( MongoServer, MongoPort ),
                WriteConcern = WriteConcern.Acknowledged /* you can use fire and forget, too! */
            };

            var mongoClient = new MongoClient( settings );

            // Connect to Server
            var server = mongoClient.GetServer( );

            // Get our Database
            var database = server.GetDatabase( MongoDbName );

            // Create our IUoW Container
            IMongoUnitOfWork uowContainer = new DbContext( database );


            // ######### Examples - Addresses
            AddressRepository addressRepository = new AddressRepository( uowContainer );

            // ### Get All
            // Read all addresses with full entity details (uses default repository type)
            var addressesFull = addressRepository.GetAll( );
            // or with type
            var addressesFullByType = addressRepository.GetAll<Address>( );
            // or only specific fields
            var addessLabels = addressRepository.GetAll<AddressLabel>( );


            if ( addressesFull != null && addressesFullByType != null && addessLabels != null )
            {

            }

            // ######### Examples - Persons
            PersonRepository personRepository = new PersonRepository( uowContainer );

            // ### Write with validation
            var person = new Person { Name = "Test Person", EMail = "this would cause an expcetion" };
            WriteConcernResult addResult = null;
            try
            {
                addResult = personRepository.Add( person );
            }
            catch ( MongoInvalidEntityException e )
            {
                Console.WriteLine( "Failed to write." );
                foreach ( var error in e.Errors )
                {
                    Console.WriteLine( ">> " + error.ErrorMessage + " Affected fields: " + String.Join( ", ", error.MemberNames ) );
                }
            }

            // if MongoInvalidEntityException is catched, we have not result information and 'addResult' is null!
            if ( addResult == null )
            {
                Console.WriteLine( "No result information (invalid?? safemode off??)" );
            }
            else if ( addResult.Ok )
            {
                Console.WriteLine( "Successfully added person" );
            }
            else
            {
                Console.WriteLine( "Failed to write person: " + addResult.ErrorMessage );
            }

            Console.Read( );
        }
    }
}
