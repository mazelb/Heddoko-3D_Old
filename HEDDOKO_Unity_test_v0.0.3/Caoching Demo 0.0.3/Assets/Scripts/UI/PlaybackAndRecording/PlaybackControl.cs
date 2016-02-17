/** 
* @file PlaybackControl.cs
* @brief Contains the PlaybackControl abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using Assets.Scripts.UI.MainMenu; 
using UnityEngine.UI;

namespace Assets.Scripts.UI.PlaybackAndRecording
{
     
    /// <summary>
    /// Provides controls for play back
    /// </summary>
   public class PlaybackControl : AbstractView
    {
        private float mPlaybackSpeed = 1f;
        public Slider SeekerBar;
        public Button PlayPauseButton;
        public Button SettingsButton;
        public PlayerStreamManager PlayerStreamManager;

        /// <summary>
        /// On awake, set all the references
        /// </summary>
        private void Awake()
        {
            PlayPauseButton = transform.FindChild("PlayButton").GetComponent<Button>();
            PlayPauseButton.onClick.AddListener(SetPlayState);
            SeekerBar = transform.FindChild("Seeker").GetComponent<Slider>();
            SettingsButton = transform.FindChild("SettingsButton").GetComponent<Button>();
            SettingsButton.onClick.AddListener(LaunchPlaySettings);
            PlayerStreamManager = FindObjectOfType<PlayerStreamManager>();

        }
        /// <summary>
        /// Plays or pauses the recording
        /// </summary>
        public void SetPlayState()
        { 
            //check what state the player manager is in 
            if (PlayerStreamManager.CurrentState == PlayerStreamManager.BodyPlaybackState.Waiting)
            {
                PlayerStreamManager.Play();
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
            
        }

        public void LaunchPlaySettings()
        {
            
        }
        /// <summary>
        /// Sets the forward playback speed, minimum value is set to 0. 
        /// </summary>
        /// <param name="vNewVal">playback speed</param>
        public  void SetForwardPlaySpeed(float vNewVal)
        {
            if (vNewVal < 0)
            {
                vNewVal = 0;
            }

            mPlaybackSpeed = vNewVal;
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
}
