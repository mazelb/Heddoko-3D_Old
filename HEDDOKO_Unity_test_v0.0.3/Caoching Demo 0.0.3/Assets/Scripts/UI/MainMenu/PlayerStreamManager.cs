
/** 
* @file PlayerStreamManager.cs
* @brief Contains the PlayerStreamManager class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System.Collections.Generic;
using Assets.Demos;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI.MainScene.Model;
using Assets.Scripts.UI.Metrics;
using Assets.Scripts.Utils.DebugContext;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MainMenu
{
    /// <summary>
    /// Preps the body for playing a recording. BodyPlayer class , essentially a player that streams from a brainpack or recording. Todo: this class holds both the view and controls. 
    /// </summary>
    public class PlayerStreamManager : MonoBehaviour
    {
        public Body CurrentBodyInPlay { get; set; }
        private BodyPlaybackState mCurrentState = BodyPlaybackState.Waiting;

        //  private bool mPlayButtonPushed; 
        public float PauseThreadTimer = 1f;
        private float mInternalTimer = 1f;
        public float OriginalSpineHeight = 0.09226318f;
        public float SpineYOffset = 2.13f;
        public Transform Spine;
        public bool UseLocalPos;
        public string SquatRecordingUUID;
        public string BikeRecordingUUID;

        // pause thread routine started
        private bool mResetRoutineStarted = false;

        private string mBodyRecordingUUID;
        private bool mUsingBrainpack = false;
        public bool SpineSplitDisabled = false;
        private bool mCanUseBrainpack = false;

        public Button[] TPoseButtons;

        public List<IResettableMetricView> ResettableViews = new List<IResettableMetricView>(4);

        /// <summary>
        /// On the start of the scene, initialize all the components to be able to start playing
        /// </summary>
        void Awake()
        {
            // mPlayButtonOriginalIcon = PlayButton.image.sprite;
            BodyFramesRecording vRec = BodySelectedInfo.Instance.CurrentSelectedRecording;
            if (vRec != null)
            {
                mBodyRecordingUUID = vRec.BodyRecordingGuid;
                mBodyRecordingUUID = vRec.BodyRecordingGuid;
                CurrentBodyInPlay = BodiesManager.Instance.GetBodyFromRecordingUUID(mBodyRecordingUUID);
            }

            if (CurrentBodyInPlay == null)
            {
                //check what the current count is
                int vBodiesCount = BodiesManager.Instance.Bodies.Count;

                if (vBodiesCount == 0)
                {
                    //create a new body from the body manager
                    BodiesManager.Instance.CreateNewBody("BrainpackPlaceholderBody");
                }

                //get the first body
                CurrentBodyInPlay = BodiesManager.Instance.Bodies[0];
            }
            for (int i = 0; i < TPoseButtons.Length; i++)
            {
                TPoseButtons[i].onClick.AddListener(ResetPlayer);
            }
        }

        /// <summary>
        /// OnEnable, hook listeners
        /// </summary>
        void OnEnable()
        {
            BodySelectedInfo.Instance.BodyRecordingChangedEvent += ListenToBodyRecordingsChange;
            BrainpackConnectionController.ConnectedStateEvent += OnBrainpackConnectSuccessListener;
            BrainpackConnectionController.DisconnectedStateEvent += OnBrainpackDisconnectListener;
        }

        /// <summary>
        /// OnDisable, unhook listeners
        /// </summary>
        public void OnDisable()
        {
            BrainpackConnectionController.ConnectedStateEvent -= OnBrainpackConnectSuccessListener;
            BrainpackConnectionController.DisconnectedStateEvent -= OnBrainpackDisconnectListener;
        }


        /**
        * Play 
        * @brief Will play the recording for the prepped body
        */
        public void Play()
        {
            if (CurrentBodyInPlay != null)
            {
                // mPlayButtonPushed = true; 
                ChangeState(BodyPlaybackState.PlayingRecording);
            }
        }

        /// <summary>
        /// Sets/unsets the torso from the hips
        /// </summary>
        /// <param name="vFlag"></param>
        public void StickTorsoToHips(bool vFlag)
        {

            if (CurrentBodyInPlay != null)
            {
                Vector3 vPos = Vector3.zero;
                if (vFlag)
                {
                    mBodyRecordingUUID = SquatRecordingUUID;
                    if (Spine)
                    {
                        if (UseLocalPos)
                        {
                            vPos = Spine.localPosition;
                            vPos.y = OriginalSpineHeight;
                            Spine.localPosition = vPos;
                        }
                        else
                        {
                            vPos = Spine.position;
                            vPos.y = OriginalSpineHeight;
                            Spine.position = vPos;
                        }

                    }
                }
                else
                {
                    mBodyRecordingUUID = BikeRecordingUUID;
                    if (!SpineSplitDisabled)
                    {
                        if (UseLocalPos)
                        {
                            vPos = Spine.localPosition;
                            vPos.y = SpineYOffset;
                            Spine.localPosition = vPos;
                        }
                        else
                        {
                            vPos = Spine.position;
                            vPos.y = SpineYOffset;
                            Spine.position = vPos;
                        }
                    }


                }

                BodySegment vSegment = CurrentBodyInPlay.BodySegments.Find(
                       x => x.SegmentType == BodyStructureMap.SegmentTypes.SegmentType_Torso);
                if (vSegment != null)
                {
                    //vSegment.IsTrackingHeight = vFlag;
                }
                //     mPlayButtonPushed = true;
                ChangeState(BodyPlaybackState.PlayingRecording);
            }
            // }
        }



        /**
       * ResetOrientations 
       * @brief will set the Initial Frame
       */
        public void ResetInitialFrame()
        {
            if (CurrentBodyInPlay != null && CurrentBodyInPlay.InitialBodyFrame != null)
            {
                CurrentBodyInPlay.View.ResetInitialFrame();
            }
        }

        /// <summary>
        /// Sets the current body to stream from recording
        /// </summary>
        public void SetBodyToStreamFromRecording()
        {
            ChangeState(BodyPlaybackState.PlayingRecording);
        }

        /// <summary>
        /// sets the current body to stream from the brainpack
        /// </summary>
        public void SetBodytoStreamFromBrainpack()
        {
            ChangeState(BodyPlaybackState.StreamingFromBrainPack);
        }

        /// <summary>
        /// Safely change the current state of the body player
        /// </summary>
        /// <param name="vNewstate"></param>
        void ChangeState(BodyPlaybackState vNewstate)
        {
            switch (mCurrentState)
            {
                case BodyPlaybackState.Waiting:
                    {
                        if (vNewstate == BodyPlaybackState.PlayingRecording)
                        {
                            CurrentBodyInPlay.StopThread();
                            if (!string.IsNullOrEmpty(mBodyRecordingUUID))
                            {
                                CurrentBodyInPlay.PlayRecording(mBodyRecordingUUID);
                            }
                            mCurrentState = vNewstate;
                            break;
                        }
                        if (vNewstate == BodyPlaybackState.StreamingFromBrainPack)
                        {
                            CurrentBodyInPlay.StopThread();
                            CurrentBodyInPlay.StreamFromBrainpack();
                            mCurrentState = vNewstate;
                        }
                        break;
                    }
                case BodyPlaybackState.PlayingRecording:
                    {
                        if (vNewstate == BodyPlaybackState.Waiting)
                        {
                            CurrentBodyInPlay.StopThread();
                            mCurrentState = vNewstate;
                            break;
                        }
                        if (vNewstate == BodyPlaybackState.StreamingFromBrainPack)
                        {
                            CurrentBodyInPlay.StopThread();
                            CurrentBodyInPlay.StreamFromBrainpack();
                            mCurrentState = vNewstate;
                        }
                        break;
                    }
                case BodyPlaybackState.StreamingFromBrainPack:
                    {
                        if (vNewstate == BodyPlaybackState.Waiting)
                        {
                            CurrentBodyInPlay.StopThread();
                            mCurrentState = vNewstate;
                            break;
                        }
                        if (vNewstate == BodyPlaybackState.PlayingRecording)
                        {
                            CurrentBodyInPlay.StopThread();
                            if (!string.IsNullOrEmpty(mBodyRecordingUUID))
                            {
                                CurrentBodyInPlay.PlayRecording(mBodyRecordingUUID);
                            }

                            mCurrentState = vNewstate;
                        }
                        break;
                    }
            }
        }

        /**
        * Pause 
        * @brief Pauses the recording's play back
        */
        public void ChangePauseState()
        {
            CurrentBodyInPlay.View.PauseFrame();
        }

        /// <summary>
        /// Listens to when a recording has been selected. sets the current state of the class accordingly
        /// </summary>
        private void ListenToBodyRecordingsChange()
        {
            if (CurrentBodyInPlay != null)
            {
                CurrentBodyInPlay.StopThread();
            }
            BodyFramesRecording vRec = BodySelectedInfo.Instance.CurrentSelectedRecording;
            mBodyRecordingUUID = vRec.BodyRecordingGuid;
            CurrentBodyInPlay = BodiesManager.Instance.GetBodyFromRecordingUUID(mBodyRecordingUUID);
            //mPlayButtonPushed = false;
            ChangeState(BodyPlaybackState.Waiting);
        }

        /// <summary>
        /// Listens to when the brainpackcontroller is in a connected state
        /// </summary>
        private void OnBrainpackConnectSuccessListener()
        {
            /*   FadeInFadeOutEffect vFadeInFadeOutEffect = PlayButton.gameObject.AddComponent<FadeInFadeOutEffect>();
               vFadeInFadeOutEffect.FadeEffectTime = 2.5f;
               vFadeInFadeOutEffect.MaxAlpha = 255f;
               vFadeInFadeOutEffect.MinAlpha = 20;
               todo: keep this here in case we add a bluetooth animation
    */
            //first check if there is a current body
            if (CurrentBodyInPlay == null)
            {
                //check what the current count is
                int vBodiesCount = BodiesManager.Instance.Bodies.Count;
                if (vBodiesCount == 0)
                {
                    //create a new body from the body manager
                    BodiesManager.Instance.CreateNewBody("BrainpackPlaceholderBody");
                }
                CurrentBodyInPlay = BodiesManager.Instance.Bodies[0]; //get the first body
            }
            mCanUseBrainpack = true;
        }


        public void StartCountingSquatsOn()
        {
            if (CurrentBodyInPlay != null)
            {

            }
        }

        /// <summary>
        /// Listens to when the BrainpackController is in a disconnected state
        /// </summary>
        private void OnBrainpackDisconnectListener()
        {
            mCanUseBrainpack = false;
        }

        /// <summary>
        /// Current state of the playback
        /// </summary>
        protected enum BodyPlaybackState
        {
            Waiting, //waiting for a response
            PlayingRecording,
            StreamingFromBrainPack
        }

        public void Stop()
        {
            CurrentBodyInPlay.StopThread();
        }

        private void Update()
        {
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.ResetFrame))
            {
                ResetPlayer();
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.Pause))
            {
                ChangePauseState();
            }
        }


        /// <summary>
        /// Resets the body and the metrics associated with body.
        /// </summary>
        public void ResetPlayer()
        {
            ResetInitialFrame();
            if (CurrentBodyInPlay != null)
            {
                RightLegAnalysis vRightLegAnalysis =
                      CurrentBodyInPlay.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as
                          RightLegAnalysis;
                vRightLegAnalysis.NumberofRightSquats = 0;
            }

            if (ResettableViews != null)
            {
                for (int i = 0; i < ResettableViews.Count; i++)
                {
                    ResettableViews[i].ResetValues();
                }
            }

        }
    }
}
