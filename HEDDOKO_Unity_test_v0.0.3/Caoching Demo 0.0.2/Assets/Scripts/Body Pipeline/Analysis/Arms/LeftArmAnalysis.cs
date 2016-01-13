/** 
* @file LeftArmAnalysis.cs
* @brief LeftArmAnalysis the Joint class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;
using System;
namespace Assets.Scripts.Body_Pipeline.Analysis.Arms
{
    /**
    * LeftArmAnalysis class 
    * @brief LeftArmAnalysis class 
    */
    [Serializable]
    internal class LeftArmAnalysis : ArmAnalysis
    {
        // Left Arm Extracted Angles, Angular velocities  and Angular accelerations, The names are chose based on the human body angles document
        public float mAngleLeftElbowFlexion = 0;
        public float mAngularVelocityLeftElbowFlexion = 0;
        public float mAngularAccelerationLeftElbowFlexion = 0;

        public float mAngleLeftElbowPronation = 0;
        public float mAngularVelocityLeftElbowPronation = 0;
        public float mAngularAccelerationLeftElbowPronation = 0;


        public float mAngleLeftShoulderFlexion = 0;
        public float mAngularVelocityLeftShoulderFlexion = 0;
        public float mAngularAccelerationLeftShoulderFlexion = 0;

        public float mAngleLeftShoulderAbduction = 0;
        public float mAngularVelocityLeftShoulderAbduction = 0;
        public float mAngularAccelerationLeftShoulderAbduction = 0;

        public float mAngleLeftShoulderRotation = 0;
        public float mAngularVelocityLeftShoulderRotation = 0;
        public float mAngularAccelerationLeftShoulderRotation = 0;

        /// <summary>
        /// Extract angles from orientations
        /// </summary>
        public override void AngleExtraction()
        {

            /*///// calculate the time difference since last call
            float vTimeDifference = Time.time - mLastTimeCalled;
            if (vTimeDifference == 0)
            {
                return;
            }

            mLastTimeCalled = Time.time;

            /// 	///	/////////////////////////////////////////////////////  Angle extraction /////////////////////////////////////////////////////////////////////
            /// 	///	/////////////////////////////////////////////////////  Angle extraction /////////////////////////////////////////////////////////////////////
            /// 	///	/////////////////////////////////////////////////////  Angle extraction /////////////////////////////////////////////////////////////////////
            /// 	///	/////////////////////////////////////////////////////  Angle extraction /////////////////////////////////////////////////////////////////////
            /// 	///	/////////////////////////////////////////////////////  Angle extraction /////////////////////////////////////////////////////////////////////
            /// 	///	/////////////////////////////////////////////////////  Angle extraction /////////////////////////////////////////////////////////////////////
            /// 	///	/////////////////////////////////////////////////////  Angle extraction /////////////////////////////////////////////////////////////////////


            // Axes 1 to 4 are intermediate variables used to calculate angles. 
            // In the first step, with appropriate matrix calculations each angle and angular velocities are calculated
            // In the second step, the sign of these angles will be determined and the angles will be updated


            //////////////// calculate the Elbow Flection angle ////////////////////////////////////////

            /// step 1///
            Vector3 vAxis1 = new Vector3(UpArOrientation[0, 2], UpArOrientation[1, 2], UpArOrientation[2, 2]);
            Vector3 vAxis2 = new Vector3(LoArOrientation[0, 2], LoArOrientation[1, 2], LoArOrientation[2, 2]);
            float vAngleLeftElbowFlexionNew = Vector3.Angle(vAxis1, vAxis2); 
            float vAngularVelocityLeftElbowFlexionNew = (vAngleLeftElbowFlexionNew - mAngleLeftElbowFlexion) / vTimeDifference;
          

            /// step 2///
            mAngularAccelerationLeftElbowFlexion = (vAngularVelocityLeftElbowFlexionNew - mAngularVelocityLeftElbowFlexion) / vTimeDifference;
            mAngularVelocityLeftElbowFlexion = vAngularVelocityLeftElbowFlexionNew;
            mAngleLeftElbowFlexion = vAngleLeftElbowFlexionNew;

            //////////////// calculate the Elbow Pronation angle ////////////////////////////////////////

            /// step 1///
            vAxis1.Set(UpArOrientation[0, 1], UpArOrientation[1, 1], UpArOrientation[2, 1]);
            vAxis2.Set(LoArOrientation[0, 1], LoArOrientation[1, 1], LoArOrientation[2, 1]);
            Vector3 vAxis3 = new Vector3(UpArOrientation[0, 0], UpArOrientation[1, 0], UpArOrientation[2, 0]);
            float vAngleLeftElbowPronationNew = Vector3.Angle(vAxis1, vAxis2);
            float vAngularVelocityLeftElbowPronationNew = (vAngleLeftElbowPronationNew - Mathf.Abs(mAngleLeftElbowFlexion)) / vTimeDifference;

            /// step 2///
            if (Vector3.Dot(vAxis2, vAxis3) > 0)
            {
                vAngleLeftElbowPronationNew = -vAngleLeftElbowPronationNew;
                vAngularVelocityLeftElbowPronationNew = -vAngularVelocityLeftElbowPronationNew;
            }
            mAngularAccelerationLeftElbowPronation = (vAngularVelocityLeftElbowPronationNew - mAngularVelocityLeftElbowPronation) / vTimeDifference;
            mAngularVelocityLeftElbowPronation = vAngularVelocityLeftElbowPronationNew;
            mAngleLeftElbowFlexion = vAngleLeftElbowPronationNew;

            //////////////// calculate the Left Shoulder Flection angle ////////////////////////////////////////

            /// step 1///
            vAxis1.Set(-UpArOrientation[0, 2], -UpArOrientation[1, 2], -UpArOrientation[2, 2]);
            vAxis2.Set(TorsoOrientation[0, 2], TorsoOrientation[1, 2], TorsoOrientation[2, 2]);
            vAxis3 = vAxis1 - (Vector3.Dot(vAxis1, vAxis2)) * vAxis2;
            vAxis3.Normalize();
            vAxis2.Set(TorsoOrientation[0, 1], TorsoOrientation[1, 1], TorsoOrientation[2, 1]);
            vAxis1.Set(TorsoOrientation[0, 0], TorsoOrientation[1, 0], TorsoOrientation[2, 0]);
            float vAngleLeftShoulderFlexionNew = 180 - Vector3.Angle(vAxis3, vAxis2);
            float vAngularVelocityLeftShoulderFlexionNew = (vAngleLeftShoulderFlexionNew - Mathf.Abs(mAngleLeftShoulderFlexion)) / vTimeDifference;

            /// step 2///
            if (Vector3.Dot(vAxis1, vAxis3) > 0)
            {
                vAngleLeftShoulderFlexionNew = -vAngleLeftShoulderFlexionNew;
                vAngularVelocityLeftShoulderFlexionNew = -vAngularVelocityLeftShoulderFlexionNew;
            }

            mAngularAccelerationLeftShoulderFlexion = (vAngularVelocityLeftShoulderFlexionNew - mAngularVelocityLeftShoulderFlexion) / vTimeDifference;
            mAngularVelocityLeftShoulderFlexion = vAngularVelocityLeftShoulderFlexionNew;
            mAngleLeftShoulderFlexion = vAngleLeftShoulderFlexionNew;

            //////////////// calculate the Left Shoulder Abduction angle ////////////////////////////////////////

            // ====================== step 1 ======================//
            vAxis1.Set(UpArOrientation[0, 2], UpArOrientation[1, 2], UpArOrientation[2, 2]);
            vAxis2.Set(TorsoOrientation[0, 0], TorsoOrientation[1, 0], TorsoOrientation[2, 0]);
            vAxis3 = vAxis1 - (Vector3.Dot(vAxis1, vAxis2)) * vAxis2;
            vAxis3.Normalize();
            vAxis2.Set(TorsoOrientation[0, 1], TorsoOrientation[1, 1], TorsoOrientation[2, 1]);
            vAxis1.Set(TorsoOrientation[0, 2], TorsoOrientation[1, 2], TorsoOrientation[2, 2]);
            float vAngleLeftShoulderAbductionNew = Vector3.Angle(vAxis3, vAxis2);
            float vAngularVelocityLeftShoulderAbductionNew = (vAngleLeftShoulderAbductionNew - Mathf.Abs(mAngleLeftShoulderAbduction)) / vTimeDifference;


            //====================== step 2 ======================//
            if (Vector3.Dot(vAxis1, vAxis3) < 0)
            {
                vAngleLeftShoulderAbductionNew = -vAngleLeftShoulderAbductionNew;
                vAngularVelocityLeftShoulderAbductionNew = -vAngularVelocityLeftShoulderAbductionNew;
            }

            mAngularAccelerationLeftShoulderAbduction = (vAngularVelocityLeftShoulderAbductionNew - mAngularVelocityLeftShoulderAbduction) / vTimeDifference;
            mAngularVelocityLeftShoulderAbduction = vAngularVelocityLeftShoulderAbductionNew;
            mAngleLeftShoulderAbduction = vAngleLeftShoulderAbductionNew;

            //================================calculate the Left Shoulder Rotation angle===========================================//

            /// step 1///
            vAxis1.Set(UpArOrientation[0, 2], UpArOrientation[1, 2], UpArOrientation[2, 2]);
            vAxis2.Set(TorsoOrientation[0, 1], TorsoOrientation[1, 1], TorsoOrientation[2, 1]);
            vAxis3 = Vector3.Cross(vAxis2, vAxis1);
            vAxis3.Normalize();
            vAxis2.Set(UpArOrientation[0, 0], UpArOrientation[1, 0], UpArOrientation[2, 0]);
            vAxis1.Set(UpArOrientation[0, 1], UpArOrientation[1, 1], UpArOrientation[2, 1]);
            float vAngleLeftShoulderRotationNew = Vector3.Angle(vAxis3, vAxis2);
            float vAngularVelocityLeftShoulderRotationNew = (vAngleLeftShoulderRotationNew - Mathf.Abs(mAngleLeftShoulderRotation)) / vTimeDifference;

            // step 2 //
            if (Vector3.Dot(vAxis1, vAxis3) > 0)
            {
                vAngleLeftShoulderRotationNew = -vAngleLeftShoulderRotationNew;
                vAngularVelocityLeftShoulderRotationNew = -vAngularVelocityLeftShoulderRotationNew;
            }

            mAngularAccelerationLeftShoulderRotation = (vAngularVelocityLeftShoulderRotationNew - mAngularVelocityLeftShoulderRotation) / vTimeDifference;
            mAngularVelocityLeftShoulderRotation = vAngularVelocityLeftShoulderRotationNew;
            mAngleLeftShoulderRotation = vAngleLeftShoulderRotationNew;//*/
        }
    }
}
