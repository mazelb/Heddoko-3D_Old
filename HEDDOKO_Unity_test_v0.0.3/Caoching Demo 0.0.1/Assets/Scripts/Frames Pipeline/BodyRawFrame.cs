/** 
* @file BodyRawFrame.cs
* @brief Contains the BodyRawFrame class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using UnityEngine;
using System.Collections;
using System;

/**
* BodyRawFrame class 
* @brief Class containing raw frame data
* Frame structure (single frame):
* BOIMECH_sensorID_1, Yaw;Pitch;Roll, ... BOIMECH_sensorID_9, Yaw;Pitch;Roll, FLEXCORE_sensorID_1, SensorValue, ... ,FLEXCORE_sensorID_4, SensorValue
*/
public class BodyRawFrame : MonoBehaviour
{
    //Maximum frame size in bytes
    public static uint sRawFrameSize = 100;

    //containing Recording GUID
    public string BodyRecordingGuid;
    
    //Containing Body GUID 
    public string BodyGuid;

    //Containing Suit GUID 
    public string SuitGuid;

    //Frame raw content 
    public string[] RawFrameData;
}
