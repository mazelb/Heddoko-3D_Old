//using UnityEngine;
//using System;
//using System.Collections;
////using Nod;
//using System.IO;

/////// joint code for the Left Arm
//public class JointArmLeft : NodJoint 
//{
//	// current Upperarm (shoulder) and lower arm (Elbow) joints orientation, UpAr stands for upper arm and LoAr stands for Lower arm (forearm) in this code
//	public float [,] UpArOrientation = new float[3,3];
//	public float [,] LoArOrientation = new float[3,3];

//	//	/**
//	//* ArmOrientation()
//	//*	@ This function calculates and updates the current orientation of the shoulder and elbow joints
//	//	*	@param Vector3 NodUpAr, UpperArm Euler Angles Inputs
//	//	*	@paramVector3  InitNodUpAr, this is the information of the initial frame for Upper Arm joint
//	//	*	@param Vector3 NodLoAr, forearm Euler Angles Inputs
//	//	*	@paramVector3  InitNodLoAr, this is the information of the initial frame for Torso joint
//	//*	@return void
//	//*/
//	public void ArmOrientation(Vector3 NodUpAr , Vector3 InitNodUpAr , Vector3 NodLoAr , Vector3 InitNodLoAr)

//	{
//		//Intermediate arrays until achieve final orientation for shoulder and elbow
//		//they are Tagged with F (forward rotation) and B (Backward rotation) and are numbered consecutively
//		// UpAr stands for upper arm sensor orientation and lower arm stands for lower arm (forearm) orientation

//		float [,] UpArF1 = new float[3,3];
//		float [,] UpArF2 = new float[3,3];
//		float [,] UpArFi = new float[3,3];


//		float [,] UpArB1 = new float[3,3];
//		float [,] UpArB2 = new float[3,3];
//		float [,] UpArB3 = new float[3,3];
//		float [,] UpArB4 = new float[3,3];
//		float [,] UpArB5 = new float[3,3];
//		float [,] UpArB6 = new float[3,3];
//		float [,] UpArB7 = new float[3,3];
//		float [,] UpArBi = new float[3,3];

//		float [,] LoArF1 = new float[3,3];
//		float [,] LoArF2 = new float[3,3];
//		float [,] LoArFi = new float[3,3];


//		float [,] LoArB1 = new float[3,3];
//		float [,] LoArB2 = new float[3,3];
//		float [,] LoArB3 = new float[3,3];
//		float [,] LoArB4 = new float[3,3];
//		float [,] LoArB5 = new float[3,3];
//		float [,] LoArB6 = new float[3,3];
//		float [,] LoArB7 = new float[3,3];
//		float [,] LoArBi = new float[3,3];

//		float [,] CurrentLoArOrientation = new float[3,3];

//		/// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
//		/// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
//		/// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
//		/// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
//		/// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
//		/// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
//		/// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
//		/// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////

//		/////////// Converting to orientation matrix ///////////////////

//		UpArB1 = RotationLocal (NodUpAr.z, NodUpAr.x, NodUpAr.y);
//		UpArF1 = RotationGlobal (NodUpAr.z,NodUpAr.x, NodUpAr.y);

//		LoArB1 = RotationLocal (NodLoAr.z, NodLoAr.x, NodLoAr.y);
//		LoArF1 = RotationGlobal (NodLoAr.z,NodLoAr.x, NodLoAr.y);

//		UpArBi = RotationLocal (InitNodUpAr.z, InitNodUpAr.x, InitNodUpAr.y);
//		UpArFi = RotationGlobal (InitNodUpAr.z,InitNodUpAr.x, InitNodUpAr.y);

//		LoArBi = RotationLocal (InitNodLoAr.z, InitNodLoAr.x, InitNodLoAr.y);
//		LoArFi = RotationGlobal (InitNodLoAr.z, InitNodLoAr.x, InitNodLoAr.y);



//		/////////// Initial Frame Adjustments ///////////////////

//		//UpArB2 = multi(UpArFi, UpArB1);
//		//LoArB2 = multi(LoArFi, LoArB1);
//        UpArB3 = multi(UpArFi, UpArB1);
//        LoArB4 = multi(LoArFi, LoArB1);


//        ///	/////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
//        /// /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
//        /// /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
//        /// /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
//        /// /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
//        /// /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
//        /// /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////


//        ////////////////// setting to Final Body orientation lower arm///////////////////////////////
//        Vector3 u = new Vector3(LoArB4[0,0] , LoArB4[1,0] , LoArB4[2,0]);
//		CurrentLoArOrientation= RVector(u, -(float)3.1415 /2 );


