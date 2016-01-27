/** 
* @file NFLDemoController.cs
* @brief Contains the NFLDemoController class
* @author Mohammed Haider(Mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using System;
using System.Collections;
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

        [SerializeField]
        private bool mResetFrameInitiated = false;
        private CurrentAnimationState vCurrentState = CurrentAnimationState.InTrainingView;


        private int mCurrentCamPos = 0;
        private int mNextCamPos;

        public ArcAngleFill ArcAngleFill;
        public AnalysisContentPanel AnalysisContentPanel;

        public float Timer = 3.79f;
        [SerializeField]
        private float mTimer = 3.79f;
        public bool StartTimer { get; set; }

        private enum CurrentAnimationState
        {
            InTrainingView,
            HidingGroupAnimating,
            CameraTransitionAnimation,
            DisplayInfo

        }

        void Awake()

        {
            mTimer = Timer;
        }

        private void Update()
        {
             
        }

        public void ResetTimer()
        {
            mTimer = Timer;
        }
        private void LateUpdate()
        {
            //is the correct context
            bool vCorrectContext = ActivitiesContextController.CurrentState ==
                                   ActivitiesContextController.ActivitiesContextViewState.LearnByRecording
                                   ||
                                   ActivitiesContextController.CurrentState ==
                                   ActivitiesContextController.ActivitiesContextViewState.Train;

            if (!vMainEventStarted && !ActivitiesContextController.UsingSquats && vCorrectContext)
            {
                
                    if (StartTimer)
                    {
                        mTimer -= Time.deltaTime;
                    }
              
                //start the event
                if (Input.GetKeyDown(HeddokoDebugKeyMappings.Pause) || (mTimer <= 0.001f))
                {
                    vMainEventStarted = true;
                    StartTimer = false;
                    mResetFrameInitiated = false;
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
                    CameraMovementPointSetting vPointParameters = NFLCamController.GetPointAt(0);
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
                    PlayerStreamManager.ClearBuffer();
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

            if (Input.GetKeyDown(HeddokoDebugKeyMappings.SwitchToRecordingFromLive) &&
                (ActivitiesContextController.CurrentState ==
                                   ActivitiesContextController.ActivitiesContextViewState.Train)

                )
            {
                BackButtonPressed();
                ActivitiesContextController.NonSquatHookFunction();
                PlayerStreamManager.ResumeFromPauseState();

            }

            if (Input.GetKeyDown(HeddokoDebugKeyMappings.SkipToLiveViewFromRecordingView) &&
                (ActivitiesContextController.CurrentState ==
                                   ActivitiesContextController.ActivitiesContextViewState.LearnByRecording)
                )
            {
                BackButtonPressed();
                ActivitiesContextController.SwitchtoTrainingViewState();
                PlayerStreamManager.ResumeFromPauseState();
            }

            /*    if (Input.GetKeyDown(HeddokoDebugKeyMappings.SkipToLiveViewFromRecordingView))
                {
                    Reset();
                    ActivitiesContextController.SwitchtoTrainingViewState();
                    PlayerStreamManager.ResumeFromPauseState();
                }
                if (Input.GetKeyDown(HeddokoDebugKeyMappings.SwitchToRecordingFromLive))
                {
                    Reset();
                    ActivitiesContextController.NonSquatHookFunction();
                    PlayerStreamManager.ResumeFromPauseState();
                }*/
        }

        /// <summary>
        /// Resets the view back to training
        /// </summary>
        public void Reset()
        {
            vMainEventStarted = false;

            /*#if  UNITY_EDITOR
                        vIsDebugBuild = true;

            #elif DEVELOPMENT_BUILD
                        vIsDebugBuild = true;
            #endif*/

            PlayerStreamManager.ChangePauseState();
            PlayerStreamManager.ClearBuffer();
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
            StartTimer = false;
            mResetFrameInitiated = false;
            mTimer = Timer;

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
            StartTimer = false;
            mResetFrameInitiated = false;
            mTimer = Timer;

        }

        private IEnumerator ClearBufferAfterNSeconds(float vSecs)
        {
            yield return new WaitForSeconds(vSecs);
            PlayerStreamManager.ClearBuffer();
        }


    }
}
