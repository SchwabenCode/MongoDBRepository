﻿// <copyright file="MongoRepository.cs" company="Benjamin Abt ( http://www.benjamin-abt.com )">
//      Copyright (c) 2015 All Rights Reserved - DO NOT REMOVE OR EDIT COPYRIGHT
// </copyright>
// <author>
//      Benjamin Abt
// </author>
// <date>
//      2015, 8. March
// </date>

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace SchwabenCode.MongoDBRepository
{
    public class MongoRepository<TEntity> : MongoRepository<TEntity, TEntity> where TEntity : class, IMongoEntity
    {
        /// <summary>
        /// Creates an instance
        /// </summary>
        /// <param name="uow">Attached Unit of Work Container</param>
        /// <param name="idFieldName">Name of the property represents the entity key.</param>
        /// <remarks>Uses the pluralized name of the given entity as collection name</remarks>
        public MongoRepository( IMongoUnitOfWork uow, string idFieldName )
            : base( uow, idFieldName )
        {
        }

        /// <summary>
        /// Creates a default instance
        /// </summary>
        /// <param name="uow">Attached Unit of Work Container</param>
        /// <param name="idFieldName">Name of the property represents the entity key.</param>
        /// <param name="collectionName">Name of table/collection</param>
        public MongoRepository( IMongoUnitOfWork uow, string idFieldName, string collectionName )
            : base( uow, idFieldName, collectionName )
        {
        }

        /// <summary>
        /// Creates an instance
        /// </summary>
        /// <param name="uow">Attached Unit of Work Container</param>
        /// <param name="idFieldName">Name of the property represents the entity key.</param>
        /// <param name="collectionNamePluralize">Appends an 's' after the entity's name to use this as collection name</param>
        public MongoRepository( IMongoUnitOfWork uow, string idFieldName, bool collectionNamePluralize )
            : base( uow, idFieldName, collectionNamePluralize )
        {
        }
    }

    /// <summary>
    ///     MongoDB Base Repository
    /// </summary>
    /// <typeparam name="TEntity">Main Entity Type</typeparam>
    /// <typeparam name="IEntity">Interface Scope</typeparam>
    public class MongoRepository<TEntity, IEntity> : IMongoRepository<TEntity, IEntity> where TEntity : class, IMongoEntity, IEntity
    {
        /// <summary>
        /// Override this with your ID field name
        /// </summary>
        public String IDFieldName { get; private set; }

        #region Ctor


        /// <summary>
        /// Creates an instance
        /// </summary>
        /// <param name="uow">Attached Unit of Work Container</param>
        /// <param name="idFieldName">Name of the property represents the entity key.</param>
        /// <remarks>Uses the pluralized name of the given entity as collection name</remarks>
        public MongoRepository( IMongoUnitOfWork uow, String idFieldName )
            : this( uow, idFieldName, true )
        {

        }

        /// <summary>
        /// Creates a default instance
        /// </summary>
        /// <param name="uow">Attached Unit of Work Container</param>
        /// <param name="idFieldName">Name of the property represents the entity key.</param>
        /// <param name="collectionName">Name of table/collection</param>
        public MongoRepository( IMongoUnitOfWork uow, String idFieldName, String collectionName )
        {
            Contract.Requires( uow != null );
            Contract.Requires( !String.IsNullOrEmpty( idFieldName ) );
            Contract.Requires( !String.IsNullOrEmpty( collectionName ) );

            UnitOfWork = uow;
            IDFieldName = idFieldName;
            CollectionName = collectionName;
        }

        /// <summary>
        /// Creates an instance
        /// </summary>
        /// <param name="uow">Attached Unit of Work Container</param>
        /// <param name="idFieldName">Name of the property represents the entity key.</param>
        /// <param name="collectionNamePluralize">Appends an 's' after the entity's name to use this as collection name</param>
        public MongoRepository( IMongoUnitOfWork uow, String idFieldName, bool collectionNamePluralize )
            : this( uow, idFieldName, typeof( TEntity ).Name + ( collectionNamePluralize ? "s" : "" ) )
        {
            Contract.Requires( uow != null );
            Contract.Requires( !String.IsNullOrEmpty( idFieldName ) );
        }




        #endregion

        /// <summary>
        /// Ensures that the desired index exists and creates it if it does not
        /// </summary>
        /// <param name="indexKeys">Keys</param>
        /// <param name="options">Index Options</param>
        /// <param name="reCreate">Re-creates index if name already exists.</param>
        /// <returns>null if creation failed or SafeMode is off.</returns>
        public WriteConcernResult CreateIndex( IMongoIndexKeys indexKeys, IMongoIndexOptions options = null, bool reCreate = false )
        {
            Contract.Requires( indexKeys != null );

            // Check key existance
            if ( !Entities.IndexExists( indexKeys ) )
            {
                return Entities.CreateIndex( indexKeys, options );
            }

            // Create secure
            if ( reCreate )
            {
                Entities.DropIndex( indexKeys );
            }
            else
            {
                return null;
            }
            return Entities.CreateIndex( indexKeys, options );
        }

        /// <summary>
        /// Drops an index
        /// </summary>
        public CommandResult DropIndex( IMongoIndexKeys indexKeys )
        {
            return Entities.DropIndex( indexKeys );
        }

        /// <summary>
        /// Drops an index
        /// </summary>
        public CommandResult DropIndexByName( String indexName )
        {
            return Entities.DropIndexByName( indexName );
        }

        #region protected Fields

        /// <summary>
        ///     Current collection name
        /// </summary>
        public String CollectionName { get; set; }

        /// <summary>
        /// Context
        /// </summary>
        private IMongoUnitOfWork _unitOfWork;

        /// <summary>
        ///     Data context
        /// </summary>
        public IMongoUnitOfWork UnitOfWork
        {
            get
            {
                Contract.Ensures( Contract.Result<IMongoUnitOfWork>( ) != null );
                return _unitOfWork;
            }
            private set
            {
                Contract.Requires( value != null );
                _unitOfWork = value;
            }
        }

        /// <summary>
        ///     Gets the current entity set.
        /// </summary>
        public MongoCollection<TEntity> Entities
        {
            get
            {
                Contract.Requires( CollectionName != null );
                Contract.Requires( UnitOfWork != null );
                Contract.Requires( UnitOfWork.Context != null );

                Contract.Ensures( Contract.Result<MongoCollection<TEntity>>( ) != null );

                return UnitOfWork.Context.GetCollection<TEntity>( CollectionName );
            }
        }

        /// <summary>
        ///     Gets the current document set.
        /// </summary>
        public MongoCollection<BsonDocument> Documents
        {
            get
            {
                Contract.Requires( CollectionName != null );
                Contract.Requires( UnitOfWork != null );
                Contract.Requires( UnitOfWork.Context != null );

                Contract.Ensures( Contract.Result<MongoCollection<BsonDocument>>( ) != null );

                return UnitOfWork.Context.GetCollection<BsonDocument>( CollectionName );
            }
        }

        /// <summary>
        ///     Returns true if passed is is known
        /// </summary>
        /// <param name="id">the ID to check</param>
        /// <returns>true if id exists in collection</returns>
        public Boolean ContainsID( ObjectId id )
        {
            Contract.Requires( id != default( ObjectId ) );
            return Count( CreateEQQueryIDField( id ) ) != 0;
        }

        #endregion

        #region Standard Repository Methods

        #region Add
        /// <summary>
        ///     Adds the passed entity to current collection
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>
        ///     <see cref="WriteConcernResult" />
        /// </returns>
        /// <exception cref="MongoInvalidEntityException">if entity is invalid</exception>
        public virtual WriteConcernResult Add( TEntity entity )
        {
            Contract.Requires( entity != null );
            Contract.Ensures( Contract.Result<WriteConcernResult>( ) != null );

            WriteConcernResult safeModeResult = Entities.Insert( entity );

            FireEntityAdded( entity );
            return safeModeResult;
        }

        /// <summary>
        ///     Adds the passed entity to current collection
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>
        ///     <see cref="WriteConcernResult" />
        /// </returns>
        /// <exception cref="MongoInvalidEntityException">if entity is invalid</exception>
        public virtual Task<WriteConcernResult> AddAsync( TEntity entity )
        {
            Contract.Requires( entity != null );
            Contract.Ensures( Contract.Result<Task<WriteConcernResult>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => Add( entity ) );
        }

        /// <summary>
        ///     Updates an existing entity or adds if new
        /// </summary>
        public virtual WriteConcernResult AddOrUpdate( TEntity entity )
        {
            Contract.Requires( entity != null );
            Contract.Ensures( Contract.Result<WriteConcernResult>( ) != null );

            return Entities.Save( entity );
        }


        /// <summary>
        ///     Updates an existing entity or adds if new
        /// </summary>
        public virtual Task<WriteConcernResult> AddOrUpdateAsync( TEntity entity )
        {
            Contract.Requires( entity != null );
            Contract.Ensures( Contract.Result<Task<WriteConcernResult>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => AddOrUpdate( entity ) );
        }
        #endregion

        #region Count
        /// <summary>
        ///     Return the count of elements in the current collection
        /// </summary>
        public Int64 Count( )
        {
            Contract.Requires( Documents != null );

            return Documents.Count( );
        }
        /// <summary>
        ///     Return the count of elements in the current collection
        /// </summary>
        public Task<Int64> CountAsync( )
        {
            Contract.Requires( Documents != null );
            Contract.Ensures( Contract.Result<Task<Int64>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( Count );
        }

        /// <summary>
        ///     Returns the count of elements of the matching query
        /// </summary>
        /// <returns>count of elements matching the query</returns>
        public virtual Int64 Count( IMongoQuery query )
        {
            Contract.Requires( query != null );
            Contract.Requires( Documents != null );

            return Documents.Count( query );
        }
        /// <summary>
        ///     Returns the count of elements of the matching query
        /// </summary>
        /// <returns>count of elements matching the query</returns>
        public virtual Task<Int64> CountAsync( IMongoQuery query )
        {
            Contract.Requires( query != null );
            Contract.Requires( Documents != null );

            Contract.Ensures( Contract.Result<Task<Int64>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => Count( query ) );
        }
        #endregion

        #region Delete
        /// <summary>
        ///     Deletes the specified entity.
        /// </summary>
        public virtual WriteConcernResult Delete( TEntity entity )
        {
            Contract.Requires( entity != null );
            Contract.Ensures( Contract.Result<WriteConcernResult>( ) != null );

            var safeModeResult = Delete( entity.GetPropertyValue<string>( IDFieldName ) );

            FireEntityDeleted( entity );

            return safeModeResult;
        }

        /// <summary>
        ///     Deletes the specified entity.
        /// </summary>
        public virtual Task<WriteConcernResult> DeleteAsync( TEntity entity )
        {
            Contract.Requires( entity != null );
            Contract.Ensures( Contract.Result<Task<WriteConcernResult>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => Delete( entity ) );
        }

        /// <summary>
        ///     Deletes the specified entity.
        /// </summary>
        public virtual WriteConcernResult Delete( String id )
        {
            var idRef = ObjectId.Parse( id );

            Contract.Requires( Documents != null );

            Contract.Requires( !String.IsNullOrEmpty( id ) );
            Contract.Ensures( Contract.Result<WriteConcernResult>( ) != null );

            return Documents.Remove( Query.EQ( IDFieldName, idRef ) );
        }
        /// <summary>
        ///     Deletes the specified entity.
        /// </summary>
        public virtual Task<WriteConcernResult> DeleteAsync( String id )
        {
            Contract.Requires( !String.IsNullOrEmpty( id ) );
            Contract.Ensures( Contract.Result<Task<WriteConcernResult>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => Delete( id ) );
        }

        /// <summary>
        ///     Deletes the specified entity.
        /// </summary>
        public virtual WriteConcernResult Delete( ObjectId id )
        {
            Contract.Requires( Documents != null );

            Contract.Requires( id != default( ObjectId ) );
            Contract.Ensures( Contract.Result<WriteConcernResult>( ) != null );

            return Documents.Remove( Query.EQ( IDFieldName, id ) );
        }
        /// <summary>
        ///     Deletes the specified entity.
        /// </summary>
        public virtual Task<WriteConcernResult> DeleteAsync( ObjectId id )
        {
            Contract.Requires( id != default( ObjectId ) );
            Contract.Ensures( Contract.Result<Task<WriteConcernResult>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => Delete( id ) );
        }

        /// <summary>
        ///     Deletes the specified entities.
        /// </summary>
        public virtual WriteConcernResult Delete( IMongoQuery query )
        {
            Contract.Requires( Documents != null );

            Contract.Requires( query != null );
            Contract.Ensures( Contract.Result<WriteConcernResult>( ) != null );

            return Documents.Remove( query );
        }
        /// <summary>
        ///     Deletes the specified entities.
        /// </summary>
        public virtual Task<WriteConcernResult> DeleteAsync( IMongoQuery query )
        {
            Contract.Requires( query != null );
            Contract.Ensures( Contract.Result<Task<WriteConcernResult>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => Delete( query ) );
        }

        /// <summary>
        ///     Deletes the specified entities by where expression.
        /// </summary>
        /// <param name="where">LINQ query</param>
        public IList<WriteConcernResult> Delete( Expression<Func<TEntity, Boolean>> where )
        {
            Contract.Requires( where != null );
            Contract.Ensures( Contract.Result<IList<WriteConcernResult>>( ) != null );

            return GetMany( @where ).Select( Delete ).ToList( );
        }
        /// <summary>
        ///     Deletes the specified entities by where expression.
        /// </summary>
        /// <param name="where">LINQ query</param>
        public Task<IList<WriteConcernResult>> DeleteAsync( Expression<Func<TEntity, Boolean>> where )
        {

            Contract.Requires( where != null );
            Contract.Ensures( Contract.Result<Task<IList<WriteConcernResult>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => Delete( where ) );
        }
        #endregion

        #region Exists
        /// <summary>
        ///     Checks whether the ID exists
        /// </summary>
        /// <returns>Calls Count and checks > 0</returns>
        public virtual Boolean Exists( ObjectId id )
        {
            Contract.Requires( id != default( ObjectId ) );

            return Exists( CreateEQQueryIDField( id ) );
        }
        /// <summary>
        ///     Checks whether the ID exists
        /// </summary>
        /// <returns>Calls Count and checks > 0</returns>
        public virtual Task<Boolean> ExistsAsync( ObjectId id )
        {
            Contract.Requires( id != default( ObjectId ) );
            Contract.Ensures( Contract.Result<Task<Boolean>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => Exists( id ) );
        }

        /// <summary>
        ///     Checks whether the query is matching an existing element
        /// </summary>
        /// <returns>Calls Count and checks > 0</returns>
        public virtual Boolean Exists( IMongoQuery query )
        {
            Contract.Requires( query != null );
            Contract.Requires( Documents != null );
            return Count( query ) > 0;
        }
        /// <summary>
        ///     Checks whether the query is matching an existing element
        /// </summary>
        /// <returns>Calls Count and checks > 0</returns>
        public virtual Task<Boolean> ExistsAsync( IMongoQuery query )
        {
            Contract.Requires( query != null );
            Contract.Ensures( Contract.Result<Task<Boolean>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => Exists( query ) );
        }
        #endregion

        #region FirstOrDefault
        /// <summary>
        ///     Returns first element by passed sort mode
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="sortBy">Sort Mode</param>
        /// <returns>First element or null if collection is empty</returns>
        public virtual T FirstOrDefault<T>( IMongoSortBy sortBy ) where T : class,IEntity
        {
            Contract.Requires( sortBy != null );

            return GetAll<T>( sortBy, 1 ).SingleOrDefault( );
        }
        /// <summary>
        ///     Returns first element by passed sort mode
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="sortBy">Sort Mode</param>
        /// <returns>First element or null if collection is empty</returns>
        public virtual Task<T> FirstOrDefaultAsync<T>( IMongoSortBy sortBy ) where T : class,IEntity
        {
            Contract.Requires( sortBy != null );
            Contract.Ensures( Contract.Result<Task<T>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => FirstOrDefault<T>( sortBy ) );
        }
        #endregion

        #region Get

        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        public virtual T Get<T>( IMongoQuery query ) where T : class,IEntity
        {
            Contract.Requires( query != null );

            return ManyDocumentsMapped<T>( query ).SingleOrDefault( );
        }
        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        public virtual Task<T> GetAsync<T>( IMongoQuery query ) where T : class,IEntity
        {
            Contract.Requires( query != null );
            Contract.Ensures( Contract.Result<Task<T>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => Get<T>( query ) );
        }

        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        public virtual T Get<T>( IMongoQuery query, IMongoSortBy sortBy ) where T : class,IEntity
        {
            Contract.Requires( query != null );
            Contract.Requires( sortBy != null );

            return ManyDocumentsMapped<T>( query ).SetSortOrder( sortBy ).SingleOrDefault( );
        }
        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        public virtual Task<T> GetAsync<T>( IMongoQuery query, IMongoSortBy sortBy ) where T : class,IEntity
        {
            Contract.Requires( query != null );
            Contract.Requires( sortBy != null );

            Contract.Ensures( Contract.Result<Task<T>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => Get<T>( query, sortBy ) );
        }

        /// <summary>
        ///     Gets en element by the specified filter.
        /// </summary>
        /// <param name="where">The filter.</param>
        /// <returns></returns>
        public virtual TEntity Get( Expression<Func<TEntity, Boolean>> where )
        {
            Contract.Requires( where != null );

            Contract.Requires( Entities != null );

            return Entities.AsQueryable( ).SingleOrDefault( where.Compile( ) );
        }
        /// <summary>
        ///     Gets en element by the specified filter.
        /// </summary>
        /// <param name="where">The filter.</param>
        /// <returns></returns>
        public virtual Task<TEntity> GetAsync( Expression<Func<TEntity, Boolean>> where )
        {
            Contract.Requires( where != null );

            Contract.Requires( Entities != null );
            Contract.Ensures( Contract.Result<Task<TEntity>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => Get( where ) );
        }
        #endregion Get

        #region GetAll

        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        public virtual IList<TEntity> GetAll( )
        {
            Contract.Requires( Documents != null );
            Contract.Ensures( Contract.Result<IList<TEntity>>( ) != null );

            return GetAll<TEntity>( );
        }
        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        public virtual IList<T> GetAll<T>( ) where T : class, IEntity
        {
            Contract.Requires( Documents != null );
            Contract.Ensures( Contract.Result<IList<T>>( ) != null );

            return AllDocumentsMapped<T>( ).ToList( );
        }

        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        public virtual Task<IList<TEntity>> GetAllAsync( )
        {
            Contract.Ensures( Contract.Result<IList<TEntity>>( ) != null );
            Contract.Ensures( Contract.Result<Task<IList<TEntity>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( GetAll );
        }
        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        public virtual Task<IList<T>> GetAllAsync<T>( ) where T : class, IEntity
        {
            Contract.Ensures( Contract.Result<Task<IList<T>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( GetAll<T> );
        }

        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        public virtual IList<T> GetAll<T>( IMongoSortBy sortBy ) where T : class, IEntity
        {
            Contract.Requires( Documents != null );
            Contract.Requires( sortBy != null );
            Contract.Ensures( Contract.Result<IList<T>>( ) != null );

            return AllDocumentsMapped<T>( ).SetSortOrder( sortBy ).ToList( );
        }
        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        public virtual Task<IList<T>> GetAllAsync<T>( IMongoSortBy sortBy ) where T : class, IEntity
        {
            Contract.Requires( sortBy != null );
            Contract.Ensures( Contract.Result<Task<IList<T>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => GetAll<T>( sortBy ) );
        }

        /// <summary>
        ///     Gets all elements limited
        /// </summary>
        public virtual IList<T> GetAll<T>( Int32 limit ) where T : class, IEntity
        {
            Contract.Requires( Documents != null );
            Contract.Ensures( Contract.Result<IList<T>>( ) != null );
            return AllDocumentsMapped<T>( ).SetLimit( limit ).ToList( );
        }
        /// <summary>
        ///     Gets all elements limited
        /// </summary>
        public virtual Task<IList<T>> GetAllAsync<T>( Int32 limit ) where T : class, IEntity
        {
            Contract.Ensures( Contract.Result<Task<IList<T>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => GetAll<T>( limit ) );
        }

        /// <summary>
        ///     Gets all elements sorted and limited
        /// </summary>
        public virtual IList<T> GetAll<T>( IMongoSortBy sortBy, Int32 limit ) where T : class, IEntity
        {
            Contract.Requires( Documents != null );
            Contract.Requires( sortBy != null );
            Contract.Ensures( Contract.Result<IList<T>>( ) != null );

            return AllDocumentsMapped<T>( ).SetSortOrder( sortBy ).SetLimit( limit ).ToList( );
        }
        /// <summary>
        ///     Gets all elements sorted and limited
        /// </summary>
        public virtual Task<IList<T>> GetAllAsync<T>( IMongoSortBy sortBy, Int32 limit ) where T : class, IEntity
        {
            Contract.Requires( sortBy != null );
            Contract.Ensures( Contract.Result<Task<IList<T>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => GetAll<T>( sortBy, limit ) );
        }

        /// <summary>
        ///     Gets the Entity by <see cref="ObjectId" />
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        public virtual IList<TEntity> GetAllByID( ObjectId id )
        {
            Contract.Requires( id != default( ObjectId ) );
            Contract.Ensures( Contract.Result<IList<TEntity>>( ) != null );

            return GetAllByID<TEntity>( id );
        }
        /// <summary>
        ///     Gets the Entity by <see cref="ObjectId" />
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        public virtual Task<IList<TEntity>> GetAllByIDAsync( ObjectId id )
        {
            Contract.Requires( id != default( ObjectId ) );
            Contract.Ensures( Contract.Result<Task<IList<TEntity>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => GetAllByID<TEntity>( id ) );
        }

        /// <summary>
        ///     Gets the Entity by <see cref="ObjectId" />
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        public virtual IList<T> GetAllByID<T>( ObjectId id ) where T : class, IEntity
        {
            Contract.Requires( id != default( ObjectId ) );
            Contract.Ensures( Contract.Result<IList<T>>( ) != null );

            return GetMany<T>( CreateEQQueryIDField( id ) );
        }
        /// <summary>
        ///     Gets the Entity by <see cref="ObjectId" />
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        public virtual Task<IList<T>> GetAllByIDAsync<T>( ObjectId id ) where T : class, IEntity
        {
            Contract.Requires( id != default( ObjectId ) );
            Contract.Ensures( Contract.Result<Task<IList<T>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => GetAllByID<T>( id ) );
        }
        #endregion GetAll

        #region GetAll Documents
        /// <summary>
        ///     Returns all documents of current collection
        /// </summary>
        /// <returns>All documents</returns>
        public virtual IList<BsonDocument> GetAllDocuments( )
        {
            Contract.Requires( Documents != null );

            Contract.Ensures( Contract.Result<IList<BsonDocument>>( ) != null );

            return Documents.FindAll( ).ToList( );
        }
        /// <summary>
        ///     Returns all documents of current collection
        /// </summary>
        /// <returns>All documents</returns>
        public virtual Task<IList<BsonDocument>> GetAllDocumentsAsync( )
        {
            Contract.Ensures( Contract.Result<Task<IList<BsonDocument>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( GetAllDocuments );
        }
        #endregion GetAll Documents

        #region Get By ID
        /// <summary>
        ///     Gets the entity by passed ID
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        public virtual TEntity GetByID( ObjectId id )
        {
            Contract.Requires( id != default( ObjectId ) );
            Contract.Ensures( Contract.Result<TEntity>( ) != null );

            return Get<TEntity>( CreateEQQueryIDField( id ) );
        }
        /// <summary>
        ///     Gets the entity by passed ID
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        public virtual Task<TEntity> GetByIDAsync( ObjectId id )
        {
            Contract.Requires( id != default( ObjectId ) );
            Contract.Ensures( Contract.Result<Task<TEntity>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => GetByID( id ) );
        }

        /// <summary>
        ///     Gets the entity by passed ID
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        public virtual T GetByID<T>( ObjectId id ) where T : class,IEntity
        {
            Contract.Requires( id != default( ObjectId ) );
            Contract.Ensures( Contract.Result<T>( ) != null );

            return Get<T>( CreateEQQueryIDField( id ) );
        }
        /// <summary>
        ///     Gets the entity by passed ID
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        public virtual Task<T> GetByIDAsync<T>( ObjectId id ) where T : class,IEntity
        {
            Contract.Requires( id != default( ObjectId ) );
            Contract.Ensures( Contract.Result<Task<T>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => GetByID<T>( id ) );
        }

        /// <summary>
        ///     Gets the entities by passed IDs
        /// </summary>
        /// <param name="ids">Entity's IDs</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        public virtual IList<TEntity> GetByID( IEnumerable<ObjectId> ids )
        {
            Contract.Requires( ids != null );
            Contract.Ensures( Contract.Result<IList<TEntity>>( ) != null );

            return GetByID<TEntity>( ids );
        }
        /// <summary>
        ///     Gets the entities by passed IDs
        /// </summary>
        /// <param name="ids">Entity's IDs</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        public virtual Task<IList<TEntity>> GetByIDAsync( IEnumerable<ObjectId> ids )
        {
            Contract.Requires( ids != null );
            Contract.Ensures( Contract.Result<Task<IList<TEntity>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => GetByID( ids ) );
        }

        /// <summary>
        ///     Gets the entities by passed IDs
        /// </summary>
        /// <param name="ids">Entity's IDs</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        public virtual IList<T> GetByID<T>( IEnumerable<ObjectId> ids ) where T : class,IEntity
        {
            Contract.Requires( ids != null );
            Contract.Ensures( Contract.Result<IList<T>>( ) != null );

            return ManyDocumentsMapped<T>( Query.In( IDFieldName, ids.Select( entry => ( BsonValue ) entry ) ) ).ToList( );
        }
        /// <summary>
        ///     Gets the entities by passed IDs
        /// </summary>
        /// <param name="ids">Entity's IDs</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        public virtual Task<IList<T>> GetByIDAsync<T>( IEnumerable<ObjectId> ids ) where T : class,IEntity
        {
            Contract.Requires( ids != null );
            Contract.Ensures( Contract.Result<Task<IList<T>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => GetByID<T>( ids ) );
        }
        #endregion

        #region Get Document By ID
        /// <summary>
        ///     Gets the document by passed ID
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        public virtual BsonDocument GetDocumentByID( ObjectId id )
        {
            Contract.Requires( id != default( ObjectId ) );

            Contract.Requires( Documents != null );

            return Documents.Find( CreateEQQueryIDField( id ) ).SingleOrDefault( );
        }
        /// <summary>
        ///     Gets the document by passed ID
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        public virtual Task<BsonDocument> GetDocumentByIDAsync( ObjectId id )
        {
            Contract.Requires( id != default( ObjectId ) );
            Contract.Ensures( Contract.Result<Task<BsonDocument>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => GetDocumentByID( id ) );
        }

        /// <summary>
        ///     Gets the documents by passed IDs
        /// </summary>
        /// <param name="ids">Entity's IDs</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        public virtual IList<BsonDocument> GetDocumentsByID( IEnumerable<ObjectId> ids )
        {
            Contract.Requires( Documents != null );

            Contract.Requires( ids != null );
            Contract.Ensures( Contract.Result<IList<BsonDocument>>( ) != null );

            return Documents.Find( Query.In( IDFieldName, ids.Select( entry => ( BsonValue ) entry ) ) ).ToList( );
        }
        /// <summary>
        ///     Gets the documents by passed IDs
        /// </summary>
        /// <param name="ids">Entity's IDs</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        public virtual Task<IList<BsonDocument>> GetDocumentsByIDAsync( IEnumerable<ObjectId> ids )
        {
            Contract.Requires( ids != null );
            Contract.Ensures( Contract.Result<Task<IList<BsonDocument>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => GetDocumentsByID( ids ) );
        }


        #endregion

        #region Get Many
        /// <summary>
        ///     Gets all matching elements
        /// </summary>
        public virtual IList<T> GetMany<T>( IMongoQuery query ) where T : class,IEntity
        {
            Contract.Requires( query != null );
            Contract.Ensures( Contract.Result<IList<T>>( ) != null );

            return ManyDocumentsMapped<T>( query ).ToList( );
        }
        /// <summary>
        ///     Gets all matching elements
        /// </summary>
        public virtual Task<IList<T>> GetManyAsync<T>( IMongoQuery query ) where T : class,IEntity
        {
            Contract.Requires( query != null );
            Contract.Ensures( Contract.Result<Task<IList<T>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => GetMany<T>( query ) );
        }

        /// <summary>
        ///     Gets the matched and sorted elements.
        /// </summary>
        public virtual IList<T> GetMany<T>( IMongoQuery query, IMongoSortBy sortBy ) where T : class,IEntity
        {
            Contract.Requires( sortBy != null );
            Contract.Requires( query != null );
            Contract.Ensures( Contract.Result<IList<T>>( ) != null );

            return ManyDocumentsMapped<T>( query ).SetSortOrder( sortBy ).ToList( );
        }
        /// <summary>
        ///     Gets the matched and sorted elements.
        /// </summary>
        public virtual Task<IList<T>> GetManyAsync<T>( IMongoQuery query, IMongoSortBy sortBy ) where T : class,IEntity
        {
            Contract.Requires( sortBy != null );
            Contract.Requires( query != null );
            Contract.Ensures( Contract.Result<Task<IList<T>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => GetMany<T>( query, sortBy ) );
        }

        /// <summary>
        ///     Gets the matched and limited elements.
        /// </summary>
        public virtual IList<T> GetMany<T>( IMongoQuery query, Int32 limit ) where T : class,IEntity
        {
            Contract.Requires( query != null );
            Contract.Ensures( Contract.Result<IList<T>>( ) != null );

            return ManyDocumentsMapped<T>( query ).SetLimit( limit ).ToList( );
        }
        /// <summary>
        ///     Gets the matched and limited elements.
        /// </summary>
        public virtual Task<IList<T>> GetManyAsync<T>( IMongoQuery query, int limit ) where T : class,IEntity
        {
            Contract.Requires( query != null );
            Contract.Ensures( Contract.Result<Task<IList<T>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => GetMany<T>( query, limit ) );
        }

        /// <summary>
        ///     Gets the matching elements sorted and limited
        /// </summary>
        public virtual IList<T> GetMany<T>( IMongoQuery query, IMongoSortBy sortBy, Int32 limit ) where T : class,IEntity
        {
            Contract.Requires( sortBy != null );
            Contract.Requires( query != null );
            Contract.Ensures( Contract.Result<IList<T>>( ) != null );

            return ManyDocumentsMapped<T>( query ).SetSortOrder( sortBy ).SetLimit( limit ).ToList( );
        }
        /// <summary>
        ///     Gets the matching elements sorted and limited
        /// </summary>
        public virtual Task<IList<T>> GetManyAsync<T>( IMongoQuery query, IMongoSortBy sortBy, int limit ) where T : class,IEntity
        {
            Contract.Requires( sortBy != null );
            Contract.Requires( query != null );
            Contract.Ensures( Contract.Result<Task<IList<T>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => GetMany<T>( query, sortBy, limit ) );
        }

        /// <summary>
        ///     Gets a collection of elements by given exception (take care, linq is slow and not fully supported by MongoDB C#
        ///     Driver!)
        /// </summary>
        /// <param name="where">The expression.</param>
        /// <returns>enumerable of entities</returns>
        public virtual IList<TEntity> GetMany( Expression<Func<TEntity, bool>> @where )
        {
            Contract.Requires( where != null );
            Contract.Ensures( Contract.Result<IList<TEntity>>( ) != null );

            return Entities.AsQueryable( ).Where( where.Compile( ) ).ToList( );
        }
        /// <summary>
        ///     Gets a collection of elements by given exception (take care, linq is slow and not fully supported by MongoDB C#
        ///     Driver!)
        /// </summary>
        /// <param name="where">The expression.</param>
        /// <returns>enumerable of entities</returns>
        public virtual Task<IList<TEntity>> GetManyAsync( Expression<Func<TEntity, bool>> @where )
        {
            Contract.Requires( where != null );
            Contract.Ensures( Contract.Result<Task<IList<TEntity>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => GetMany( @where ) );
        }
        #endregion


        #region Update
        /// <summary>
        ///     Updates given entity
        /// </summary>
        public virtual WriteConcernResult Update( TEntity entity )
        {
            Contract.Requires( entity != null );
            Contract.Ensures( Contract.Result<WriteConcernResult>( ) != null );

            return Entities.Save( entity );
        }
        /// <summary>
        ///     Updates given entity
        /// </summary>
        public virtual Task<WriteConcernResult> UpdateAsync( TEntity entity )
        {
            Contract.Requires( entity != null );
            Contract.Ensures( Contract.Result<Task<WriteConcernResult>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => Update( ( entity ) ) );
        }

        /// <summary>
        ///     Updates given document of collection
        /// </summary>
        public virtual WriteConcernResult Update( BsonDocument doc )
        {
            Contract.Requires( Documents != null );
            Contract.Requires( doc != null );
            Contract.Ensures( Contract.Result<WriteConcernResult>( ) != null );

            return Documents.Save( doc );
        }
        /// <summary>
        ///     Updates given document of collection
        /// </summary>
        public virtual Task<WriteConcernResult> UpdateAsync( BsonDocument doc )
        {
            Contract.Requires( doc != null );
            Contract.Ensures( Contract.Result<WriteConcernResult>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( ( ) => Update( ( doc ) ) );
        }
        #endregion

        #endregion

        #region Map Helpers
        /// <summary>
        ///     Calls
        ///     <see>
        ///         <cref>CurrentDocumentCollection.FindAllAs</cref>
        ///     </see>
        ///     and only loads the fields of given type.
        /// </summary>
        /// <typeparam name="T">Given type to map</typeparam>
        /// <remarks>If T is assignable from <see cref="IMongoDiscoverable"/> only the implemented fields are loaded.</remarks>
        /// <returns>Cursor of typed documents</returns>
        private MongoCursor<T> AllDocumentsMapped<T>( ) where T : class,IEntity
        {
            Contract.Requires( Documents != null );
            Contract.Ensures( Contract.Result<MongoCursor<T>>( ) != null );

            var cursor = Documents.FindAllAs<T>( );
            return MapFieldsByInheritance<T>( cursor );
        }

        private MongoCursor<T> MapFieldsByInheritance<T>( MongoCursor<T> cursor ) where T : class,IEntity
        {
            if ( MongoDiscoverer.IsDiscoverable<T>( ) )
            {
                string[ ] fieldNames = MongoDiscoverer.GetEntityFieldNames<T>( ).Distinct( ).ToArray( );
                cursor = cursor.SetFields( fieldNames );
            }

            return cursor;
        }

        /// <summary>
        ///     Calls
        ///     <see>
        ///         <cref>CurrentDocumentCollection.FindAs</cref>
        ///     </see>
        ///     and only loads the fields of given type.
        /// </summary>
        /// <typeparam name="T">Given type to map</typeparam>
        /// <param name="query">Many query</param>
        /// <returns>Cursor of typed documents</returns>
        private MongoCursor<T> ManyDocumentsMapped<T>( IMongoQuery query ) where T : class,IEntity
        {
            Contract.Requires( Documents != null );

            Contract.Requires( query != null );
            Contract.Ensures( Contract.Result<MongoCursor<T>>( ) != null );

            var cursor = Documents.FindAs<T>( query );
            return MapFieldsByInheritance<T>( cursor );
        }
        #endregion

        #region Query Helpers
        /// <summary>
        /// Creates a query for <see cref="Query.EQ"/> on <see cref="IDField"/>
        /// </summary>
        /// <param name="id">the ID</param>
        /// <returns><see cref="IMongoQuery"/></returns>
        private IMongoQuery CreateEQQueryIDField( ObjectId id )
        {
            Contract.Requires( id != default( ObjectId ) );
            Contract.Ensures( Contract.Result<IMongoQuery>( ) != null );

            return Query.EQ( IDFieldName, id );
        }

        #endregion

        #region Dispose Code Styling

        /// <summary>
        ///     Empty disposing for easier code styling
        /// </summary>
        public void Dispose( )
        {
        }

        #endregion

        #region Events

        /// <summary>
        ///     Fires <see cref="OnEntityAdded" />
        /// </summary>
        /// <param name="entity">Entity that has been added</param>
        protected virtual void FireEntityAdded( TEntity entity )
        {
            Contract.Requires( entity != null );

            if ( OnEntityAdded != null )
            {
                OnEntityAdded( this, new RepositoryEntityAddedEventArgs<TEntity>( entity ) );
            }
        }

        /// <summary>
        ///     Fires <see cref="OnEntityDeleted" />
        /// </summary>
        /// <param name="entity">Entity that has been deleted</param>
        protected virtual void FireEntityDeleted( TEntity entity )
        {
            Contract.Requires( entity != null );

            if ( OnEntityDeleted != null )
            {
                OnEntityDeleted( this, new RepositoryEntityDeletedEventArgs<TEntity>( entity ) );
            }
        }

        /// <summary>
        ///     Gets fired if an typed entity has been added. Does not work for documents!
        /// </summary>
        public event EventHandler<RepositoryEntityAddedEventArgs<TEntity>> OnEntityAdded;

        /// <summary>
        ///     Gets fired if an typed entity has been deleted. Does not work for documents!
        /// </summary>
        public event EventHandler<RepositoryEntityDeletedEventArgs<TEntity>> OnEntityDeleted;

        #endregion
    }
}