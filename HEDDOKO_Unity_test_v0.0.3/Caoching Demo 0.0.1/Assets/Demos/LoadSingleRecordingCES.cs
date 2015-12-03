/** 
* @file LoadSingleRecordingCES.cs
* @brief Contains the LoadSingleRecordingCES class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.UI.MainScene.Model;
using UnityEngine;

namespace Assets.Demos
{
    /// <summary>
    /// For the CES Demo, loads a single recording file for playback. 
    /// </summary>
   public  class LoadSingleRecordingCES: MonoBehaviour
    {
        public int RecordingNumber;
        void Awake()
        {
        
            BodySelectedInfo.Instance.UpdateNumberOfRecordings();
            BodySelectedInfo.Instance.UpdateSelectedRecording(RecordingNumber);
        }

    }
}
