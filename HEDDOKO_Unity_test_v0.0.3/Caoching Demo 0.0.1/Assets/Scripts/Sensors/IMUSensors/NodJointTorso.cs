using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

///// joint code for the Torso Orientation
public class JointTorso : NodJoint 
{


	// current Torso orientation for will be stored in this variable 
	public float [,] vaTorsoOrientation = new float[3,3];
	public float [,] vaSpinOrientation = new float[3,3];
	// Torso Extracted Angles, Angular velocities  and Angular accelerations, The names are choosed based on the human body angles document
	public float vAngleTorsoFlection = 0;
	public float vAngularVelocityTorsoFlection = 0;
	public float vAngularAccelerationTorsoFlection = 0;

	public float vAngleTorsoLateral = 0;
	public float vAngularVelocityTorsoLateral = 0;
	public float vAngularAccelerationTorsoLateral = 0;

	public float vAngleTorsoRotation = 0;
	public float vAngularVelocityTorsoRotation = 0;
	public float vAngularAccelerationTorsoRotation = 0;

	public float vAngleTorsoVertical = 0;
	public float vAngularVelocityTorsoVertical = 0;
	public float vAngularAccelerationTorsoVertical = 0;

	public float vNumberOfTurns = 0;
	public float vNumberOfFlips = 0;
	public float vAngleIntegrationTurns = 0;
	public float vAngleIntegrationFlips = 0;

	// This variable stores the time of curretn frame. It is used for angular velocity and acceleration extraction
	public float vTime= 0 ;

	//angles buffer save data for turn detection
	public List<float> rot_angles = new List<float> ();
	public List<int> rot_angles2 = new List<int> ();

	//	
	//	 ReturnTorsoOrientation()
	//	* @ This returns the current orientation of the Torso, The curretn Torso Orientation is necessary for angle extraction of shoulders and hips
	//	* @return void float[,] vaTorsoOrientation
	//	*/// 
	public override float[,] ReturnTorsoOrientation()
	{

		return vaTorsoOrientation;
	}


		/**
	* TorsoOrientation()
	*	@This Fuction Provides The compensated Update for the Torso Orientation
	*	@param Vector3 NodTorso, Torso Euler Angles Inputs
	*	@paramVector3  InitNodTorso, this is the information of the initial frame for Torso joint
	*	@return void
	*/
	public float[,] TorsoOrientation(Vector3 NodTorso , Vector3 InitNodTorso)
	{
		//Intermediate arrays until achive final orienation for Torso, they are Taged with F ( Global to local rotation) and B (Local to Global rotation) and are numbered consecutively

		float[,] TorsoF1 = new float[3, 3];
		float[,] TorsoFi = new float[3, 3];

		float[,] TorsoB1 = new float[3, 3];
		float[,] TorsoBi = new float[3, 3];

		float[,] vaTorsoOrientationLocal = new float[3, 3];

		/////////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
		/////////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////


		/////////// Converting to orientation matrix ///////////////////

		TorsoB1 = RotationLocal (NodTorso.z, NodTorso.x, NodTorso.y);
		TorsoF1 = RotationGlobal (NodTorso.z, NodTorso.x, NodTorso.y);

		TorsoBi = RotationLocal (InitNodTorso.z, InitNodTorso.x, InitNodTorso.y);
		TorsoFi = RotationGlobal (InitNodTorso.z, InitNodTorso.x, InitNodTorso.y);

		/////////// Initial Frame Adjustments ///////////////////


		vaTorsoOrientationLocal = multi (TorsoFi, TorsoB1);
		return vaTorsoOrientationLocal;

	}



	//	/************
	//* TorsoAngles()
	//*	@This Fuction Calculates the important angles of the Torso and updates them
	//*	@param float [,] vaTorsoOrientation, current orientation of the Torso
	//*	@return void
	//*/

