using UnityEngine;
using System;
using System.Threading;

public struct NodQuaternionOrientation
{
    public float x;
    public float y;
    public float z;
    public float w;

    public NodQuaternionOrientation(float _x, float _y, float _z, float _w)
    {
        x = _x;
        y = _y;
        z = _z;
        w = _w;
    }
};

public class NodContainer : MonoBehaviour 
{
	//each joint can be composed of one or multiple sensors simultaneously
	private static NodJoint[] mNodJoints;

	// This variable is used to specify the angle information of which part of body be shown on the screen
	public static float vKey;

    /**
     * Data from StretchSense module.
     * This is accessible from other scripts.
     */
    public static int[] svaModuleData = new int[6];

    /// <summary>
    /// Call this function to start reading data from the sensors for the joint values.
    /// </summary>
    public void StartJoints () 
	{
		for (int ndx = 0; ndx < mNodJoints.Length; ndx++) 
		{
			if(!mNodJoints[ndx].independantUpdate)
			{
				mNodJoints[ndx].StartJoint();
			}
		}
	}
	
	/// <summary>
	/// Call this function to update current Joint values.
	/// </summary>
	public void UpdateJoints () 
	{
		for (int ndx = 0; ndx < mNodJoints.Length; ndx++) 
		{
			if(!mNodJoints[ndx].independantUpdate)
			{
				mNodJoints[ndx].UpdateJoint();
			}
		}
	}
	
	/// <summary>
	/// Reset the stretch joint sensors.
	/// </summary>
	public void ResetJoints ()
	{
		for (int ndx = 0; ndx < mNodJoints.Length; ndx++) 
		{
			if(!mNodJoints[ndx].independantUpdate)
			{
				mNodJoints[ndx].ResetJoint();
			}
		}
	}

    //	/ <summary>
	//	/ Provides the Torso Orientation for other joints 
	//	/ </summary>
	public static float [,] GetTorsoOrientation ()
	{
		return mNodJoints [0].ReturnTorsoOrientation();
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
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake ()
	{
		Application.targetFrameRate = 300;
		mNodJoints = GetComponentsInChildren<NodJoint>();
	}
	
	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start() 
	{
		ResetJoints();
		StartJoints();
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update() 
	{
		UpdateJoints();
	}
	
	/// <summary>
	/// GUI Update.
	/// </summary>
	void OnGUI()
	{
		if (GUI.Button (new Rect (20, 70, 200, 50), "Start Sensors"))
		{
			ResetJoints();
			StartJoints();
		}

		if (GUI.Button (new Rect (220, 70, 200, 50), "Reset Sensors "))
		{			
			ResetJoints();        
		}


		if (GUI.Button (new Rect (880, 550, 120 , 25), "Thoracolumbar"))
		{			
			vKey = 1;        
		}

		if (GUI.Button (new Rect (1005, 550, 110 , 25), "Right Arm"))
		{			
			vKey = 2;        
		}


		if (GUI.Button (new Rect (1120, 550, 110 , 25), "Left Arm"))
		{			
			vKey = 3;         
		}


		if (GUI.Button (new Rect (1235, 550, 110 , 25), "Right Leg"))
		{			
			vKey = 4;        
		}

		if (GUI.Button (new Rect (1350, 550, 110 , 25), "Left Leg"))
		{			
			vKey = 5;         
		}
	}
}
