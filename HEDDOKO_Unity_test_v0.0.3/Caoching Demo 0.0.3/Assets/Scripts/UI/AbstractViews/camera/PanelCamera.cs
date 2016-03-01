/** 
* @file PanelCamera.cs
* @brief Contains the PanelCamera  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.camera
{
    /// <summary>
    /// 
    /// </summary>
   public  class PanelCamera
    {
        private Guid mId = new Guid();
        private Camera mPanelRenderingCamera;
        private PanelCameraSettings mSettings;
    }
}
