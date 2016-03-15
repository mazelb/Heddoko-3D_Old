/** 
* @file RecordingDBCRUDTest .cs
* @brief Contains the RecordingDBCRUDTest class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using Assets.Scripts.Communication.DatabaseConnectionPipe;
using Assets.Scripts.UI.Settings;
using UnityEngine;

namespace Assets.Scripts.Tests.database
{
    /// <summary>
    /// this class demonstrates how to use the database in a recordings context
    /// </summary>
    public class RecordingDBCRUDTest : MonoBehaviour
    {
        public string CurrentRecordingPath;
        public string RecordingGuid;
        public Database Database;

        void Start()
        {
            Debug.Log("create connection to db");
            Database = new Database(DatabaseConnectionType.Local);
            Database.Init();
            RecordingAddTest();
        }

        void RecordingGetTest()
        {
            Database.Connection.GetRawRecording(RecordingGuid);
        }

        void RecordingAddTest()
        { 
            Debug.Log("scanning recordings from " + ApplicationSettings.PreferedRecordingsFolder);
            int value = BodyRecordingsMgr.Instance.ScanRecordings(ApplicationSettings.PreferedRecordingsFolder);
            Debug.Log("found " + value + " recordings");
            int vRand = Random.Range(0, value);
         
            CurrentRecordingPath = BodyRecordingsMgr.Instance.FilePaths[vRand];
            Debug.Log(" selected recording " + CurrentRecordingPath + ". Scanning... ");
          
            BodyRecordingsMgr.Instance.ReadRecordingFile(CurrentRecordingPath, RecordingAddCallback);
        }
        void RecordingAddCallback(BodyFramesRecording vBfRec)
        {
            Debug.Log("found " + vBfRec.BodyRecordingGuid + "... now adding to DB");
            Database.Connection.CreateRecording(vBfRec);
            
        }

        void OnApplicationQuit()
        {
            Database.CleanUp();

        }
    }
}
