/** 
* @file PanelNodeTemplate.cs
* @brief contains a template for panel nodes to use
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Layouts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.Templates
{
    /// <summary>
    /// A template for panel nodes to use
    /// </summary>
   public  class PanelNodeTemplate
    {
        public Dictionary<ControlPanelType,SubControlType> Mapping = new Dictionary<ControlPanelType, SubControlType>();

        public RectOffset HorizontalOrVerticalPadding;
        public float PrefferedWidthOrHeight;
        public HorizontalOrVerticalLayoutGroupType HorizontalOrVerticalLayoutType;

        /// <summary>
        /// Because monobehaviours do not like being called by the new keyword, this is a workaround method that attaches a 
        /// new HorizontalOrVerticalLayoutGroup to the calling PanelNode.
        /// </summary>
        /// <param name="vPanelNode"></param>
        /// <returns></returns>
        public HorizontalOrVerticalLayoutGroup AttachHorizontalOrVerticalLayoutGroup(PanelNode vPanelNode)
        {
            if (vPanelNode.GetComponent<HorizontalOrVerticalLayoutGroup>() != null)
            {
                GameObject.Destroy(vPanelNode.GetComponent<HorizontalOrVerticalLayoutGroup>()); 
            }
            if (HorizontalOrVerticalLayoutType == HorizontalOrVerticalLayoutGroupType.Horizontal)
            {
                vPanelNode.gameObject.AddComponent<HorizontalLayoutGroup>();
            }
            else if((HorizontalOrVerticalLayoutType == HorizontalOrVerticalLayoutGroupType.Vertical))
            {
                vPanelNode.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            return vPanelNode.GetComponent<HorizontalOrVerticalLayoutGroup>();
        }
    }

    public enum HorizontalOrVerticalLayoutGroupType
    {
        Null,
        Horizontal,
        Vertical
    }
}
