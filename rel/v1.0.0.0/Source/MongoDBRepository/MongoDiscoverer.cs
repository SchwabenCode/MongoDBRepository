﻿// <copyright file="MongoDiscoverer.cs" company="Benjamin Abt ( http://www.benjamin-abt.com )">
//      Copyright (c) 2015 All Rights Reserved - DO NOT REMOVE OR EDIT COPYRIGHT
// </copyright>
// <author>
//      Benjamin Abt
// </author>
// <date>
//      2015, 8. March
// </date>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MongoDB.Bson;

namespace SchwabenCode.MongoDBRepository
{

    /// <summary>
    /// Serveral MongoDB Extensions
    /// </summary>
    public static class MongoDiscoverer
    {
        /// <summary>
        /// MongoDB supports not more than 100 levels of nested documents.
        /// </summary>
        /// <remarks>http://docs.mongodb.org/manual/reference/limits/</remarks>
        public const Int32 MaxMongoDocumentDepth = 100;


        /// <summary>
        /// Simples Types. This types are important to determine the depth of bson objects
        /// </summary>
        public static readonly Type[ ] SimpleTypes =
        {
            typeof( String ),
            typeof( ObjectId ), 
            typeof( BsonObjectId ), 
            //typeof( Decimal ), = ValueType
            //typeof( DateTime ), = ValueType
            //typeof( DateTime ), = ValueType
            //typeof( TimeSpan ), = ValueType
            //typeof( Guid ), = ValueType
            typeof( Enum )
        };

        /// <summary>
        /// Returns the value of given property name
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="source">Affected Object</param>
        /// <param name="propertyName">Name of Property</param>
        /// <returns>Returns the value of given property</returns>
        /// <exception cref="InvalidCastException">if type is incompatible</exception>
        public static T GetPropertyValue<T>( this Object source, String propertyName )
        {
            Object retval = GetPropertyValue( source, propertyName );
            if ( retval == null ) { return default( T ); }

            // throws InvalidCastException if types are incompatible
            return ( T ) retval;
        }

        /// <summary>
        /// Returns the value of given property name
        /// </summary>
        /// <param name="source">Affected Object</param>
        /// <param name="propertyName">Name of Property</param>
        /// <returns>Returns the value of given property</returns>
        public static Object GetPropertyValue( this Object source, String propertyName )
        {
            foreach ( String part in propertyName.Split( '.' ) )
            {
                if ( source == null ) { return null; }

                Type type = source.GetType( );
                PropertyInfo info = type.GetProperty( part );
                if ( info == null ) { return null; }

                source = info.GetValue( source, null );
            }
            return source;
        }


        /// <summary>
        /// Returns all property names of given member and expression
        /// </summary>
        public static String GetMemberPropertyNames<T>( Expression<Func<T>> expr ) where T : class
        {
            Contract.Requires( expr != null );
            Contract.Ensures( !String.IsNullOrEmpty( Contract.Result<string>( ) ) );
            return ( ( MemberExpression ) expr.Body ).Member.Name;
        }




        #region Field Automatisms

        /// <summary>
        ///     Field cache
        /// </summary>
        private static readonly IDictionary<Type, IEnumerable<string>> FieldCache = new Dictionary<Type, IEnumerable<string>>( );

        #endregion

        /// <summary>
        /// Tries to get cached property names of given type
        /// </summary>
        /// <param name="key">Type</param>
        /// <param name="propertyNames">Property names</param>
        /// <returns>True if key is in cache</returns>
        private static Boolean TryGetFieldCacheKey( Type key, out IEnumerable<string> propertyNames )
        {
            lock ( FieldCache )
            {
                if ( FieldCache.ContainsKey( key ) )
                {
                    propertyNames = FieldCache[ key ];
                    return true;
                }
            }

            propertyNames = null;
            return false;

        }
        /// <summary>
        /// Adds property names to cache
        /// </summary>
        /// <param name="key">Type</param>
        /// <param name="propertyNames">Property names</param>
        private static void FieldCacheAdd( Type key, IEnumerable<string> propertyNames )
        {
            lock ( FieldCache )
            {
                if ( !FieldCache.ContainsKey( key ) )
                {
                    FieldCache.Add( key, propertyNames );
                }
            }

        }

