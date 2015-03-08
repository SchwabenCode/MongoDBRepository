﻿// <copyright file="MongoExtensions.cs" company="Benjamin Abt ( http://www.benjamin-abt.com )">
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
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace SchwabenCode.MongoDBRepository
{

    /// <summary>
    /// Serveral MongoDB Extensions
    /// </summary>
    public static class MongoExtensions
    {
        /// <summary>
        /// Returns all documents
        /// </summary>
        public static IEnumerable<BsonDocument> All( this MongoCollection<BsonDocument> source )
        {
            Contract.Requires( source != null );
            Contract.Ensures( Contract.Result<IEnumerable<BsonDocument>>( ) != null );

            return source.FindAll( ).All( );
        }

        /// <summary>
        /// Returns all documents
        /// </summary>
        public static IEnumerable<BsonDocument> All( this IEnumerable<BsonDocument> source )
        {
            Contract.Requires( source != null );
            Contract.Ensures( Contract.Result<IEnumerable<BsonDocument>>( ) != null );

            return source.Where( x => x != null && x.IsBsonDocument );
        }

        /// <summary>
        /// Converts a String to an ObjectId
        /// </summary>
        public static ObjectId AsObjectId( this string source )
        {
            Contract.Requires( !String.IsNullOrEmpty( source ) );
            Contract.Ensures( Contract.Result<ObjectId>( ) != default( ObjectId ) );

            return new ObjectId( source );
        }

        /// <summary>
        /// Converts an nullable ObjectId to an ObjectId
        /// </summary>
        public static ObjectId AsObjectId( this ObjectId? source )
        {
            Contract.Requires( source != null );
            Contract.Ensures( Contract.Result<ObjectId>( ) != default( ObjectId ) );
            return ( ObjectId ) source;
        }

        /// <summary>
        /// Checks whether the given value is not null
        /// </summary>
        public static Boolean HasValue( this BsonValue source )
        {
            Contract.Requires( source != null );
            return source != null && !source.IsBsonNull;
        }

        /// <summary>
        /// Sets the value by given expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void SetValue<T>( this BsonDocument doc, Expression<Func<T>> expr, T defaultValue = default(T) )
        {
            Contract.Requires( expr != null );
            Contract.Requires( doc != null );

            // Expression to get member name

            var body = ( ( MemberExpression ) expr.Body );
            MemberInfo member = body.Member;
            var propName = body.Member.Name;

            // get value
            var value = doc.GetBsonValueByName( propName );

            // Try to cast
            var propType = typeof( T );
            var propInfo = ( ( PropertyInfo ) member );

            #region String

            #endregion
            if ( propType == typeof( String ) )
            {
                if ( value != null && value.IsString )
                {
                    propInfo.SetValue( member, value.AsString, null );
                }
                else
                {
                    propInfo.SetValue( member, defaultValue, null );
                }
            }


            throw new NotSupportedException( "Unknown type " + propType );
        }

        /// <summary>
        /// Gets the value by given expression
        /// </summary>
        public static T GetValue<T>( this BsonDocument doc, Expression<Func<T>> expr, T defaultValue = default(T) )
        {
            Contract.Requires( expr != null );
            Contract.Requires( doc != null );

            // return if null
            if ( doc.IsBsonNull )
            {
                return defaultValue;
            }

            // Expression to get member name

            var body = ( ( MemberExpression ) expr.Body );
            var propName = body.Member.Name;

            // get value
            var value = doc.GetBsonValueByName( propName );
            if ( value == null )
            {
                return defaultValue;
            }

            // Try to cast
            var propType = typeof( T );

            #region String
            #endregion
            if ( propType == typeof( String ) )
            {
                if ( !value.HasValue( ) || !value.IsString )
                {
                    return defaultValue;
                }
                return ( T ) Convert.ChangeType( value.AsString, typeof( T ) );
            }
            #region Int32
            if ( propType == typeof( Int32 ) )
            {
                if ( !value.IsInt32 )
                {
                    return defaultValue;
                }
                return ( T ) Convert.ChangeType( value.AsInt32, typeof( T ) );
            }
            #endregion
            #region Int64
            if ( propType == typeof( Int64 ) )
            {
                if ( !value.IsInt64 )
                {
                    return defaultValue;
                }
                return ( T ) Convert.ChangeType( value.AsInt64, typeof( T ) );
            }
            #endregion
            #region Boolean

            if ( propType == typeof( Boolean ) )
            {
                if ( !value.IsBoolean )
                {
                    return defaultValue;
                }
                return ( T ) Convert.ChangeType( value.AsBoolean, typeof( T ) );
            }
            if ( propType == typeof( Boolean? ) )
            {
                if ( !value.IsBoolean )
                {
                    return defaultValue;
                }
                return ( T ) Convert.ChangeType( value.AsNullableBoolean, typeof( T ) );
            }
            #endregion
            #region ObjectId
            if ( propType == typeof( ObjectId ) )
            {
                if ( !value.IsObjectId )
                {
                    return defaultValue;
                }
                return ( T ) Convert.ChangeType( value.AsObjectId, typeof( T ) );
            }
            if ( propType == typeof( ObjectId? ) )
            {
                if ( !value.IsObjectId )
                {
                    return defaultValue;
                }
                return ( T ) Convert.ChangeType( value.AsNullableObjectId, typeof( T ) );
            }
            #endregion

            throw new NotSupportedException( "Unknown type " + propType );
        }


        /// <summary>
        /// returns the value by field name
        /// </summary>
        private static BsonValue GetBsonValueByName( this BsonDocument doc, string propName )
        {
            Contract.Requires( doc != null );
            Contract.Requires( !String.IsNullOrEmpty( propName ) );
            return doc.GetValue( propName, null );
        }

        /// <summary>
        /// Returns the string by field name
        /// </summary>
        public static String GetStringValue( this BsonDocument doc, String name, String defaultValue = null )
        {
            Contract.Requires( doc != null );
            Contract.Requires( !String.IsNullOrEmpty( name ) );

            try
            {
                if ( doc.IsBsonNull )
                {
                    return defaultValue;
                }

                BsonValue value = doc.GetValue( name, null );
                if ( !value.HasValue( ) || !value.IsString )
                {
                    return defaultValue;
                }


                return value.AsString;
            }
            catch ( Exception )
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the given value as local datetime
        /// </summary>
        public static DateTime AsLocalDateTime( this BsonValue source )
        {
            Contract.Requires( source != null );

            return source.ToLocalTime( );
        }

        /// <summary>
        /// Returns the given value as local datetime or null if value is no datetime
        /// </summary>
        public static DateTime? AsNullableLocalDateTime( this BsonValue source )
        {

            if ( source == BsonNull.Value )
            {
                return null;
            }
            return source.ToLocalTime( );
        }

        /// <summary>
        /// Returns the given value as ObjectId
        /// </summary>
        public static ObjectId? AsNullableObjectId( this BsonValue source )
        {
            if ( source == BsonNull.Value )
            {
                return null;
            }
            return source.AsObjectId;
        }


    }
}
