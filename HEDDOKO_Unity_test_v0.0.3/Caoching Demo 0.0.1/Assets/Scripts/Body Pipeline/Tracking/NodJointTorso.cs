//using UnityEngine;
//using System;
//using System.Collections;
//using System.Collections.Generic;
////using Nod;
//using System.IO;

/////// joint code for the Torso Orientation
//public class JointTorso : NodJoint 
//{


//	// current Torso orientation for will be stored in this variable 
//	public float [,] vaTorsoOrientation = new float[3,3];
//	public float [,] vaSpinOrientation = new float[3,3];

//	// This variable stores the time of current frame. It is used for angular velocity and acceleration extraction
//	public float vTime= 0 ;

//	//angles buffer save data for turn detection
//	public List<float> rot_angles = new List<float> ();
//	public List<int> rot_angles2 = new List<int> ();

//	//	
//	//	 ReturnTorsoOrientation()
//	//	* @ This returns the current orientation of the Torso, The current Torso Orientation is necessary for angle extraction of shoulders and hips
//	//	* @return void float[,] vaTorsoOrientation
//	//	*/// 
//	public override float[,] ReturnTorsoOrientation()
//	{

//		return vaTorsoOrientation;
//	}


//		/**
//	* TorsoOrientation()
//	*	@This Function Provides The compensated Update for the Torso Orientation
//	*	@param Vector3 NodTorso, Torso Euler Angles Inputs
//	*	@paramVector3  InitNodTorso, this is the information of the initial frame for Torso joint
//	*	@return void
//	*/
//	public float[,] TorsoOrientation(Vector3 NodTorso , Vector3 InitNodTorso)
//	{
//		//Intermediate arrays until achieve final orientation for Torso, they are Taged with F ( Global to local rotation) and B (Local to Global rotation) and are numbered consecutively

//		float[,] TorsoF1 = new float[3, 3];
//		float[,] TorsoFi = new float[3, 3];

//		float[,] TorsoB1 = new float[3, 3];
//		float[,] TorsoBi = new float[3, 3];

//		float[,] vaTorsoOrientationLocal = new float[3, 3];

//		/////////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
//		/////////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
//		/////////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
//		/////////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
//		/////////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
//		/////////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
//		/////////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
//		/////////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////


//		/////////// Converting to orientation matrix ///////////////////

//		TorsoB1 = RotationLocal (NodTorso.z, NodTorso.x, NodTorso.y);
//		TorsoF1 = RotationGlobal (NodTorso.z, NodTorso.x, NodTorso.y);

//		TorsoBi = RotationLocal (InitNodTorso.z, InitNodTorso.x, InitNodTorso.y);
//		TorsoFi = RotationGlobal (InitNodTorso.z, InitNodTorso.x, InitNodTorso.y);

//		/////////// Initial Frame Adjustments ///////////////////
//		vaTorsoOrientationLocal = multi (TorsoFi, TorsoB1);
//		return vaTorsoOrientationLocal;
//	}


//	public override void ResetJoint ()
//	{
	
//		base.ResetJoint ();

//		}


//		public override void UpdateJoint () 
//	{
//		for (int ndx = 0; ndx < mNodSensors.Length; ndx++)
//		{
//			mNodSensors [ndx].UpdateSensor ();
//		}

//		Vector3 vNodRawEuler1 = mNodSensors[0].curRotationRawEuler;
//		vNodRawEuler1 = new Vector3(vNodRawEuler1.x , vNodRawEuler1.y , vNodRawEuler1.z );
//		Vector3 NodIniEuler1 = mNodSensors[0].initRotationEuler;
//		NodIniEuler1 = new Vector3(NodIniEuler1.x , NodIniEuler1.y , NodIniEuler1.z );


//		Vector3 vNodRawEuler2 = mNodSensors[1].curRotationRawEuler;
//		Vector3 NodIniEuler2 = mNodSensors[1].initRotationEuler;


//		// call TorsoOrientation function to calculate current orientation of the Torso joint
//		vaTorsoOrientation=TorsoOrientation (vNodRawEuler1, NodIniEuler1);
//		vaSpinOrientation=TorsoOrientation (vNodRawEuler2, NodIniEuler2);


//		// convert the orientation matrices to quaternion to be used in unity model
//		NodQuaternionOrientation vNodRawQuat = MatToQuat(vaTorsoOrientation);
//		NodQuaternionOrientation vNodRawQuat2 = MatToQuat(vaSpinOrientation);

//		Quaternion vNodQuat = new Quaternion(vNodRawQuat.x, vNodRawQuat.y, vNodRawQuat.z, vNodRawQuat.w);
//		Quaternion vJointQuat = inverseInitRotation * vNodQuat * Quaternion.Inverse(Quaternion.Euler(quaternionFactor));

//		Quaternion vNodQuat2 = new Quaternion(vNodRawQuat2.x, vNodRawQuat2.y, vNodRawQuat2.z, vNodRawQuat2.w);
//		Quaternion vJointQuat2 = inverseInitRotation * vNodQuat2 * Quaternion.Inverse(Quaternion.Euler(quaternionFactor));

//		if(jointTransform != null)
//		{
//			jointTransform.rotation = vJointQuat;
//			jointTransform2.rotation = vJointQuat2;
			
//			//*************************************Anchor feet to ground***********************
//			// Length of upper and lower legs and foot height should be added
//			float vUpperLegLength=1f;
//			float vLowerLegLength=1f;
//			float vFootHeight=0.1f;
			
//			float vRightLegHeight= JointLegRight.RightLegMovement (vUpperLegLength,vLowerLegLength);
//			float vLeftLegHeight= JointLegLeft.LeftLegMovement (vUpperLegLength,vLowerLegLength);
//			float vLegMovement;
			
//			if(vRightLegHeight > vLeftLegHeight) 
//			{
//				vLegMovement = vRightLegHeight;
//			}
//			else 
//			{
//				vLegMovement = vLeftLegHeight;
//			}
//			Vector3 v3 = jointTransform2.localPosition;
//			//*****To move the body down when sitting uncomment next line*****//
//			//v3.y = vLegMovement+vFootHeight;
//			jointTransform2.localPosition = v3;
			
//		}


//	}

//}
