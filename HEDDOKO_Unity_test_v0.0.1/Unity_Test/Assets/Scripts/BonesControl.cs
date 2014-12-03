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
	//public Transform rightShoulderTransform = null;
	public Transform upperSpineTransform = null;
	public Transform lowerSpineTransform = null;
	//public Transform leftShoulderTransform = null;
	//public Transform leftHipTransform = null;
	//public Transform rightHipTransform = null;

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
	//public float rightShoulderFac = 0.5f;
	public float upperSpineFac = 0.5f;
	public float lowerSpineFac = 0.5f;
	//public float leftShoulderFac = 0.5f;
	//public float leftHipFac = 0.5f;
	//public float rightHipFac = 0.5f;
	public float rightThighFac = 0.5f;
	public float rightCalfFac = 0.5f;
	public float leftCalfFac = 0.5f;
	public float leftThighFac = 0.5f;

	//Quaternion targets
	private Quaternion rightUpperArmTarget;
	private Quaternion rightForeArmTarget;
	private Quaternion leftUpperArmTarget;
	private Quaternion leftForeArmTarget;
	private Quaternion rightShoulderTarget;
	private Quaternion spineTarget;
	private Quaternion upperSpineTarget;
	private Quaternion lowerSpineTarget;
	private Quaternion leftShoulderTarget;
	private Quaternion leftHipTarget;
	private Quaternion rightHipTarget;
	private Quaternion rightThighTarget;
	private Quaternion rightCalfTarget;
	private Quaternion leftCalfTarget;
	private Quaternion leftThighTarget;

	//test 
	private Vector3 currentDirection = Vector3.forward;

	//protected Animator animator;
	
	//public bool ikActive = false;

	// Use this for initialization
	void Start () 
	{
		Debug.Log("Initializing connection to service");

		//animator = GetComponent<Animator>();

		initSensorsConnection();

		Debug.Log("connecting to sensors");

		connect6DSensor(0);
		connect6DSensor(1);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnApplicationQuit() 
	{
		if (getNumberConnectedDevices () >= 1) 
		{
			//shutDown6DSensor (0);
		}
	}

	void OnDestroy() 
	{
		//if (getNumberConnectedDevices () >= 1) 
		{
			//shutDown6DSensor (0);
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

			upperSpineTarget 	= Quaternion.Euler (getSensorLatestPitch (0) * 360 / Mathf.PI, getSensorLatestYaw (0) * 360 / Mathf.PI, getSensorLatestRoll (0) * 360 / Mathf.PI);
			lowerSpineTarget 	= Quaternion.Euler (getSensorLatestPitch (1) * 360 / Mathf.PI, getSensorLatestYaw (1) * 360 / Mathf.PI, getSensorLatestRoll (1) * 360 / Mathf.PI);

			/*rightUpperArmTarget = Quaternion.Euler (getSensorLatestPitch (2) * 360 / Mathf.PI, getSensorLatestYaw (2) * 360 / Mathf.PI, getSensorLatestRoll (2) * 360 / Mathf.PI);
			rightForeArmTarget 	= Quaternion.Euler (getSensorLatestPitch (3) * 360 / Mathf.PI, getSensorLatestYaw (3) * 360 / Mathf.PI, getSensorLatestRoll (3) * 360 / Mathf.PI);

			leftUpperArmTarget 	= Quaternion.Euler (getSensorLatestPitch (4) * 360 / Mathf.PI, getSensorLatestYaw (4) * 360 / Mathf.PI, getSensorLatestRoll (4) * 360 / Mathf.PI);
			leftForeArmTarget 	= Quaternion.Euler (getSensorLatestPitch (5) * 360 / Mathf.PI, getSensorLatestYaw (5) * 360 / Mathf.PI, getSensorLatestRoll (5) * 360 / Mathf.PI);

			rightThighTarget 	= Quaternion.Euler (getSensorLatestPitch (6) * 360 / Mathf.PI, getSensorLatestYaw (6) * 360 / Mathf.PI, getSensorLatestRoll (6) * 360 / Mathf.PI);
			rightCalfTarget 	= Quaternion.Euler (getSensorLatestPitch (7) * 360 / Mathf.PI, getSensorLatestYaw (7) * 360 / Mathf.PI, getSensorLatestRoll (7) * 360 / Mathf.PI);

			leftThighTarget 	= Quaternion.Euler (getSensorLatestPitch (8) * 360 / Mathf.PI, getSensorLatestYaw (8) * 360 / Mathf.PI, getSensorLatestRoll (8) * 360 / Mathf.PI);
			leftCalfTarget 		= Quaternion.Euler (getSensorLatestPitch (9) * 360 / Mathf.PI, getSensorLatestYaw (9) * 360 / Mathf.PI, getSensorLatestRoll (9) * 360 / Mathf.PI);
			//*/
			Debug.Log("sensors rotation");

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
