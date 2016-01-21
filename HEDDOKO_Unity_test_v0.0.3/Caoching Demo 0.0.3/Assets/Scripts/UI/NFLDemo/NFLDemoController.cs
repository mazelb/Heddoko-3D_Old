/** 
* @file NFLDemoController.cs
* @brief Contains the NFLDemoController class
* @author Mohammed Haider(Mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using System;
using Assets.Scripts.Cameras;
using Assets.Scripts.UI.ActivitiesContext.Controller;
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
        public ActivitiesContextController ActivitiesContextController;
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

        public ArcAngleFill ArcAngleFill;
        public AnalysisContentPanel AnalysisContentPanel;


        private enum CurrentAnimationState
        {
            InTrainingView,
            HidingGroupAnimating,
            CameraTransitionAnimation,
            DisplayInfo

        }
        private void Update()
        {
            if (!vMainEventStarted && !ActivitiesContextController.UsingSquats)
            {
                //start the event
                if (Input.GetKeyDown(HeddokoDebugKeyMappings.Pause))
                {
                    vMainEventStarted = true;
#if ( !UNITY_EDITOR    )
                    PlayerStreamManager.ChangePauseState();
#endif                   


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
                    CameraMovementPointSetting vPointParameters = NFLCamController.GetPointAt(0);
                    //CameraMovementPointSetting vNextPoint = NFLCamController.GetPointAt(1);
                    CamLookAt.TargetPos = vPointParameters.LookAtTarget.position;

                    NFLCamController.MoveTowardsStartPos();
                    ArcAngleFill.Show();
                    AnalysisContentPanel.Show();
                    ArcAngleFill.SetParametersFromPoint(vPointParameters);
                    AnalysisContentPanel.UpdateFeedbackText(vPointParameters);
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
                                    CameraMovementPointSetting vPointParameters = NFLCamController.GetPointAt(NFLCamController.NextCamIndex);
                                    ArcAngleFill.SetParametersFromPoint(vPointParameters);
                                    AnalysisContentPanel.UpdateFeedbackText(vPointParameters);
                                }
                            }

                            // there is no analysis view attached to the current index
                            else if (vCurrAnalysisView == null)
                            {
                                if (Input.GetKeyDown(HeddokoDebugKeyMappings.MoveNext))
                                {
                                    NFLCamController.MoveToNextPos();
                                    CameraMovementPointSetting vPointParameters = NFLCamController.GetPointAt(NFLCamController.NextCamIndex);
                                    ArcAngleFill.SetParametersFromPoint(vPointParameters);
                                    AnalysisContentPanel.UpdateFeedbackText(vPointParameters);
                                }

                            }
                            if (vCurrAnalysisView != null && !vCurrAnalysisView.InView)
                            {
                                vCurrAnalysisView.Show();
                            }
                        }

                      /*  //check if next pos is 0, go back into training view
                        if (NFLCamController.NextCamIndex == 0)
                        {
                            Reset();
                        }*/
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
            bool vIsDebugBuild = false;
#if  UNITY_EDITOR
            vIsDebugBuild = true;

#elif DEVELOPMENT_BUILD
            vIsDebugBuild = true;
#endif
            if (!vIsDebugBuild)
            {
                PlayerStreamManager.ChangePauseState();
            }
            CamLookAt.enabled = false;
            MoveCameraToPositon.gameObject.SetActive(MoveCamState);
            CameraController.enabled = true;
            MoveCameraToPositon.enabled = true;
            CamBlur.enabled = false;
            NFLCamController.Reset();
            NFLCamController.enabled = false;
            GroupFadeEffect.Show();
            CameraMovementPointSetting vPointParameters = NFLCamController.GetPointAt(0);

            CamLookAt.TargetPos = vPointParameters.LookAtTarget.position;
            ArcAngleFill.SetParametersFromPoint(vPointParameters);
            AnalysisContentPanel.Hide();
            ArcAngleFill.Hide();

        }

        /// <summary>
        /// In the event that the back button was pressed, reset the camera back to its original position
        /// </summary>
        public void BackButtonPressed()
        {
            AnalysisContentPanel.Hide();
            GroupFadeEffect.ForceShow();
            vMainEventStarted = false;
            PlayerStreamManager.ResumeFromPauseState();
            CamLookAt.enabled = false;
            MoveCameraToPositon.gameObject.SetActive(MoveCamState);
            CameraController.enabled = true;
            MoveCameraToPositon.enabled = true;
            CamBlur.enabled = false;
            NFLCamController.Reset();
            NFLCamController.enabled = false;

            ArcAngleFill.Hide();
        }


    }
}
