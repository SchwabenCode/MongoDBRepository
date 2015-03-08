﻿// <copyright file="RepositoryEntityDeletedEventArgs.cs" company="Benjamin Abt ( http://www.benjamin-abt.com )">
//      Copyright (c) 2015 All Rights Reserved - DO NOT REMOVE OR EDIT COPYRIGHT
// </copyright>
// <author>
//      Benjamin Abt
// </author>
// <date>
//      2015, 8. March
// </date>

using System;

namespace SchwabenCode.MongoDBRepository
{
    /// <summary>
    /// Event of removed entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class RepositoryEntityDeletedEventArgs<TEntity> : EventArgs
    {
        /// <summary>
        /// The entity that has been removed
        /// </summary>
        public TEntity Entity { get; private set; }

        /// <summary>
        /// Creates the instance
        /// </summary>
        /// <param name="entity">The entity that has been removed</param>
        public RepositoryEntityDeletedEventArgs( TEntity entity )
        {
            this.Entity = entity;
        }
    }
}
