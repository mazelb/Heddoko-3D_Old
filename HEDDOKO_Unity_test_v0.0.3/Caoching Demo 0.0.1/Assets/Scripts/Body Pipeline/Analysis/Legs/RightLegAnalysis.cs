using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis.Legs
{
   public class RightLegAnalysis : LegAnalysis
    {
        public float mAngleKneeFlexion = 0;
        public float mAngularVelocityKneeFlexion = 0;
        public float mAngularAccelerationKneeFlexion = 0;

        public float mAngleKneeRotation = 0;
        public float mAngularVelocityKneeRotation = 0;
        public float mAngularAccelerationKneeRotation = 0;

        public float mAngleRightHipFlexion = 0;
        public float mAngularVelocityRightHipFlexion = 0;
        public float mAngularAccelerationRightHipFlexion = 0;


        public float mAngleRightHipAbduction = 0;
        public float mAngularVelocityRightHipAbduction = 0;
        public float mAngularAccelerationRightHipAbduction = 0;

        public static float mAngleRightHipRotation = 0;
        public float mAngularVelocityRightHipRotation = 0;
        public float mAngularAccelerationRightHipRotation = 0;

        // This variable stores the time of current frame. It is used for angular velocity and acceleration extraction
 
        public static float mNumberofRightSquats = 0;
        public float mAngleSumRight = 0;
        private bool mStartCountingSquats;

        /// <summary>
        /// Listens to events where squats need to be counted
        /// </summary>
        /// <param name="vFlag"></param>
       private void StartCountingSquatsListener(bool vFlag)
        {
            mStartCountingSquats = vFlag;
        }
         
        /// <summary>
        /// Extract angles from orientations for the right leg
        /// </summary>
        public override void AngleExtraction()
        {
            float vDeltaTime = Time.time - mLastTimeCalled;
            if ( vDeltaTime == 0)
            {
                return;
            }
            mLastTimeCalled = Time.time;


            //=====================================  Angle extraction =====================================//


            // Axes 1 to 4 are intermediate variables used to calculate angles. 
            // In the first step, with appropriate matrix calculations each angle and angular velocities are calculated
            // In the second step, the sign of these angles will be determined and the angles will be updated


            //=====================================calculate the Knee Flection angle  =====================================/

            //===================================== step1 =====================================//
            Vector3 vAxis1 = new Vector3(HipOrientation[0, 1], HipOrientation[1, 1], HipOrientation[2, 1]);
            Vector3 vAxis2 = new Vector3(KneeOrientation[0, 1], KneeOrientation[1, 1], KneeOrientation[2, 1]);
            float vAngleKneeFlexionNew = Vector3.Angle(vAxis1, vAxis2);
            float vAngularVelocityKneeFlexionNew = (vAngleKneeFlexionNew - mAngleKneeFlexion) / vDeltaTime;

            //        print(mNumberofRightSquats + "angle" + mAngleKneeFlexion + "vAngularVelocityKneeFlexionNew= "+ vAngularVelocityKneeFlexionNew);
            if ( mStartCountingSquats)
            {
                if (Math.Abs(vAngleKneeFlexionNew) < 15)
                {
                    mAngleSumRight = 0;
                }
                else
                {
                    mAngleSumRight += (Math.Abs(vAngularVelocityKneeFlexionNew) * vDeltaTime);
                }

                //        print(mAngleSumRight + "mAngleSumRight");
                if (Math.Abs(mAngleSumRight) > 140)
                {

                    mAngleSumRight = 0;
                    mNumberofRightSquats++;
                }
            }


            /// step2 ///
            mAngularAccelerationKneeFlexion = (vAngularVelocityKneeFlexionNew - mAngularVelocityKneeFlexion) / vDeltaTime;
            mAngularVelocityKneeFlexion = vAngularVelocityKneeFlexionNew;
            mAngleKneeFlexion = vAngleKneeFlexionNew;



            //////////////// calculate the Knee Rotation angle ////////////////////////////////////////

            /// step1 ///
            vAxis1.Set(HipOrientation[0, 2], HipOrientation[1, 2], HipOrientation[2, 2]);
            vAxis2.Set(KneeOrientation[0, 2], KneeOrientation[1, 2], KneeOrientation[2, 2]);
            Vector3 vAxis3 = new Vector3(HipOrientation[0, 1], HipOrientation[1, 1], HipOrientation[2, 1]);

            float vAngleKneeRotationNew = Vector3.Angle(vAxis1, vAxis2);
            //	Debug.Log ("Knee Flection Angles" + vAngleKneeRotationNew);
            float vAngularVelocityKneeRotationNew = (vAngleKneeRotationNew - Mathf.Abs(mAngleKneeRotation)) / vDeltaTime;

            /// step2 ///
            if (Vector3.Dot(vAxis2, vAxis3) < 0)
            {
                vAngleKneeRotationNew = -vAngleKneeRotationNew;
                vAngularVelocityKneeRotationNew = -vAngularVelocityKneeRotationNew;
            }

            mAngularAccelerationKneeRotation = (vAngularVelocityKneeRotationNew - mAngularVelocityKneeRotation) / vDeltaTime;
            mAngularVelocityKneeRotation = vAngularVelocityKneeRotationNew;
            mAngleKneeRotation = vAngleKneeRotationNew;


            //Debug.Log ("Knee Rotation Angles" + mAngleKneeRotation + ", and, " + mAngularVelocityKneeRotation + ", and, " + mAngularAccelerationKneeRotation);


            //////////////// calculate the Hip Flection angle ////////////////////////////////////////

            /// step1 ///
            vAxis1.Set(HipOrientation[0, 1], HipOrientation[1, 1], HipOrientation[2, 1]);
            vAxis2.Set(TorsoOrientation[0, 2], TorsoOrientation[1, 2], TorsoOrientation[2, 2]);
            vAxis3 = vAxis1 - (Vector3.Dot(vAxis1, vAxis2)) * vAxis2;
            vAxis3.Normalize();
            vAxis2.Set(TorsoOrientation[0, 1], TorsoOrientation[1, 1], TorsoOrientation[2, 1]);
            vAxis1.Set(TorsoOrientation[0, 0], TorsoOrientation[1, 0], TorsoOrientation[2, 0]);
            float vAngleRightHipFlexionNew = Vector3.Angle(vAxis3, vAxis2);
            float vAngularVelocityRightHipFlexionNew = (vAngleRightHipFlexionNew - Mathf.Abs(mAngleRightHipFlexion)) / vDeltaTime;

            /// step2 ///
            if (Vector3.Dot(vAxis1, vAxis3) < 0)
            {
                vAngleRightHipFlexionNew = -vAngleRightHipFlexionNew;
                vAngularVelocityRightHipFlexionNew = -vAngularVelocityRightHipFlexionNew;
            }

            mAngularAccelerationRightHipFlexion = (vAngularVelocityRightHipFlexionNew - mAngularVelocityRightHipFlexion) / vDeltaTime;
            mAngularVelocityRightHipFlexion = vAngularVelocityRightHipFlexionNew;
            mAngleRightHipFlexion = vAngleRightHipFlexionNew;

            //Debug.Log ("EHip Flexion Angles" + vAngleRightHipFlexionNew + ", and, " + mAngularVelocityRightHipFlexion + ", and, " + mAngularAccelerationRightHipFlexion);



            //////////////// calculate the Hip Abduction angle ////////////////////////////////////////

            /// step1 ///
            vAxis1.Set(HipOrientation[0, 1], HipOrientation[1, 1], HipOrientation[2, 1]);
            vAxis2.Set(TorsoOrientation[0, 0], TorsoOrientation[1, 0], TorsoOrientation[2, 0]);
            vAxis3 = vAxis1 - (Vector3.Dot(vAxis1, vAxis2)) * vAxis2;
            vAxis3.Normalize();
            vAxis2.Set(TorsoOrientation[0, 1], TorsoOrientation[1, 1], TorsoOrientation[2, 1]);
            vAxis1.Set(TorsoOrientation[0, 2], TorsoOrientation[1, 2], TorsoOrientation[2, 2]);
            float vAngleRightHipAbductionNew = Vector3.Angle(vAxis3, vAxis2);
            float vAngularVelocityRightHipAbductionNew = (vAngleRightHipAbductionNew - Mathf.Abs(mAngleRightHipAbduction)) / vDeltaTime;

            /// step2 ///
            if (Vector3.Dot(vAxis1, vAxis3) > 0)
            {
                vAngleRightHipAbductionNew = -vAngleRightHipAbductionNew;
                vAngularVelocityRightHipAbductionNew = -vAngularVelocityRightHipAbductionNew;
            }
            mAngularAccelerationRightHipAbduction = (vAngularVelocityRightHipAbductionNew - mAngularVelocityRightHipAbduction) / vDeltaTime;
            mAngularVelocityRightHipAbduction = vAngularVelocityRightHipAbductionNew;
            mAngleRightHipAbduction = vAngleRightHipAbductionNew;

            //Debug.Log ("Hip Abduction Angles" + vAngleRightHipAbductionNew + ", and, " + mAngularVelocityRightHipAbduction + ", and, " + mAngularAccelerationRightHipAbduction);


            //////////////// calculate the Hip Rotation angle ////////////////////////////////////////

            /// step1 ///
            vAxis1.Set(HipOrientation[0, 2], HipOrientation[1, 2], HipOrientation[2, 2]);
            vAxis2.Set(TorsoOrientation[0, 1], TorsoOrientation[1, 1], TorsoOrientation[2, 1]);
            vAxis3 = vAxis1 - (Vector3.Dot(vAxis1, vAxis2)) * vAxis2;
            vAxis3.Normalize();
            vAxis1.Set(TorsoOrientation[0, 2], TorsoOrientation[1, 2], TorsoOrientation[2, 2]);
            vAxis2.Set(TorsoOrientation[0, 0], TorsoOrientation[1, 0], TorsoOrientation[2, 0]);
            float vAngleRightHipRotationNew = Vector3.Angle(vAxis3, vAxis1);
            float vAngularVelocityRightHipRotationNew = (vAngleRightHipRotationNew - Mathf.Abs(mAngleRightHipRotation)) / vDeltaTime;

            /// step2 ///
            if (Vector3.Dot(vAxis2, vAxis3) < 0)
            {
                vAngleRightHipRotationNew = -vAngleRightHipRotationNew;
                vAngularVelocityRightHipRotationNew = -vAngularVelocityRightHipRotationNew;
            }

            mAngularAccelerationRightHipRotation = (vAngularVelocityRightHipRotationNew - mAngularVelocityRightHipRotation) / vDeltaTime;
            mAngularVelocityRightHipRotation = vAngularVelocityRightHipRotationNew;
            mAngleRightHipRotation = vAngleRightHipRotationNew;
             
        }
    }
}
