/** 
* @file BodySelectedInfo.cs
* @brief Contains the BodySelectedInfo class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

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
                if (mBodyRecordingMap.ContainsKey(mSelectedRecordingPath))
                {
                    return mBodyRecordingMap[mSelectedRecordingPath];
                }
                else
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
                    GameObject vGo = new GameObject("Info holder");
                    mInstance = vGo.AddComponent<BodySelectedInfo>();
                    DontDestroyOnLoad(mInstance.gameObject);
                }
                return mInstance;
            }
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
            if (vRecordingIndex >= 0 && vRecordingIndex < TotalRecordingsAvailable)
            {
                mSelectedRecordingPath = BodyRecordingsMgr.Instance.FilePaths[vRecordingIndex];
               
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
            }
        }
    }
}
