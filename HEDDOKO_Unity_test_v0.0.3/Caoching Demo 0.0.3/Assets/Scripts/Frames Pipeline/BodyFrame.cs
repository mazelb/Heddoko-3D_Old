
/** 
* @file BodyFrame.cs
* @brief Contains the BodyFrame class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using HeddokoLib.utils;

/// <summary>
/// The frame of data that is populated to sensors, and contains the list of sensors to access sensors data
/// </summary>
[Serializable]
public class BodyFrame
{
    [SerializeField]
    //The frame of data populated to sensors 
    private Dictionary<BodyStructureMap.SensorPositions, Vector3> mFrameData;

    //The timestamp of a bodybody frame 
    private float mTimeStamp;

    internal Dictionary<BodyStructureMap.SensorPositions, Vector3> FrameData
    {
        get
        {
            if (mFrameData == null)
            {
                mFrameData = new Dictionary<BodyStructureMap.SensorPositions, Vector3>(18);
            }
            return mFrameData;
        }
        set
        {
            mFrameData = value;
        }
    }

    internal float Timestamp
    {
        get { return mTimeStamp; }
        set { mTimeStamp = value; }
    }

    /**
    * ToString()
    * @brief Prepares the current body frame as a string 
    * @return current body frame as a string
    * 
    */
    public override string ToString()
    {
        string vOutput = "";
        foreach (KeyValuePair<BodyStructureMap.SensorPositions, Vector3> vPair in FrameData)
        {
            vOutput += "" + vPair.Key + " " + vPair.Value + "\n";
        }
        return vOutput;
    }

    public string ToCSVString()
    {
        string vOutput = "" + (long)Timestamp + ",";
        foreach (KeyValuePair<BodyStructureMap.SensorPositions, Vector3> vPair in FrameData)
        {
            vOutput += (int)vPair.Key + "," + vPair.Value.x + ";" + vPair.Value.y + ";" + vPair.Value.z + ",";
        }
        return vOutput;
    }
    public BodyFrame()
    {

    }




   

    /// <summary>
    /// Returns a sensor position of a stretch sensor from the given int pos
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    internal BodyStructureMap.SensorPositions StretchSensorFromPos(int pos)
    {
        if (pos == 0)
        {
            return BodyStructureMap.SensorPositions.SP_RightElbow;
        }
        if (pos == 1)
        {
            return BodyStructureMap.SensorPositions.SP_LeftElbow;
        }
        if (pos == 2)
        {
            return BodyStructureMap.SensorPositions.SP_RightKnee;
        }
        else
        {
            return BodyStructureMap.SensorPositions.SP_LeftKnee;
        }
    }
}
