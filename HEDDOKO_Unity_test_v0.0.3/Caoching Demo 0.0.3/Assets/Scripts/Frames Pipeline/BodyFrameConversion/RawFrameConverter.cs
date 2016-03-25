﻿using System; 
using HeddokoLib.utils;
using UnityEngine;

namespace Assets.Scripts.Frames_Pipeline.BodyFrameConversion
{
    /// <summary>
    /// provides a set of functions to help with converting RawFrames into BodyFrames
    /// </summary>
    public static class RawFrameConverter
    {
        /// <summary>
        /// holds a reference to previously valid values
        /// </summary>
        public static Vector3[] PreviouslyValidOrientations = new Vector3[9];

        private static float sStartTime = 0;

        /// <summary>
        /// listens to events where the start time needs to be reset 
        /// </summary>
        public static void ResetStartTimeListener(BodyFrame vBodyFrame)
        {
            if (vBodyFrame != null)
            {
                sStartTime = vBodyFrame.Timestamp / 1000f;
            }
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
            if (rawData.IsDecoded)
            {
                return ConvertDecodedRawFrame(rawData);
            }
            else
            {
                return ConvertEncondedRawFrame(rawData);
            }

        }


        /// <summary>
        /// Converts encoded raw frame data to a bodyframe
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public  static BodyFrame ConvertEncondedRawFrame(BodyRawFrame rawData)
        {
            float vTimeStamp = (float)(Convert.ToInt32(rawData[0]));
            vTimeStamp =  (vTimeStamp / 1000f) - sStartTime ;
            Int16 vBitmask = Convert.ToInt16(rawData[1], 16);

            int vStartIndex = 2;
            int vEndIndex = 11;
            int vBitmaskCheck = 0;

            //is used to set vPreviouslyValid values indicies 
            int vSetterIndex = 0;
            for (int i = vStartIndex; i < vEndIndex; i++, vBitmaskCheck++, vSetterIndex++)
            {
                //get the bitmask and check if the sensors values are valid(not disconnected)
                //data is valid 
                if ((vBitmask & (1 << vBitmaskCheck)) == (1 << vBitmaskCheck))
                {
                    string[] v3data = rawData[i].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    float vRoll = ConversionTools.ConvertHexStringToFloat((v3data[0]));
                    float vPitch = ConversionTools.ConvertHexStringToFloat((v3data[1]));
                    float vYaw = ConversionTools.ConvertHexStringToFloat((v3data[2]));

                    PreviouslyValidOrientations[vSetterIndex] = new Vector3(vPitch, vRoll, vYaw);
                }
            }
            BodyFrame vBodyFrame = CreateBodyFrame(PreviouslyValidOrientations);
            vBodyFrame.Timestamp = vTimeStamp;
            return vBodyFrame;
        }

        private static BodyFrame ConvertDecodedRawFrame(BodyRawFrame rawData)
        {
            float vTimestamp = 0;
            float.TryParse(rawData.RawFrameData[0], out vTimestamp);

            //from startIndex to endIndex, we check the subframes and extrapolate the IMU data. 
            int vStartIndex = 1;
            int vEndIndex = 20;

            //The check index is made such that when we iterate through the list, it is possible that the 19th index isn't of an IMU type, then it must be that it is a stretch sensor
            //at this index we check if we actually hold data for the lower spine. If we do, then we continue, otherwise, we clear and the stretch data is gathered. 
            int vCheckIndex = 19;
            bool vFinishLoop = false;
            BodyFrame vBodyFrame = new BodyFrame();
            vBodyFrame.Timestamp = vTimestamp;

            //placeholder data to be used in the dictionary until it gets populated by the following loop
            Vector3 vPlaceholderV3 = Vector3.zero;

            int vKey = 0;
            //initializing sensor positions to some default value
            BodyStructureMap.SensorPositions vSensorPosAsKey = BodyStructureMap.SensorPositions.SP_RightElbow;
            for (int i = vStartIndex; i < vEndIndex; i++)
            {
                //first check if the current index falls on a position that can be interpreted as an int
                if (i % 2 == 1)
                {
                    if (i == vCheckIndex)
                    {
                        try
                        {
                            int.TryParse(rawData.RawFrameData[i], out vKey);
                        }
                        finally
                        {
                            if (vKey != 10)
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
                    int.TryParse(rawData.RawFrameData[i], out vKey);
                    vKey--;
                    vSensorPosAsKey = ImuSensorFromPos(vKey);

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
            if (!vBodyFrame.FrameData.ContainsKey(BodyStructureMap.SensorPositions.SP_LowerSpine))
            {
                vBodyFrame.FrameData.Add(BodyStructureMap.SensorPositions.SP_LowerSpine, Vector3.zero);
            }

            //todo stretch sense data extrapolation starting from the updated startingIndex

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
        /// Converts a string received by the Brainpack server service into a bodyframe
        /// </summary>
        /// <param name="vHexPacket"></param>
        /// <returns></returns>
        public static BodyFrame ConvertFromHexaString(string vHexPacket)
        {
            string[] VRawData = vHexPacket.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

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
                if (i % 2 == 1)
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
                    string[] v3data = VRawData[i].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
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
    }

}
