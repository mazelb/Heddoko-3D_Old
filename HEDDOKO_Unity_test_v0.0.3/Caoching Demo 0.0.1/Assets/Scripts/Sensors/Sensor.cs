using UnityEngine;
using System;
using System.Collections;

public class Sensor : MonoBehaviour
{
    //Available sensors types
    public enum SensorTypes
    {
        ST_Default   = 0,   //No data in SensorData
        ST_Biomech   = 1,   //Data: Yaw, Pitch, Roll
        ST_Flexcore  = 2,   //Data: Flex Value
        ST_HeartRate = 3,
        ST_SensorTypeCount
    }

    //Sensor type
    public SensorTypes SensorType;
    //Sensor Unique GUID for ease of cloud access
    public string SensorGuid;
    //Sensor ID on the body
    public int SensorBodyId;
    //Data contained in the sensor
    public SensorsData SensorData;

    public Sensor()
    {
        SensorType = SensorTypes.ST_Default;
    }

    public void CreateNewUUID()
    {
        SensorGuid = Guid.NewGuid().ToString();
    }
}

public class SensorTuple : MonoBehaviour
{
    public Sensor InitSensor;
    public Sensor CurrentSensor;
}