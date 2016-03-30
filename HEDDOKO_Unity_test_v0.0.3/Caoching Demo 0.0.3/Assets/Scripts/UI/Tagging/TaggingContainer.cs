/** 
* @file TaggingContainer.cs
* @brief Contains the TaggingContainer class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews.ContextSpecificContainers.Tags;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Tagging
{
    /// <summary>
    /// A container of tags.
    /// </summary>
    [Serializable]
    public class TaggingContainer : MonoBehaviour, ITaggingManagerConsumer
    {
        private LayoutElement mLayoutElement;
        private Dictionary<string, Tag> mTags = new Dictionary<string, Tag>();
        public Button AddTagButton;
        [SerializeField]
        public TagAutocomplete AutoComplete;
        private ListView mFloatListView;
        private TaggingManager mTaggingManager;
        public TagView TagViewPrefab;

        public ListView FloatListView
        {
            get { return mFloatListView; }
            set
            {
                mFloatListView = value;
                AutoComplete.FloatingListView = FloatListView;
            }
        }
        public TaggingManager TaggingManager
        {
            get { return mTaggingManager; }
            set
            {
                mTaggingManager = value;
                AutoComplete.TaggingManager = mTaggingManager;
            }
        }
        public LayoutElement LayoutElement
        {
            get
            {
                if (mLayoutElement == null)
                {
                    mLayoutElement = GetComponent<LayoutElement>();
                }
                return mLayoutElement;
            }
            set { mLayoutElement = value; }
        }

        /// <summary>
        /// Adds a Tag to the container, will result in false if the Tag is already in the container
        /// </summary>
        /// <param name="vNewTag">The tag to add</param>
        /// <returns>Was the addition succesful?</returns>
        public bool AddTag(Tag vNewTag)
        {
            if (mTags.ContainsKey(vNewTag.Title))
            {
                return false;
            }
            mTags.Add(vNewTag.Title, vNewTag);
            return true;
        }
        /// <summary>
        /// Removes a tag from the container
        /// Note, this function will throw an TagNotFoundException if the tag wasn't found
        /// </summary>
        /// <param name="vTagToRemove">The tag to remove</param>
        /// <returns>The tag that was removed</returns>
        public Tag RemoveTag(Tag vTagToRemove)
        {
            if (!mTags.ContainsKey(vTagToRemove.Title))
            {
                throw new TagNotFoundException();
            }
            Tag vTag = mTags[vTagToRemove.Title];
            mTags.Remove(vTag.Title);
            return vTag;
        }



    }
}