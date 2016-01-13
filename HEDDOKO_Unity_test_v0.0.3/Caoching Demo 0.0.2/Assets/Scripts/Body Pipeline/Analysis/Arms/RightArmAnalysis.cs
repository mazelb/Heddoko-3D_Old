
/** 
* @file RightArmAnalysis.cs
* @brief RightArmAnalysis the Joint class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using UnityEngine;
using System;
namespace Assets.Scripts.Body_Pipeline.Analysis.Arms
{
    /**
    * RightArmAnalysis class 
    * @brief RightArmAnalysis class 
    */
    [Serializable]
    public class RightArmAnalysis: ArmAnalysis
    {
        //Elbow Angles
        public float AngleElbowFlexion = 0;
        public float AngleElbowPronation = 0;

        //Upper Arm Angles
        public float AngleShoulderFlexion = 0;
        public float AngleShoulderVertAbduction = 0;
        public float AngleShoulderHorAbduction = 0;
        public float AngleShoulderRotation = 0;

        //Velocities and Accelerations
        public float AngularVelocityElbowFlexion = 0;
        public float AngularAccelerationElbowFlexion = 0;
        public float AngularVelocityPronation = 0;
        public float AngularAccelerationElbowPronation = 0;
        public float AngularVelocityShoulderFlexion = 0;
        public float AngularAccelerationShoulderFlexion = 0;
        public float AngularVelocityShoulderVertAbduction = 0;
        public float AngularAccelerationShoulderVertAbduction = 0;
        public float AngularVelocityShoulderHorAbduction = 0;
        public float AngularAccelerationShoulderHorAbduction = 0;
        public float AngularVelocityShoulderRotation = 0;
        public float AngularAccelerationShoulderRotation = 0;

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

            //Get necessary Axis info
            Vector3 vTorsoAxisUp, vTorsoAxisRight, vTorsoAxisForward;
            Vector3 vShoulderAxisUp, vShoulderAxisRight, vShoulderAxisForward;
            Vector3 vElbowAxisUp, vElbowAxisRight, vElbowAxisForward;

            //Get the 3D axis and angles
            vTorsoAxisUp = TorsoTransform.up;
            vTorsoAxisRight = TorsoTransform.right;
            vTorsoAxisForward = TorsoTransform.forward;

            vShoulderAxisUp = UpArTransform.up;
            vShoulderAxisRight = UpArTransform.right;
            vShoulderAxisForward = UpArTransform.forward;

            vElbowAxisUp = LoArTransform.up;
            vElbowAxisRight = LoArTransform.right;
            vElbowAxisForward = LoArTransform.forward;

            //calculate the Elbow Flexion angle
            float vAngleElbowFlexionNew = Vector3.Angle(Vector3.ProjectOnPlane(vShoulderAxisRight, vShoulderAxisForward), Vector3.ProjectOnPlane(vElbowAxisRight, vShoulderAxisForward));
            float vAngularVelocityElbowFlexionNew = (vAngleElbowFlexionNew - AngleElbowFlexion) / vDeltaTime;
            AngularAccelerationElbowFlexion = (vAngularVelocityElbowFlexionNew - AngularVelocityElbowFlexion) / vDeltaTime;
            AngularVelocityElbowFlexion = vAngularVelocityElbowFlexionNew;
            AngleElbowFlexion = vAngleElbowFlexionNew;

            //calculate the Elbow Pronation angle
            float vAngleElbowPronationNew = Vector3.Angle(Vector3.ProjectOnPlane(vShoulderAxisUp, vShoulderAxisRight), Vector3.ProjectOnPlane(vElbowAxisUp, vShoulderAxisRight));
            float vAngularVelocityElbowPronationNew = (vAngleElbowPronationNew - Mathf.Abs(AngleElbowPronation)) / vDeltaTime;
            AngularAccelerationElbowPronation = (vAngularVelocityElbowPronationNew - AngularVelocityPronation) / vDeltaTime;
            AngularVelocityPronation = vAngularVelocityElbowPronationNew;
            AngleElbowPronation = vAngleElbowPronationNew;

            //calculate the Shoulder Flexion angle
            float vAngleShoulderFlexionNew = Vector3.Angle(-vTorsoAxisUp, Vector3.ProjectOnPlane(vShoulderAxisRight, vTorsoAxisRight));
            float vAngularVelocityShoulderFlexionNew = (vAngleShoulderFlexionNew - Mathf.Abs(AngleShoulderFlexion)) / vDeltaTime;
            AngularAccelerationShoulderFlexion = (vAngularVelocityShoulderFlexionNew - AngularVelocityShoulderFlexion) / vDeltaTime;
            AngularVelocityShoulderFlexion = vAngularVelocityShoulderFlexionNew;
            AngleShoulderFlexion = vAngleShoulderFlexionNew;

            //calculate the Shoulder Abduction Vertical angle
            float vAngleShoulderVertAbductionNew = Vector3.Angle(-vTorsoAxisUp, Vector3.ProjectOnPlane(vShoulderAxisRight, vTorsoAxisForward));
            float vAngularVelocityShoulderVertAbductionNew = (vAngleShoulderVertAbductionNew - Mathf.Abs(AngleShoulderVertAbduction)) / vDeltaTime;
            AngularAccelerationShoulderVertAbduction = (vAngularVelocityShoulderVertAbductionNew - AngularVelocityShoulderVertAbduction) / vDeltaTime;
            AngularVelocityShoulderVertAbduction = vAngularVelocityShoulderVertAbductionNew;
            AngleShoulderVertAbduction = vAngleShoulderVertAbductionNew;

            //calculate the Shoulder Abduction Horizontal angle
            float vAngleShoulderHorAbductionNew = Vector3.Angle(vTorsoAxisForward, Vector3.ProjectOnPlane(vShoulderAxisRight, vTorsoAxisUp));
            float vAngularVelocityShoulderHorAbductionNew = (vAngleShoulderHorAbductionNew - Mathf.Abs(AngleShoulderHorAbduction)) / vDeltaTime;
            AngularAccelerationShoulderHorAbduction = (vAngularVelocityShoulderHorAbductionNew - AngularVelocityShoulderHorAbduction) / vDeltaTime;
            AngularVelocityShoulderHorAbduction = vAngularVelocityShoulderHorAbductionNew;
            AngleShoulderHorAbduction = vAngleShoulderHorAbductionNew;

            //calculate the Shoulder Rotation angle
            float vAngleShoulderRotationNew = Vector3.Angle(vTorsoAxisForward, Vector3.ProjectOnPlane(vShoulderAxisRight, vTorsoAxisUp));
            float vAngularVelocityShoulderRotationNew = (vAngleShoulderRotationNew - Mathf.Abs(AngleShoulderRotation)) / vDeltaTime;
            AngularAccelerationShoulderRotation = (vAngularVelocityShoulderRotationNew - AngularVelocityShoulderRotation) / vDeltaTime;
            AngularVelocityShoulderRotation = vAngularVelocityShoulderRotationNew;
            AngleShoulderRotation = vAngleShoulderRotationNew; //*/
        }
    }
}