	public void TorsoAngles(float [,] vaTorsoOrientation)
	{
		///// calculate the time difference since last call
		float vTimeDifference = Time.time - vTime;
	//	Debug.Log ("current time" +Time.time);
		if (vTimeDifference == 0) {
			return;
		}

		vTime = Time.time;


		/// 	///	/////////////////////////////////////////////////////  Angle extraction /////////////////////////////////////////////////////////////////////
		/// 	///	/////////////////////////////////////////////////////  Angle extraction /////////////////////////////////////////////////////////////////////
		/// 	///	/////////////////////////////////////////////////////  Angle extraction /////////////////////////////////////////////////////////////////////
		/// 	///	/////////////////////////////////////////////////////  Angle extraction /////////////////////////////////////////////////////////////////////
		/// 	///	/////////////////////////////////////////////////////  Angle extraction /////////////////////////////////////////////////////////////////////
		/// 	///	/////////////////////////////////////////////////////  Angle extraction /////////////////////////////////////////////////////////////////////
		/// 	///	/////////////////////////////////////////////////////  Angle extraction /////////////////////////////////////////////////////////////////////

		//////////////// calculate the Torso Flection angle ////////////////////////////////////////

		// Axes 1 to 4 are intermediate variables used to calculate angles. 
		//	with appropriate matrix calculations each angle and angular velocities are calculated in the first step
		// In the second step the sign of these angles will be determined and the angles will be updated


		/// step 1///
		Vector3 vAxis1 = new Vector3 (vaTorsoOrientation [0, 1], vaTorsoOrientation [1, 1], vaTorsoOrientation [2, 1]);
		Vector3 vAxis2 = new Vector3 (vaTorsoOrientation [0, 0], vaTorsoOrientation [1, 0], vaTorsoOrientation [2, 0]);
		Vector3 vAxis3 = new Vector3 (0, 1, 0);
		Vector3 vAxis4 = Vector3.Cross (vAxis2, vAxis3);
		vAxis4.Normalize ();
		vAxis3 = vAxis1 - (Vector3.Dot (vAxis1, vAxis4)) * vAxis4;
		vAxis3.Normalize ();
		vAxis2.Set (0, 1, 0);
		float vAngleTorsoFlectionNew = Vector3.Angle (vAxis3, vAxis2);
		vAxis1.Set (vaTorsoOrientation [0, 0], 0, vaTorsoOrientation [2, 0]);
		float vAngularVelocityTorsoFlectionNew = (vAngleTorsoFlectionNew - Mathf.Abs (vAngleTorsoFlection)) / vTimeDifference;

		///step 2///
		if ((Vector3.Dot (vAxis3, vAxis1) * Vector3.Dot (vAxis3, vAxis2)) > 0) {
			vAngleTorsoFlectionNew = -vAngleTorsoFlectionNew;
			vAngularVelocityTorsoFlectionNew = -vAngularVelocityTorsoFlectionNew;
		}

		vAngularAccelerationTorsoFlection = (vAngularVelocityTorsoFlectionNew - vAngularVelocityTorsoFlection) / vTimeDifference;
		vAngularVelocityTorsoFlection = vAngularVelocityTorsoFlectionNew;
		vAngleTorsoFlection = vAngleTorsoFlectionNew;


		//Debug.Log ("Torso Angles" + vAngleTorsoFlection + ", and, " + vAngularVelocityTorsoFlection + ", and, " + vAngularAccelerationTorsoFlection);

	
		/////////////////// calculate the Torso lateral angle ////////////////////////////////////////

		// step 1///
		vAxis1.Set (vaTorsoOrientation [0, 1], vaTorsoOrientation [1, 1], vaTorsoOrientation [2, 1]);
		vAxis2.Set (vaTorsoOrientation [0, 2], vaTorsoOrientation [1, 2], vaTorsoOrientation [2, 2]);
		vAxis3.Set (0, 1, 0);
		vAxis4 = Vector3.Cross (vAxis2, vAxis3);
		vAxis4.Normalize ();
		vAxis3 = vAxis1 - (Vector3.Dot (vAxis1, vAxis4)) * vAxis4;
		vAxis3.Normalize ();
		vAxis2.Set (0, 1, 0);
		float vAngleTorsoLateralNew = Vector3.Angle (vAxis3, vAxis2);
		float vAngularVelocityTorsoLateralNew = (vAngleTorsoLateralNew - Mathf.Abs (vAngleTorsoLateral)) / vTimeDifference;
		vAxis1.Set (vaTorsoOrientation [0, 2], 0, vaTorsoOrientation [2, 2]);


		///step 2///
		if ((Vector3.Dot (vAxis3, vAxis1) * Vector3.Dot (vAxis3, vAxis2)) < 0) {
			vAngleTorsoLateralNew = -vAngleTorsoLateralNew;
			vAngularVelocityTorsoLateralNew = -vAngularVelocityTorsoLateralNew;
		}

		vAngularAccelerationTorsoLateral = (vAngularVelocityTorsoLateralNew - vAngularVelocityTorsoLateral) / vTimeDifference;
		vAngularVelocityTorsoLateral = vAngularVelocityTorsoLateralNew;
		vAngleTorsoLateral = vAngleTorsoLateralNew;

		//Debug.Log ("Torso Angles" + vAngleTorsoLateral + ", and, " + vAngularVelocityTorsoLateral + ", and, " + vAngularAccelerationTorsoLateral);



		/////////////////// calculate the Torso Rotational angle ////////////////////////////////////////

		///step 1///
		vAxis1.Set (vaTorsoOrientation [0, 2], 0, vaTorsoOrientation [2, 2]);
		vAxis2.Set (0, 0, 1);
		float vAngleTorsoRotationNew = Vector3.Angle (vAxis1, vAxis2);

		float vAngularVelocityTorsoRotationNew = (vAngleTorsoRotationNew - Mathf.Abs (vAngleTorsoRotation)) / vTimeDifference;


		///step 2///
		if (vaTorsoOrientation [0, 2] < 0) 
		{
			vAngleTorsoRotationNew = -vAngleTorsoRotationNew;
			vAngularVelocityTorsoRotationNew = -vAngularVelocityTorsoRotationNew;
		}
	




//		//////////////////////Turn detection/////////////////////
		if (Math.Abs (vAngleTorsoRotationNew) < 3) 
		{
			vAngleIntegrationTurns = 0;
		} 
		else 
		{
			vAngleIntegrationTurns += (vAngularVelocityTorsoRotationNew * vTimeDifference);
		}

		//Debug.Log ("Angle" +vAngularVelocityTorsoRotationNew  +"vAngleIntegrationTurns" + vAngleIntegrationTurns + "Angular Velocity" + vAngularVelocityTorsoRotation );

		if ( Math.Abs(vAngleIntegrationTurns) > 330 ) 
		{

			vAngleIntegrationTurns = 0;
			vNumberOfTurns++; 

		}



		//		//////////////////////Turn detection///////////////////// Javad
//		if (Math.Abs (vAngleTorsoRotationNew) < 30) 
//		{
//			vAngleIntegrationTurns = 0;
//		} 
//		else 
//		{
//			vAngleIntegrationTurns += (vAngularVelocityTorsoRotationNew * vTimeDifference);;
//		}
//
//
//		if ((vAngleTorsoRotationNew * vAngleTorsoRotation < 0) && (Math.Abs(vAngleIntegrationTurns) > 90) ) 
//		{
//
//			vNumberOfTurns++; 
//			vAngleIntegrationTurns = 0;
//
//		}
//
		//		//////////////////End of turn detection/////////////////// Javad


		//writing data to a file for processing 

		TextWriter tw1 = new StreamWriter("AnglularVelocity.txt",true);

		// write lines of text to the file
		tw1.WriteLine(vAngularVelocityTorsoRotationNew);

		// close the stream     
		tw1.Close();


		TextWriter tw2 = new StreamWriter("AngleIntegratationSum.txt",true);
		tw2.WriteLine(vAngleIntegrationTurns);   
		tw2.Close();

		float Ang = vAngularVelocityTorsoRotationNew * vTimeDifference;
//		int Ang2= (int)Ang ;
		TextWriter tw3 = new StreamWriter("AngleIntegratation.txt",true);
		tw3.WriteLine(Ang);   
		tw3.Close();

		TextWriter tw4 = new StreamWriter("Angle.txt",true);
		tw4.WriteLine(vAngleTorsoRotationNew);   
		tw4.Close();

		TextWriter tw5 = new StreamWriter("NumberOfTurns.txt",true);
		tw5.WriteLine(vNumberOfTurns);   
		tw5.Close();
//		rot_angles2.Add (Ang2);
//		//to check
//		int AngleDif;
//


//		for (int ii = 0; ii < rot_angles2.Count; ii++) 
//		{
//			AngleDif = 360 - Math.Abs(rot_angles2 [ii]);
//			int result = rot_angles2.Find(item => item==AngleDif);
//
////			if (result = null) {
////				Boolean r = true;
////			} else {
////				Boolean r = false;
////			}
////
////			Debug.Log ("Angle" + rot_angles2 [ii] + "    " + "counter" + vNumberOfTurns + "    " + ii);
//			Debug.Log ("Angle" + result+ "    " + AngleDif + "    Ang"+ Ang);
//
//

//			int jj;
//			jj = ii;
//			for (int jj = ii; jj < rot_angles.Count; jj++) 
//			{
//				while (jj < ii+10) //turn time threshhold will be 100 frames ::to be adjusted later
//				{
//					AngleDif = rot_angles [ii] - rot_angles [jj];
//
//					if (AngleDif == 360)
//					{
//						vNumberOfTurns++; 
//					}
//			jj++;
//					Debug.Log ("Angle" + rot_angles [ii] + "    " + "counter" + vNumberOfTurns + "    " + ii);
//				}
//			}
//		}


		//	Debug.Log ("number of turns: " + vNumberOfTurns +"amount:  " + rot_angles [ii] - rot_angles [jj] );
//		//////////////////End of turn detection///////////////////


		vAngularAccelerationTorsoRotation = (vAngularVelocityTorsoRotationNew - vAngularVelocityTorsoRotation) / vTimeDifference;
		vAngularVelocityTorsoRotation = vAngularVelocityTorsoRotationNew;
		vAngleTorsoRotation = vAngleTorsoRotationNew;

	//	Debug.Log ("Torso Angles" + vAngleTorsoRotation + ", and, " + vAngularVelocityTorsoRotation + ", and, " + vAngularAccelerationTorsoRotation);


		/////////////////// calculate the Torso Vertical angle ////////////////////////////////////////

		///step 1///
		vAxis1.Set(vaTorsoOrientation[0,1], vaTorsoOrientation[1,1], vaTorsoOrientation[2,1]);
		vAxis2.Set(0,1,0);
		float vAngleTorsoVerticalNew = Vector3.Angle (vAxis1, vAxis2);
		float vAngularVelocityTorsoVerticalNew = (vAngleTorsoVerticalNew - Mathf.Abs(vAngleTorsoVertical)) / vTimeDifference;



		if (vaTorsoOrientation [1, 0] < 0) 
		{
			vAngleTorsoVerticalNew = -vAngleTorsoVerticalNew;
			vAngularVelocityTorsoVerticalNew = -vAngularVelocityTorsoVerticalNew;
		}




//		//		//////////////////////Flip detection///////////////////// 
		if (Math.Abs (vAngleTorsoVerticalNew) < 3) 
		{
			vAngleIntegrationFlips = 0;
		} 
		else 
		{
			vAngleIntegrationFlips += (vAngularVelocityTorsoVerticalNew * vTimeDifference);
		}
		if (Math.Abs(vAngleIntegrationFlips) > 330) 
		{

				vNumberOfFlips++; 
				vAngleIntegrationFlips = 0;
		
			}
		
		//
		//		//////////////////End of Flip detection///////////////////


//		//////////////////////Flip detection///////////////////// Javad
//		if (Math.Abs (vAngleTorsoVerticalNew) < 30) 
//		{
//			vAngleIntegrationFlips = 0;
//		} 
//		else 
//		{
//			vAngleIntegrationFlips += (vAngularVelocityTorsoVerticalNew * vTimeDifference);
//		}
//
//
//		if ((vAngleTorsoVerticalNew * vAngleTorsoVertical < 0) && (Math.Abs(vAngleIntegrationFlips) > 90) ) 
//		{
//
//			vNumberOfFlips++; 
//			vAngleIntegrationFlips = 0;
//
//		}
//
//		//////////////////End of Flip detection///////////////////

		///step 2///
		vAngularAccelerationTorsoVertical = (vAngularVelocityTorsoVerticalNew - vAngularVelocityTorsoVertical) / vTimeDifference;
		vAngularVelocityTorsoVertical = vAngularVelocityTorsoVerticalNew;
		vAngleTorsoVertical = vAngleTorsoVerticalNew;

		//Debug.Log ("Torso Angles" + vAngleTorsoVertical + ", and, " + vAngularVelocityTorsoVertical + ", and, " + vAngularAccelerationTorsoVertical);

	}

/// <summary>
// To Show appropriate Angles for each joint in GUI
/// <summary>
	public override void OnGUIAngles()
	{

//		/////////////////////// Flip and turn Presentation ///////////////////////////////
//
		GUI.contentColor = Color.black;
		GUI.Label (new Rect (1200, 10, 100 , 25), "Number of Turns :  " + vNumberOfTurns , "color");
		GUI.Label (new Rect (1000, 10, 100 , 25), "Turn Magnitude :  " + vAngleIntegrationTurns.ToString("0") , "color");
		GUI.Label (new Rect (1200, 60, 100 , 25), "Number of Flips :  " + vNumberOfFlips  , "color");
		GUI.Label (new Rect (1000, 60, 100 , 25), " Flip Magnitude :  " + vAngleIntegrationFlips.ToString("0")  , "color");
//
////		///////////////// turn presentation ///////////////////
//
//
//
		var progressBarEmpty = new Texture2D(0,0);
		var progressBarFull = new Texture2D(0,0);
		GUI.BeginGroup (new Rect (1000, 30, 400, 20));
		GUI.Box (new Rect (0,0,  400, 20),progressBarEmpty);
		
				// draw the filled-in part:
		if (vAngleTorsoRotation >= 0)
		{	
			GUI.BeginGroup (new Rect (200 , 0, vAngleTorsoRotation * 200 / 180, 20f));
		} 
		else 
		{
			GUI.BeginGroup (new Rect ( (180 + vAngleTorsoRotation) * 200 / 180, 0, Math.Abs(vAngleTorsoRotation) * 10 / 9, 20f));
		}
				GUI.Box (new Rect (0,0,  400, 20),progressBarFull);
				GUI.EndGroup ();
		
				GUI.EndGroup ();
//
////		///////////// flip presentation /////////////////
		GUI.BeginGroup (new Rect (1380, 60, 20, 300));
		GUI.Box (new Rect (0, 0, 20, 300),progressBarEmpty);

		// draw the filled-in part:

		GUI.BeginGroup (new Rect ( 0 , 0 , 20, vAngleTorsoVertical * 300/180));

		GUI.Box (new Rect (0,0,  20, 300),progressBarFull);
		GUI.EndGroup ();

		GUI.EndGroup ();
//

		///////// Displaying the torso angles ////////////

		if (NodContainer.vKey == 1)
		{
			GUI.contentColor = Color.black;

			GUI.Label (new Rect (880, 450, 400 , 25), "Thoracolumbar Flexion/Extension: " + vAngleTorsoFlection , "color");

			GUI.Label (new Rect (880, 480, 400 , 25), "Thoracolumbar Lateral Flexion: " + vAngleTorsoLateral , "color");

			GUI.Label (new Rect (880, 510, 400 , 25), "Thoracolumbar Rotation: " + vAngleTorsoRotation, "color");

			GUI.Label (new Rect (1180, 450, 400, 25), "Thoracolumbar Vertical: " + vAngleTorsoVertical , "color");
		}

	}



