﻿
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
using System.Runtime.Serialization;
using HeddokoLib.utils;
using Newtonsoft.Json;

/// <summary>
/// The frame of data that is populated to sensors, and contains the list of sensors to access sensors data
/// </summary>
[DataContract]
public class BodyFrame 
{

    //The frame of data populated to sensors 
    [JsonProperty]
    private Dictionary<BodyStructureMap.SensorPositions, Vector3> mFrameData;
     
    //The timestamp of a bodybody frame 
 [JsonProperty]
    private float mTimeStamp;

    [JsonIgnore]
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
    [JsonIgnore]
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
        foreach(KeyValuePair<BodyStructureMap.SensorPositions, Vector3> vPair in FrameData)
        {
            vOutput += "" + vPair.Key + " " + vPair.Value + "\n";
        }
        return vOutput;
    }

    public string ToCSVString()
    {
        string vOutput =""+(long)Timestamp+",";
        foreach (KeyValuePair<BodyStructureMap.SensorPositions, Vector3> vPair in FrameData)
        {
            vOutput += (int) vPair.Key + "," + vPair.Value.x + ";"+vPair.Value.y + ";"+vPair.Value.z + ",";
        }
        return vOutput;
    }
    public BodyFrame()
    {
        
    }
 
    /**
    * ConvertRawFrame(BodyRawFrame rawData)
    * @brief Pass in a BodyRawFrame and convert it to a body frame
    * @param BodyRawFrame rawData
    * @return void
    * 
    */
    public static BodyFrame ConvertRawFrame(BodyRawFrame rawData)
    {
        float vTimestamp = 0;
        float.TryParse(rawData.RawFrameData[0], out vTimestamp);
        
        //from startIndex to endIndex, we check the subframes and extrapolate the IMU data. 
        int vStartIndex= 1;
        int vEndIndex = 20;

        //The check index is made such that when we iterate through the list, it is possible that the 19th index isn't of an IMU type, then it must be that it is a stretch sensor
        //at this index we check if we actually hold data for the lower spine. If we do, then we continue, otherwise, we clear and the stretch data is gathered. 
        int vCheckIndex = 19; 
        bool vFinishLoop = false;
        BodyFrame vBodyFrame = new BodyFrame();
        vBodyFrame.Timestamp = vTimestamp;
        
        //placeholder data to be used in the dictionary until it gets populated by the following loop
        Vector3 vPlaceholderV3 = Vector3.zero; 
      
        int key = 0;
        BodyStructureMap.SensorPositions vSensorPosAsKey = BodyStructureMap.SensorPositions.SP_RightElbow; //initializing sensor positions to some default value
        for (int i = vStartIndex; i < vEndIndex; i++)
        {
            //first check if the current index falls on a position that can be interpreted as an int
            if (i%2 == 1)
            {
                if (i == vCheckIndex)
                {
                    try
                    {
                        int.TryParse(rawData.RawFrameData[i], out key);
                    }
                    finally
                    {
                        if (key != 10)
                        {
                            vFinishLoop = true;
                        } 
                    }
                    if (vFinishLoop)
                    {
                        //set the start index for the next iteration
                        vStartIndex = i;
                        break;
                    } 
                }
                int.TryParse(rawData.RawFrameData[i], out key);
                key--;
                vSensorPosAsKey = ImuSensorFromPos(key);
                vBodyFrame.FrameData.Add(vSensorPosAsKey, vPlaceholderV3);
            }
            else
            {
                //split the string into three floats
                string[] v3data = rawData.RawFrameData[i].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                float[] value = new float[3];
                for (int j = 0; j < 3; j++)
                {
                    float.TryParse(v3data[j], out value[j]);
                }
                vBodyFrame.FrameData[vSensorPosAsKey] = new Vector3(value[0], value[1], value[2]);
            } 

        }

        //check if lower spine exists
        if(!vBodyFrame.FrameData.ContainsKey(BodyStructureMap.SensorPositions.SP_LowerSpine))
        {
            vBodyFrame.FrameData.Add(BodyStructureMap.SensorPositions.SP_LowerSpine, Vector3.zero);
        }
        
        //todo stretch sense data extrapolation starting from the updated startingIndex

        return vBodyFrame;

    }
 
    /// <summary>
    /// Converts a string received by the Brainpack server service into a bodyframe
    /// </summary>
    /// <param name="vHexPacket"></param>
    /// <returns></returns>
    public static BodyFrame ConvertFromHexaString(string vHexPacket)
    {
        string[] VRawData = vHexPacket.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
    
        //from startIndex to endIndex, we check the subframes and extrapolate the IMU data. 
        int vStartIndex = 1;
        int vEndIndex = 20;
        //The check index is made such that when we iterate through the list, it is possible that the 19th index isn't of an IMU type, then it must be that it is a stretch sensor
        //at this index we check if we actually hold data for the lower spine. If we do, then we continue, otherwise, we clear and the stretch data is gathered. 
        int vCheckIndex = 19; 
        bool vFinishLoop = false;
        BodyFrame vBodyFrame = new BodyFrame();

        //placeholder data to be used in the dictionary until it gets populated by the following loop
        Vector3 vPlaceholderV3 = Vector3.zero; 
         
        int key = 0;

        //initializing sensor positions to some default value
        BodyStructureMap.SensorPositions vSensorPosAsKey = BodyStructureMap.SensorPositions.SP_RightElbow; 

        for (int i = vStartIndex; i < vEndIndex; i++)
        {
            //first check if the current index falls on a position that can be interpreted as an int
            if (i%2 == 1)
            {
                if (i == vCheckIndex)
                {
                    try
                    {
                        int.TryParse(VRawData[i], out key);
                    }
                    finally
                    {
                        if (key != 10)
                        {
                            vFinishLoop = true;
                        }
                    }
                    if (vFinishLoop)
                    {
                        //set the start index for the next iteration
                        //todo:Start index in the hexa string method is to be used when grabbing stretch sense data
                        vStartIndex = i; 
                        break;
                    }
                }
                int.TryParse(VRawData[i], out key);
                key--;
                vSensorPosAsKey = ImuSensorFromPos(key);
                vBodyFrame.FrameData.Add(vSensorPosAsKey, vPlaceholderV3); 
            }
            else
            {
                //split the string into three floats
                string[] v3data = VRawData[i].Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                float[] value = new float[3];
                for (int j = 0; j < 3; j++)
                {
                    value[j] = ConversionTools.ConvertHexStringToFloat((v3data[j])); 
                }
                vBodyFrame.FrameData[vSensorPosAsKey] = new Vector3(value[0], value[1], value[2]);
            }
        } 
        return vBodyFrame;
    }

    /// <summary>
    /// Creates a bodyframe from an array of vectors
    /// </summary>
    /// <param name="vBodyFrameData"></param>
    /// <returns></returns>
    public static BodyFrame CreateBodyFrame(Vector3[] vBodyFrameData)
    {
        BodyFrame vBodyFrame = new BodyFrame();
        vBodyFrame.FrameData.Add(BodyStructureMap.SensorPositions.SP_LowerSpine, Vector3.zero);
        for (int i = 0; i < vBodyFrameData.Length; i++)
        {
            BodyStructureMap.SensorPositions vSenPos = ImuSensorFromPos(i);
            vBodyFrame.FrameData.Add(vSenPos, vBodyFrameData[i]);
        }
        return vBodyFrame;
    }

    /// <summary>
    /// Returns a sensor position fromt eh given position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    internal static BodyStructureMap.SensorPositions ImuSensorFromPos(int pos)
    {
        if (pos == 0)
        {
            return BodyStructureMap.SensorPositions.SP_UpperSpine;
        }
        if (pos == 1)
        {
            return BodyStructureMap.SensorPositions.SP_RightUpperArm;
        }
        if (pos == 2)
        {
            return BodyStructureMap.SensorPositions.SP_RightForeArm;
        }
        if (pos == 3)
        {
            return BodyStructureMap.SensorPositions.SP_LeftUpperArm;
        }
        if (pos == 4)
        {
            return BodyStructureMap.SensorPositions.SP_LeftForeArm;
        }
        if (pos == 5)
        {
            return BodyStructureMap.SensorPositions.SP_RightThigh;
        }
        if (pos == 6)
        {
            return BodyStructureMap.SensorPositions.SP_RightCalf;
        }
        if (pos == 7)
        {
            return BodyStructureMap.SensorPositions.SP_LeftThigh;
        }
        if (pos == 8)
        {
            return BodyStructureMap.SensorPositions.SP_LeftCalf;
        }
        else
        {
            return BodyStructureMap.SensorPositions.SP_LowerSpine;
        }
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
