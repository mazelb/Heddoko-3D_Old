/** 
* @file PlaybackSlider .cs
* @brief Contains the PlaybackSlider abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Frames_Pipeline;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls
{
    /// <summary>
    /// A slider that controls the position of the slider when playing back a recording. 
    /// </summary>
    public class PlaybackSlider: AbstractSubControl
    {
        private float mTotalRecordingTime;

        public Text CurrentPlayTimeText;
        public Text TotalRecordingTimeText;
        public Slider PlaySlider;
        public PlaybackControl PlayControl;
        private RecordingPlaybackTask mPlaybackTask;

        private void Awake()
        {
            PlaySlider.onValueChanged.AddListener(UpdateSliderPosition);
        }
        public void UpdateNewRecording(RecordingPlaybackTask vPlaybackTask)
        {
            //retreive the total count of converted frames
            mPlaybackTask = vPlaybackTask;
            int vTotal = vPlaybackTask.RecordingCount;
            mTotalRecordingTime = vPlaybackTask.TotalRecordingTime;
            TotalRecordingTimeText.text = FormatMSToStr(mTotalRecordingTime);
            PlaySlider.maxValue = vTotal;
           
        }

        /// <summary>
        /// Updates the slider position
        /// </summary> 
        public void UpdateSliderPosition(float vNewPos)
        {
            
        }

        /// <summary>
        /// Updates the total 
        /// </summary>
        /// <param name="vTotalTime"></param>
        string FormatMSToStr(float vTotalTime)
        {
            //seperate the total time passed in into three ints. 
            float vTempTime = vTotalTime;
            int vHour = (int)(vTempTime / 3600000);
            vTempTime -= vHour*3600000;
            int vMin = (int)(vTempTime / 60000);
            vTempTime -= vMin * 60000;
            int vSec = (int)(vTempTime / 6000);

            //Format the time segements in case the values are less than 10
            string vToHour = vHour < 10 ? "0" + vHour + ":" : vHour + ":";
            string vToMin  = vMin < 10 ? "0" + vMin + ":" : vMin + ":";
            string vToSec = vSec < 10 ? "0" + vSec + ":" : vSec + ":";

            return vToHour + vToMin + vToSec;
        }


        /// <summary>
        /// Updates the current time on CurrentPlayTimeText ui text
        /// </summary>
        /// <param name="vIndex"></param>
        void UpdateCurrentTime(int vIndex)
        {
            int vTempIndex = vIndex;
            if (vTempIndex < 0)
            {
                vTempIndex = 0;
            }
            else if (vTempIndex > PlaySlider.maxValue)
            {
                vTempIndex = (int)PlaySlider.maxValue - 1;
            }

            //update the slider
            PlaySlider.value = vTempIndex;
            if (mPlaybackTask == null)
            {
                return;
            }

            float vCurrTimeStamp = mPlaybackTask.GetBodyFrameAtIndex(vIndex).Timestamp;
            float vStartTime = mPlaybackTask.GetBodyFrameAtIndex(0).Timestamp;
            float vCurrentTime = vCurrTimeStamp - vStartTime;
            CurrentPlayTimeText.text = "/" + FormatMSToStr(vCurrentTime);
        }


    }
}