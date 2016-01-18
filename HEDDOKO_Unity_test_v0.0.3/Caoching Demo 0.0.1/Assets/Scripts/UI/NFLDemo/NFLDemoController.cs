/** 
* @file NFLDemoController.cs
* @brief Contains the NFLDemoController class
* @author Mohammed Haider(Mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Cameras;
using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.Utils.DebugContext;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Assets.Scripts.UI.NFLDemo
{
    /// <summary>
    /// Controller used for the NFL demo tech crunch
    /// </summary>
    [Serializable]
    public class NFLDemoController : MonoBehaviour
    {
        public CameraController CameraController;
        public MoveCameraToPositon MoveCameraToPositon;
        [SerializeField]
        public GroupFadeEffect GroupFadeEffect;
        //the state of the cam look at will be saved once the event is triggered
        private bool MoveCamState;

        public CameraMotionBlur CamBlur;
        public CameraLookAt CamLookAt;

 
        public PlayerStreamManager PlayerStreamManager;

        [SerializeField]
        public NFLCameraController NFLCamController;
        [SerializeField]
        private bool vMainEventStarted;
        private CurrentAnimationState vCurrentState = CurrentAnimationState.InTrainingView;
        private int mCurrentCamPos = 0;
        private int mNextCamPos;



        //     private bool v
        private enum CurrentAnimationState
        {
            InTrainingView,
            HidingGroupAnimating,
            CameraTransitionAnimation,
            DisplayInfo

        }
        private void Update()
        {
            if (!vMainEventStarted)
            {
                //start the event
                if (Input.GetKeyDown(HeddokoDebugKeyMappings.Pause))
                {
                    vMainEventStarted = true;
                    PlayerStreamManager.ChangePauseState();
                    NFLCamController.Reset();
                    MoveCamState = MoveCameraToPositon.isActiveAndEnabled;
                    CameraController.enabled = false;
                    MoveCameraToPositon.enabled = false;
                    CamLookAt.enabled = true;
                    CamBlur.enabled = true;
                    NFLCamController.enabled = true;
                    GroupFadeEffect.Hide();
                    NFLCamController.Curve.SetDirty();
                    //set the look at target
                    CameraMovementPointSetting vPointSetting  = NFLCamController.GetPointAt(0);
                    CamLookAt.TargetPos = vPointSetting.LookAtTarget.position;
                    NFLCamController.MoveToNextPos();
                }
            }
            else if (vMainEventStarted)
            {
                //stop the event
                if (Input.GetKeyDown(HeddokoDebugKeyMappings.Pause))
                {
                    Reset();
                }
                else
                {
                    //get the current index and next index
                    int vCurrentCamIdx = NFLCamController.CurrCamIndex;
                    int vNextCamIdx = NFLCamController.NextCamIndex;

                    //Is the group effect still animating?
                    if (GroupFadeEffect.FinishedAnimating)
                    {
                        //Is the camera still moving?
                        if (NFLCamController.FinishedMovingCam)
                        {
                            //Check if the AnalysisView is in display
                            AnalysisView vCurrAnalysisView = NFLCamController.GetAnalysisView(vCurrentCamIdx);
                            if (vCurrAnalysisView != null && vCurrAnalysisView.InView)
                            {
                                //Check if the user is requesting to go to next position
                                if (Input.GetKeyDown(HeddokoDebugKeyMappings.MoveNext))
                                {
                                    vCurrAnalysisView.Hide();
                                    NFLCamController.MoveToNextPos();
                                }
                            }

                            // there is no analysis view attached to the current index
                            else if (vCurrAnalysisView == null)
                            {
                                if (Input.GetKeyDown(HeddokoDebugKeyMappings.MoveNext))
                                {
                                    NFLCamController.MoveToNextPos();
                                }
                            }
                            if (vCurrAnalysisView != null && !vCurrAnalysisView.InView)
                            {
                                vCurrAnalysisView.Show();
                            }
                        }

                        //check if next pos is 0, go back into training view
                        if (NFLCamController.FinishedMovingCam && NFLCamController.NextCamIndex == 0)
                        {
                            Reset();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resets the view back to training
        /// </summary>
        public void Reset()
        {
            vMainEventStarted = false;
            PlayerStreamManager.ChangePauseState();
            CamLookAt.enabled = false;
            MoveCameraToPositon.gameObject.SetActive(MoveCamState);
            CameraController.enabled = true;
            MoveCameraToPositon.enabled = true;
            CamBlur.enabled = false;
            NFLCamController.Reset();
            NFLCamController.enabled = false;
            GroupFadeEffect.Show();
        }

        /// <summary>
        /// In the event that the back button was pressed, reset the camera back to its original position
        /// </summary>
        public void BackButtonPressed()
        {
            GroupFadeEffect.ForceShow();
            vMainEventStarted = false;
            PlayerStreamManager.ChangePauseState();
            CamLookAt.enabled = false;
            MoveCameraToPositon.gameObject.SetActive(MoveCamState);
            CameraController.enabled = true;
            MoveCameraToPositon.enabled = true;
            CamBlur.enabled = false;
            NFLCamController.Reset();
            NFLCamController.enabled = false;
           
        }
        /// <summary>
        /// Vertifies the current state of the controller
        /// </summary>
        private void VerifyCurrentState()
        {
            switch (vCurrentState)
            {
                case (CurrentAnimationState.InTrainingView):
                    {
                        break;
                    }
                case (CurrentAnimationState.HidingGroupAnimating):
                    {

                        break;
                    }
                case (CurrentAnimationState.CameraTransitionAnimation):
                    {
                        break;
                    }
                case (CurrentAnimationState.DisplayInfo):
                    {
                        break;
                    }


            }
        }

        
    }
}
