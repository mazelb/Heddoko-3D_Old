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
	//public Transform rightShoulderTransform = null;
	//public Transform leftShoulderTransform = null;
	//public Transform rightHipTransform = null;
	//public Transform leftHipTransform = null;

	//Legs transforms
	public Transform rightThighTransform = null;
	public Transform rightCalfTransform = null;
	public Transform leftCalfTransform = null;
	public Transform leftThighTransform = null;

	//factors
	public float rightUpperArmFac = 0.5f;
	public float rightForeArmFac = 0.5f;
	public float leftUpperArmFac = 0.5f;
	public float leftForeArmFac = 0.5f;
	public float upperSpineFac = 0.5f;
	public float lowerSpineFac = 0.5f;
	public float rightThighFac = 0.5f;
	public float rightCalfFac = 0.5f;
	public float leftCalfFac = 0.5f;
	public float leftThighFac = 0.5f;
	//public float rightShoulderFac = 0.5f;
	//public float leftShoulderFac = 0.5f;
	//public float leftHipFac = 0.5f;
	//public float rightHipFac = 0.5f;

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
	//private Quaternion leftShoulderTarget;
	//private Quaternion rightShoulderTarget;
	//private Quaternion spineTarget;
	//private Quaternion leftHipTarget;
	//private Quaternion rightHipTarget;

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
	//private Quaternion spineInitEulers;
	//private Quaternion rightShoulderInitEulers;
	//private Quaternion leftShoulderInitEulers;
	//private Quaternion rightHipInitEulers;
	//private Quaternion leftHipInitEulers;

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
	//private Quaternion spineCurrentEulers;
	//private Quaternion rightShoulderCurrentEulers;
	//private Quaternion leftShoulderCurrentEulers;
	//private Quaternion rightHipCurrentEulers;
	//private Quaternion leftHipCurrentEulers;


	private Boolean isInitialized = false;

	private void ConnectSensor(int vIdx) 
	{
		if (indexExist (vIdx)) 
		{
			connect6DSensor(vIdx);
		}
	}

	// Use this for initialization
	void Start () 
	{
		initSensorsConnection();

		for (int i = 0; i < 10; i++) 
		{
			connect6DSensor(i);		
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (getNumberConnectedDevices () >= 2 && !isInitialized) 
		{
			isInitialized = true;

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

			upperSpineInitEulers.x = getSensorLatestPitch (0) * 360 / Mathf.PI; 
			upperSpineInitEulers.y = getSensorLatestYaw (0) * 360 / Mathf.PI;
			upperSpineInitEulers.z = getSensorLatestRoll (0) * 360 / Mathf.PI;

			lowerSpineInitEulers.x = getSensorLatestPitch (1) * 360 / Mathf.PI; 
			lowerSpineInitEulers.y = getSensorLatestYaw (1) * 360 / Mathf.PI;
			lowerSpineInitEulers.z = getSensorLatestRoll (1) * 360 / Mathf.PI;

			/*rightUpperArmInitEulers.x = getSensorLatestPitch (2) * 360 / Mathf.PI; 
			rightUpperArmInitEulers.y = getSensorLatestYaw (2) * 360 / Mathf.PI;
			rightUpperArmInitEulers.z = getSensorLatestRoll (2) * 360 / Mathf.PI;

			rightForeArmInitEulers.x = getSensorLatestPitch (3) * 360 / Mathf.PI; 
			rightForeArmInitEulers.y = getSensorLatestYaw (3) * 360 / Mathf.PI;
			rightForeArmInitEulers.z = getSensorLatestRoll (3) * 360 / Mathf.PI;

			leftUpperArmInitEulers.x = getSensorLatestPitch (4) * 360 / Mathf.PI; 
			leftUpperArmInitEulers.y = getSensorLatestYaw (4) * 360 / Mathf.PI;
			leftUpperArmInitEulers.z = getSensorLatestRoll (4) * 360 / Mathf.PI;

			leftForeArmInitEulers.x = getSensorLatestPitch (5) * 360 / Mathf.PI; 
			leftForeArmInitEulers.y = getSensorLatestYaw (5) * 360 / Mathf.PI;
			leftForeArmInitEulers.z = getSensorLatestRoll (5) * 360 / Mathf.PI;

			rightThighInitEulers.x = getSensorLatestPitch (6) * 360 / Mathf.PI; 
			rightThighInitEulers.y = getSensorLatestYaw (6) * 360 / Mathf.PI;
			rightThighInitEulers.z = getSensorLatestRoll (6) * 360 / Mathf.PI;

			rightCalfInitEulers.x = getSensorLatestPitch (7) * 360 / Mathf.PI; 
			rightCalfInitEulers.y = getSensorLatestYaw (7) * 360 / Mathf.PI;
			rightCalfInitEulers.z = getSensorLatestRoll (7) * 360 / Mathf.PI;

			leftThighInitEulers.x = getSensorLatestPitch (8) * 360 / Mathf.PI; 
			leftThighInitEulers.y = getSensorLatestYaw (8) * 360 / Mathf.PI;
			leftThighInitEulers.z = getSensorLatestRoll (8) * 360 / Mathf.PI;

			leftCalfInitEulers.x = getSensorLatestPitch (9) * 360 / Mathf.PI; 
			leftCalfInitEulers.y = getSensorLatestYaw (9) * 360 / Mathf.PI;
			leftCalfInitEulers.z = getSensorLatestRoll (9) * 360 / Mathf.PI;
			//*/
		}
	}

	void OnApplicationQuit() 
	{
		if (getNumberConnectedDevices () >= 1) 
		{
			shutDown6DSensor (0);
		}
	}

	void OnDestroy() 
	{
		if (getNumberConnectedDevices () >= 1) 
		{
			shutDown6DSensor (0);
		}
	}


	void LateUpdate()
	{
		if (getNumberConnectedDevices () >= 2) 
		{
			Debug.Log(">1 sensors connected");
			//currentDirection.x = Mathf.Clamp(currentDirection.x + Input.GetAxis("Mouse X"), -1, 1);
			//currentDirection.y = Mathf.Clamp(currentDirection.y + Input.GetAxis("Mouse Y"), -1, 1);
			//rightUpperArmTarget = Quaternion.LookRotation(currentDirection);


			upperSpineCurrentEulers.x = getSensorLatestPitch (0) * 360 / Mathf.PI; 
			upperSpineCurrentEulers.y = getSensorLatestYaw (0) * 360 / Mathf.PI;
			upperSpineCurrentEulers.z = getSensorLatestRoll (0) * 360 / Mathf.PI;
			upperSpineCurrentEulers += upperSpineCurrentEulers - upperSpineInitEulers;

			lowerSpineCurrentEulers.x = getSensorLatestPitch (1) * 360 / Mathf.PI; 
			lowerSpineCurrentEulers.y = getSensorLatestYaw (1) * 360 / Mathf.PI;
			lowerSpineCurrentEulers.z = getSensorLatestRoll (1) * 360 / Mathf.PI;
			lowerSpineCurrentEulers += lowerSpineCurrentEulers - lowerSpineInitEulers;

			/*rightUpperArmCurrentEulers.x = getSensorLatestPitch (2) * 360 / Mathf.PI; 
			rightUpperArmCurrentEulers.y = getSensorLatestYaw (2) * 360 / Mathf.PI;
			rightUpperArmCurrentEulers.z = getSensorLatestRoll (2) * 360 / Mathf.PI;
			rightUpperArmCurrentEulers += rightUpperArmCurrentEulers - rightUpperArmInitEulers;
			
			rightForeArmCurrentEulers.x = getSensorLatestPitch (3) * 360 / Mathf.PI; 
			rightForeArmCurrentEulers.y = getSensorLatestYaw (3) * 360 / Mathf.PI;
			rightForeArmCurrentEulers.z = getSensorLatestRoll (3) * 360 / Mathf.PI;
			rightForeArmCurrentEulers += rightForeArmCurrentEulers - rightForeArmInitEulers;

			leftUpperArmCurrentEulers.x = getSensorLatestPitch (4) * 360 / Mathf.PI; 
			leftUpperArmCurrentEulers.y = getSensorLatestYaw (4) * 360 / Mathf.PI;
			leftUpperArmCurrentEulers.z = getSensorLatestRoll (4) * 360 / Mathf.PI;
			leftUpperArmCurrentEulers += leftUpperArmCurrentEulers - leftUpperArmInittEulers;

			leftForeArmCurrentEulers.x = getSensorLatestPitch (5) * 360 / Mathf.PI; 
			leftForeArmCurrentEulers.y = getSensorLatestYaw (5) * 360 / Mathf.PI;
			leftForeArmCurrentEulers.z = getSensorLatestRoll (5) * 360 / Mathf.PI;
			leftForeArmCurrentEulers += leftForeArmCurrentEulers - leftForeArmInitEulers;

			rightThighCurrentEulers.x = getSensorLatestPitch (6) * 360 / Mathf.PI; 
			rightThighCurrentEulers.y = getSensorLatestYaw (6) * 360 / Mathf.PI;
			rightThighCurrentEulers.z = getSensorLatestRoll (6) * 360 / Mathf.PI;
			rightThighCurrentEulers += rightThighCurrentEulers - rightThighInitEulers;

			rightCalfCurrentEulers.x = getSensorLatestPitch (7) * 360 / Mathf.PI; 
			rightCalfCurrentEulers.y = getSensorLatestYaw (7) * 360 / Mathf.PI;
			rightCalfCurrentEulers.z = getSensorLatestRoll (7) * 360 / Mathf.PI;
			rightCalfCurrentEulers += rightCalfCurrentEulers - rightCalfInittEulers;

			leftThighCurrentEulers.x = getSensorLatestPitch (8) * 360 / Mathf.PI; 
			leftThighCurrentEulers.y = getSensorLatestYaw (8) * 360 / Mathf.PI;
			leftThighCurrentEulers.z = getSensorLatestRoll (8) * 360 / Mathf.PI;
			leftThighCurrentEulers += leftThighCurrentEulers - leftThighInitEulers;

			leftCalfCurrentEulers.x = getSensorLatestPitch (9) * 360 / Mathf.PI; 
			leftCalfCurrentEulers.y = getSensorLatestYaw (9) * 360 / Mathf.PI;
			leftCalfCurrentEulers.z = getSensorLatestRoll (9) * 360 / Mathf.PI;
			leftCalfCurrentEulers += leftCalfCurrentEulers - leftCalfInitEulers;
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

			//Quaternion.Slerp(rightUpperArmTransform.localRotation, rightUpperArmTarget, rightUpperArmFac);
			//elbow.localRotation = Quaternion.Slerp(elbow.localRotation, rotation, elbowControl);
			//hand.localRotation = Quaternion.Slerp(hand.localRotation, rotation, handControl);
		}
	}
}
