#if UNITY_EDITOR
/** 
* @file ContainerTest.cs
* @brief Contains the ContainerTest class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Layouts;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    /// <summary>
    /// Layout, and layoutcontainer testing
    /// </summary>
    public class ContainerTest : AbstractView
    {
        private LayoutType LayoutType = LayoutType.HalfHalfHorizontal;
        public Layout CurrentLayout;
        private PanelNode[] mPanelNodes;
        public List<List<ControlPanelType>> ControlPanelTypeList = new List<List<ControlPanelType>>(2);

        void Start()
        {
            List<ControlPanelType> vLeftSide = new List<ControlPanelType>();
            vLeftSide.Add(ControlPanelType.RecordingPlaybackControlPanel);
            List<ControlPanelType> vRightSide = new List<ControlPanelType>();
            vRightSide.Add(ControlPanelType.RecordingPlaybackControlPanel);
            ControlPanelTypeList.Add(vLeftSide);
            ControlPanelTypeList.Add(vRightSide);
            TestCreateLayout();
        }

        void TestCreateLayout()
        {
            BodiesManager.Instance.CreateNewBody("SingleRec_LeftPanelBody");
            BodiesManager.Instance.CreateNewBody("SingleRec_RightPanelBody");
            Body vBodyLeft = BodiesManager.Instance.GetBodyFromUUID("SingleRec_LeftPanelBody");
            Body vBodyRight = BodiesManager.Instance.GetBodyFromUUID("SingleRec_RightPanelBody");
            if (CurrentLayout == null)
            {
                CurrentLayout = new Layout(LayoutType, this);
                mPanelNodes = CurrentLayout.ContainerStructure.PanelNodes; 
            }
            mPanelNodes[0].name = "Left";
            mPanelNodes[1].name = "Right";
            mPanelNodes[0].PanelSettings.Init(ControlPanelTypeList[0], true, vBodyLeft);
            mPanelNodes[1].PanelSettings.Init(ControlPanelTypeList[1], true, vBodyRight);
        }

        public override void CreateDefaultLayout()
        {

        }

  

        /// <summary>
        /// releases current resources
        /// </summary>
        public override void Hide()
        {
            foreach (var vPanelNodes in mPanelNodes)
            {
                vPanelNodes.PanelSettings.ReleaseResources();
            }
            base.Hide();
        }
    }

}
#endif