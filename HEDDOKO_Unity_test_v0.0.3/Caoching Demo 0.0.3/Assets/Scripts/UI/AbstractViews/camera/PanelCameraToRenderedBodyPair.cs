/** 
* @file PanelCameraToRenderedBodyPair.cs
* @brief Contains the PanelCameraToRenderedBodyPair  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.Body_Data;
using Assets.Scripts.UI.AbstractViews.camera;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews
{
    /// <summary>
    /// A one to one relationship between a rendered body and a camera rendering it
    /// </summary>
    public class PanelCameraToRenderedBodyPair
    { 
        private LayerMask mRenderingLayerMask;
        private PanelCamera mCamera;
        private RenderedBody mAvatar;

        public RenderedBody Avatar
        {
            get { return mAvatar; }
            set { mAvatar = value; }
        }

        public PanelCamera Camera
        {
            get { return mCamera; }
            set { mCamera = value; }
        }
    }
}
