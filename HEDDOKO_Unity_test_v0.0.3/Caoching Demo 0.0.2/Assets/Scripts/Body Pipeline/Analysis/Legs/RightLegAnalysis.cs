
/** 
* @file RightLegAnalysis.cs
* @brief RightLegAnalysis class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System; 
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis.Legs
{
   /// <summary>
   /// Represents the anaylsis of the right leg segment
   /// </summary>
   public class RightLegAnalysis : LegAnalysis
    {
        //Angles extracted
        public float AngleKneeFlexion { get; private set; }
        public float AngleKneeRotation { get; private set; }
        public float AngleRightHipFlexion { get; private set; }
        public float AngleRightHipAbduction { get; private set; }
        public float AngleRightHipRotation { get; private set; }

        //Accelerations and velocities
        public float mAngularVelocityKneeFlexion = 0;
        public float mAngularAccelerationKneeFlexion = 0;
        public float mAngularVelocityKneeRotation = 0;
        public float mAngularAccelerationKneeRotation = 0;
        public float mAngularVelocityHipFlexion = 0;
        public float mAngularAccelerationHipFlexion = 0;
        public float mAngularVelocityHipAbduction = 0;
        public float mAngularAccelerationHipAbduction = 0;
        public float mAngularVelocityHipRotation = 0;
        public float mAngularAccelerationHipRotation = 0;

        //Squats Analytics
        public float NumberofSquats { get;   set; }
        public float AngleSum { get; private set; }
        private bool mStartCountingSquats = true;

        /// <summary>
        /// Listens to events where squats need to be counted
        /// </summary>
        /// <param name="vFlag"></param>
        public void StartCountingSquats(bool vFlag)
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

            //calculate the Knee Flexion angle
            float vAngleKneeFlexionNew = KneeOrientation.eulerAngles.x;

            if(vAngleKneeFlexionNew > 180 && vAngleKneeFlexionNew < 360)
            {
                vAngleKneeFlexionNew = Math.Abs(360 - vAngleKneeFlexionNew);
            }

            float vAngularVelocityKneeFlexionNew = (vAngleKneeFlexionNew - AngleKneeFlexion) / vDeltaTime;

            if ( mStartCountingSquats)
            {
                if (Math.Abs(vAngleKneeFlexionNew) > 25) 
                {
                    AngleSum += Math.Abs(vAngleKneeFlexionNew - AngleKneeFlexion);
                }
                else
                {
                    AngleSum = 0;
                }

                if (Math.Abs(AngleSum) > 140)
                {
                    AngleSum = 0;
                    NumberofSquats++;
                }
            }

            mAngularAccelerationKneeFlexion = (vAngularVelocityKneeFlexionNew - mAngularVelocityKneeFlexion) / vDeltaTime;
            mAngularVelocityKneeFlexion = vAngularVelocityKneeFlexionNew;
            AngleKneeFlexion = vAngleKneeFlexionNew;

            //calculate the Knee Rotation angle
            float vAngleKneeRotationNew = KneeOrientation.eulerAngles.y;
            float vAngularVelocityKneeRotationNew = (vAngleKneeRotationNew - Mathf.Abs(AngleKneeRotation)) / vDeltaTime;
            mAngularAccelerationKneeRotation = (vAngularVelocityKneeRotationNew - mAngularVelocityKneeRotation) / vDeltaTime;
            mAngularVelocityKneeRotation = vAngularVelocityKneeRotationNew;
            AngleKneeRotation = vAngleKneeRotationNew;

            /*//calculate the Hip Flexion angle
            float vAngleHipFlexionNew = Quaternion.Angle(TorsoOrientation, HipOrientation);
            float vAngularVelocityHipFlexionNew = (vAngleHipFlexionNew - Mathf.Abs(AngleRightHipFlexion)) / vDeltaTime;
            mAngularAccelerationHipFlexion = (vAngularVelocityHipFlexionNew - mAngularVelocityHipFlexion) / vDeltaTime;
            mAngularVelocityHipFlexion = vAngularVelocityHipFlexionNew;
            AngleRightHipFlexion = vAngleHipFlexionNew;

            //calculate the Hip Abduction angle 
            /// step1 ///
            vAxis1.Set(HipOrientation[0, 1], HipOrientation[1, 1], HipOrientation[2, 1]);
            vAxis2.Set(TorsoOrientation[0, 0], TorsoOrientation[1, 0], TorsoOrientation[2, 0]);
            vAxis3 = vAxis1 - (Vector3.Dot(vAxis1, vAxis2)) * vAxis2;
            vAxis3.Normalize();
            vAxis2.Set(TorsoOrientation[0, 1], TorsoOrientation[1, 1], TorsoOrientation[2, 1]);
            vAxis1.Set(TorsoOrientation[0, 2], TorsoOrientation[1, 2], TorsoOrientation[2, 2]);
            float vAngleRightHipAbductionNew = Vector3.Angle(vAxis3, vAxis2);
            float vAngularVelocityRightHipAbductionNew = (vAngleRightHipAbductionNew - Mathf.Abs(AngleRightHipAbduction)) / vDeltaTime;

            /// step2 ///
            if (Vector3.Dot(vAxis1, vAxis3) > 0)
            {
                vAngleRightHipAbductionNew = -vAngleRightHipAbductionNew;
                vAngularVelocityRightHipAbductionNew = -vAngularVelocityRightHipAbductionNew;
            }
            mAngularAccelerationRightHipAbduction = (vAngularVelocityRightHipAbductionNew - mAngularVelocityRightHipAbduction) / vDeltaTime;
            mAngularVelocityRightHipAbduction = vAngularVelocityRightHipAbductionNew;
            AngleRightHipAbduction = vAngleRightHipAbductionNew;

            //////////////// calculate the Hip Rotation angle ////////////////////////////////////////

            /// step1 ///
            vAxis1.Set(HipOrientation[0, 2], HipOrientation[1, 2], HipOrientation[2, 2]);
            vAxis2.Set(TorsoOrientation[0, 1], TorsoOrientation[1, 1], TorsoOrientation[2, 1]);
            vAxis3 = vAxis1 - (Vector3.Dot(vAxis1, vAxis2)) * vAxis2;
            vAxis3.Normalize();
            vAxis1.Set(TorsoOrientation[0, 2], TorsoOrientation[1, 2], TorsoOrientation[2, 2]);
            vAxis2.Set(TorsoOrientation[0, 0], TorsoOrientation[1, 0], TorsoOrientation[2, 0]);
            float vAngleRightHipRotationNew = Vector3.Angle(vAxis3, vAxis1);
            float vAngularVelocityRightHipRotationNew = (vAngleRightHipRotationNew - Mathf.Abs(AngleRightHipRotation)) / vDeltaTime;

            /// step2 ///
            if (Vector3.Dot(vAxis2, vAxis3) < 0)
            {
                vAngleRightHipRotationNew = -vAngleRightHipRotationNew;
                vAngularVelocityRightHipRotationNew = -vAngularVelocityRightHipRotationNew;
            }

            mAngularAccelerationRightHipRotation = (vAngularVelocityRightHipRotationNew - mAngularVelocityRightHipRotation) / vDeltaTime;
            mAngularVelocityRightHipRotation = vAngularVelocityRightHipRotationNew;
            AngleRightHipRotation = vAngleRightHipRotationNew;
            //*/
        }
    }
}