	public override void ResetJoint ()
	{
	
		base.ResetJoint ();
		vNumberOfTurns = 0;
		vNumberOfFlips = 0;
		vAngleIntegrationTurns = 0;
		vAngleIntegrationFlips = 0;

	}



	public override void UpdateJoint () 
	{
		for (int ndx = 0; ndx < mNodSensors.Length; ndx++)
		{
			mNodSensors [ndx].UpdateSensor ();
		}

		Vector3 vNodRawEuler1 = mNodSensors[0].curRotationRawEuler;
		vNodRawEuler1 = new Vector3(vNodRawEuler1.x , vNodRawEuler1.y , vNodRawEuler1.z );
		Vector3 NodIniEuler1 = mNodSensors[0].initRotationEuler;
		NodIniEuler1 = new Vector3(NodIniEuler1.x , NodIniEuler1.y , NodIniEuler1.z );


		Vector3 vNodRawEuler2 = mNodSensors[1].curRotationRawEuler;
		Vector3 NodIniEuler2 = mNodSensors[1].initRotationEuler;


		// call TorsoOrientation function to calculate current orientation of the Torso joint
		vaTorsoOrientation=TorsoOrientation (vNodRawEuler1, NodIniEuler1);
		vaSpinOrientation=TorsoOrientation (vNodRawEuler2, NodIniEuler2);

		// call TorsoAngles function to calculate current angles of Torso
		TorsoAngles(vaTorsoOrientation);

		// convert the orientation matrices to quatrenion to be used in unity model
		NodQuaternionOrientation vNodRawQuat = MatToQuat(vaTorsoOrientation);
		NodQuaternionOrientation vNodRawQuat2 = MatToQuat(vaSpinOrientation);

		Quaternion vNodQuat = new Quaternion(vNodRawQuat.x, vNodRawQuat.y, vNodRawQuat.z, vNodRawQuat.w);
		Quaternion vJointQuat = inverseInitRotation * vNodQuat * Quaternion.Inverse(Quaternion.Euler(quaternionFactor));

		Quaternion vNodQuat2 = new Quaternion(vNodRawQuat2.x, vNodRawQuat2.y, vNodRawQuat2.z, vNodRawQuat2.w);
		Quaternion vJointQuat2 = inverseInitRotation * vNodQuat2 * Quaternion.Inverse(Quaternion.Euler(quaternionFactor));

		if(jointTransform != null)
		{
			jointTransform.rotation = vJointQuat;
			jointTransform2.rotation = vJointQuat2;
		}


	}

}
