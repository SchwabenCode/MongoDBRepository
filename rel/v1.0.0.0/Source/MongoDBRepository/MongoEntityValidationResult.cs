﻿// <copyright file="MongoEntityValidationResult.cs" company="Benjamin Abt ( http://www.benjamin-abt.com )">
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

namespace SchwabenCode.MongoDBRepository
{
    /// <summary>
    /// Validation result container
    /// </summary>
    public sealed class MongoEntityValidationResult
    {
        /// <summary>
        /// Creates container
        /// </summary>
        /// <param name="results">result collection</param>
        public MongoEntityValidationResult( IEnumerable<ValidationResult> results )
        {
            Contract.Requires( results != null );

            _results = results;
        }

        private IEnumerable<ValidationResult> _results;

        /// <summary>
        /// Indicates whether the validation was successfully
        /// </summary>
        public Boolean IsValid
        {
            get { return _results != null && !_results.Any( ); }
        }

        /// <summary>
        /// Error Collection. 
        /// </summary>
        public IEnumerable<ValidationResult> Results
        {
            get
            {
                Contract.Ensures( Contract.Result<IEnumerable<ValidationResult>>( ) != null );
                return _results;
            }
            set
            {
                Contract.Requires( value != null );
                _results = value;
            }
        }
    }
}