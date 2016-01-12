
/** 
* @file RightArmAnalysis.cs
* @brief RightArmAnalysis the Joint class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis.Arms
{
    /**
    * RightArmAnalysis class 
    * @brief RightArmAnalysis class 
    */
    public class RightArmAnalysis: ArmAnalysis
    {
        // Right Arm Extracted Angles, Angular velocities  and Angular accelerations, The names are choosed based on the human body angles document
        public float mAngleRightElbowFlexion = 0;
        public float mAngularVelocityRightElbowFlexion = 0;
        public float mAngularAccelerationRightElbowFlexion = 0;

        public float mAngleRightElbowPronation = 0;
        public float mAngularVelocityRightElbowPronation = 0;
        public float mAngularAccelerationRightElbowPronation = 0;

        public float mAngleRightShoulderFlexion = 0;
        public float mAngularVelocityRightShoulderFlexion = 0;
        public float mvAngularAccelerationRightShoulderFlexion = 0;


        public float mAngleRightShoulderAbduction = 0;
        public float mAngularVelocityRightShoulderAbduction = 0;
        public float mAngularAccelerationRightShoulderAbduction = 0;

        public float mAngleRightShoulderRotation = 0;
        public float mAngularVelocityRightShoulderRotation = 0;
        public float mAngularAccelerationRightShoulderRotation = 0;

        /// <summary>
        /// Extract angles from orientations
        /// </summary>
        public override void AngleExtraction()
        {
            float vDeltaTime = Time.time - mLastTimeCalled;
            if (vDeltaTime == 0)
            {
                return;
            }
            mLastTimeCalled = Time.time;

            // ============================================== Angle extraction ==============================================// 

            // Axes 1 to 4 are intermediate variables used to calculate angles. 
            // In the first step, with appropriate matrix calculations each angle and angular velocities are calculated
            // In the second step, the sign of these angles will be determined and the angles will be updated


            //////////////// calculate the Elbow Flection angle ////////////////////////////////////////
            /// step 1///
            Vector3 vAxis1 = new Vector3(UpArOrientation[0, 2], UpArOrientation[1, 2], UpArOrientation[2, 2]);
            Vector3 vAxis2 = new Vector3(LoArOrientation[0, 2], LoArOrientation[1, 2], LoArOrientation[2, 2]);
            float vAngleRightElbowFlexionNew = Vector3.Angle(vAxis1, vAxis2);
            float vAngularVelocityRightElbowFlexionNew = (vAngleRightElbowFlexionNew - mAngleRightElbowFlexion) / vDeltaTime;

            /// step 2///
            mAngularAccelerationRightElbowFlexion = (vAngularVelocityRightElbowFlexionNew - mAngularVelocityRightElbowFlexion) / vDeltaTime;
            mAngularVelocityRightElbowFlexion = vAngularVelocityRightElbowFlexionNew;
            mAngleRightElbowFlexion = vAngleRightElbowFlexionNew;

            //////////////// calculate the Elbow Pronation angle ////////////////////////////////////////
            /// step 1///
            vAxis1.Set(UpArOrientation[0, 1], UpArOrientation[1, 1], UpArOrientation[2, 1]);
            vAxis2.Set(LoArOrientation[0, 1], LoArOrientation[1, 1], LoArOrientation[2, 1]);
            Vector3 vAxis3 = new Vector3(UpArOrientation[0, 0], UpArOrientation[1, 0], UpArOrientation[2, 0]);
            float vAngleRightElbowPronationNew = Vector3.Angle(vAxis1, vAxis2);
            float vAngularVelocityRightElbowPronationNew = (vAngleRightElbowPronationNew - Mathf.Abs(mAngleRightElbowPronation)) / vDeltaTime;

            /// step 2///
            if (Vector3.Dot(vAxis2, vAxis3) < 0)
            {
                vAngleRightElbowPronationNew = -vAngleRightElbowPronationNew;
                vAngularVelocityRightElbowPronationNew = -vAngularVelocityRightElbowPronationNew;
            }

            mAngularAccelerationRightElbowPronation = (vAngularVelocityRightElbowPronationNew - mAngularVelocityRightElbowPronation) / vDeltaTime;
            mAngularVelocityRightElbowPronation = vAngularVelocityRightElbowPronationNew;
            mAngleRightElbowPronation = vAngleRightElbowPronationNew;

            //////////////// calculate the Shoulder Flection angle ////////////////////////////////////////
            /// step 1///
            vAxis1.Set(UpArOrientation[0, 2], UpArOrientation[1, 2], UpArOrientation[2, 2]);
            vAxis2.Set(TorsoOrientation[0, 2], TorsoOrientation[1, 2], TorsoOrientation[2, 2]);
            vAxis3 = vAxis1 - (Vector3.Dot(vAxis1, vAxis2)) * vAxis2;
            vAxis3.Normalize();
            vAxis2.Set(TorsoOrientation[0, 1], TorsoOrientation[1, 1], TorsoOrientation[2, 1]);
            vAxis1.Set(TorsoOrientation[0, 0], TorsoOrientation[1, 0], TorsoOrientation[2, 0]);
            float vAngleRightShoulderFlexionNew = 180 - Vector3.Angle(vAxis3, vAxis2);
            float vAngularVelocityRightShoulderFlexionNew = (vAngleRightShoulderFlexionNew - Mathf.Abs(mAngleRightShoulderFlexion)) / vDeltaTime;


            /// step 2///
            if (Vector3.Dot(vAxis1, vAxis3) > 0)
            {
                vAngleRightShoulderFlexionNew = -vAngleRightShoulderFlexionNew;
                vAngularVelocityRightShoulderFlexionNew = -vAngularVelocityRightShoulderFlexionNew;
            }

            mvAngularAccelerationRightShoulderFlexion = (vAngularVelocityRightShoulderFlexionNew - mAngularVelocityRightShoulderFlexion) / vDeltaTime;
            mAngularVelocityRightShoulderFlexion = vAngularVelocityRightShoulderFlexionNew;
            mAngleRightShoulderFlexion = vAngleRightShoulderFlexionNew;

            //////////////// calculate the Shoulder Abduction angle ////////////////////////////////////////
            /// step 1///
            vAxis1.Set(UpArOrientation[0, 2], UpArOrientation[1, 2], UpArOrientation[2, 2]);
            vAxis2.Set(TorsoOrientation[0, 0], TorsoOrientation[1, 0], TorsoOrientation[2, 0]);
            vAxis3 = vAxis1 - (Vector3.Dot(vAxis1, vAxis2)) * vAxis2;
            vAxis3.Normalize();
            vAxis2.Set(TorsoOrientation[0, 1], TorsoOrientation[1, 1], TorsoOrientation[2, 1]);
            vAxis1.Set(TorsoOrientation[0, 2], TorsoOrientation[1, 2], TorsoOrientation[2, 2]);
            float vAngleRightShoulderAbductionNew = Vector3.Angle(vAxis3, -vAxis2);
            float vAngularVelocityRightShoulderAbductionNew = (vAngleRightShoulderAbductionNew - Mathf.Abs(mAngleRightShoulderAbduction)) / vDeltaTime;

            /// step 2///
            if (Vector3.Dot(vAxis1, vAxis3) < 0)
            {
                vAngleRightShoulderAbductionNew = -vAngleRightShoulderAbductionNew;
                vAngularVelocityRightShoulderAbductionNew = -vAngularVelocityRightShoulderAbductionNew;
            }

            mAngularAccelerationRightShoulderAbduction = (vAngularVelocityRightShoulderAbductionNew - mAngularVelocityRightShoulderAbduction) / vDeltaTime;
            mAngularVelocityRightShoulderAbduction = vAngularVelocityRightShoulderAbductionNew;
            mAngleRightShoulderAbduction = vAngleRightShoulderAbductionNew;

            //////////////// calculate the Shoulder Rotation angle ////////////////////////////////////////
            /// step 1///
            vAxis1.Set(UpArOrientation[0, 2], UpArOrientation[1, 2], UpArOrientation[2, 2]);
            vAxis2.Set(TorsoOrientation[0, 1], TorsoOrientation[1, 1], TorsoOrientation[2, 1]);
            vAxis3 = Vector3.Cross(vAxis2, vAxis1);
            vAxis3.Normalize();
            vAxis2.Set(UpArOrientation[0, 0], UpArOrientation[1, 0], UpArOrientation[2, 0]);
            vAxis1.Set(UpArOrientation[0, 1], UpArOrientation[1, 1], UpArOrientation[2, 1]);
            float vAngleRightShoulderRotationNew = Vector3.Angle(vAxis3, vAxis2);
            float vAngularVelocityRightShoulderRotationNew = (vAngleRightShoulderRotationNew - Mathf.Abs(mAngleRightShoulderRotation)) / vDeltaTime;

            /// step 2///
            if (Vector3.Dot(vAxis1, vAxis3) > 0)
            {
                vAngleRightShoulderRotationNew = -vAngleRightShoulderRotationNew;
                vAngularVelocityRightShoulderRotationNew = -vAngularVelocityRightShoulderRotationNew;
            }

            mAngularAccelerationRightShoulderRotation = (vAngularVelocityRightShoulderRotationNew - mAngularVelocityRightShoulderRotation) / vDeltaTime;
            mAngularVelocityRightShoulderRotation = vAngularVelocityRightShoulderRotationNew;
            mAngleRightShoulderRotation = vAngleRightShoulderRotationNew; 
        }
    }
}
