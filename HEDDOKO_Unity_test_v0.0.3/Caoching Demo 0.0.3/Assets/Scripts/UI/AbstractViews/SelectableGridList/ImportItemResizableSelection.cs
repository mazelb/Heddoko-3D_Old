/** 
* @file SelectableGridItemDescritor.cs
* @brief Contains the SelectableGridItemDescritor class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.SelectableGridList
{
    /// <summary>
    /// resizable items specific to importing views
    /// </summary>
    [Serializable]
    public class ImportItemResizableSelection : SelectableGridItemDescriptor
    {
        public Text MovementTitle;
        public Text CreatedAtDescription;
        public Text Tag;

        public override GameObject[] ResizableGameObjects
        {
            get { return new GameObject[] {MovementTitle.gameObject, CreatedAtDescription.gameObject ,Tag.gameObject}; }
        }
    }
}
