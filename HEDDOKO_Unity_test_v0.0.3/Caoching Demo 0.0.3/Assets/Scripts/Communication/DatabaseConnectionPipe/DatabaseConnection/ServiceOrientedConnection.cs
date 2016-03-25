
/** 
* @file ServiceOrientedConnection.cs
* @brief Contains the ServiceOrientedConnection class, implementing IDatabaseConnection interface
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using System;
using System.Collections.Generic;
using Assets.Scripts.UI;
using Assets.Scripts.UI.AbstractViews.SelectableGridList.Descriptors;

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

        public bool ContinueWorking { get; set; }

        public bool Query(string vQuery, Action vCallback = null)
        {
            throw new NotImplementedException();
        }

        public bool CreateRecording(BodyFramesRecording vRecording)
        {
            throw new NotImplementedException();
        }

        public bool CreateRecording(BodyFramesRecording vRecording, ImportItemDescriptor vDescriptor, Action<int> vTotalImportProgress)
        {
            throw new NotImplementedException();
        }

        public BodyFramesRecording GetRawRecording(string vRecordingId)
        {
            throw new NotImplementedException();
        }

        public Tag GetTagById(string vUid)
        {
            throw new NotImplementedException();
        }

        public void AddNewTag(Tag vTag)
        {
            throw new NotImplementedException();
        }

        public List<Tag> GetTagsOfRecording(string vRecGuid)
        {
            throw new NotImplementedException();
        }

        public void AddTagToRecording(BodyFramesRecording vRec, Tag vTag)
        {
            throw new NotImplementedException();
        }

        public List<Tag> LoadAllTags()
        {
            throw new NotImplementedException();
        }

        public List<string> GetRecordingGuidFromTagId(string vTagId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRecordingFolder(string vRecordingUid, string vFolderUid)
        {
            throw new NotImplementedException();
        }
    }
}