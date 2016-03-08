﻿/** 
* @file RecordingPlaybackSpeedDisplay.cs
* @brief Contains the RecordingPlaybackSpeedDisplay  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls;
using Assets.Scripts.UI.AbstractViews.Enums;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording
{
    /// <summary>
    /// Displays the current playback speed
    /// </summary>
    public class RecordingPlaybackSpeedDisplay : AbstractSubControl
    {
        private SubControlType mType = SubControlType.RecordingPlaybackSpeedDisplay;
        public Text CurrentSpeed;
        private string mDisplaySpeedString = "100%";
        private bool mIsPaused;
        public PlaybackControlPanel ParentPanel;
        public override SubControlType SubControlType
        {
            get { return mType; }
        }


        /// <summary>
        /// Getter, setter. Changes the Current Speed display to Paused if passed in value is true
        /// </summary>
        public bool IsPaused
        {
            get { return mIsPaused; }
            set
            {
                if (value)
                {
                    CurrentSpeed.text = "Paused.";
                }
                else
                {
                    CurrentSpeed.text = mDisplaySpeedString;
                }
                mIsPaused = value;
            }
        }

        /// <summary>
        /// Updates the current speed according to the value passed in. Multiplies it by 100 and shows up as a 
        /// whole number on screen.
        /// </summary>
        /// <param name="vNewVal"></param>
        public void UpdateSpeedText(float vNewVal)
        {
            int vVal = (int)(100*vNewVal);
            mDisplaySpeedString = vVal + "%";
            CurrentSpeed.text = mDisplaySpeedString;
        }

        public void Init(PlaybackControlPanel vParentPanel)
        {
            ParentPanel = vParentPanel;
        }
        public override void Disable()
        {
            CurrentSpeed.text = "";
        }

        public override void Enable()
        {
            CurrentSpeed.text = "100 %";
        }
 
    }
}