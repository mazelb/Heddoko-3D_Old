﻿/** 
* @file BodyRecordingReader.cs
* @brief Contains the BodyFramesReader class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

/**
* BodyRecordingReader class 
* @brief Class reads body frames from Recorded CSV file
* CSV line structure (single frame):
* RECORDING GUID
* CONTAINING BODY GUID
* SUIT GUID
* BOIMECH_sensorID_1, Yaw;Pitch;Roll, ... BOIMECH_sensorID_9, Yaw;Pitch;Roll, FLEXCORE_sensorID_1, SensorValue, ... ,FLEXCORE_sensorID_4, SensorValue
*/
public class BodyRecordingReader : MonoBehaviour
{
    //The file path to read
    private string mFilePath;
    //The entire file content
    private string mFileContents;
    //Line by line content
    private string[] mFileLines;

    /**
    * ReadFile()
    * @param vFilePath: The file path to read
    * @brief Reads full content of file to memory
    * if the file contents are not empty automatically 
    * populates line data
    */
    public int ReadFile(string vFilePath)
    {
        mFilePath = vFilePath;
        
        //open file from the disk (file path is the path to the file to be opened)
        using (StreamReader vStreamReader = new StreamReader(File.OpenRead(mFilePath)))
        {
            mFileContents = vStreamReader.ReadToEnd();

            if (mFileContents.Length > 0)
            {
                PopulateRecordingLines(mFileContents);
            }
        }

        return mFileContents.Length;
    }

    /**
    * PopulateRecordingLines()
    * @brief splits the string into frame lines
    */
    public void PopulateRecordingLines()
    {
        if(mFileContents.Length > 0)
        {
            PopulateRecordingLines(mFileContents);
        }
    }

    /**
    * PopulateRecordingLines()
    * @param vFileContent: The file content
    * @brief splits the string into frame lines
    */
    public void PopulateRecordingLines(string vFileContent)
    {
        if (vFileContent.Length > 0)
        {
            //Split the data into single lines. Each line is a Frame
            mFileLines = vFileContent.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }
    }

    /**
    * GetRecordingLines()
    * @brief Returns all lines read from CSV
    */
    public string[] GetRecordingLines()
    {
        return mFileLines;
    }
}
