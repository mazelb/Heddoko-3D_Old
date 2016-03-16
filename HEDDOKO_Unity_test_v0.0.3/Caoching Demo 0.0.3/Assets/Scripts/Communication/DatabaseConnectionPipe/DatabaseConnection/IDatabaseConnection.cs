
/** 
* @file IDatabaseConnection.cs
* @brief Contains the IDatabaseConnection interface
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.UI;
using JetBrains.Annotations;

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
        bool Connect(Action  vCallback = null);

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
        bool Query(string  vQuery, Action vCallback = null);
        /// <summary>
        /// Creates a recording entry on the database
        /// </summary>
        /// <param name="vRecording"></param>
        /// <returns></returns>
        bool CreateRecording(BodyFramesRecording vRecording);

        /// <summary>
        /// get a recording based on the id passed in
        /// </summary>
        /// <param name="vRecordingId">the recording id</param>
        /// <returns></returns>
        BodyFramesRecording GetRawRecording(string vRecordingId);

        Tag GetTagById(string vUid);
        void AddNewTag(Tag vTag);

        /// <summary>
        /// Get a list of tags current attached to this recording
        /// </summary>
        /// <param name="vRecGuid">the recording guid from which to retrieve the tag from</param>
        /// <returns></returns>
        List<Tag> GetTagsOfRecording(string vRecGuid);

        /// <summary>
        /// Attaches a tag to a recording
        /// </summary>
        /// <param name="vRec">the recording to add a tag to</param>
        /// <param name="vTag">the tag to attach</param>
        void AddTagToRecording(BodyFramesRecording vRec, Tag vTag);

        /// <summary>
        /// Loads all tags from the database. 
        /// </summary>
        /// <returns></returns>
        List<Tag> LoadAllTags();

        /// <summary>
        /// Retrieve a list of recording guids that have been tagged 
        /// </summary>
        /// <param name="vTagId"></param>
        /// <returns></returns>
        List<string> GetRecordingGuidFromTagId(string vTagId);

        /// <summary>
        /// Update the the folder a recording is in 
        /// </summary>
        /// <param name="vRecordingUid"></param>
        /// <param name="vFolderUid"></param>
        /// <returns></returns>
        bool UpdateRecordingFolder(string vRecordingUid, string vFolderUid);
    }
}
