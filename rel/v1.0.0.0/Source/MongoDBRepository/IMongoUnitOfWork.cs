﻿// <copyright file="IMongoUnitOfWork.cs" company="Benjamin Abt ( http://www.benjamin-abt.com )">
//      Copyright (c) 2015 All Rights Reserved - DO NOT REMOVE OR EDIT COPYRIGHT
// </copyright>
// <author>
//      Benjamin Abt
// </author>
// <date>
//      2015, 8. March
// </date>

using MongoDB.Driver;

namespace SchwabenCode.MongoDBRepository
{
    /// <summary>
    /// Unit of Work pattern for MongoDB DBMS
    /// </summary>
    public interface IMongoUnitOfWork
    {
        /// <summary>
        /// Context
        /// </summary>
        MongoDatabase Context { get; }
    }
}