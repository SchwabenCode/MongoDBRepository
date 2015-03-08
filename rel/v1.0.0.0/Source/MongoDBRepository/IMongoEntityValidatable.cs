﻿// <copyright file="IMongoEntityValidatable.cs" company="Benjamin Abt ( http://www.benjamin-abt.com )">
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
using System.Threading.Tasks;
using MongoDB.Bson;

namespace SchwabenCode.MongoDBRepository
{
    /// <summary>
    /// Frame Interface
    /// </summary>
    public interface IMongoEntityValidatable : IMongoEntity
    {
        /// <summary>
        /// Validations the entity
        /// </summary>
        /// <param name="validationResults">Errors</param>
        /// <returns>true if entity is valid</returns>
        Boolean IsValid( out IEnumerable<ValidationResult> validationResults );

        /// <summary>
        /// Entity validation
        /// </summary>
        /// <returns><see cref="MongoEntityValidationResult"/></returns>
        MongoEntityValidationResult IsValid();

        /// <summary>
        /// Entity validation
        /// </summary>
        /// <returns><see cref="MongoEntityValidationResult"/></returns>
        Task<MongoEntityValidationResult> IsValidAsync();

        /// <summary>
        /// Please implement your validate logic
        /// </summary>
        /// <returns>Collection of errors. Null is not allowed.</returns>
        IEnumerable<ValidationResult> Validate();

        /// <summary>
        /// Async validation
        /// </summary>
        /// <returns>Collection of errors. Null is not allowed.</returns>
        Task<IEnumerable<ValidationResult>> ValidateAsync();

        /// <summary>
        /// Implementation of <see cref="IValidatableObject"/>
        /// </summary>
        /// <param name="validationContext">Context</param>
        /// <returns>Collection of errors. Null is not allowed.</returns>
        IEnumerable<ValidationResult> Validate( ValidationContext validationContext );

        /// <summary>
        /// Implementation of <see cref="IValidatableObject"/>
        /// </summary>
        /// <param name="validationContext">Context</param>
        /// <returns>Collection of errors. Null is not allowed.</returns>
        Task<IEnumerable<ValidationResult>> ValidateAsync( ValidationContext validationContext );
    }
}