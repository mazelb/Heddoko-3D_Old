﻿/** 
* @file PanelCameraPool.cs
* @brief Contains the PanelCameraPool  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
 
using UnityEngine;
using System.Collections.Generic;
namespace Assets.Scripts.UI.AbstractViews.camera
{
    /// <summary>
    /// A pool of Panel cameras
    /// </summary>
    public class PanelCameraPool
    {

        private static List<PanelCamera> sAvailablePanelCams = new List<PanelCamera>();
        private static List<PanelCamera> sInUseCameras = new List<PanelCamera>();
        private static Transform sCameraParent;

   

        public static Transform CameraParent { get; set; }

        /// <summary>
        /// Get a panel camera resource from the available pool
        /// </summary>
        /// <param name="vSettings"></param>
        /// <returns></returns>
        public static PanelCamera GetPanelCamResource(PanelCameraSettings vSettings)
        {
            if (sAvailablePanelCams.Count != 0)
            {
                PanelCamera vPooledObject = sAvailablePanelCams[0];
                sInUseCameras.Add(vPooledObject);
                sAvailablePanelCams.RemoveAt(0);
                vPooledObject.PanelRenderingCamera.gameObject.SetActive(false);
                vPooledObject.SetupCamera(vSettings);
                vPooledObject.PanelRenderingCamera.gameObject.SetActive(true);
                return vPooledObject;
            }
            else
            {
                GameObject vPooledGo = new GameObject();
                Camera vCam = vPooledGo.AddComponent<Camera>();
                vPooledGo.transform.parent = CameraParent.transform;
                PanelCamera vSubcamCtrl = new PanelCamera();
                vSubcamCtrl.PanelRenderingCamera = vCam;
                vSubcamCtrl.SetupCamera(vSettings);
                vPooledGo.name = "PanelCamera";

                return vSubcamCtrl;
            }

        }

        
        public static void Release(PanelCamera vPanelCam)
        {
            sInUseCameras.Remove(vPanelCam);
            sAvailablePanelCams.Add(vPanelCam);
            vPanelCam.PanelRenderingCamera.gameObject.SetActive(false);
        }
    }
}