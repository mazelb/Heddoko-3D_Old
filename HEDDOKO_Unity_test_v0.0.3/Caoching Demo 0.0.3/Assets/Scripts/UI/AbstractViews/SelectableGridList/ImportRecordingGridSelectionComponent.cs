/** 
* @file ImportRecordingGridSelectionComponent.cs
* @brief Contains the ImportRecordingGridSelectionComponent class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.UI.AbstractViews.SelectableGridList.Descriptors;
using UnityEngine;
using UnityEngine.UI;

using UIWidgets;
namespace Assets.Scripts.UI.AbstractViews.SelectableGridList
{
    /// <summary>
    /// resizable items specific to importing views
    /// </summary>
    [Serializable]
    public class ImportRecordingGridSelectionComponent : ListViewItem, IResizableItem
    {
        public MarkedForDeletion MarkedForDeletionItem;
        public Text MovementTitle;
        public Text CreatedAtDescription;
        public Text DurationDescription;
        public Text Tag;

        /// <summary>
        /// The gameobject that can be resized
        /// </summary>
        public GameObject[] ObjectsToResize
        {
            get { return new GameObject[] { MovementTitle.gameObject, CreatedAtDescription.gameObject, DurationDescription.gameObject, Tag.gameObject }; }
        }

        /// <summary>
        /// Sets data from the given 
        /// </summary>
        /// <param name="vComponent"></param>
        /// <param name="vDescriptorItem"></param>
        public void SetData(ImportItemDescriptor vDescriptorItem)
        {
            MovementTitle.text = vDescriptorItem.MovementTitle;
            CreatedAtDescription.text = vDescriptorItem.CreatedAtDescription;
            DurationDescription.text = vDescriptorItem.RecordingDurationToString;
            if (MarkedForDeletionItem.Item == null)
            {
                MarkedForDeletionItem.Item = vDescriptorItem;
            }
            MarkedForDeletionItem.Item = vDescriptorItem;
            MarkedForDeletionItem.IsMarkedForDeletion = vDescriptorItem.IsMarkedForDeletion;

        }

        public void AddTag(string vTagText)
        {
            //todo
        }
    }
}
