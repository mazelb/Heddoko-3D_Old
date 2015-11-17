
/** 
* @file RecordingPlayMove.cs
* @brief Contains the RecordingPlayMove class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System.Collections;
using Assets.Scripts.UI.MainScene.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Demos
{
    /**
    * RecordingPlayMove class 
    * @brief RecordingPlayMove class  
    * @note something 
    */

    /// <summary>
    /// Preps the body for playing a recording 
    /// </summary>
    public class RecordingPlayMove: MonoBehaviour
    {
        Body mBody;
        public Button PlayButton;
        private bool mPlayButtonPushed;
        public Button ResetButton; 
        public float PauseThreadTimer = 1f;
        private float mInternalTimer = 1f;
        private bool mResetRoutStarted = false; // pause thread routine started
        private string mBodyRecordingUUID;
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
            BodyFramesRecording vRec =  BodySelectedInfo.Instance.CurrentSelectedRecording;
            mBodyRecordingUUID = vRec.BodyRecordingGuid;
            mBody = BodiesManager.Instance.GetBodyFromRecordingUUID(mBodyRecordingUUID);
            PlayButton.onClick.AddListener(Play);
            ResetButton.onClick.AddListener(ResetInitialFrame);
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
                if (mBody != null)
                {
                    mPlayButtonPushed = true;
                    PlayButton.gameObject.SetActive(false);
                    mBody.PlayRecording(mBodyRecordingUUID);
                }
            }
        }
        /**
        * ResetInitialFrame 
        * @brief will set the Initial Frame
        */
        public void ResetInitialFrame()
        {
            mBody.View.ResetInitialFrame();
            StartCoroutine(StartCountdown());
        }
        /**
        * Pause 
        * @brief Pauses the recording's play back
        */
        public void ChangePauseState()
        {
            mBody.View.PauseFrame();
        }

        private IEnumerator StartCountdown()
        {
            if (mResetRoutStarted)
            {
                mInternalTimer += PauseThreadTimer; //if this has already started, just add to the timer and then exit
                yield break;
            }
            mInternalTimer = PauseThreadTimer;
            mResetRoutStarted = true;
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
            mResetRoutStarted = false;
        }

    }
}
