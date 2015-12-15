
/** 
* @file PlayerStreamManager.cs
* @brief Contains the PlayerStreamManager class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Demos;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI.MainScene.Model; 
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

        public string SquatRecordingUUID;
        public string BikeRecordingUUID;


        // pause thread routine started
        private bool mResetRoutineStarted = false;

        private string mBodyRecordingUUID;
        private bool mUsingBrainpack = false;

        private bool mCanUseBrainpack = false;


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
        /// Play a squat or bike
        /// </summary>
        /// <param name="vPlaySquat"></param>
        public void PlaySquats(bool vPlaySquat)
        {
           // if (!mPlayButtonPushed)
       //     {
                if (CurrentBodyInPlay != null)
                {
                    if (vPlaySquat)
                    {
                        mBodyRecordingUUID = SquatRecordingUUID;
                    }
                    else
                    {
                        mBodyRecordingUUID = BikeRecordingUUID;
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
            // ChangeState(BodyPlaybackState.StreamingFromBrainPack);
        }

 
        public void StartCountingSquatsOn()
        {
            if(CurrentBodyInPlay != null)
            {
                
            }
        }
 
        /// <summary>
        /// Listens to when the BrainpackController is in a disconnected state
        /// </summary>
        private void OnBrainpackDisconnectListener()
        {
            mCanUseBrainpack = false;
            //need to check if the brainpack is disconnected during the view. TODO : BIG TODO



            //remove the halo effect 
            /*    FadeInFadeOutEffect vFadeInFadeOutEffectToDestroy = PlayButton.GetComponent<FadeInFadeOutEffect>();
            if (vFadeInFadeOutEffectToDestroy != null)
            {
                Destroy(vFadeInFadeOutEffectToDestroy);
            }*/
            //PlayButton.image.sprite = mPlayButtonOriginalIcon; //reset the play button back to its original sprite
            //PlayButton.GetComponentInChildren<Text>().text = "Play";
            //    PlayButton.interactable = true;
            //   ChangeState(BodyPlaybackState.Waiting);
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
    }
}
