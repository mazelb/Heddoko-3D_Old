/** 
* @file RecordingDBCRUDTest .cs
* @brief Contains the RecordingDBCRUDTest class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

 
using System.Collections.Generic;
 
using Assets.Scripts.Communication.DatabaseConnectionPipe;
using Assets.Scripts.UI;
using Assets.Scripts.UI.Settings;
using Assets.Scripts.UI.Tagging;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

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
            CreateTagsForRecordings();
        }

        BodyFramesRecording RecordingGetTest()
        {
            BodyFramesRecording vRec = Database.Connection.GetRawRecording(RecordingGuid);
            if (vRec != null)
            {
                Debug.Log("<color=green>Success! </color> Found recording GUID " + vRec.BodyRecordingGuid);
                Debug.Log("<color=green>Success! </color> Recording length " + vRec.RecordingRawFrames.Count);

            }
            return vRec;
        }

        void RecordingAddTest()
        {
            Debug.Log("scanning recordings from " + ApplicationSettings.PreferedRecordingsFolder);
            int value = BodyRecordingsMgr.Instance.ScanRecordings(ApplicationSettings.PreferedRecordingsFolder);
            Debug.Log("found " + value + " recordings");

            bool[] vAddedRec = new bool[value];
            for (int j = 0; j < vAddedRec.Length; j++)
            {
                vAddedRec[j] = false;

            }
            int i = 0;
            int max = 15;
            while (i < max)
            {
                int vRand = Random.Range(0, value);

                if (!vAddedRec[vRand])
                {
                    CurrentRecordingPath = BodyRecordingsMgr.Instance.FilePaths[i];
                    vAddedRec[vRand] = true;
                    i++;
                    BodyRecordingsMgr.Instance.ReadRecordingFile(CurrentRecordingPath, RecordingAddCallback);
                }
            }



        }
        void RecordingAddCallback(BodyFramesRecording vBfRec)
        {
            Debug.Log("found " + vBfRec.BodyRecordingGuid + "... now adding to DB");
            Database.Connection.CreateRecording(vBfRec);

        }

        void CreateTagsForRecordings()
        {
            string vTest = "Testag";
            string vPath = "Assets/Resources/english-words.dict";
            string vFileContent = ""; 
            BodyFramesRecording vGet = RecordingGetTest();
          /*  
            using (StreamReader vStreamReader = new StreamReader(File.OpenRead(vPath)))
            {
                vFileContent = vStreamReader.ReadToEnd();
                vDictionaryContent = vFileContent.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);
            }
*/
            List<Tag> vTags = new List<Tag>();
            vTags = TaggingManager.Instance.LoadAllTags();
            Debug.Log("<color=blue><b>Total loaded tags:</b></color> " + vTags.Count);

            for (int i = Random.Range(0, vTags.Count/2); i < vTags.Count; i++)
            {
                TaggingManager.Instance.AttachTagToRecording(vGet, vTags[i]);
            }
        }

        void OnApplicationQuit()
        {
            if (Database != null)
            {
                Database.CleanUp();
            }

        }
    }
}
