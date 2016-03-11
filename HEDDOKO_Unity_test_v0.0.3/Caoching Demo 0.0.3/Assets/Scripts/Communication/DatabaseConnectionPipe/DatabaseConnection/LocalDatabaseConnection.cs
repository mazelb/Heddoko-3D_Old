
/** 
* @file LocalDatabaseConnection.cs
* @brief Contains the LocalDatabaseConnection class, implementing IDatabaseConnection interface
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using System;
using Assets.Scripts.Communication.DatabaseConnectionPipe.DatabaseQueries;
using JetBrains.Annotations;

namespace Assets.Scripts.Communication.DatabaseConnectionPipe.DatabaseConnection
{
    /// <summary>
    /// A connection to a local database. This connection will persist until the 
    /// </summary>
    public class LocalDatabaseConnection: IDatabaseConnection
    {
        
        
        public bool Equals([NotNull] IDatabaseConnection other)
        {
            if (other == null) throw new ArgumentNullException("other");
            throw new NotImplementedException();
        }

        public Guid UUID { get; private set; }

        public string DbConnectionUUID { get; private set; }

        public bool Connection(Action vCallback = null)
        {
            throw new NotImplementedException();
        }

        public void Disconnect(Action vCallback = null)
        {
            throw new NotImplementedException();
        }

        public bool Query(IDatabaseQuery vQuery, Action vCallback = null)
        {
            throw new NotImplementedException();
        }
    }
}
