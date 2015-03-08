﻿// <copyright file="MongoInvalidEntityException.cs" company="Benjamin Abt ( http://www.benjamin-abt.com )">
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

namespace SchwabenCode.MongoDBRepository
{
    /// <summary>
    /// Gets fired when an entity validation fails
    /// </summary>
    public class MongoInvalidEntityException : Exception
    {
        /// <summary>
        /// Validation errors
        /// </summary>
        public IEnumerable<ValidationResult> Errors { get; private set; }

        /// <summary>
        /// Gets fired when an entity validation fails
        /// </summary>
        /// <param name="errors">Collection of errors</param>
        public MongoInvalidEntityException( IEnumerable<ValidationResult> errors )
        {
            Errors = errors;
        }
    }
}