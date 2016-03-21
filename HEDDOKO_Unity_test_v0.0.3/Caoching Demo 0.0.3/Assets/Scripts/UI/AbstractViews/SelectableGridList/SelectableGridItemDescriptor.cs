/** 
* @file SelectableGridItemDescritor.cs
* @brief Contains the SelectableGridItemDescritor class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.SelectableGridList
{
    [Serializable]
    public abstract class SelectableGridItemDescriptor
    {
        /// <summary>
        /// A list of gameobjects that can be resized
        /// </summary>
        public abstract GameObject[] ResizableGameObjects { get; }
    }
}
