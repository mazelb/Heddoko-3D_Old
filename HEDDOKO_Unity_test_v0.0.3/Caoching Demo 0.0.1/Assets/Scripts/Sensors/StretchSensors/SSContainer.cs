/**
 * @file SSContainer.cs
 * @brief This script acts as the parent to all joint scripts for a given Unity game object. The game object
 * must be a person animated exclusively with StretchSense sensors (or any stretchable sensor).
 * @note Add notes here.
 * @author Francis Amankrah (frank@heddoko.com)
 * @date June 2015
 */
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

public class SSContainer : MonoBehaviour 
{
    /**
    * Shortcut to apply CSV data sets to all sensors at the same time.
    */
    public string vCSVDataSet = "default";

    /**
     * Array of joints in container.
     */
    private SSJoint[] maJoints;

    /**
     * Data from StretchSense module.
     * This is accessible from other scripts.
     */
    public static int[] svaModuleData = new int[6];

    /**
     * @brief           Starts the joint scripts.
     * @return void
     */
    public void StartJoints() 
    {
        // Loop through joint scripts to initialize them.
        for (int i = 0; i < maJoints.Length; i++)
        {
            if (!maJoints[i].vIndependentUpdate)
            {
                // vCSVDataSet allows us to set specific CSV files (by folder) to all sensors.
                maJoints[i].StartJoint(vCSVDataSet);
            }
        }
    }

    /**
     * @brief           Updates the joint objects.
     * @return void
     */
    public void UpdateJoints() 
    {
        for (int i = 0; i < maJoints.Length; i++) 
        {
            if(!maJoints[i].vIndependentUpdate)
            {
                maJoints[i].UpdateJoint();
            }
        }
    }

    /**
     * @brief           Resets joint objects.
     * @return void
     */
    public void ResetJoints()
    {
        for (int i = 0; i < maJoints.Length; i++) 
        {
            if(!maJoints[i].vIndependentUpdate)
            {
                maJoints[i].ResetJoint();
            }
        }
    }
    
    /**
     * @brief           Called once when the application quits.
     * @return void
     */
    public void OnApplicationQuit()
    {
    }



    /////////////////////////////////////////////////////////////////////////////////////
    /// UNITY GENERATED FUNCTIONS 
    //////////////////////////////////////////////////////////////////////////////////////



    /**
     * @brief           This method is called by Unity when the program is launched.
     * @return void
     */
    void Awake ()
    {
        Application.targetFrameRate = 300;
        maJoints = GetComponentsInChildren<SSJoint>();
    }
  
    /**
     * @brief           This method is called by Unity when the program is started.
     * @return void
     */
    void Start() 
    {
        ResetJoints();
        StartJoints();
    }
  
    /**
     * @brief           This method is called by Unity once per frame.
     * @return void
     */
    void Update() 
    {
        UpdateJoints();
    }
  
    /**
     * @brief           Sets up the GUI buttons.
     * @return void
     */
    void OnGUI()
    {
        if (GUI.Button (new Rect (20, 20, 150, 30), "Start Sensors"))
        {
            ResetJoints();
            StartJoints();
        }

        if (GUI.Button (new Rect (20, 52, 150, 30), "Reset Sensors "))
        {
            ResetJoints();        
        }
    }
}

