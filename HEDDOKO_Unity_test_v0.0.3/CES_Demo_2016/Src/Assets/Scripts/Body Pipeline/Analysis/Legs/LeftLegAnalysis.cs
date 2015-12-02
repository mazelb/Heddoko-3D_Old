/** 
* @file LeftLegAnalysis.cs
* @brief LeftArmAnalysis class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis.Legs
{
    /// <summary>
    /// Analysis to be performed on the left leg 
    /// </summary>
    public class LeftLegAnalysis : LegAnalysis
    {
        public float AngleKneeFlexion { get; private set; }
        private float mAngularAccelerationKneeFlexion;
        private float mAngularVelocityKneeFlexion;

        public float AngleKneeRotation { get; private set; }
        private float mAngularAccelerationKneeRotation;

        public float AngleLeftHipRotation { get; private set; }
        private float mAngularVelocityKneeRotation;

        public float AngleLeftHipFlexion { get; private set; }
        private float mAngularVelocityLeftHipFlexion;
        private float mAngularAccelerationLeftHipFlexion;

        public float AngleLeftHipAbduction { get; private set; }
        private float mAngularAccelerationLeftHipAbduction;
        private float mAngularVelocityLeftHipAbduction;
        private float mAngularAccelerationLeftHipRotation;
        private float mAngularVelocityLeftHipRotation;

        /// <summary>
        /// Extract angles from orientations
        /// </summary>
        public override void AngleExtraction()
        {
            float vDeltaTime = Time.time - mLastTimeCalled;
            if ( vDeltaTime == 0)
            {
                return;
            }
            mLastTimeCalled = Time.time;

            /// step1 ///
            Vector3 vAxis1 = new Vector3(HipOrientation[0, 1], HipOrientation[1, 1], HipOrientation[2, 1]);
            Vector3 vAxis2 = new Vector3(KneeOrientation[0, 1], KneeOrientation[1, 1], KneeOrientation[2, 1]);
            float vAngleKneeFlexionNew = Vector3.Angle(vAxis1, vAxis2);
            float vAngularVelocityKneeFlexionNew = (vAngleKneeFlexionNew - AngleKneeFlexion) / vDeltaTime;

            /// step2 ///
            mAngularAccelerationKneeFlexion = (vAngularVelocityKneeFlexionNew - mAngularVelocityKneeFlexion) / vDeltaTime;
            mAngularVelocityKneeFlexion = vAngularVelocityKneeFlexionNew;
            AngleKneeFlexion = vAngleKneeFlexionNew;

            //////////////// calculate the Knee Rotation angle ////////////////////////////////////////

            /// step1 ///
            vAxis1.Set(HipOrientation[0, 2], HipOrientation[1, 2], HipOrientation[2, 2]);
            vAxis2.Set(KneeOrientation[0, 2], KneeOrientation[1, 2], KneeOrientation[2, 2]);
            Vector3 vAxis3 = new Vector3(HipOrientation[0, 0], HipOrientation[1, 0], HipOrientation[2, 0]);
            float vAngleKneeRotationNew = Vector3.Angle(vAxis1, vAxis2);

            float vAngularVelocityKneeRotationNew = (vAngleKneeRotationNew - Mathf.Abs(AngleKneeRotation)) / vDeltaTime;

            /// step2 ///
            if (Vector3.Dot(vAxis2, vAxis3) < 0)
            {
                vAngleKneeRotationNew = -vAngleKneeRotationNew;
                vAngularVelocityKneeRotationNew = -vAngularVelocityKneeRotationNew;
            }
            mAngularAccelerationKneeRotation = (vAngularVelocityKneeRotationNew - mAngularVelocityKneeRotation) / vDeltaTime;
            mAngularVelocityKneeRotation = vAngularVelocityKneeRotationNew;
            AngleKneeRotation = vAngleKneeRotationNew;

            //////////////// calculate the Left Hip Flection angle ////////////////////////////////////////

            /// step1 ///
            vAxis1.Set(HipOrientation[0, 1], HipOrientation[1, 1], HipOrientation[2, 1]);
            vAxis2.Set(TorsoOrientation[0, 2], TorsoOrientation[1, 2], TorsoOrientation[2, 2]);
            vAxis3 = vAxis1 - (Vector3.Dot(vAxis1, vAxis2)) * vAxis2;
            vAxis3.Normalize();
            vAxis2.Set(TorsoOrientation[0, 1], TorsoOrientation[1, 1], TorsoOrientation[2, 1]);
            vAxis1.Set(TorsoOrientation[0, 0], TorsoOrientation[1, 0], TorsoOrientation[2, 0]);
            float vAngleLeftHipFlexionNew = Vector3.Angle(vAxis3, vAxis2);
            float vAngularVelocityLeftHipFlexionNew = (vAngleLeftHipFlexionNew - Mathf.Abs(AngleLeftHipFlexion)) / vDeltaTime;

            /// step1 ///
            if (Vector3.Dot(vAxis1, vAxis3) > 0)
            {
                vAngleLeftHipFlexionNew = -vAngleLeftHipFlexionNew;
                vAngularVelocityLeftHipFlexionNew = -vAngularVelocityLeftHipFlexionNew;
            }

            mAngularAccelerationLeftHipFlexion = (vAngularVelocityLeftHipFlexionNew - mAngularVelocityLeftHipFlexion) / vDeltaTime;
            mAngularVelocityLeftHipFlexion = vAngularVelocityLeftHipFlexionNew;
            AngleLeftHipFlexion = vAngleLeftHipFlexionNew;

            //////////////// calculate the Left Hip Abduction angle ////////////////////////////////////////

            /// step1 ///
            vAxis1.Set(HipOrientation[0, 1], HipOrientation[1, 1], HipOrientation[2, 1]);
            vAxis2.Set(TorsoOrientation[0, 0], TorsoOrientation[1, 0], TorsoOrientation[2, 0]);
            vAxis3 = vAxis1 - (Vector3.Dot(vAxis1, vAxis2)) * vAxis2;
            vAxis3.Normalize();
            vAxis2.Set(TorsoOrientation[0, 1], TorsoOrientation[1, 1], TorsoOrientation[2, 1]);
            vAxis1.Set(TorsoOrientation[0, 2], TorsoOrientation[1, 2], TorsoOrientation[2, 2]);
            float vAngleLeftHipAbductionNew = Vector3.Angle(vAxis3, vAxis2);
            float vAngularVelocityLeftHipAbductionNew = (vAngleLeftHipAbductionNew - Mathf.Abs(AngleLeftHipAbduction)) / vDeltaTime;


            /// step1 ///
            if (Vector3.Dot(vAxis1, vAxis3) < 0)
            {
                vAngleLeftHipAbductionNew = -vAngleLeftHipAbductionNew;
                vAngularVelocityLeftHipAbductionNew = -vAngularVelocityLeftHipAbductionNew;
            }

            mAngularAccelerationLeftHipAbduction = (vAngularVelocityLeftHipAbductionNew - mAngularVelocityLeftHipAbduction) / vDeltaTime;
            mAngularVelocityLeftHipAbduction = vAngularVelocityLeftHipAbductionNew;
            AngleLeftHipAbduction = vAngleLeftHipAbductionNew;
 

            //////////////// calculate the Left Hip Rotation angle ////////////////////////////////////////

            /// step1 ///
            vAxis1.Set(HipOrientation[0, 2], HipOrientation[1, 2], HipOrientation[2, 2]);
            vAxis2.Set(TorsoOrientation[0, 1], TorsoOrientation[1, 1], TorsoOrientation[2, 1]);
            vAxis3 = vAxis1 - (Vector3.Dot(vAxis1, vAxis2)) * vAxis2;
            vAxis3.Normalize();
            vAxis1.Set(TorsoOrientation[0, 2], TorsoOrientation[1, 2], TorsoOrientation[2, 2]);
            vAxis2.Set(TorsoOrientation[0, 0], TorsoOrientation[1, 0], TorsoOrientation[2, 0]);
            float vAngleLeftHipRotationNew = Vector3.Angle(vAxis3, vAxis1);
            float vAngularVelocityLeftHipRotationNew = (vAngleLeftHipRotationNew - Mathf.Abs(AngleLeftHipRotation)) / vDeltaTime;

            /// step2 ///
            if (Vector3.Dot(vAxis2, vAxis3) > 0)
            {
                vAngleLeftHipRotationNew = -vAngleLeftHipRotationNew;
                vAngularVelocityLeftHipRotationNew = -vAngularVelocityLeftHipRotationNew;
            }

            mAngularAccelerationLeftHipRotation = (vAngularVelocityLeftHipRotationNew - mAngularVelocityLeftHipRotation) / vDeltaTime;
            mAngularVelocityLeftHipRotation = vAngularVelocityLeftHipRotationNew;
            AngleLeftHipRotation = vAngleLeftHipRotationNew;
             
        }
    }
}
