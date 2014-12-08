using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;

[RequireComponent(typeof(Animator))] 

public class BonesControl : MonoBehaviour 
{

	[DllImport("SensorsLib")]
	public static extern void initSensorsConnection();

	[DllImport("SensorsLib")]
	public static extern bool indexExist(int vIdx);

	[DllImport("SensorsLib")]
	public static extern void connect6DSensor(int vIdx);

	[DllImport("SensorsLib")]
	public static extern void shutDown6DSensor(int vIdx);

	[DllImport("SensorsLib")]
	public static extern int  getNumberConnectedDevices();

	[DllImport("SensorsLib")]
	public static extern float getSensorLatestPitch(int vIdx);

	[DllImport("SensorsLib")]
	public static extern float getSensorLatestRoll(int vIdx);

	[DllImport("SensorsLib")]
	public static extern float getSensorLatestYaw(int vIdx);

	//Arms transforms
	public Transform rightUpperArmTransform = null;
	public Transform rightForeArmTransform = null;
	public Transform leftUpperArmTransform = null;
	public Transform leftForeArmTransform = null;

	//Torso transforms
	public Transform upperSpineTransform = null;
	public Transform lowerSpineTransform = null;

	//Legs transforms
	public Transform rightThighTransform = null;
	public Transform rightCalfTransform = null;
	public Transform leftCalfTransform = null;
	public Transform leftThighTransform = null;

	//Quaternion targets
	private Quaternion rightUpperArmTarget;
	private Quaternion rightForeArmTarget;
	private Quaternion leftUpperArmTarget;
	private Quaternion leftForeArmTarget;
	private Quaternion upperSpineTarget;
	private Quaternion lowerSpineTarget;
	private Quaternion rightThighTarget;
	private Quaternion rightCalfTarget;
	private Quaternion leftCalfTarget;
	private Quaternion leftThighTarget;

	//Vector Initial targets
	private Vector3 rightUpperArmInitEulers;
	private Vector3 rightForeArmInitEulers;
	private Vector3 leftUpperArmInitEulers;
	private Vector3 leftForeArmInitEulers;
	private Vector3 upperSpineInitEulers;
	private Vector3 lowerSpineInitEulers;
	private Vector3 rightThighInitEulers;
	private Vector3 rightCalfInitEulers;
	private Vector3 leftThighInitEulers;
	private Vector3 leftCalfInitEulers;

	//Vector Current targets
	private Vector3 rightUpperArmCurrentEulers;
	private Vector3 rightForeArmCurrentEulers;
	private Vector3 leftUpperArmCurrentEulers;
	private Vector3 leftForeArmCurrentEulers;
	private Vector3 upperSpineCurrentEulers;
	private Vector3 lowerSpineCurrentEulers;
	private Vector3 rightThighCurrentEulers;
	private Vector3 rightCalfCurrentEulers;
	private Vector3 leftThighCurrentEulers;
	private Vector3 leftCalfCurrentEulers;

	private Boolean isInitialized = false;

	private float mapRange(float a1,float a2,float b1,float b2,float s)
	{
		return b1 + (s-a1)*(b2-b1)/(a2-a1);
	}

	private void ConnectSensor(int vIdx) 
	{
		Boolean vExists = indexExist (vIdx);
		if (vExists) 
		{
			connect6DSensor(vIdx);
		}
	}

	private void ShutdownSensor(int vIdx) 
	{
		Boolean vExists = indexExist (vIdx);
		if (vExists) 
		{
			shutDown6DSensor(vIdx);
		}
	}

	private Vector3 getEulerOrientation(int vIdx)
	{
		Vector3 vEulerVector = Vector3.zero;

		if (indexExist (vIdx)) 
		{
			vEulerVector.x = getSensorLatestPitch (vIdx) * 360 / Mathf.PI; 
			//vEulerVector.x = mapRange(-180, 180, 0, 360, vEulerVector.x);
			vEulerVector.y = getSensorLatestYaw (vIdx) * 360 / Mathf.PI;
			//vEulerVector.y = mapRange(-180, 180, 0, 360, vEulerVector.y);
			vEulerVector.z = getSensorLatestRoll (vIdx) * 360 / Mathf.PI;
			//vEulerVector.z = mapRange(-360, 360, 0, 360, vEulerVector.z);
		}

		return vEulerVector;
	}

