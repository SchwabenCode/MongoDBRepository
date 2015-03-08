﻿// <copyright file="MongoEntityValidatable.cs" company="Benjamin Abt ( http://www.benjamin-abt.com )">
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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace SchwabenCode.MongoDBRepository
{
    /// <summary>
    /// Frame class for all entities
    /// </summary>
    public abstract class MongoEntityValidatable : IValidatableObject, IMongoEntityValidatable
    {

        #region IsValid( out IEnumerable<ValidationResult> validationResults )
        /// <summary>
        /// Validations the entity
        /// </summary>
        /// <param name="validationResults">Errors</param>
        /// <returns>true if entity is valid</returns>
        public Boolean IsValid( out IEnumerable<ValidationResult> validationResults )
        {
            validationResults = Validate( );
            if ( validationResults != null )
            {
                return !validationResults.Any( );
            }

            throw new NotImplementedException( "Validate() implementation returns null. That's not allowed." );

        }
        #endregion

        #region IsValid( )
        /// <summary>
        /// Entity validation
        /// </summary>
        /// <returns><see cref="MongoEntityValidationResult"/></returns>
        public MongoEntityValidationResult IsValid()
        {
            Contract.Ensures( Contract.Result<MongoEntityValidationResult>( ) != null );

            return new MongoEntityValidationResult( Validate( ) );
        }
        /// <summary>
        /// Entity validation
        /// </summary>
        /// <returns><see cref="MongoEntityValidationResult"/></returns>
        public Task<MongoEntityValidationResult> IsValidAsync()
        {
            Contract.Ensures( Contract.Result<MongoEntityValidationResult>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( IsValid );
        }
        #endregion

        #region Validate()
        /// <summary>
        /// Please implement your validate logic
        /// </summary>
        /// <returns>Collection of errors. Null is not allowed.</returns>
        public abstract IEnumerable<ValidationResult> Validate();

        /// <summary>
        /// Async validation
        /// </summary>
        /// <returns>Collection of errors. Null is not allowed.</returns>
        public Task<IEnumerable<ValidationResult>> ValidateAsync()
        {
            Contract.Ensures( Contract.Result<Task<IEnumerable<ValidationResult>>>( ) != null );
            return AsyncAll.AsyncAll.GetAsyncResult( Validate );
        }
        #endregion Validate()

        #region Validate( ValidationContext validationContext )
        /// <summary>
        /// Implementation of <see cref="IValidatableObject"/>
        /// </summary>
        /// <param name="validationContext">Context</param>
        /// <returns>Collection of errors. Null is not allowed.</returns>
        public IEnumerable<ValidationResult> Validate( ValidationContext validationContext )
        {
            Contract.Requires( validationContext != null );
            Contract.Ensures( Contract.Result<IEnumerable<ValidationResult>>( ) != null );

            return Validate( );
        }
        /// <summary>
        /// Implementation of <see cref="IValidatableObject"/>
        /// </summary>
        /// <param name="validationContext">Context</param>
        /// <returns>Collection of errors. Null is not allowed.</returns>
        public Task<IEnumerable<ValidationResult>> ValidateAsync( ValidationContext validationContext )
        {
            Contract.Requires( validationContext != null );
            Contract.Ensures( Contract.Result<Task<IEnumerable<ValidationResult>>>( ) != null );

            return AsyncAll.AsyncAll.GetAsyncResult( () => Validate( validationContext ) );
        }
        #endregion Validate( ValidationContext validationContext )

        /// <summary>
        /// Creates an error
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="args">format elements</param>
        /// <returns><see cref="ValidationResult"/></returns>
        protected ValidationResult NewError( String message, params object[ ] args )
        {
            Contract.Requires( !String.IsNullOrEmpty( message ) );
            Contract.Ensures( Contract.Result<ValidationResult>( ) != null );

            return new ValidationResult( String.Format( message, args ) );
        }
    }
}