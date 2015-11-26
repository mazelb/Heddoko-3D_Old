
/** 
* @file BodyPlayer.cs
* @brief Contains the BodyPlayer class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System.Collections;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI.MainScene.Model;
using Assets.Scripts.Utils.UnityUtilities;
using Assets.Scripts.Utils.UnityUtilities.Repos;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Demos
{
    /**
    * BodyPlayer class , essentially a player that streams from a brainpack or recording. Todo: this class holds both the view and controls. 
    * @brief BodyPlayer class  
    */

    /// <summary>
    /// Preps the body for playing a recording 
    /// </summary>
    public class BodyPlayer : MonoBehaviour
    {
        public Body CurrentBodyInPlay { get; set; }
        public Button PlayButton;
        private BodyPlaybackState mCurrentState = BodyPlaybackState.Waiting;
        private bool mPlayButtonPushed;
        public Button ResetButton;
        public float PauseThreadTimer = 1f;
        private float mInternalTimer = 1f;
        private bool mResetRoutineStarted = false; // pause thread routine started
        private string mBodyRecordingUUID;
        private bool mUsingBrainpack = false;
        private Sprite mBluetoothIcon;
        private Sprite mPlayButtonOriginalIcon;
        public DisplayLegAngleExtractions DisplayLegAngleExtractions;
        private Sprite BluetoothIcon 
        {
            get
            {
                if (mBluetoothIcon == null)
                {
                    mBluetoothIcon = SpriteRepo.BluetoothIcon;
                }
                return mBluetoothIcon;
            }
        }
        
        #region unity functions
        /**
        * Start 
        * @brief  On the start of the scene, initialize all the components to be able to start playing
        */
        /// <summary>
        /// On the start of the scene, initialize all the components to be able to start playing
        /// </summary>
        void Start()
        {
            mPlayButtonOriginalIcon = PlayButton.image.sprite;
            BodyFramesRecording vRec = BodySelectedInfo.Instance.CurrentSelectedRecording;
            if (vRec != null)
            {
                mBodyRecordingUUID = vRec.BodyRecordingGuid;
                mBodyRecordingUUID = vRec.BodyRecordingGuid;
                CurrentBodyInPlay = BodiesManager.Instance.GetBodyFromRecordingUUID(mBodyRecordingUUID);
            }

            PlayButton.onClick.AddListener(Play);
            ResetButton.onClick.AddListener(ResetInitialFrame);

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
        void OnDisable()
        { 
            BrainpackConnectionController.ConnectedStateEvent -= OnBrainpackConnectSuccessListener;
            BrainpackConnectionController.DisconnectedStateEvent -= OnBrainpackDisconnectListener;
        }

        #endregion
        /**
        * Play 
        * @brief Will play the recording for the prepped body
        */
        public void Play()
        {
            if (!mPlayButtonPushed)
            {
                if (CurrentBodyInPlay != null)
                {
                    mPlayButtonPushed = true;
                    PlayButton.gameObject.SetActive(false);
                    ChangeState(BodyPlaybackState.PlayingRecording);
                    DisplayLegAngleExtractions.CurrentBody = CurrentBodyInPlay;
                }
            }
        }
        /**
        * ResetInitialFrame 
        * @brief will set the Initial Frame
        */
        public void ResetInitialFrame()
        {
             CurrentBodyInPlay.View.ResetInitialFrame();
            StartCoroutine(StartPausingCountdown());
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
                            CurrentBodyInPlay.PlayRecording(mBodyRecordingUUID);
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
                            CurrentBodyInPlay.PlayRecording(mBodyRecordingUUID);
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

        private IEnumerator StartPausingCountdown()
        {
            if (mResetRoutineStarted)
            {
                mInternalTimer += PauseThreadTimer; //if this has already started, just add to the timer and then exit
                yield break;
            }
            mInternalTimer = PauseThreadTimer;
            mResetRoutineStarted = true;
      ChangePauseState();
            while (true)
            {
                mInternalTimer -= Time.deltaTime;
                if (mInternalTimer < 0)
                {
                    break;
                }
                yield return null;
            }
            ChangePauseState();
            mResetRoutineStarted = false;
        }
        /// <summary>
        /// Listens to when a recording has been selected. sets the current state of the class accordingly
        /// </summary>
        private void ListenToBodyRecordingsChange()
        {
            PlayButton.gameObject.SetActive(true);
            if (CurrentBodyInPlay != null)
            {
                CurrentBodyInPlay.StopThread(); 
            }
            BodyFramesRecording vRec = BodySelectedInfo.Instance.CurrentSelectedRecording;
            mBodyRecordingUUID = vRec.BodyRecordingGuid;
            CurrentBodyInPlay = BodiesManager.Instance.GetBodyFromRecordingUUID(mBodyRecordingUUID);
            mPlayButtonPushed = false;
            ChangeState(BodyPlaybackState.Waiting);
        }
        /// <summary>
        /// Listens to when the brainpackcontroller is in a connected state
        /// </summary>
        private void OnBrainpackConnectSuccessListener()
        {
           FadeInFadeOutEffect vFadeInFadeOutEffect= PlayButton.gameObject.AddComponent<FadeInFadeOutEffect>();
            vFadeInFadeOutEffect.FadeEffectTime = 2.5f;
            vFadeInFadeOutEffect.MaxAlpha = 255f;
            vFadeInFadeOutEffect.MinAlpha = 20;
            PlayButton.image.sprite = BluetoothIcon;
            PlayButton.GetComponentInChildren<Text>().text = "";
            PlayButton.interactable = false;
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
             
            ChangeState(BodyPlaybackState.StreamingFromBrainPack);
        }
        /// <summary>
        /// Listens to when the BrainpackController is in a disconnected state
        /// </summary>
        private void OnBrainpackDisconnectListener()
        {
            //remove the halo effect 
            FadeInFadeOutEffect vFadeInFadeOutEffectToDestroy = PlayButton.GetComponent<FadeInFadeOutEffect>();
            if (vFadeInFadeOutEffectToDestroy != null)
            {
                Destroy(vFadeInFadeOutEffectToDestroy);
            }
            PlayButton.image.sprite = mPlayButtonOriginalIcon; //reset the play button back to its original sprite
            PlayButton.GetComponentInChildren<Text>().text = "Play";
            PlayButton.interactable = true;
            ChangeState(BodyPlaybackState.Waiting);
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
    }
}
