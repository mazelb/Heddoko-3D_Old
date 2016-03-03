

/* @file LayoutCreationManager.cs
* @brief Contains the LayoutCreationManager class
* @author Mohammed Haider(mohamed @heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.UI.AbstractViews.Layouts;
 

namespace Assets.Scripts.UI.AbstractViews.Templates
{
    public class LayoutCreationManager
    {
        private Dictionary<LayoutType, Func<LayoutContainer>> mLayoutToCreationMapping = new Dictionary<LayoutType, Func<LayoutContainer>>();
        private static LayoutCreationManager mInstance;
        private AbstractView mParentAbstractView;


        public static LayoutCreationManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new LayoutCreationManager();
                    mInstance.Init();
                }
                return mInstance;
            }
        }

        /// <summary>
        /// Set up mappings
        /// </summary>
        private void Init()
        {
            mLayoutToCreationMapping.Add(LayoutType.Single, SingleLayoutCreation);
            mLayoutToCreationMapping.Add(LayoutType.HalfHalfHorizontal, HalfHalfHorizontalLayoutCreation);
            mLayoutToCreationMapping.Add(LayoutType.HalfHalfVertical, HalfHalfVerticalLayoutCreation);
            mLayoutToCreationMapping.Add(LayoutType.OneLeftByTwoRight, OneLeftByTwoRightCreation);
            mLayoutToCreationMapping.Add(LayoutType.OneRightByTwoLeft, OneRightByTwoLeftCreation);
            mLayoutToCreationMapping.Add(LayoutType.VerticalTSplitBot, VertTSplitBotCreation);
            mLayoutToCreationMapping.Add(LayoutType.VerticalTSplitTop, TSplitTopCreation);
        }
        /// <summary>
        /// a layout with  a panel sitting at the foot of two panel nodes  that are 50% vertically split
        /// </summary>
        /// <returns></returns>
        private LayoutContainer VertTSplitBotCreation()
        {
            
            LayoutContainer vLayoutContainer = SingleLayoutCreatorHelper(mParentAbstractView, HorizontalOrVerticalLayoutGroupType.Vertical);
            PanelNode vRoot = vLayoutContainer.GetRoot();
            float vRootHeight = vRoot.Settings.RectTransform.rect.height;
            float vBottomSplit = vRootHeight * (0.25f);
            float vTopSplit = vRootHeight - vBottomSplit;

            RectOffset vFivesTop = new RectOffset(5, 5, 5, 5);
            RectOffset vFivesBot = new RectOffset(5, 5, 5, 5);
            PanelNodeTemplate vBottomSplitTemplate = new PanelNodeTemplate(vFivesTop, vBottomSplit, 5, HorizontalOrVerticalLayoutGroupType.Null);
            PanelNodeTemplate vTopSplitTemplate = new PanelNodeTemplate(vFivesBot, vTopSplit, 5, HorizontalOrVerticalLayoutGroupType.Null);
           
            PanelNode vTopSplitNode =vLayoutContainer.AddPanelNode(vRoot, vRoot.Settings.RectTransform, vTopSplitTemplate);
            PanelNode vBotSplitNode = vLayoutContainer.AddPanelNode(vRoot, vRoot.Settings.RectTransform, vBottomSplitTemplate);

            RectOffset vFivesBot1 = new RectOffset(5, 5, 5, 5);
            RectOffset vFivesBot2 = new RectOffset(5, 5, 5, 5);
            float vSplitBotHalf = vBotSplitNode.Settings.RectTransform.rect.height/2f;

            PanelNodeTemplate vBotSplit1 = new PanelNodeTemplate(vFivesBot1, vSplitBotHalf, 5, HorizontalOrVerticalLayoutGroupType.Null);
            PanelNodeTemplate vBotSplit2 = new PanelNodeTemplate(vFivesBot2, vSplitBotHalf, 5, HorizontalOrVerticalLayoutGroupType.Null);

            vTopSplitNode.SplitPanelInHalf(HorizontalOrVerticalLayoutGroupType.Horizontal,vBotSplit1, vBotSplit2);
            Cleanup(); 
            return vLayoutContainer;
        }
        private LayoutContainer TSplitTopCreation()
        {
            throw new NotImplementedException();
        }
        private LayoutContainer OneRightByTwoLeftCreation()
        {
            throw new NotImplementedException();
        }

        private LayoutContainer OneLeftByTwoRightCreation()
        {
            return null;
        }

        private LayoutContainer HalfHalfVerticalLayoutCreation()
        {
            return null;
        }

        /// <summary>
        /// A layout container with a 50/50 horizontal split
        /// </summary>
        /// <returns></returns>
        private LayoutContainer HalfHalfHorizontalLayoutCreation()
        {

            return null;
        }

        /// <summary>
        /// A layout container with a single planel node
        /// </summary>
        /// <returns></returns>
        private LayoutContainer SingleLayoutCreation()
        {
            LayoutContainer vLayoutContainer = SingleLayoutCreatorHelper(mParentAbstractView,HorizontalOrVerticalLayoutGroupType.Null);
            Cleanup();
            return vLayoutContainer;
        }

        /// <summary>
        /// Helper method that creates a layout container with a single panel node
        /// </summary>
        /// <param name="vParentAbstractView">the parent abstract view</param>
        /// <returns>A newly created LayoutContainer</returns>
        private static LayoutContainer SingleLayoutCreatorHelper(AbstractView vParentAbstractView, HorizontalOrVerticalLayoutGroupType vType)
        {
            RectOffset vTens = new RectOffset(10, 10, 10, 10);
            PanelNodeTemplate vTemplate = new PanelNodeTemplate(vTens, 0, 5, vType);
            LayoutContainer vLayoutContainer = new LayoutContainer();
            PanelNode vRoot= vLayoutContainer.AddRoot(vParentAbstractView.RectTransform, vTemplate);
            vRoot.Settings.RectTransform.anchorMin = Vector2.zero;
            vRoot.Settings.RectTransform.anchorMax = Vector2.one;
            vRoot.Settings.RectTransform.offsetMin = vRoot.Settings.RectTransform.offsetMax = new Vector2(0, 0);
            return vLayoutContainer;
        }
        /// <summary>
        /// Clean up resources
        /// </summary>
        private void Cleanup()
        {
            mParentAbstractView = null;
        }

        /// <summary>
        /// Returns a layout container based on the layout type passed in
        /// </summary>
        /// <param name="vLayoutType"></param>
        /// <returns>the newly created layoutcontainer</returns>
        public LayoutContainer CreateLayoutContainer(AbstractView vParentView,LayoutType vLayoutType)
        {
            LayoutContainer vNewContainer = null;
            mParentAbstractView = vParentView;
            if (mLayoutToCreationMapping.ContainsKey(vLayoutType))
            {
                vNewContainer = mLayoutToCreationMapping[vLayoutType].Invoke();
                return vNewContainer;
            }
            return vNewContainer;
        }
    }
}
