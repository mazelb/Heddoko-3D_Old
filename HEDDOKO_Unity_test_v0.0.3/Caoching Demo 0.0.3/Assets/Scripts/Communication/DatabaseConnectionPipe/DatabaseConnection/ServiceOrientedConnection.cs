
/** 
* @file ServiceOrientedConnection.cs
* @brief Contains the ServiceOrientedConnection class, implementing IDatabaseConnection interface
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using System; 

namespace Assets.Scripts.Communication.DatabaseConnectionPipe.DatabaseConnection
{
    /// <summary>
    /// A service oriented connection, this connects to a local database service that handles
    /// all database connections and results. //todo
    /// </summary>
    public class ServiceOrientedConnection: IDatabaseConnection
    {

        private Guid mGuid = new Guid();
        public bool Equals(IDatabaseConnection other)
        {
            throw new NotImplementedException();
        }

 

        public string DbConnectionUUID { get; private set; }

        public bool Connect(Action vCallback = null)
        {
            throw new NotImplementedException();
        }

        public void Disconnect(Action vCallback = null)
        {
            throw new NotImplementedException();
        }

        public bool Query(string vQuery, Action vCallback = null)
        {
            throw new NotImplementedException();
        }

        public bool CreateRecording(BodyFramesRecording vRecording)
        {
            throw new NotImplementedException();
        }
    }
}