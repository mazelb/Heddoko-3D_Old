/** 
* @file PanelSettings.cs
* @brief Contains the PanelSettings  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections.Generic;
using Assets.Scripts.UI.AbstractPanels;
using Assets.Scripts.UI.AbstractViews.AbstractPanels;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.Layouts
{
    /// <summary>
    /// Settings of a Panel Node
    /// </summary>
    public class PanelSettings
    {
        private LayoutElement mLayoutElementComponent;
        private HorizontalOrVerticalLayoutGroup mLayoutGroup;
        private RectTransform mRectTransform;
        private HashSet<AbstractControlPanel> mControlPanelSet = new HashSet<AbstractControlPanel>();
        private PanelCameraToRenderedBodyPair mPanelCameraToRenderedBodyPair;
        private float mLayoutElementModifier;



        public PanelCameraToRenderedBodyPair CameraToRenderedBodyPair
        {
            get { return mPanelCameraToRenderedBodyPair; }
            set { mPanelCameraToRenderedBodyPair = value; }
        }

        public LayoutElement LayoutElementComponent
        {
            get { return mLayoutElementComponent; }
            set { mLayoutElementComponent = value; }
        }

        public HorizontalOrVerticalLayoutGroup Group
        {
            get { return mLayoutGroup; }
            set { mLayoutGroup = value; }
        }

        public RectTransform RectTransform
        {
            get { return mRectTransform; }
            set { mRectTransform = value; }
        }

        public HashSet<AbstractControlPanel> ControlPanelSet
        {
            get { return mControlPanelSet; }
            set { mControlPanelSet = value; }
        }


        /// <summary>
        /// Modify the layout element according to the parent layout group
        /// </summary>
        /// <param name="vValue"></param>
        /// <param name="vParentHorizontalLayoutGroup"></param>
        public void ModifyLayoutElement(float vValue, HorizontalOrVerticalLayoutGroup vParentHorizontalLayoutGroup)
        {
            if (vParentHorizontalLayoutGroup != null)
            {
                mLayoutElementModifier = vValue;
                if (vParentHorizontalLayoutGroup is HorizontalLayoutGroup)
                {
                    mLayoutElementComponent.flexibleWidth = 0;
                    mLayoutElementComponent.flexibleHeight = mLayoutElementModifier;
                }
                else if (vParentHorizontalLayoutGroup is VerticalLayoutGroup)
                {
                    mLayoutElementComponent.flexibleHeight = 0;
                    mLayoutElementComponent.flexibleWidth = mLayoutElementModifier;
                }
            }
        }

        /// <summary>
        /// updates the control panel in the control panel set with the list of subcontrols
        /// </summary>
        /// <param name="vAbstractControlPanel"></param>
        /// <param name="vSubcontrols"></param>
        public void UpdatePanel(AbstractControlPanel vAbstractControlPanel, List<AbstractSubControl> vSubcontrols)
        {
            if (ControlPanelSet.Contains(vAbstractControlPanel))
            {
                vAbstractControlPanel.UpdateSubControlList(vSubcontrols);
            }
            else
            {
                ControlPanelSet.Add(vAbstractControlPanel);
                vAbstractControlPanel.UpdateSubControlList(vSubcontrols);
            }

        }

        public AbstractControlPanel GetControlPanel(AbstractControlPanel vControlPanel)
        {
            return null;
        }

    }
}
