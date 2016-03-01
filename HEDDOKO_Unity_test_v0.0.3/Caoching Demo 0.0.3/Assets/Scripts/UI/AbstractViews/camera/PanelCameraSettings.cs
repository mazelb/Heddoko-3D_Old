/** 
* @file PanelCameraSettings.cs
* @brief Contains the PanelCameraSettings  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.camera
{
    /// <summary>
    /// Contains settings for the panel camera
    /// </summary>
    public class PanelCameraSettings
    {
        private LayerMask mRenderingLayerMask;
        private Vector2 mMaxSize;
        private Vector2 mMinSize;

        public PanelCameraSettings(LayerMask vMask, Vector2 vMinSize, Vector2 vMaxSize)
        {
            mRenderingLayerMask = vMask;
            mMinSize= vMinSize ;
            mMaxSize = vMaxSize;
        }

        public LayerMask RenderingLayerMask
        {
            get { return mRenderingLayerMask; }
            set { mRenderingLayerMask = value; }
        }

        public Vector2 MaxSize
        {
            get { return mMaxSize; }
            set { mMaxSize = value; }
        }

        public Vector2 MinSize
        {
            get { return mMinSize; }
            set { mMinSize = value; }
        }
    }
}