        /// <summary>
        /// Returns all property names of given type
        /// </summary>
        /// <returns>array of public property names</returns>
        public static IEnumerable<string> GetEntityFieldNames<T>()
        {
            Contract.Ensures( Contract.Result<string[ ]>( ) != null );
            return GetEntityFieldNames( typeof( T ) );
        }

        /// <summary>
        ///     Gets field names of passed type
        /// </summary>
        /// <param propertyName="type">Type</param>
        /// <returns>collection of public field names (=> property names)</returns>
        public static IEnumerable<string> GetEntityFieldNames( Type type )
        {
            Contract.Requires( type != null );
            Contract.Ensures( Contract.Result<IEnumerable<string>>( ) != null );

            if ( type.IsClass && !IsDiscoverable( type ) )
            {
                yield break;
            }


            IEnumerable<string> typeFieldNames;
            if ( TryGetFieldCacheKey( type, out typeFieldNames ) )
            {
                if ( typeFieldNames == null )
                {
                    yield break;
                }

                foreach ( var element in typeFieldNames )
                {
                    yield return element;

                }
            }
            else
            {

                IList<string> propNameCache = new List<string>( );

                foreach ( PropertyInfo prop in GetFields( type ).Where( x => x != null ) )
                {
                    // Prüfen, ob es sich um einen simplen Typ handeln
                    // dann brauchen wir nur den Feldnamen verwenden

                    HandleType( prop.PropertyType, prop.Name, ref propNameCache );
                }

                // Zum Cache hinzufügen
                FieldCacheAdd( type, propNameCache );

                foreach ( var element in propNameCache )
                {
                    yield return element;
                }
            }
        }




        /// <summary>
        /// Checks value field type 
        /// </summary>
        public static Boolean IsSimpleField( Type t )
        {
            Contract.Requires( t != null );

            return ( t.IsValueType || t.IsPrimitive || t.IsEnum || SimpleTypes.Contains( t ) );
        }

        /// <summary>
        /// Checks for generic // array
        /// </summary>
        public static Boolean IsComplexField( Type t )
        {
            Contract.Requires( t != null );

            return ( t.IsClass || t.IsArray || typeof( IEnumerable ).IsAssignableFrom( t ) );
        }

        /// <summary>
        /// Checks specified whether type where a discoverable entity.
        /// </summary>
        /// <remarks>A partial entity is assignable from <see cref="IMongoDiscoverable"/></remarks>
        public static Boolean IsDiscoverable<T>()
        {
            return ( typeof( IMongoDiscoverable ).IsAssignableFrom( typeof( T ) ) );
        }
        /// <summary>
        /// Checks specified whether type where a discoverable entity.
        /// </summary>
        /// <remarks>A partial entity is assignable from <see cref="IMongoDiscoverable"/></remarks>
        public static Boolean IsDiscoverable( Type type )
        {
            return ( typeof( IMongoDiscoverable ).IsAssignableFrom( type ) );
        }

        /// <summary>
        /// Checks value field type and pushes the field names into the passed property name list
        /// </summary>
        /// <exception cref="NotSupportedException">on unknown type</exception>
        private static void HandleType( Type type, String fieldName, ref IList<string> propNameCache )
        {
            if ( IsSimpleField( type ) )
            {
                propNameCache.Add( fieldName );
            }
            else if ( IsComplexField( type ) )
            {
                HandleComplexType( type, fieldName, ref propNameCache );
            }
            else
            {
                throw new NotSupportedException( "Unknown type " + type + ". Unable to get fields." );
            }
        }


