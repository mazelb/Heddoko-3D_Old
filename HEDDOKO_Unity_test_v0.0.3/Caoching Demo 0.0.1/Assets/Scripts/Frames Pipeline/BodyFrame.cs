using UnityEngine; 
using System.Collections.Generic;
using System; 

/** 
* @file BodyFrame.cs
* @brief Contains the BodyFrame class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date October 2015
*/

/// <summary>
/// The frame of data that is populated to sensors, and contains the list of sensors to access sensors data
/// </summary>
public class BodyFrame 
{
    //The frame of data populated to sensors 
    private Dictionary<BodyStructureMap.SensorPositions, Vector3> mFrameData; 

    private List<Sensor> mListOfSensors;
    public Dictionary<BodyStructureMap.SensorPositions, Vector3> MapSensorPosToValue
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



    #region static helper functions
    /**
    * ConvertRawFrame(BodyRawFrame vRawData)
    * @brief Pass in a BodyRawFrame and convert it to a body frame
    * @param BodyRawFrame vRawData:  Raw data that will be converted to a body frame
    * @return a converted BodyFrame
    */
    public static BodyFrame ConvertRawFrame(BodyRawFrame vRawData)
    {
        //from startIndex to endIndex, we check the subframes and extrapolate the IMU data. 
        int vStartIndex = 1;
        int vEndIndex = 20;
        //The check index is made such that when we iterate through the list, it is possible that the 19th index isn't of an IMU type, then it must be that it is a stretch sensor
        int vCheckIndex = 19; //at this index we check if we actually hold data for the lower spine. If we do, then we continue, otherwise, we clear and the stretch data is gathered. 
        bool vFinishLoop = false;
        BodyFrame vBodyFrame = new BodyFrame();
        Vector3 vPlaceholderCoords = Vector3.zero; //placeholder data to be used in the dictionary until it gets populated by the following loop

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
                        int.TryParse(vRawData.RawFrameData[i], out key);
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
                int.TryParse(vRawData.RawFrameData[i], out key);
                key--;
                vSensorPosAsKey = ImuSensorFromPos(key);
                vBodyFrame.MapSensorPosToValue.Add(vSensorPosAsKey, vPlaceholderCoords);  
            }
            else
            {
                //split the string into three floats
                string[] v3data = vRawData.RawFrameData[i].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                float[] value = new float[3];
                for (int j = 0; j < 3; j++)
                {
                    float.TryParse(v3data[j], out value[j]);
                
                }
                vBodyFrame.MapSensorPosToValue[vSensorPosAsKey] = new Vector3(value[0], value[1], value[2]);
            } 

        }

        //todo stretch sense data extrapolation starting from the updated startingIndex
 
        return vBodyFrame;

    }
    /**
    * ImuSensorFromPos(int pos)
    * @brief Retrieve the imu's position from the given parameter
    * @param int pos: the position in int of an IMU
    * @return BodyStructureMap.SensorPositions the imu's position with respect to the pased in parameter
    * 
    */
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
    /**
    * StretchSensorFromPos(int pos)
    * @brief Retrieve the stretch sensor's position from the given parameter
    * @param int pos: the position in int of a stretch sensor
    * @return BodyStructureMap.SensorPositions the stretch sensor's position with respect to the pased in parameter
    * 
    */
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
    
#endregion
}
