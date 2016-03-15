/** 
* @file TaggingManager.cs
* @brief Contains the TaggingManager class
* @author Mohammed Haider(Mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Communication.DatabaseConnectionPipe;

namespace Assets.Scripts.UI.Tagging
{
    /// <summary>
    /// 
    /// </summary>
    public class TaggingManager
    {
        private static TaggingManager sInstance = new TaggingManager();
        private Dictionary<string, Tag> mTags = new Dictionary<string, Tag>();
        private Database mDatabase;
        public static TaggingManager Instance
        {
            get { return sInstance; }
        }

        /// <summary>
        /// creates a tag
        /// </summary>
        /// <param name="vTitle"></param>
        public Tag CreateTag(string vTitle)
        {
            Tag vTag = null;
            bool vFound = mTags.Values.Any(value => value.Title.Equals(vTitle));
            if (!vFound)
            {
                  vTag = new Tag() { TagUid = Guid.NewGuid().ToString(), Title = vTitle };
                mTags.Add(vTag.TagUid, vTag);
                mDatabase.Connection.AddNewTag(vTag);
            }
            else
            {
                vTag = mTags.First(x => x.Value.Title.Equals(vTitle)).Value;
            }
            return vTag;
        }

        /// <summary>
        /// Loads all tags from the database
        /// </summary>
        /// <returns></returns>
        public List<Tag> LoadAllTags()
        {
            List<Tag> vTags = mDatabase.Connection.LoadAllTags();
            return vTags;
        }

        public void AttachTagToRecording(BodyFramesRecording vRec, Tag vTag)
        {
            mDatabase.Connection.AddTagToRecording(vRec, vTag);
            vRec.AddTag(vTag);
        }
        public void SetDatabase(Database vDatabase)
        {
            mDatabase = vDatabase;
        }

        /// <summary>
        /// Get a tag but its unique identifier
        /// Note: this will return null if no tags have been found
        /// </summary>
        /// <param name="vUid"></param>
        public Tag GetTagByUid(string vUid)
        {
            Tag vFoundObj = null;
            mTags.TryGetValue(vUid, out vFoundObj);
            if (vFoundObj == null)
            {
                vFoundObj = mDatabase.Connection.GetTagById(vUid);
                if (vFoundObj != null)
                {
                    mTags.Add(vUid, vFoundObj);
                }
            }
            return vFoundObj;
        }

        /// <summary>
        /// Returns a list of Tags of a recording  
        /// </summary>
        /// <param name="vRecGuid"></param>
        /// <returns></returns>
        public List<Tag> GetTagsFromRecordingUid(string vRecGuid)
        {
            List<Tag> vFoundTags = new List<Tag>();
            vFoundTags = mDatabase.Connection.GetTagsOfRecording(vRecGuid);
            //update the recording from body recordings manager
            BodyFramesRecording vRec = BodyRecordingsMgr.Instance.GetRecordingByUuid(vRecGuid);
            vRec.Tags = vFoundTags;
            return vFoundTags;
        }
         
    }
}
