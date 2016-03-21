/** 
* @file SelectableGridComponent.cs
* @brief Contains the SelectableGridComponent class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System.Collections.Generic;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.SelectableGridList
{
    /// <summary>
    /// A component for the selectable grid list
    /// </summary>
    public class SelectableGridComponent : ListViewItem, IResizableItem
    {
        [SerializeField]
        public SelectableGridItemDescriptor ResizableItemComponents;

        /// <summary>
        /// Get the objects to resize
        /// </summary>
        public GameObject[] ObjectsToResize
        {
            get
            { 
                return ResizableItemComponents.ResizableGameObjects;
            }
        }

    }
}