        /// <summary>
        /// Processes a complex class and pushes the property names into the cache
        /// </summary>
        /// <param name="type">Affected Type</param>
        /// <param name="propertyName">Name of the parent field</param>
        /// <param name="propNameCache">name cache</param>
        /// <exception cref="NotSupportedException">on unknown type</exception>
        private static void HandleComplexType( Type type, String propertyName, ref IList<string> propNameCache )
        {
            Contract.Requires( type != null );
            Contract.Requires( !String.IsNullOrEmpty( propertyName ) );
            Contract.Requires( propNameCache != null );

            // Check if the passed type is an array or a collection.
            // The type of an array can be complex (class, other array) or simple (primitive)
            if ( type.IsArray || typeof( IEnumerable ).IsAssignableFrom( type ) )
            {
                Type[ ] genArgs = type.GetGenericArguments( );
                if ( !genArgs.Any( ) )
                {
                    // There is no argument. It must be an array.
                    propNameCache.Add( propertyName );
                }
                else if ( genArgs.Count( ) == 1 ) // Only one type is supported
                {
                    HandleType( genArgs.First( ), propertyName, ref propNameCache );
                }
                else
                {
                    throw new NotSupportedException( "Multiple generic arguments are not supported." );
                }
            }
            else if ( type.IsClass )
            {
                if ( IsDiscoverable( type ) )
                {
                    PushEmbeddedFieldNamesOfComplexType( type, propertyName, ref propNameCache );
                }
                else
                {
                    propNameCache.Add( propertyName );
                }
            }
            else
            {
                throw new NotSupportedException( "Unknown complex type '" + type + "'" );
            }
        }


        /// <summary>
        /// Determines all field names of the complex type and add it to the cache
        /// </summary>
        /// <param name="type">The type of the field names are to be determined.</param>
        /// <param name="parentName">Parent name for the combination of field names</param>
        /// <param name="propNameCache">Field name cache</param>
        public static void PushEmbeddedFieldNamesOfComplexType( Type type, String parentName, ref IList<string> propNameCache )
        {
            foreach ( var fieldName in GetEntityFieldNames( type ).Select( innerFieldName => MongoQuery.CombineFields( parentName, innerFieldName ) ) )
            {
                propNameCache.Add( fieldName );
            }
        }


        /// <summary>
        ///     Gets public field of given type
        /// </summary>
        /// <param name="type">type</param>
        /// <returns>all public fields as property info</returns>
        public static PropertyInfo[ ] GetFields( Type type )
        {
            Contract.Requires( type != null );
            Contract.Ensures( Contract.Result<PropertyInfo[ ]>( ) != null );

            return type.GetProperties( BindingFlags.Public | BindingFlags.Instance );
        }

        /// <summary>
        /// Gets the FieldName of the given MongoDBBaseEntity by expression (avoids magic strings).
        /// </summary>
        /// <typeparam name="T">MongoDBBaseEntity</typeparam>
        /// <param name="expr">Expression</param>
        /// <returns>Returns the given PropertyName</returns>
        /// <example>var nameField = MyEntity.GetFieldName( () => </example>
        public static string GetFieldName<T>( Expression<Func<T, object>> expr )
        {
            Contract.Requires( expr != null );
            Contract.Ensures( !String.IsNullOrEmpty( Contract.Result<string>( ) ) );

            var body = expr.Body as MemberExpression;
            if ( body != null )
            {
                return body.Member.Name;
            }

            var op = ( ( UnaryExpression ) expr.Body ).Operand;
            return ( ( MemberExpression ) op ).Member.Name;
        }

        /// <summary>
        /// Gets the property of given class
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="expr">Property expression</param>
        /// <returns>Property's name</returns>
        public static String PropName<T>( Expression<Func<T>> expr ) where T : class
        {
            Contract.Requires( expr != null );
            Contract.Ensures( Contract.Result<string>( ) != null );

            return ( ( MemberExpression ) expr.Body ).Member.Name;
        }
    }

}
