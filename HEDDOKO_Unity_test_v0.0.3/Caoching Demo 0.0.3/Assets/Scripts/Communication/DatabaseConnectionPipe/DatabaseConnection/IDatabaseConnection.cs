﻿
/** 
* @file IDatabaseConnection.cs
* @brief Contains the IDatabaseConnection interface
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.Communication.DatabaseConnectionPipe.DatabaseQueries; 
namespace Assets.Scripts.Communication.DatabaseConnectionPipe.DatabaseConnection
{
    /// <summary>
    /// Provides an interface for database connections 
    /// </summary>
   public interface IDatabaseConnection : IEquatable<IDatabaseConnection>
    {
       /// <summary>
       /// Unique id
       /// </summary>
        string DbConnectionUUID { get; }

        /// <summary>
        /// Connect to the database
        /// </summary>
        /// <param name="vCallback">(optional)Action to invoke on result of the connection</param> 
        bool Connection(Action vCallback = null);

        /// <summary>
        /// Disconnect from the database
        /// </summary>
        /// <param name="vCallback">(optional) action to invoke after disconnection</param>

        void Disconnect(Action vCallback = null);


        /// <summary>
        /// Query the database
        /// </summary>
        /// <param name="vQuery">Query to make to the database </param>
        /// <param name="vCallback">optional callback on results</param>
        /// <returns></returns>
        bool Query(IDatabaseQuery vQuery, Action vCallback = null);

    }
}