//		LoArB5= multi(CurrentLoArOrientation, LoArB4);


//		u.Set( LoArB5[0,1] , LoArB5[1,1] , LoArB5[2,1]);
//		CurrentLoArOrientation= RVector(u, (float)3.1415 );

//		LoArB6 = multi(CurrentLoArOrientation, LoArB5);

//		u.Set(1 , 0 , 0);
//		CurrentLoArOrientation= RVector(u, -(float)3.1415 /2 );

//		LoArB7 = multi(CurrentLoArOrientation, LoArB6);


//		u.Set(0 , 0 , 1);
//		CurrentLoArOrientation= RVector(u, (float)3.1415);

//		LoArOrientation = multi(CurrentLoArOrientation, LoArB7);



//		////////////////// setting to Final Body orientation upper arm///////////////////////////////
//		Vector3 u2 = new Vector3(UpArB3[0,0] , UpArB3[1,0] , UpArB3[2,0]);
//		CurrentLoArOrientation= RVector(u2, -(float)3.1415/2);

//		UpArB4 = multi(CurrentLoArOrientation, UpArB3);

//		u2.Set (UpArB4[0,1] , UpArB4[1,1] , UpArB4[2,1]);
//		CurrentLoArOrientation= RVector(u2, (float)3.1415 );

//		UpArB5 = multi(CurrentLoArOrientation, UpArB4);

//		u2.Set(1 , 0 ,0);
//		CurrentLoArOrientation= RVector(u2, -(float)3.1415/2 );

//		UpArB6 = multi(CurrentLoArOrientation, UpArB5);


//		u2.Set(0 , 0 ,1);
//		CurrentLoArOrientation= RVector(u2, (float)3.1415 );

//		UpArOrientation = multi(CurrentLoArOrientation, UpArB6);

//	}

    
//	public override void UpdateJoint () 
//	{
//		for (int ndx = 0; ndx < mNodSensors.Length; ndx++)
//		{
//			mNodSensors [ndx].UpdateSensor ();
//		}
			
//				Vector3 vNodRawEuler1 = mNodSensors[0].curRotationRawEuler;
//				Vector3 vNodRawEuler2 = mNodSensors[1].curRotationRawEuler;
//				vNodRawEuler1 = new Vector3(vNodRawEuler1.x , vNodRawEuler1.y , vNodRawEuler1.z );
//				vNodRawEuler2 = new Vector3(vNodRawEuler2.x , vNodRawEuler2.y , vNodRawEuler2.z );

//				Vector3 NodIniEuler1 = mNodSensors[0].initRotationEuler;
//				Vector3 NodIniEuler2 = mNodSensors[1].initRotationEuler;
//				NodIniEuler1 = new Vector3(NodIniEuler1.x , NodIniEuler1.y , NodIniEuler1.z );
//				NodIniEuler2 = new Vector3(NodIniEuler2.x , NodIniEuler2.y , NodIniEuler2.z );

//				// getting the current Torso orientation for shoulder angle extraction
//				float[,] vaTorsoOrientation = new float[3, 3];
//				vaTorsoOrientation =  NodContainer.GetTorsoOrientation();   
				
//				// call ArmOrientation function to calculate current orientation of the Torso joint
//				ArmOrientation (vNodRawEuler1, NodIniEuler1 , vNodRawEuler2 ,  NodIniEuler2);

//				// convert upper arm and forearm orientation from 3*3 matrix to quaternion
//				NodQuaternionOrientation vNodRawQuat = MatToQuat(UpArOrientation);
//				Quaternion vNodQuat = new Quaternion(vNodRawQuat.x, vNodRawQuat.y, vNodRawQuat.z, vNodRawQuat.w);
//				Quaternion vJointQuat = inverseInitRotation * vNodQuat * Quaternion.Inverse(Quaternion.Euler(quaternionFactor));
				
//				NodQuaternionOrientation vNodRawQuat2 = MatToQuat(LoArOrientation);
//				Quaternion vNodQuat2 = new Quaternion(vNodRawQuat2.x, vNodRawQuat2.y, vNodRawQuat2.z, vNodRawQuat2.w);
//				Quaternion vJointQuat2 = inverseInitRotation * vNodQuat2 * Quaternion.Inverse(Quaternion.Euler(quaternionFactor));

//				if(jointTransform != null)
//				{
//				jointTransform.rotation = vJointQuat;
//				jointTransform2.rotation = vJointQuat2;
//				}
			

//	}

//}