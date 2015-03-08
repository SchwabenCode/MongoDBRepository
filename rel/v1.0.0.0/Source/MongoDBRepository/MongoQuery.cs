﻿// <copyright file="MongoQuery.cs" company="Benjamin Abt ( http://www.benjamin-abt.com )">
//      Copyright (c) 2015 All Rights Reserved - DO NOT REMOVE OR EDIT COPYRIGHT
// </copyright>
// <author>
//      Benjamin Abt
// </author>
// <date>
//      2015, 8. March
// </date>

using System;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using MongoDB.Bson;

namespace SchwabenCode.MongoDBRepository
{
    /// <summary>
    /// Several helpers to build MongoDB queries
    /// </summary>
    public static class MongoQuery
    {
        /// <summary>
        /// Combines field names
        /// </summary>
        /// <returns>fieldA.fieldB</returns>
        public static string CombineFields( String fieldA, String fieldB )
        {
            Contract.Requires( !String.IsNullOrEmpty( fieldA ) );
            Contract.Requires( !String.IsNullOrEmpty( fieldB ) );

            return fieldA + "." + fieldB;
        }

        /// <summary>
        /// Checks whether the string is a valid ObjectId
        /// </summary>
        public static Boolean IsValidObjectId( String id )
        {
            Contract.Requires( !String.IsNullOrEmpty( id ) );

            ObjectId parsedId;
            return ObjectId.TryParse( id, out parsedId );
        }
        /// <summary>
        /// Query for string search wit ignoring case
        /// </summary>
        public static String QueryExactPattern( String text )
        {
            Contract.Requires( !String.IsNullOrEmpty( text ) );

            return "^" + text + "$";
        }

        /// <summary>
        /// Equal query - case-insenstivity
        /// </summary>
        public static BsonRegularExpression Equals( String text, bool isCaseSensetive = false )
        {
            Contract.Requires( !String.IsNullOrEmpty( text ) );

            var match = QueryExactPattern( text );
            if ( isCaseSensetive )
            {
                return new BsonRegularExpression( new Regex( match ) );
            }

            return new BsonRegularExpression( new Regex( match, RegexOptions.IgnoreCase ) );
        }

        /// <summary>
        /// Equal query - case-insenstivity
        /// </summary>
        public static BsonRegularExpression Contains( String text, bool isCaseSensetive = false )
        {
            Contract.Requires( !String.IsNullOrEmpty( text ) );

            if ( isCaseSensetive )
            {
                return new BsonRegularExpression( new Regex( text ) );
            }

            return new BsonRegularExpression( new Regex( text, RegexOptions.IgnoreCase ) );
        }

        /// <summary>
        /// StartsWith query - case-senstivity
        /// </summary>
        public static BsonRegularExpression StartsWith( String text, bool isCaseSensetive = false )
        {
            Contract.Requires( !String.IsNullOrEmpty( text ) );

            var match = "^" + text;
            if ( isCaseSensetive )
            {
                return new BsonRegularExpression( new Regex( match ) );
            }

            return new BsonRegularExpression( new Regex( match, RegexOptions.IgnoreCase ) );
        }

        /// <summary>
        /// EndsWith query - case-senstivity
        /// </summary>
        public static BsonRegularExpression EndsWith( String text, bool isCaseSensetive = false )
        {
            Contract.Requires( !String.IsNullOrEmpty( text ) );

            var match = text + "$";
            if ( isCaseSensetive )
            {
                return new BsonRegularExpression( new Regex( match ) );
            }

            return new BsonRegularExpression( new Regex( match, RegexOptions.IgnoreCase ) );
        }
    }
}
