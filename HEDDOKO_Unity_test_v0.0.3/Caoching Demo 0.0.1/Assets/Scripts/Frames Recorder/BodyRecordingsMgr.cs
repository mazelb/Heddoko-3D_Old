/** 
* @file BodyRecordingsMgr.cs
* @brief Contains the BodyRecordingsMgr class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
*/

using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;

/**
* RecordingsManager class 
* @brief manager class for recordings (interface later)
*/
public class BodyRecordingsMgr : MonoBehaviour
{
    #region Singleton definition
    private static readonly BodyRecordingsMgr instance = new BodyRecordingsMgr();

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static BodyRecordingsMgr()
    {
    }

    private BodyRecordingsMgr()
    {
    }

    public static BodyRecordingsMgr Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    //Scanned files 
    private string[] mFilePaths;
    //Main recordings directory path 
    private string mDirectoryPath;
    //Recordings available
    public List<BodyFramesRecording> Recordings = new List<BodyFramesRecording>();
    //Map Body UUID to Recording UUID
    Dictionary<string, List<string>> RecordingsDictionary = new Dictionary<string, List<string>>();


    /**
    * ScanRecordings()
    * @param vDirectoryPath: The path in which the function will scan
    * @return int: the number of files found
    * @brief Scans a specific folder for recordings
    */
    public int  ScanRecordings(string vDirectoryPath)
    {
        mDirectoryPath = vDirectoryPath;
        mFilePaths = Directory.GetFiles(mDirectoryPath);
        return mFilePaths.Length;
    }

    /**
    * ReadAllRecordings()
    * @brief reads all recodings in the mDirectoryPath
    */
    public void ReadAllRecordings()
    {
        //For each recording found
        for (int i = 0; i < mFilePaths.Length; i++)
        {
            ReadRecordingFile(mFilePaths[i]);
        }
    }

    /**
    * ReadRecordingFile()
    * @param vFilePath: The recording file path
    * @brief Reads a specific recording file
    */
    public void ReadRecordingFile(string vFilePath)
    {
        //Read recording file
        //if the file doesn't end with CSV then return
        if(vFilePath.EndsWith("meta"))
        {
            return;
        }

        BodyRecordingReader vTempReader = new BodyRecordingReader();

        if (vTempReader.ReadFile(vFilePath) > 0)
        {
            AddNewRecording(vTempReader.GetRecordingLines());            
        }
    }

    /**
    * AddNewRecording()
    * @param vRecordingLines: The recording file content in lines
    * @brief Adds a recording to the list
    */
    public void AddNewRecording(string[] vRecordingLines)
    {
        BodyFramesRecording vTempRecording = new BodyFramesRecording();

        vTempRecording.ExtractRecordingUUIDs(vRecordingLines);

        //If recording already exists, do nothing
        if (!RecordingExist(vTempRecording.BodyRecordingGuid))
        {
            vTempRecording.ExtractRawFramesData(vRecordingLines);

            //Add body to the body manager
            BodiesManager.Instance.AddNewBody(vTempRecording.BodyGuid);

            //Map Body to Recording for future play
            MapRecordingToBody(vTempRecording.BodyGuid, vTempRecording.BodyRecordingGuid);

            //Add recording to the list 
            Recordings.Add(vTempRecording);
        }
    }

    /**
    * CreateNewRecording()
    * @brief Creates a new recording and adds it to the list
    */
    public void CreateNewRecording()
    {
        //TODO:
    }

    /**
    * MapRecordingToBody()
    * @param vBodyUUID: The containing Body UUID
    * @param vRecUUID: The recording UUID
    * @brief maps Recording UUID to Body UUID for future searches
    */
    public void MapRecordingToBody(string vBodyUUID, string vRecUUID)
    {
        if (BodiesManager.Instance.BodyExist(vBodyUUID))
        {
            List<string> vListOfRecordings;
            RecordingsDictionary.TryGetValue(vBodyUUID, out vListOfRecordings);

            //if there are no recordings, create a new mapping
            //if recordings are already mapped, add more to it
            if (vListOfRecordings == null)
            {
                vListOfRecordings = new List<string>();
                vListOfRecordings.Add(vRecUUID);
                RecordingsDictionary.Add(vBodyUUID, vListOfRecordings);
            }
            else
            {
                vListOfRecordings.Add(vRecUUID);
            }
        }
    }

    /**
    * RecordingExist()
    * @param vRecUUID: The recording UUID
    * @return bool: True if the recording exists
    * @brief searches if the recording exists in the manager
    */
    public bool RecordingExist(string vRecUUID)
    {
        return Recordings.Exists(x => x.BodyRecordingGuid == vRecUUID);
    }

    /**
    * GetRecordingsForBody()
    * @param vBodyUUID: The containing body UUID
    * @return List<BodyFramesRecording>: the list of recordings assigned to the body
    * @brief returns all the recordings assigned to a body if they exist
    */
    public List<BodyFramesRecording> GetRecordingsForBody(string vBodyUUID)
    {
        //look for the recording only if the body exists
        if (BodiesManager.Instance.BodyExist(vBodyUUID))
        {
            //get the recordings from the list of recording IDs assigned to that body
            List<string> vListOfRecordingIds;
            List<BodyFramesRecording> vListOfRecordings = new List<BodyFramesRecording>();
            if (RecordingsDictionary.TryGetValue(vBodyUUID, out vListOfRecordingIds))
            {
                for(int i=0; i < vListOfRecordingIds.Count; i++)
                {
                    BodyFramesRecording vRecording = GetRecordingByUUID(vListOfRecordingIds[i]);
                    if (vRecording != null)
                    {
                        vListOfRecordings.Add(vRecording);
                    }
                }

                if(vListOfRecordings.Count > 0)
                {
                    return vListOfRecordings;
                }
            }
        }

        return null;
    }

    /**
    * GetRecordingByUUID()
    * @param vRecUUID: The recording UUID
    * @return BodyFramesRecording: The recording
    * @brief looks for a recording by its UUID
    */
    public BodyFramesRecording GetRecordingByUUID(string vRecUUID)
    {
        if (RecordingExist(vRecUUID))
        {
            return Recordings.Find(x => x.BodyRecordingGuid == vRecUUID);
        }

        return null;
    }


 
}
