﻿// <copyright file="MongoValidatableRepository.cs" company="Benjamin Abt ( http://www.benjamin-abt.com )">
//      Copyright (c) 2015 All Rights Reserved - DO NOT REMOVE OR EDIT COPYRIGHT
// </copyright>
// <author>
//      Benjamin Abt
// </author>
// <date>
//      2015, 8. March
// </date>

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using MongoDB.Driver;

namespace SchwabenCode.MongoDBRepository
{
    public class MongoValidatableRepository<TEntity> : MongoRepository<TEntity, TEntity> where TEntity : class, IMongoEntityValidatable
    {
        /// <summary>
        /// Creates an instance
        /// </summary>
        /// <param name="uow">Attached Unit of Work Container</param>
        /// <param name="idFieldName">Name of the property represents the entity key.</param>
        /// <remarks>Uses the pluralized name of the given entity as collection name</remarks>
        public MongoValidatableRepository( IMongoUnitOfWork uow, string idFieldName )
            : base( uow, idFieldName )
        {
        }

        /// <summary>
        /// Creates a default instance
        /// </summary>
        /// <param name="uow">Attached Unit of Work Container</param>
        /// <param name="idFieldName">Name of the property represents the entity key.</param>
        /// <param name="collectionName">Name of table/collection</param>
        public MongoValidatableRepository( IMongoUnitOfWork uow, string idFieldName, string collectionName )
            : base( uow, idFieldName, collectionName )
        {
        }

        /// <summary>
        /// Creates an instance
        /// </summary>
        /// <param name="uow">Attached Unit of Work Container</param>
        /// <param name="idFieldName">Name of the property represents the entity key.</param>
        /// <param name="collectionNamePluralize">Appends an 's' after the entity's name to use this as collection name</param>
        public MongoValidatableRepository( IMongoUnitOfWork uow, string idFieldName, bool collectionNamePluralize )
            : base( uow, idFieldName, collectionNamePluralize )
        {
        }
    }

    /// <summary>
    ///     MongoDB Base Repository with entity validation
    /// </summary>
    /// <typeparam name="TEntity">Main Entity Type</typeparam>
    /// <typeparam name="IEntity">Interface Scope</typeparam>
    public class MongoValidatableRepository<TEntity, IEntity> : MongoRepository<TEntity, IEntity> where TEntity : class, IMongoEntityValidatable, IEntity
    {

        #region Standard Repository Methods

        #region Add

        /// <summary>
        /// Creates an instance
        /// </summary>
        /// <param name="uow">Attached Unit of Work Container</param>
        /// <param name="idFieldName">Name of the property represents the entity key.</param>
        public MongoValidatableRepository( IMongoUnitOfWork uow, string idFieldName )
            : base( uow, idFieldName )
        {
        }

        /// <summary>
        /// Creates an instance
        /// </summary>
        /// <param name="uow">Attached Unit of Work Container</param>
        /// <param name="idFieldName">Name of the property represents the entity key.</param>
        /// <param name="collectionNamePluralize">Appends an 's' after the entity's name to use this as collection name</param>
        public MongoValidatableRepository( IMongoUnitOfWork uow, string idFieldName, bool collectionNamePluralize )
            : base( uow, idFieldName, collectionNamePluralize )
        {
        }

        /// <summary>
        /// Creates a default instance
        /// </summary>
        /// <param name="uow">Attached Unit of Work Container</param>
        /// <param name="idFieldName">Name of the property represents the entity key.</param>
        /// <param name="tableName">Name of table/collection</param>
        public MongoValidatableRepository( IMongoUnitOfWork uow, string idFieldName, string tableName )
            : base( uow, idFieldName, tableName )
        {
        }

        /// <summary>
        ///     Adds the passed entity to current collection
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>
        ///     <see cref="WriteConcernResult" />
        /// </returns>
        /// <exception cref="MongoInvalidEntityException">if entity is invalid</exception>
        public new virtual WriteConcernResult Add( TEntity entity )
        {
            Contract.Requires( entity != null );
            Contract.Ensures( Contract.Result<WriteConcernResult>( ) != null );

            ThrowOnInvalidEntity( entity );
            return base.Add( entity );
        }


        /// <summary>
        ///     Updates an existing entity or adds if new
        /// </summary>
        public new virtual WriteConcernResult AddOrUpdate( TEntity entity )
        {
            Contract.Requires( entity != null );
            Contract.Ensures( Contract.Result<WriteConcernResult>( ) != null );

            ThrowOnInvalidEntity( entity );
            return base.AddOrUpdate( entity );
        }

        #endregion


        #region Update
        /// <summary>
        ///     Updates given entity
        /// </summary>
        public new virtual WriteConcernResult Update( TEntity entity )
        {
            Contract.Requires( entity != null );
            Contract.Ensures( Contract.Result<WriteConcernResult>( ) != null );

            ThrowOnInvalidEntity( entity );
            return base.Update( entity );
        }
        #endregion

        #endregion

        #region Validation

        #region ThrowOnInvalidEntity( TEntity entity )
        /// <summary>
        ///     Validates an entity.
        /// </summary>
        /// <exception cref="MongoInvalidEntityException">is thrown if invalid</exception>
        private static void ThrowOnInvalidEntity( TEntity entity )
        {
            Contract.Requires( entity != null );

            IEnumerable<ValidationResult> errors;
            if ( !entity.IsValid( out errors ) )
            {
                throw new MongoInvalidEntityException( errors );
            }
        }

        #endregion ThrowOnInvalidEntity( TEntity entity )
        #endregion

    }
}