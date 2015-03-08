﻿// <copyright file="IMongoRepository.cs" company="Benjamin Abt ( http://www.benjamin-abt.com )">
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
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace SchwabenCode.MongoDBRepository
{
    public interface IMongoRepository<TEntity, in IEntity> where TEntity : class, IMongoEntity, IEntity
    {
        /// <summary>
        /// Override this with your ID field name
        /// </summary>
        String IDFieldName { get; }

        /// <summary>
        ///     Current table name
        /// </summary>
        String CollectionName { get; set; }

        /// <summary>
        ///     Data context
        /// </summary>
        IMongoUnitOfWork UnitOfWork { get; }

        /// <summary>
        ///     Gets the current entity set.
        /// </summary>
        MongoCollection<TEntity> Entities { get; }

        /// <summary>
        ///     Gets the current document set.
        /// </summary>
        MongoCollection<BsonDocument> Documents { get; }

        /// <summary>
        /// Ensures that the desired index exists and creates it if it does not
        /// </summary>
        /// <param name="indexKeys">Keys</param>
        /// <param name="options">Index Options</param>
        /// <param name="reCreate">Re-creates index if name already exists.</param>
        /// <returns>null if creation failed or SafeMode is off.</returns>
        WriteConcernResult CreateIndex( IMongoIndexKeys indexKeys, IMongoIndexOptions options = null, bool reCreate = false );

        /// <summary>
        /// Drops an index
        /// </summary>
        CommandResult DropIndex( IMongoIndexKeys indexKeys );

        /// <summary>
        /// Drops an index
        /// </summary>
        CommandResult DropIndexByName( String indexName );

        /// <summary>
        ///     Returns true if passed is is known
        /// </summary>
        /// <param name="id">the ID to check</param>
        /// <returns>true if id exists in collection</returns>
        Boolean ContainsID( ObjectId id );

        /// <summary>
        ///     Adds the passed entity to current collection
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>
        ///     <see cref="WriteConcernResult" />
        /// </returns>
        /// <exception cref="MongoInvalidEntityException">if entity is invalid</exception>
        WriteConcernResult Add( TEntity entity );

        /// <summary>
        ///     Adds the passed entity to current collection
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>
        ///     <see cref="WriteConcernResult" />
        /// </returns>
        /// <exception cref="MongoInvalidEntityException">if entity is invalid</exception>
        Task<WriteConcernResult> AddAsync( TEntity entity );

        /// <summary>
        ///     Updates an existing entity or adds if new
        /// </summary>
        WriteConcernResult AddOrUpdate( TEntity entity );

        /// <summary>
        ///     Updates an existing entity or adds if new
        /// </summary>
        Task<WriteConcernResult> AddOrUpdateAsync( TEntity entity );

        /// <summary>
        ///     Return the count of elements in the current collection
        /// </summary>
        Int64 Count( );

        /// <summary>
        ///     Return the count of elements in the current collection
        /// </summary>
        Task<Int64> CountAsync( );

        /// <summary>
        ///     Returns the count of elements of the matching query
        /// </summary>
        /// <returns>count of elements matching the query</returns>
        Int64 Count( IMongoQuery query );

        /// <summary>
        ///     Returns the count of elements of the matching query
        /// </summary>
        /// <returns>count of elements matching the query</returns>
        Task<Int64> CountAsync( IMongoQuery query );

        /// <summary>
        ///     Deletes the specified entity.
        /// </summary>
        WriteConcernResult Delete( TEntity entity );

        /// <summary>
        ///     Deletes the specified entity.
        /// </summary>
        Task<WriteConcernResult> DeleteAsync( TEntity entity );

        /// <summary>
        ///     Deletes the specified entity.
        /// </summary>
        WriteConcernResult Delete( String id );

        /// <summary>
        ///     Deletes the specified entity.
        /// </summary>
        Task<WriteConcernResult> DeleteAsync( String id );

        /// <summary>
        ///     Deletes the specified entity.
        /// </summary>
        WriteConcernResult Delete( ObjectId id );

        /// <summary>
        ///     Deletes the specified entity.
        /// </summary>
        Task<WriteConcernResult> DeleteAsync( ObjectId id );

        /// <summary>
        ///     Deletes the specified entities.
        /// </summary>
        WriteConcernResult Delete( IMongoQuery query );

        /// <summary>
        ///     Deletes the specified entities.
        /// </summary>
        Task<WriteConcernResult> DeleteAsync( IMongoQuery query );

        /// <summary>
        ///     Deletes the specified entities by where expression.
        /// </summary>
        /// <param name="where">LINQ query</param>
        IList<WriteConcernResult> Delete( Expression<Func<TEntity, Boolean>> where );

        /// <summary>
        ///     Deletes the specified entities by where expression.
        /// </summary>
        /// <param name="where">LINQ query</param>
        Task<IList<WriteConcernResult>> DeleteAsync( Expression<Func<TEntity, Boolean>> where );

        /// <summary>
        ///     Checks whether the ID exists
        /// </summary>
        /// <returns>Calls Count and checks > 0</returns>
        Boolean Exists( ObjectId id );

        /// <summary>
        ///     Checks whether the ID exists
        /// </summary>
        /// <returns>Calls Count and checks > 0</returns>
        Task<Boolean> ExistsAsync( ObjectId id );

        /// <summary>
        ///     Checks whether the query is matching an existing element
        /// </summary>
        /// <returns>Calls Count and checks > 0</returns>
        Boolean Exists( IMongoQuery query );

        /// <summary>
        ///     Checks whether the query is matching an existing element
        /// </summary>
        /// <returns>Calls Count and checks > 0</returns>
        Task<Boolean> ExistsAsync( IMongoQuery query );

        /// <summary>
        ///     Returns first element by passed sort mode
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="sortBy">Sort Mode</param>
        /// <returns>First element or null if collection is empty</returns>
        T FirstOrDefault<T>( IMongoSortBy sortBy ) where T : class,IEntity;

        /// <summary>
        ///     Returns first element by passed sort mode
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="sortBy">Sort Mode</param>
        /// <returns>First element or null if collection is empty</returns>
        Task<T> FirstOrDefaultAsync<T>( IMongoSortBy sortBy ) where T : class,IEntity;

        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        T Get<T>( IMongoQuery query ) where T : class,IEntity;

        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        Task<T> GetAsync<T>( IMongoQuery query ) where T : class,IEntity;

        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        T Get<T>( IMongoQuery query, IMongoSortBy sortBy ) where T : class,IEntity;

        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        Task<T> GetAsync<T>( IMongoQuery query, IMongoSortBy sortBy ) where T : class,IEntity;

        /// <summary>
        ///     Gets en element by the specified filter.
        /// </summary>
        /// <param name="where">The filter.</param>
        /// <returns></returns>
        TEntity Get( Expression<Func<TEntity, Boolean>> where );

        /// <summary>
        ///     Gets en element by the specified filter.
        /// </summary>
        /// <param name="where">The filter.</param>
        /// <returns></returns>
        Task<TEntity> GetAsync( Expression<Func<TEntity, Boolean>> where );

        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        IList<TEntity> GetAll( );

        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        IList<T> GetAll<T>( ) where T : class, IEntity;

        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        Task<IList<TEntity>> GetAllAsync( );

        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        Task<IList<T>> GetAllAsync<T>( ) where T : class, IEntity;

        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        IList<T> GetAll<T>( IMongoSortBy sortBy ) where T : class, IEntity;

        /// <summary>
        ///     Gets all elements.
        /// </summary>
        /// <returns></returns>
        Task<IList<T>> GetAllAsync<T>( IMongoSortBy sortBy ) where T : class, IEntity;

        /// <summary>
        ///     Gets all elements limited
        /// </summary>
        IList<T> GetAll<T>( Int32 limit ) where T : class, IEntity;

        /// <summary>
        ///     Gets all elements limited
        /// </summary>
        Task<IList<T>> GetAllAsync<T>( Int32 limit ) where T : class, IEntity;

        /// <summary>
        ///     Gets all elements sorted and limited
        /// </summary>
        IList<T> GetAll<T>( IMongoSortBy sortBy, Int32 limit ) where T : class, IEntity;

        /// <summary>
        ///     Gets all elements sorted and limited
        /// </summary>
        Task<IList<T>> GetAllAsync<T>( IMongoSortBy sortBy, Int32 limit ) where T : class, IEntity;

        /// <summary>
        ///     Gets the Entity by <see cref="ObjectId" />
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        IList<TEntity> GetAllByID( ObjectId id );

        /// <summary>
        ///     Gets the Entity by <see cref="ObjectId" />
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        Task<IList<TEntity>> GetAllByIDAsync( ObjectId id );

        /// <summary>
        ///     Gets the Entity by <see cref="ObjectId" />
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        IList<T> GetAllByID<T>( ObjectId id ) where T : class, IEntity;

        /// <summary>
        ///     Gets the Entity by <see cref="ObjectId" />
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        Task<IList<T>> GetAllByIDAsync<T>( ObjectId id ) where T : class, IEntity;

        /// <summary>
        ///     Returns all documents of current collection
        /// </summary>
        /// <returns>All documents</returns>
        IList<BsonDocument> GetAllDocuments( );

        /// <summary>
        ///     Returns all documents of current collection
        /// </summary>
        /// <returns>All documents</returns>
        Task<IList<BsonDocument>> GetAllDocumentsAsync( );

        /// <summary>
        ///     Gets the entity by passed ID
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        TEntity GetByID( ObjectId id );

        /// <summary>
        ///     Gets the entity by passed ID
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        Task<TEntity> GetByIDAsync( ObjectId id );

        /// <summary>
        ///     Gets the entity by passed ID
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        T GetByID<T>( ObjectId id ) where T : class,IEntity;

        /// <summary>
        ///     Gets the entity by passed ID
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        Task<T> GetByIDAsync<T>( ObjectId id ) where T : class,IEntity;

        /// <summary>
        ///     Gets the entities by passed IDs
        /// </summary>
        /// <param name="ids">Entity's IDs</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        IList<TEntity> GetByID( IEnumerable<ObjectId> ids );

        /// <summary>
        ///     Gets the entities by passed IDs
        /// </summary>
        /// <param name="ids">Entity's IDs</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        Task<IList<TEntity>> GetByIDAsync( IEnumerable<ObjectId> ids );

        /// <summary>
        ///     Gets the entities by passed IDs
        /// </summary>
        /// <param name="ids">Entity's IDs</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        IList<T> GetByID<T>( IEnumerable<ObjectId> ids ) where T : class,IEntity;

        /// <summary>
        ///     Gets the entities by passed IDs
        /// </summary>
        /// <param name="ids">Entity's IDs</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        Task<IList<T>> GetByIDAsync<T>( IEnumerable<ObjectId> ids ) where T : class,IEntity;

        /// <summary>
        ///     Gets the document by passed ID
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        BsonDocument GetDocumentByID( ObjectId id );

        /// <summary>
        ///     Gets the document by passed ID
        /// </summary>
        /// <param name="id">Entity's ID</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        Task<BsonDocument> GetDocumentByIDAsync( ObjectId id );

        /// <summary>
        ///     Gets the documents by passed IDs
        /// </summary>
        /// <param name="ids">Entity's IDs</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        IList<BsonDocument> GetDocumentsByID( IEnumerable<ObjectId> ids );

        /// <summary>
        ///     Gets the documents by passed IDs
        /// </summary>
        /// <param name="ids">Entity's IDs</param>
        /// <returns>
        ///     Entity or
        ///     <value>null</value>
        /// </returns>
        Task<IList<BsonDocument>> GetDocumentsByIDAsync( IEnumerable<ObjectId> ids );

        /// <summary>
        ///     Gets all matching elements
        /// </summary>
        IList<T> GetMany<T>( IMongoQuery query ) where T : class,IEntity;

        /// <summary>
        ///     Gets all matching elements
        /// </summary>
        Task<IList<T>> GetManyAsync<T>( IMongoQuery query ) where T : class,IEntity;

        /// <summary>
        ///     Gets the matched and sorted elements.
        /// </summary>
        IList<T> GetMany<T>( IMongoQuery query, IMongoSortBy sortBy ) where T : class,IEntity;

        /// <summary>
        ///     Gets the matched and sorted elements.
        /// </summary>
        Task<IList<T>> GetManyAsync<T>( IMongoQuery query, IMongoSortBy sortBy ) where T : class,IEntity;

        /// <summary>
        ///     Gets the matched and limited elements.
        /// </summary>
        IList<T> GetMany<T>( IMongoQuery query, Int32 limit ) where T : class,IEntity;

        /// <summary>
        ///     Gets the matched and limited elements.
        /// </summary>
        Task<IList<T>> GetManyAsync<T>( IMongoQuery query, int limit ) where T : class,IEntity;

        /// <summary>
        ///     Gets the matching elements sorted and limited
        /// </summary>
        IList<T> GetMany<T>( IMongoQuery query, IMongoSortBy sortBy, Int32 limit ) where T : class,IEntity;

        /// <summary>
        ///     Gets the matching elements sorted and limited
        /// </summary>
        Task<IList<T>> GetManyAsync<T>( IMongoQuery query, IMongoSortBy sortBy, int limit ) where T : class,IEntity;

        /// <summary>
        ///     Gets a collection of elements by given exception (take care, linq is slow and not fully supported by MongoDB C#
        ///     Driver!)
        /// </summary>
        /// <param name="where">The expression.</param>
        /// <returns>enumerable of entities</returns>
        IList<TEntity> GetMany( Expression<Func<TEntity, bool>> @where );

        /// <summary>
        ///     Gets a collection of elements by given exception (take care, linq is slow and not fully supported by MongoDB C#
        ///     Driver!)
        /// </summary>
        /// <param name="where">The expression.</param>
        /// <returns>enumerable of entities</returns>
        Task<IList<TEntity>> GetManyAsync( Expression<Func<TEntity, bool>> @where );

        /// <summary>
        ///     Updates given entity
        /// </summary>
        WriteConcernResult Update( TEntity entity );

        /// <summary>
        ///     Updates given entity
        /// </summary>
        Task<WriteConcernResult> UpdateAsync( TEntity entity );

        /// <summary>
        ///     Updates given document of collection
        /// </summary>
        WriteConcernResult Update( BsonDocument doc );

        /// <summary>
        ///     Updates given document of collection
        /// </summary>
        Task<WriteConcernResult> UpdateAsync( BsonDocument doc );

        /// <summary>
        ///     Empty disposing for easier code styling
        /// </summary>
        void Dispose( );

        /// <summary>
        ///     Gets fired if an typed entity has been added. Does not work for documents!
        /// </summary>
        event EventHandler<RepositoryEntityAddedEventArgs<TEntity>> OnEntityAdded;

        /// <summary>
        ///     Gets fired if an typed entity has been deleted. Does not work for documents!
        /// </summary>
        event EventHandler<RepositoryEntityDeletedEventArgs<TEntity>> OnEntityDeleted;
    }
}