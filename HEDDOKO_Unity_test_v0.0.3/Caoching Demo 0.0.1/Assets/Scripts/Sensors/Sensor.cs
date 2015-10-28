using UnityEngine;
using System;  
using System.Collections.Generic;
public class Sensor //: MonoBehaviour
{
    //Sensor type
    public BodyStructureMap.SensorTypes mSensorType;
    //Sensor position
    public BodyStructureMap.SensorPositions mSensorPosition;
    //Sensor Unique GUID for ease of cloud access
    public string SensorGuid;
    //Sensor ID on the body
    public int mSensorBodyID;
    private SensorsData sensorData = new SensorsData();
    //Data contained in the sensor
    public SensorsData SensorData
    {
        get
        {
            return sensorData;
        }
        set { sensorData = value; }
    }

    public Sensor()
    {
        SensorGuid = "";
        SensorData = new SensorsData();
        
    }

    /**
    * Sensor(Sensor sensor)
    * @param Sensor sensor: a sensor who's fields need to be copied 
    * @brief Perform a deep copy of the passed in sensor parameter
    */
    public Sensor(Sensor sensor)
    {
        this.mSensorType = sensor.mSensorType;
        this.mSensorPosition = sensor.mSensorPosition;
        this.SensorGuid = string.Copy(sensor.SensorGuid);
        this.mSensorBodyID = sensor.mSensorBodyID;
       //deep copy
        this.SensorData.Data = new List<short>(sensor.SensorData.Data);
        this.SensorData.PositionalData = sensor.SensorData.PositionalData; 
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