	void ResetOrientations()
	{
		if (getNumberConnectedDevices () >= 1) 
		{
			upperSpineCurrentEulers 	= Vector3.zero;
			lowerSpineCurrentEulers 	= Vector3.zero;
			rightUpperArmCurrentEulers 	= Vector3.zero;
			rightForeArmCurrentEulers 	= Vector3.zero;
			leftUpperArmCurrentEulers 	= Vector3.zero;
			leftForeArmCurrentEulers 	= Vector3.zero;
			rightThighCurrentEulers 	= Vector3.zero;
			rightCalfCurrentEulers 		= Vector3.zero;
			leftThighCurrentEulers 		= Vector3.zero;
			leftCalfCurrentEulers 		= Vector3.zero;
			
			upperSpineTarget 	= Quaternion.Euler(upperSpineCurrentEulers);
			lowerSpineTarget 	= Quaternion.Euler(lowerSpineCurrentEulers);
			rightUpperArmTarget = Quaternion.Euler(rightUpperArmCurrentEulers);
			rightForeArmTarget 	= Quaternion.Euler(rightForeArmCurrentEulers);
			leftUpperArmTarget 	= Quaternion.Euler(leftUpperArmCurrentEulers);
			leftForeArmTarget 	= Quaternion.Euler(leftForeArmCurrentEulers);
			rightThighTarget 	= Quaternion.Euler(rightThighCurrentEulers);
			rightCalfTarget 	= Quaternion.Euler(rightCalfCurrentEulers);
			leftThighTarget 	= Quaternion.Euler(leftThighCurrentEulers);
			leftCalfTarget 		= Quaternion.Euler(leftCalfCurrentEulers);
			
			upperSpineTransform.localRotation 	 = upperSpineTarget;
			lowerSpineTransform.localRotation 	 = lowerSpineTarget;			
			rightUpperArmTransform.localRotation = rightUpperArmTarget;
			rightForeArmTransform.localRotation  = rightForeArmTarget;			
			leftUpperArmTransform.localRotation  = leftUpperArmTarget;
			leftForeArmTransform.localRotation   = leftForeArmTarget;			
			rightThighTransform.localRotation 	 = rightThighTarget;
			rightCalfTransform.localRotation 	 = rightCalfTarget;			
			leftThighTransform.localRotation 	 = leftThighTarget;
			leftCalfTransform.localRotation 	 = leftCalfTarget;
			
			/*upperSpineInitEulers = getEulerOrientation(0);
			lowerSpineInitEulers = getEulerOrientation(1);//*/
			/*rightUpperArmInitEulers = getEulerOrientation(2);
			rightForeArmInitEulers = getEulerOrientation(3);//*/
			leftUpperArmInitEulers = getEulerOrientation(0);
			/*leftForeArmInitEulers = getEulerOrientation(1);//*/
			/*rightThighInitEulers = getEulerOrientation(6);
			rightCalfInitEulers = getEulerOrientation(7);//*/
			/*leftThighInitEulers = getEulerOrientation(8);
			leftCalfInitEulers = getEulerOrientation(9);//*/
			//*/
		}
	}


	// Use this for initialization
	void Start () 
	{
		initSensorsConnection();

		for (int i = 0; i < 10; i++) 
		{
			ConnectSensor(i);		
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!isInitialized) 
		{
			isInitialized = true;
			ResetOrientations ();
		}
	}

	void OnApplicationQuit() 
	{
		for (int i = 0; i < 10; i++) 
		{
			ShutdownSensor(i);		
		}
	}

	void OnDestroy() 
	{
		for (int i = 0; i < 10; i++) 
		{
			ShutdownSensor(i);		
		}
	}


