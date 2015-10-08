using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SensorsData : MonoBehaviour
{
    //Size of Data contained in the list
    public int DataSize { get { return Data.Count; } }

    //List of sensor raw data
    public List<Int16> Data = new List<Int16>();
}
