using UnityEngine;
using System;  
using System.Collections.Generic;
public class Sensor //: MonoBehaviour
{
    //Sensor type
    public BodyStructureMap.SensorTypes SensorType;
    //Sensor position
    public BodyStructureMap.SensorPositions SensorPosition;
    //Sensor Unique GUID for ease of cloud access
    public string SensorGuid;
    //Sensor ID on the body
    public int SensorBodyId;
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

    /// <summary>
    /// Deep copy constructor
    /// </summary>
    public Sensor(Sensor sensor)
    {
        this.SensorType = sensor.SensorType;
        this.SensorPosition = sensor.SensorPosition;
        this.SensorGuid = string.Copy(sensor.SensorGuid);
        this.SensorBodyId = sensor.SensorBodyId;
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