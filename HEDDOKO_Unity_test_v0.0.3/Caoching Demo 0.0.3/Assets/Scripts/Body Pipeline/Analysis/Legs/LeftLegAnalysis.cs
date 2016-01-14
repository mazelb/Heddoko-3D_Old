/** 
* @file LeftLegAnalysis.cs
* @brief LeftArmAnalysis class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;
using System;

namespace Assets.Scripts.Body_Pipeline.Analysis.Legs
{
    /// <summary>
    /// Analysis to be performed on the left leg 
    /// </summary>
    [Serializable]
    public class LeftLegAnalysis : LegAnalysis
    {
        //Knee Angles
        public float AngleKneeFlexion;
        public float AngleKneeRotation;

        //Hip Angles
        public bool UseGlobalReference = false;
        public float AngleHipFlexion;
        public float AngleHipAbduction;
        public float AngleHipRotation;

        //Accelerations and velocities
        public float AngularVelocityKneeFlexion = 0;
        public float AngularAccelerationKneeFlexion = 0;
        public float AngularVelocityKneeRotation = 0;
        public float AngularAccelerationKneeRotation = 0;
        public float AngularVelocityHipFlexion = 0;
        public float AngularAccelerationHipFlexion = 0;
        public float AngularVelocityHipAbduction = 0;
        public float AngularAccelerationHipAbduction = 0;
        public float AngularVelocityHipRotation = 0;
        public float AngularAccelerationHipRotation = 0;

        //Squats Analytics
        public float NumberofSquats;
        public float AngleSum;
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
            if (vDeltaTime == 0)
            {
                return;
            }
            mLastTimeCalled = Time.time;

            //Get necessary Axis info
            Vector3 vTorsoAxisUp, vTorsoAxisRight, vTorsoAxisForward;
            Vector3 vHipAxisUp, vHipAxisRight, vHipAxisForward;
            Vector3 vKneeAxisUp, vKneeAxisRight, vKneeAxisForward;

            //Get the 3D axis and angles
            vTorsoAxisUp = TorsoTransform.up;
            vTorsoAxisRight = TorsoTransform.right;
            vTorsoAxisForward = TorsoTransform.forward;

            vHipAxisUp = HipTransform.up;
            vHipAxisRight = HipTransform.right;
            vHipAxisForward = HipTransform.forward;

            vKneeAxisUp = KneeTransform.up;
            vKneeAxisRight = KneeTransform.right;
            vKneeAxisForward = KneeTransform.forward;

            //calculate the Knee Flexion angle (angles between axis projection in YZ plane)
            float vAngleKneeFlexionNew = Vector3.Angle(Vector3.ProjectOnPlane(vHipAxisUp, vHipAxisRight), Vector3.ProjectOnPlane(vKneeAxisUp, vHipAxisRight));
            float vAngularVelocityKneeFlexionNew = Mathf.Abs(vAngleKneeFlexionNew - AngleKneeFlexion) / vDeltaTime;
            AngularAccelerationKneeFlexion = Mathf.Abs(vAngularVelocityKneeFlexionNew - AngularVelocityKneeFlexion) / vDeltaTime;
            AngularVelocityKneeFlexion = vAngularVelocityKneeFlexionNew;

            //Squatts counting
            if (mStartCountingSquats)
            {
                if (Math.Abs(vAngleKneeFlexionNew) > 15)
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

            AngleKneeFlexion = vAngleKneeFlexionNew;

            //calculate the Knee Rotation angle (angles between axis projection in XZ plane)
            float vAngleKneeRotationNew = 180 - Mathf.Abs(180 - KneeTransform.rotation.eulerAngles.y);
            float vAngularVelocityKneeRotationNew = Mathf.Abs(vAngleKneeRotationNew - Mathf.Abs(AngleKneeRotation)) / vDeltaTime;
            AngularAccelerationKneeRotation = Mathf.Abs(vAngularVelocityKneeRotationNew - AngularVelocityKneeRotation) / vDeltaTime;
            AngularVelocityKneeRotation = vAngularVelocityKneeRotationNew;
            AngleKneeRotation = vAngleKneeRotationNew;

            //calculate the Hip Flexion angle (angles between axis projection in YZ plane)
            float vAngleHipFlexionNew;

            if (UseGlobalReference)
            {
                vAngleHipFlexionNew = Vector3.Angle(HipGlobalTransform.up, Vector3.ProjectOnPlane(vHipAxisUp, HipGlobalTransform.right));
            }
            else
            {
                vAngleHipFlexionNew = Vector3.Angle(Vector3.ProjectOnPlane(vTorsoAxisUp, vTorsoAxisRight), Vector3.ProjectOnPlane(vHipAxisUp, vTorsoAxisRight));
            }

            float vAngularVelocityHipFlexionNew = Mathf.Abs(vAngleHipFlexionNew - Mathf.Abs(AngleHipFlexion)) / vDeltaTime;
            AngularAccelerationHipFlexion = Mathf.Abs(vAngularVelocityHipFlexionNew - AngularVelocityHipFlexion) / vDeltaTime;
            AngularVelocityHipFlexion = vAngularVelocityHipFlexionNew;
            AngleHipFlexion = vAngleHipFlexionNew;

            //calculate the Hip Abduction angle (angles between axis projection in XY plane)
            float vAngleHipAbductionNew;

            if (UseGlobalReference)
            {
                vAngleHipAbductionNew = Vector3.Angle(HipGlobalTransform.up, Vector3.ProjectOnPlane(vHipAxisUp, HipGlobalTransform.forward));
            }
            else
            {
                vAngleHipAbductionNew = Vector3.Angle(Vector3.ProjectOnPlane(vTorsoAxisUp, vTorsoAxisForward), Vector3.ProjectOnPlane(vHipAxisUp, vTorsoAxisForward));
            }

            float vAngularVelocityHipAbductionNew = Mathf.Abs(vAngleHipAbductionNew - Mathf.Abs(AngleHipAbduction)) / vDeltaTime;
            AngularAccelerationHipAbduction = Mathf.Abs(vAngularVelocityHipAbductionNew - AngularVelocityHipAbduction) / vDeltaTime;
            AngularVelocityHipAbduction = vAngularVelocityHipAbductionNew;
            AngleHipAbduction = vAngleHipAbductionNew;

            //calculate the Hip Rotation angle (angles between axis projection in XZ plane) 
            float vAngleHipRotationNew = 180 - Mathf.Abs(180 - HipTransform.rotation.eulerAngles.y);
            float vAngularVelocityRHipRotationNew = Mathf.Abs(vAngleHipRotationNew - Mathf.Abs(AngleHipRotation)) / vDeltaTime;
            AngularAccelerationHipRotation = Mathf.Abs(vAngularVelocityRHipRotationNew - AngularVelocityHipRotation) / vDeltaTime;
            AngularVelocityHipRotation = vAngularVelocityRHipRotationNew;
            AngleHipRotation = vAngleHipRotationNew;
            //*/
        }
    }
}
