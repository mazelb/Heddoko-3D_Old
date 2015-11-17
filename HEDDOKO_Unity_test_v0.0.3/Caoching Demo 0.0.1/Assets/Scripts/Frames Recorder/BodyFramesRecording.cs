/** 
* @file BodyFramesRecording.cs
* @brief Contains the BodyFramesRecording class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
*/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;

/**
* BodyFramesRecording class 
* @brief Class containing raw frames from Recorded CSV file
* CSV line structure (single frame):
* RECORDING GUID
* CONTAINING BODY GUID
* SUIT GUID
* BOIMECH_sensorID_1, Yaw;Pitch;Roll, ... BOIMECH_sensorID_9, Yaw;Pitch;Roll, FLEXCORE_sensorID_1, SensorValue, ... ,FLEXCORE_sensorID_4, SensorValue
*/
public class BodyFramesRecording : IFrameStream
{
    //Number of UUIDs (lines) before the beginning of the frames recorded in CSV
    public static uint sNumberOfUUIDs = 3;

    //Recording Unique GUID for ease of cloud access
    public String BodyRecordingGuid;
    //Unique GUID of the Body it belongs to
    public string BodyGuid;
    //Unique GUID of the suit it belongs to
    public string SuitGuid;
    //Recording content
    public List<BodyRawFrame> RecordingRawFrames = new List<BodyRawFrame>();
    //current raw frame index
    private int currentRawFrameIndex;

    /**
    * CreateNewRecordingUUID()
    * @brief Creates a new recording UUID
    */
    public void CreateNewRecordingUUID()
    {
        BodyRecordingGuid = Guid.NewGuid().ToString();
    }

    /**
    * AddRecordingRawFrame()
    * @param vFrame: The frame to add to the recording
    * @brief Adds a frame to the recording
    */
    public void AddRecordingRawFrame(BodyRawFrame vFrame)
    {
        RecordingRawFrames.Add(vFrame);
    }

    public void SaveRecording()
    {
        //TODO:
    }

    /**
    * GetNextFrame()
    * @param 
    * @brief Adds a frame to the recording
    * @return Returns the next frame from the Raw Frame Recordings
    */
    public BodyRawFrame GetNextFrame()
    {
        BodyRawFrame returnedFrame = RecordingRawFrames[currentRawFrameIndex];
        currentRawFrameIndex++;
        if (currentRawFrameIndex >= RecordingRawFrames.Count)
        {
            currentRawFrameIndex = RecordingRawFrames.Count - 1;
        }
        return returnedFrame;
    }
    /**
     * PopulateRecordingUUIDs()
     * @param vRecordingLines: The recording content
     * @brief Gets the UUID of the recording and Body from the content
     */
    public void ExtractRecordingUUIDs(string[] vRecordingLines)
    {
        if (vRecordingLines.Length > sNumberOfUUIDs) //The minimum amount of lines in the recording
        {
            //Get the GUIDs of the recording and containing body
            BodyRecordingGuid = vRecordingLines[0];
            BodyGuid = vRecordingLines[1];
            SuitGuid = vRecordingLines[2];
        }
    }

    /**
    * PopulateRawFramesData()
    * @param vRecordingLines: The recording full content
    * @brief Creates raw frames from CSV data
    */
    public void ExtractRawFramesData(string[] vRecordingLines)
    {
        if (vRecordingLines.Length > sNumberOfUUIDs) //The minimum amount of lines in the recording
        {
            //Get the data line by line and add them as frames
            //for (uint i = (sNumberOfUUIDs-1); i < vRecordingLines.Length; i++)
            for (uint i = (sNumberOfUUIDs); i < vRecordingLines.Length; i++)
            {
                BodyRawFrame vTempRaw = new BodyRawFrame();
                vTempRaw.BodyRecordingGuid = BodyRecordingGuid;
                vTempRaw.BodyGuid = BodyGuid;
                vTempRaw.SuitGuid = SuitGuid;
                vTempRaw.RawFrameData = vRecordingLines[i].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                RecordingRawFrames.Add(vTempRaw);
            }
        }
    }



    //    //Send each line to a BodyFrameStream
    //    //Each line start by sending Frame start "S"
    //    //Send the frame data 
    //    //Send Frame End: "E"

    //private void TransmitAllFrames()
    //{
    //    foreach (string vFileLine in mFileLines)
    //    {
    //        //string[] vFrameData = vFileLine.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    //    }

    //    //Send each line to a BodyFrameStream
    //    //Each line start by sending Frame start "S"
    //    //Send the frame data 
    //    //Send Frame End: "E"
    //}
    

}
