/** 
* @file AnalysisSubViewPool.cs
* @brief Contains the AnalysisSubViewPool abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Analysis
{
    /// <summary>
    ///  A pool of anaylsis views
    /// </summary>
    public static class AnalysisSubViewPool
    {
        private static List<AnalysisLayoutSubViewComponent> sInUseSubViews = new List<AnalysisLayoutSubViewComponent>();
        private static List<AnalysisLayoutSubViewComponent> sAvailableSubviews = new List<AnalysisLayoutSubViewComponent>();
        public static RectTransform AnalysisSubviewParent;

        /// <summary>
        /// Returns a camera from the available pool
        /// </summary>
        public static AnalysisLayoutSubViewComponent GetSubViewFromPool
        {
            get
            {
                if (sAvailableSubviews.Count != 0)
                {
                    AnalysisLayoutSubViewComponent vPooledObject = sAvailableSubviews[0];
                    sInUseSubViews.Add(vPooledObject);
                    sAvailableSubviews.RemoveAt(0);
                    vPooledObject.gameObject.SetActive(true);
                    vPooledObject.CameraControl = SubViewCameraPool.GetCamControl;
                    SetButtonFunctionality(vPooledObject);
                    return vPooledObject;
                }
                else
                {
                    GameObject vPooledGo = new GameObject();
                    vPooledGo.transform.parent = vPooledGo.transform;
                    vPooledGo.transform.parent = AnalysisSubviewParent.transform;

                    vPooledGo.name = "AnalysisSubViewComponent";
                    vPooledGo.AddComponent<GraphicRaycaster>();
                    Image v = vPooledGo.AddComponent<Image>();
                    Color vcolor = v.color;
                    vcolor.a = 0;
                    v.color = vcolor;
                    AnalysisLayoutSubViewComponent vSubviewComp =
                        vPooledGo.AddComponent<AnalysisLayoutSubViewComponent>();
                    vSubviewComp.ControlCanvas = vPooledGo.GetComponent<Canvas>();
                    if (vSubviewComp.ControlCanvas == null)
                    {
                        vSubviewComp.ControlCanvas = vPooledGo.AddComponent<Canvas>();
                    }
                    vSubviewComp.CameraControl = SubViewCameraPool.GetCamControl;
                    SetButtonFunctionality(vSubviewComp);
                    sInUseSubViews.Add(vSubviewComp); 


                    return vSubviewComp;
                }

            }
        }

        public static void ReleaseSubview(AnalysisLayoutSubViewComponent vSubview)
        {
            vSubview.CameraControl.ReleaseButtons();
            sInUseSubViews.Remove(vSubview);
            sAvailableSubviews.Add(vSubview);
            SubViewCameraPool.Release(vSubview.CameraControl);
            vSubview.gameObject.SetActive(false);
        }

        /// <summary>
        /// Helper function that will set the button functionality of the camera controls inside the Analysis subview component
        /// </summary>
        /// <param name="vParentSubViewcomponent">the parent of the camera control gui object</param>
        private static void SetButtonFunctionality(AnalysisLayoutSubViewComponent vParentSubViewcomponent)
        {
            Transform vCamCtrlTransformPane = vParentSubViewcomponent.transform.FindChild("Control Cam Button Panel");
            GameObject vCamControlPanel = null;
            if (vCamCtrlTransformPane == null)
            {
                vCamControlPanel = GameObject.Instantiate(Resources.Load("Prefabs/UI/Control Cam Button Panel")) as GameObject;
                vCamControlPanel.name = "Control Cam Button Panel";
            }
            else
            {
                vCamControlPanel = vCamCtrlTransformPane.gameObject;
            }


            vCamControlPanel.transform.parent = vParentSubViewcomponent.transform;
 

            Button vRightBut = vCamControlPanel.transform.FindChild("RightButton").GetComponent<Button>();
            Button vLeftBut = vCamControlPanel.transform.FindChild("LeftButton").GetComponent<Button>();
            vParentSubViewcomponent.CameraControl.MoveToNextPosButton = vRightBut;
            vParentSubViewcomponent.CameraControl.MoveToPrevPosButton = vLeftBut;
            if (vRightBut != null)
            {
                vRightBut.onClick.AddListener(vParentSubViewcomponent.CameraControl.MoveToNextPosition);
            }

            if (vLeftBut != null)
            {

                vLeftBut.onClick.AddListener(vParentSubViewcomponent.CameraControl.MoveToPrevPosition);
            }

        }
    }
}
