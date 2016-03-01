using Assets.Scripts.UI.Layouts;
using System.Collections.Generic;
using Assets.Scripts.UI.AbstractViews.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.Layouts
{
    public class PanelNode: MonoBehaviour
    {
        private PanelID mID = new PanelID();
        private PanelNode mParent;
        private List<PanelNode> mChildren = new List<PanelNode>();
        private PanelSettings mSettings;


        public List<PanelNode> Children
        {
            get { return mChildren; }
            set { mChildren = value; }
        }

        public PanelNode Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }

        public PanelID Id
        {
            get { return mID; }
        }

        public PanelSettings Settings
        {
            get { return mSettings; }
            set { mSettings = value; }
        }

        /// <summary>
        /// Initializes the PanelNode with the passed in template
        /// </summary>
        /// <param name="vParent">The panel nodes' parent</param>
        /// <param name="vTemplate"></param>
        public void Init(PanelNode vParent, PanelNodeTemplate vTemplate)
        {
            this.mParent = vParent;
            RectTransform vRectTransform = GetComponent<RectTransform>();
            if (vRectTransform == null)
            {
                vRectTransform = this.gameObject.AddComponent<RectTransform>();
                
            }
            Settings = new PanelSettings();
            Settings.RectTransform = vRectTransform;
            Settings.CameraToRenderedBodyPair = new PanelCameraToRenderedBodyPair();
            Settings.Group = vTemplate.AttachHorizontalOrVerticalLayoutGroup(this);
            Settings.Group.padding = vTemplate.HorizontalOrVerticalPadding;
            Settings.LayoutElementComponent = gameObject.AddComponent<LayoutElement>();
            if (vParent != null)
            {
                Settings.ModifyLayoutElement(vTemplate.PrefferedWidthOrHeight,vParent.Settings.Group);
            }
        //    Settings.UpdatePanel(vTemplate.Mapping)
            


        }
        public override bool Equals(object obj)
        {
            var node = obj as PanelNode;
            if (node != null)
            {
                var mNode = node;
                return mID.Id == mNode.mID.Id;
            }
            return false;
        }

        /// <summary>
        /// Inserts a child at the current index. 
        /// </summary>
        /// <param name="vChild"></param>
        /// <param name="vIdx"></param>
        /// <returns></returns>
        public PanelNode InsertChildAt(PanelNode vChild, int vIdx)
        {
            return null;
        }

        /// <summary>
        /// Inserts the child and sets it at the top of the children hierarchy
        /// </summary>
        /// <param name="vChild"></param>
        /// <returns></returns>
        public PanelNode InsertChildAtFront(PanelNode vChild)
        {
            return InsertChildAt(vChild, 0);
        }
        /// <summary>
        /// Inserts the child and sets it at the bottom of the children hierarchy
        /// </summary>
        /// <param name="vChild"></param>
        /// <returns></returns>
        public PanelNode InsertChildAtEnd(PanelNode vChild)
        {
            return InsertChildAt(vChild, Children.Count - 1);
        }

        /// <summary>
        /// Removes a child from the list of children(subsequently all its children are also removed)
        /// </summary>
        /// <param name="vChild"></param>
        /// <returns></returns>
        public PanelNode RemoveChild(PanelNode vChild)
        {
            return null;
        }

        /// <summary>
        /// Removes a child from the list of children(subsequently all its children are also removed) with an index parameter
        /// </summary>
        /// <param name="vChild"></param>
        /// <param name="vIdx"></param>
        /// <returns></returns>
        public PanelNode RemoveChildAt(int vIdx)
        {
            return null;
        }


        /// <summary>
        /// Splits the panel in half and copies over the current template over to the new panel
        /// </summary>
        /// <returns>the newly created panel's children</returns>
        public List<PanelNode> SplitPanelInHalf()
        {
            return null;
        }
        /// <summary>
        /// Splits the panel in half, applies a template to the first half of the panel and a second template to the second half
        /// </summary>
        /// <param name="vFirstChildTemplate"></param>
        /// <param name="vSecondChildTemplate"></param>
        /// <returns>the newly created panel's children</returns>
        public List<PanelNode> SplitPanelInHalf(PanelNodeTemplate vFirstChildTemplate,
            PanelNodeTemplate vSecondChildTemplate)
        {
            return null;
        }
        /// <summary>
        /// Cleans up any resources on removal
        /// </summary>
        public void CleanUpOnRemoval()
        {
            this.Children = null;
            this.Parent = null;
        }
    }
}