﻿ 
using System; 
using System.Collections.Generic;
using UnityEngine;

public class SensorsData  
{
    //Size of Data contained in the list
    public int DataSize { get { return Data.Count; } }

    //List of sensor raw data
    public List<Int16> Data = new List<Int16>();

    public Vector3 PositionalData;
}
