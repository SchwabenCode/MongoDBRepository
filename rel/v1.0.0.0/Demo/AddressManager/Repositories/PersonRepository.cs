using System;
using System.Threading.Tasks;
using AddressManager.Models;
using MongoDB.Driver.Builders;
using SchwabenCode.AsyncAll;
using SchwabenCode.MongoDBRepository;

namespace AddressManager.Repositories
{
    public class PersonRepository : MongoValidatableRepository<Person, IPerson> // With validation!
    {
        /// <summary>
        /// Avoid magic strings. Use generics here!
        /// </summary>
        private static readonly String EMailField = MongoDiscoverer.GetFieldName<Person>( p => p.EMail );

        /// <summary>
        /// Use the constructor to set keys!
        /// </summary>
        public PersonRepository( IMongoUnitOfWork uow, bool pluralizeTableName = true )
            : base( uow, MongoDiscoverer.GetFieldName<Person>( person => person.UserID ), pluralizeTableName )
        {
            CreateIndex( IndexKeys.Ascending( EMailField ), IndexOptions.SetUnique( true ), reCreate: false );
        }

        /// <summary>
        /// Returns single users. EMail should here be unique!
        /// </summary>
        public T GetByEMail<T>( String email ) where T : class, IPerson
        {
            var query = Query.EQ( EMailField, MongoQuery.Equals( email, isCaseSensetive: false ) );
            return Get<T>( query );
        }

        /// <summary>
        /// Async implementation of GetByEMail
        /// </summary>
        public Task<T> GetByEMailAsync<T>( String email ) where T : class, IPerson
        {
            return AsyncAll.GetAsyncResult( ( ) => GetByEMail<T>( email ) );
        }
    }
}