	void LateUpdate()
	{
		if (getNumberConnectedDevices () >= 1 && isInitialized) 
		{
			/*upperSpineCurrentEulers = getEulerOrientation(0);
			upperSpineCurrentEulers = upperSpineCurrentEulers - upperSpineInitEulers;

			lowerSpineCurrentEulers = getEulerOrientation(1);
			lowerSpineCurrentEulers = lowerSpineCurrentEulers - lowerSpineInitEulers;
			//*/

			/*rightUpperArmCurrentEulers = getEulerOrientation(0);
			rightUpperArmCurrentEulers = rightUpperArmCurrentEulers - rightUpperArmInitEulers;
			rightUpperArmCurrentEulers.x = rightUpperArmCurrentEulers.z;
			rightUpperArmCurrentEulers.z = rightUpperArmCurrentEulers.x;//*/
			/*rightUpperArmCurrentEulers.x = -tempVector.y;
			rightUpperArmCurrentEulers.y = tempVector.z;
			rightUpperArmCurrentEulers.z = -tempVector.x;//*/

			/*rightForeArmCurrentEulers = getEulerOrientation(1);
			rightForeArmCurrentEulers = rightForeArmCurrentEulers - rightForeArmInitEulers;
			//*/

			//vPitch: -176.545425 vRoll: -32.676498 vYaw: 178.251984 
			leftUpperArmCurrentEulers = getEulerOrientation(0);
			Vector3 tempVector = leftUpperArmCurrentEulers - leftUpperArmInitEulers;
			/*tempVector.x = mapRange(-360, 360, 0, 360, tempVector.x);
			tempVector.y = mapRange(-360, 360, 0, 360, tempVector.y);
			tempVector.z = mapRange(-360, 360, 0, 360, tempVector.z);*/
			leftUpperArmCurrentEulers = new Vector3(tempVector.x, tempVector.y, tempVector.z);
			//leftUpperArmCurrentEulers = new Vector3(tempVector.y, 0, 0);
			//leftUpperArmCurrentEulers = new Vector3(-176.5F, -32.6F, 178.25F);

			/*leftForeArmCurrentEulers = getEulerOrientation(1);
			leftForeArmCurrentEulers = leftForeArmCurrentEulers - leftForeArmInitEulers;
			//*/

			/*rightThighCurrentEulers = getEulerOrientation(6);
			rightThighCurrentEulers = rightThighCurrentEulers - rightThighInitEulers;

			rightCalfCurrentEulers = getEulerOrientation(7);
			rightCalfCurrentEulers = rightCalfCurrentEulers - rightCalfInitEulers;
			//*/

			/*leftThighCurrentEulers = getEulerOrientation(8);
			leftThighCurrentEulers = leftThighCurrentEulers - leftThighInitEulers;

			leftCalfCurrentEulers = getEulerOrientation(9);
			leftCalfCurrentEulers = leftCalfCurrentEulers - leftCalfInitEulers;
			//*/

			upperSpineTarget 	= Quaternion.Euler(upperSpineCurrentEulers);
			lowerSpineTarget 	= Quaternion.Euler(lowerSpineCurrentEulers);
			rightUpperArmTarget = Quaternion.Euler(rightUpperArmCurrentEulers);
			rightForeArmTarget 	= Quaternion.Euler(rightForeArmCurrentEulers);
			leftUpperArmTarget 	= Quaternion.Euler(leftUpperArmCurrentEulers);
			leftForeArmTarget 	= Quaternion.Euler(leftForeArmCurrentEulers);
			rightThighTarget 	= Quaternion.Euler(rightThighCurrentEulers);
			rightCalfTarget 	= Quaternion.Euler(rightCalfCurrentEulers);
			leftThighTarget 	= Quaternion.Euler(leftThighCurrentEulers);
			leftCalfTarget 		= Quaternion.Euler(leftCalfCurrentEulers);

			upperSpineTransform.localRotation 	 = upperSpineTarget;
			lowerSpineTransform.localRotation 	 = lowerSpineTarget;
			rightUpperArmTransform.localRotation = rightUpperArmTarget;
			rightForeArmTransform.localRotation  = rightForeArmTarget;
			leftUpperArmTransform.localRotation  = leftUpperArmTarget;
			leftForeArmTransform.localRotation   = leftForeArmTarget;
			rightThighTransform.localRotation 	 = rightThighTarget;
			rightCalfTransform.localRotation 	 = rightCalfTarget;
			leftThighTransform.localRotation 	 = leftThighTarget;
			leftCalfTransform.localRotation 	 = leftCalfTarget;
		}
	}

	void OnGUI()
	{
		if (GUI.Button (new Rect (20, 20, 100, 50), "RESET")) 
		{
			ResetOrientations();
		}
	}
}
