/** 
* @file PlaybackControl.cs
* @brief Contains the PlaybackControl  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using Assets.Scripts.Frames_Pipeline;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls;
using Assets.Scripts.UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording
{

    /// <summary>
    /// Provides controls for recording play back
    /// </summary>
    public class PlaybackControl : AbstractControlPanel
    {
        private float mPlaybackSpeed = 1f;

        private RecordingPlaybackTask PlaybackTask;
        public Slider SeekerBar;
        public Button PlayPauseButton;
        public Button SettingsButton;
        private Body mBody; 
        public PlaybackSlider PlaybackSlider;
        public PlaybackControlView PlaybackControlView;
        private PlayerStreamManager PlayerStreamManager;
        private PlaybackState mCurrentState;
        public PlaybackState CurrentState
        {
            get
            {
                return mCurrentState;
            }
            set { mCurrentState = value; }
        }
        /// <summary>
        /// Sets the playback speed. Get;Set;
        /// </summary>
        public float PlaybackSpeed
        {
            get
            {
                return mPlaybackSpeed;
            }
            set
            {
                if (mPlaybackSpeed == 0)
                {
                    if (PlaybackTask != null)
                    {
                        PlaybackTask.IsPaused = true;
                    }
                }
                mPlaybackSpeed = value;
            }
        }

        /// <summary>
        /// Updates recording 
        /// </summary>
        public void UpdateBody(Body vNewbody)
        {
            mBody = vNewbody;
            PlaybackTask = vNewbody.MBodyFrameThread.PlaybackTask;
        }

        /// <summary>
        /// Changes the current state of the player
        /// </summary>
        /// <param name="vNewState"></param>
        public void ChangeState(PlaybackState vNewState)
        {
            if (ValidateStateChange(vNewState))
            {
                switch (CurrentState)
                {
                    //from a new recording. Tell the current player stream manager to start playing 
                    case PlaybackState.Null:
                        if (vNewState == PlaybackState.Play)
                        {
                            
                        }
                        break;
                    case PlaybackState.Play:
                        if (vNewState == PlaybackState.Null)
                        {
                            PlayerStreamManager.PlayCurrentBody();
                        }
                        break;


                }
            }
        }
        /// <summary>
        /// checks the validity of the new state compared to the current state and checks if the transition is valid
        /// </summary>
        /// <param name="vNewState">new state to change to </param>
        /// <returns>valid state change</returns>
        private bool ValidateStateChange(PlaybackState vNewState)
        {

            switch (CurrentState)
            {
                case PlaybackState.Null:
                    if (vNewState == PlaybackState.Play)
                    {
                        return true;
                    }
                    break;

                    //play state can freely transition to any other state
                case PlaybackState.Play:
                    return true;
                    break;
                case PlaybackState.GoForward:
                    if (vNewState == PlaybackState.Null || 
                        vNewState == PlaybackState.Play
                        || vNewState == PlaybackState.GoBackward)
                    {
                        return true;
                    }

                    break;
                case PlaybackState.GoBackward:
                    if (vNewState == PlaybackState.Null ||
                       vNewState == PlaybackState.Play
                       || vNewState == PlaybackState.GoForward)
                    {
                        return true;
                    }
                    break;
                case PlaybackState.Pause:
                    if (vNewState == PlaybackState.Null ||
                       vNewState == PlaybackState.Play
                       || vNewState == PlaybackState.GoForward
                       || vNewState == PlaybackState.GoBackward)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// On awake, set all the references
        /// </summary>
        private void Awake()
        {
            PlayerStreamManager = GameObject.FindObjectOfType<PlayerStreamManager>();
           // UpdateBody();
        }
        /// <summary>
        /// Plays or pauses the recording
        /// </summary>
        public void SetPlayState()
        {
            //check what state the player manager is in 
            if (PlayerStreamManager.CurrentState == PlayerStreamManager.BodyPlaybackState.Waiting)
            {
                PlayerStreamManager.PlayCurrentBody();
            }
            else
            {
                PlayerStreamManager.ChangePauseState();
            }

        }

        /// <summary>
        /// Pauses recording
        /// </summary>
        public void Pause()
        {
            PlayerStreamManager.CurrentBodyInPlay.MBodyFrameThread.FlipPauseState();
        }

        public void LaunchPlaySettings()
        {

        }


        /// <summary>
        /// Sets the position of the playback
        /// </summary>
        /// <param name="vNewPos">the new position, clamped between 0 and 1</param>
        public virtual void Tracking(float vNewPos)
        {

        }

        /// <summary>
        /// Plays back recording in reverse
        /// </summary>
        public virtual void Rewind()
        {

        }



    }
    public enum PlaybackState
    {
        Null,
        Play,
        Pause,
        GoBackward,
        GoForward
    }
}
