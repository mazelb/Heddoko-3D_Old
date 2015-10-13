using UnityEngine;
using System;
using System.Collections;

public class Sensor : MonoBehaviour
{
    //Sensor type
    public BodyStructureMap.SensorTypes SensorType;
    //Sensor position
    public BodyStructureMap.SensorPositions SensorPosition;
    //Sensor Unique GUID for ease of cloud access
    public string SensorGuid;
    //Sensor ID on the body
    public int SensorBodyId;
    //Data contained in the sensor
    public SensorsData SensorData;

    public Sensor()
    {

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