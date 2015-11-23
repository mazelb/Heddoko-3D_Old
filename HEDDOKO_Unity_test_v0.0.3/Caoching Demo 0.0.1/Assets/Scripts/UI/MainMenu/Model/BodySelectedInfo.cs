/** 
* @file BodySelectedInfo.cs
* @brief Contains the BodySelectedInfo class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Communication;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.UI.MainScene.Model
{
    /// <summary>
    /// Represents which recording a user has chosen
    /// </summary>
    public class BodySelectedInfo : MonoBehaviour
    {
        private static BodySelectedInfo mInstance;

        public delegate void BodyRecordingChanged();

        public event BodyRecordingChanged BodyRecordingChangedEvent;

        [SerializeField]
        private SuitSocketClientSettings mSocketClientSettings; 

        
        public string mSelectedRecordingPath;
        public string mSelectedBody;
        public int TotalRecordingsAvailable;
        private Dictionary<string, BodyFramesRecording> mBodyRecordingMap = new Dictionary<string, BodyFramesRecording>(1); 
        /**
        * UpdateNumberOfRecordings 
        * @brief Updates the number of recordings 
        */

        /// <summary>
        /// Updates the number of recordings 
        /// </summary>
        public void UpdateNumberOfRecordings()
        {
            TotalRecordingsAvailable = BodyRecordingsMgr.Instance.ScanRecordings(FilePathReferences.RecordingsDirectory);

        }

        /**
        * CurrentSelectedRecording 
        * @brief Returns the current selected Recording based on the selected recording from the main menu
        */
        /// <summary>
        /// Returns the current selected Recording based on the selected recording from the main menu
        /// </summary>
        public BodyFramesRecording CurrentSelectedRecording
        {
            get
            {
                if (!string.IsNullOrEmpty(mSelectedRecordingPath)  && mBodyRecordingMap.ContainsKey(mSelectedRecordingPath))
                {
                    return mBodyRecordingMap[mSelectedRecordingPath];
                }
                else if(!string.IsNullOrEmpty(mSelectedRecordingPath))
                {
                    UpdateCurrentBodyFrameRecording();
                    return mBodyRecordingMap[mSelectedRecordingPath];
                }
                return null;
            }
        }

        /// <summary>
        /// Singleton instance of the class
        /// </summary>
        public static BodySelectedInfo Instance
        {
            get
            {
                if (mInstance == null)
                { 
                   
                   GameObject vGo = GameObject.FindGameObjectWithTag("Controllers");
                    if (vGo != null)
                    {
                        mInstance = vGo.GetComponent<BodySelectedInfo>();
                    }
                    if(mInstance ==null)
                    {
                        mInstance = FindObjectOfType<BodySelectedInfo>();
                    }
                    if (mInstance == null)
                    {
                        GameObject vNewGo = new GameObject("BodySelectedInfo");
                        mInstance= vNewGo.AddComponent<BodySelectedInfo>();
                    }
                    DontDestroyOnLoad(mInstance.gameObject);
                }
                return mInstance;
            }
        }

        void Awake()
        {
            Instance.Init();
        }

        void OnApplicationQuit()
        {
            BodyRecordingChangedEvent = null;
        }
        void Init()
        {
            
        }
        /**
        * UpdateNumberOfRecordings 
        * @brief Updates the number of recordings 
        */ 
        /// <summary>
        /// Updates the number of recordings 
        /// </summary>
        public void UpdateSelectedRecording(int vRecordingIndex)
        {
            if (TotalRecordingsAvailable == 0)
            {
                TotalRecordingsAvailable = BodyRecordingsMgr.Instance.FilePaths.Length;
            }
            if (vRecordingIndex >= 0 && vRecordingIndex < TotalRecordingsAvailable)
            {
                mSelectedRecordingPath = BodyRecordingsMgr.Instance.FilePaths[vRecordingIndex];
                if (BodyRecordingChangedEvent != null)
                {
                    BodyRecordingChangedEvent();
                } 
            }

        }
        /**
        * UpdateCurrentBodyFrameRecording 
        * @brief helper method update that the current bodyframes recording
        */
        /// <summary>
        ///helper method update that updates the current bodyframes recording
        /// </summary>
        public void UpdateCurrentBodyFrameRecording()
        {
            if (!mBodyRecordingMap.ContainsKey(mSelectedRecordingPath))
            {
                BodyRecordingsMgr.Instance.ReadRecordingFile(mSelectedRecordingPath);
                //the latest item to be placed in the list is now the current body frame recording
                BodyFramesRecording vCurrBFR = BodyRecordingsMgr.Instance.Recordings[BodyRecordingsMgr.Instance.Recordings.Count - 1];
                mBodyRecordingMap.Add(mSelectedRecordingPath,vCurrBFR);
                //notify interested listeners that the recording has changed
          
            }
        }
    }
}
