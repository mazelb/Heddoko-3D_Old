﻿
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
        public float SignedAngleElbowFlexion = 0;
        public float AngleElbowPronation = 0;

        //Upper Arm Angles
        public float AngleShoulderFlexion = 0;
        public float AngleShoulderVertAbduction = 0;
        public float AngleShoulderHorAbduction = 0;
        public float AngleShoulderRotation = 0;
        public float AngleShoulderReference = 0;
        public float AngleShoulderReferenceXY = 0;
        public float AngleShoulderReferenceXZ = 0;
        public float AngleShoulderReferenceYZ = 0;

        //Velocities and Accelerations
        public float AngularVelocityElbowFlexion = 0;
        public float PeakAngularVelocityElbowFlexion = 0;
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
        /// Reset the metrics calculations
        /// </summary>
        public override void ResetMetrics()
        {/*
            AngleElbowFlexion = 0;
            SignedAngleElbowFlexion = 0;
            AngleElbowPronation = 0;
            AngleShoulderFlexion = 0;
            AngleShoulderVertAbduction = 0;
            AngleShoulderHorAbduction = 0;
            AngleShoulderRotation = 0;
            AngularVelocityElbowFlexion = 0;*/
            PeakAngularVelocityElbowFlexion = 0;
          /*  AngularAccelerationElbowFlexion = 0;
            AngularVelocityPronation = 0;
            AngularAccelerationElbowPronation = 0;
            AngularVelocityShoulderFlexion = 0;
            AngularAccelerationShoulderFlexion = 0;
            AngularVelocityShoulderVertAbduction = 0;
            AngularAccelerationShoulderVertAbduction = 0;
            AngularVelocityShoulderHorAbduction = 0;
            AngularAccelerationShoulderHorAbduction = 0;
            AngularVelocityShoulderRotation = 0;
            AngularAccelerationShoulderRotation = 0;
            mLastTimeCalled = Time.time;*/
        }

        /// <summary>
        /// Extract angles from orientations
        /// </summary>
        public override void AngleExtraction()
        {
            //Debug.Log(DeltaTime);

            //float vDeltaTime = Time.deltaTime;
            // Time.time - mLastTimeCalled;
            if (DeltaTime == 0)
            {
                return;
            }
            //mLastTimeCalled = Time.time;

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
            Vector3 vProjectedShoulderAxisRight = Vector3.ProjectOnPlane(vShoulderAxisRight, vShoulderAxisForward);
            Vector3 vProjectedElbowAxisRight = Vector3.ProjectOnPlane(vElbowAxisRight, vShoulderAxisForward);
            float vAngleElbowFlexionNew = Vector3.Angle(vProjectedShoulderAxisRight, vProjectedElbowAxisRight);
            float vAngularVelocityElbowFlexionNew = (vAngleElbowFlexionNew - AngleElbowFlexion) / DeltaTime;
            AngularAccelerationElbowFlexion = (vAngularVelocityElbowFlexionNew - AngularVelocityElbowFlexion) / DeltaTime;
            AngularVelocityElbowFlexion = vAngularVelocityElbowFlexionNew;
            PeakAngularVelocityElbowFlexion = Mathf.Max(Mathf.Abs(AngularVelocityElbowFlexion), PeakAngularVelocityElbowFlexion);
            AngleElbowFlexion = vAngleElbowFlexionNew;
            SignedAngleElbowFlexion = GetSignedAngle(vElbowAxisRight, vShoulderAxisRight, vElbowAxisUp.normalized);

            //calculate the Elbow Pronation angle
            float vAngleElbowPronationNew = 180 - Mathf.Abs(180 - LoArTransform.rotation.eulerAngles.x);
            float vAngularVelocityElbowPronationNew = (vAngleElbowPronationNew - Mathf.Abs(AngleElbowPronation)) / DeltaTime;
            AngularAccelerationElbowPronation = (vAngularVelocityElbowPronationNew - AngularVelocityPronation) / DeltaTime;
            AngularVelocityPronation = vAngularVelocityElbowPronationNew;
            AngleElbowPronation = vAngleElbowPronationNew;

            //calculate the Shoulder Flexion angle
            float vAngleShoulderFlexionNew = Vector3.Angle(-vTorsoAxisUp, Vector3.ProjectOnPlane(vShoulderAxisRight, vTorsoAxisRight));
            float vAngularVelocityShoulderFlexionNew = (vAngleShoulderFlexionNew - Mathf.Abs(AngleShoulderFlexion)) / DeltaTime;
            AngularAccelerationShoulderFlexion = (vAngularVelocityShoulderFlexionNew - AngularVelocityShoulderFlexion) / DeltaTime;
            AngularVelocityShoulderFlexion = vAngularVelocityShoulderFlexionNew;
            AngleShoulderFlexion = vAngleShoulderFlexionNew;

            //calculate the Shoulder Abduction Vertical angle
            float vAngleShoulderVertAbductionNew = Vector3.Angle(-vTorsoAxisUp, Vector3.ProjectOnPlane(vShoulderAxisRight, vTorsoAxisForward));
            float vAngularVelocityShoulderVertAbductionNew = (vAngleShoulderVertAbductionNew - Mathf.Abs(AngleShoulderVertAbduction)) / DeltaTime;
            AngularAccelerationShoulderVertAbduction = (vAngularVelocityShoulderVertAbductionNew - AngularVelocityShoulderVertAbduction) / DeltaTime;
            AngularVelocityShoulderVertAbduction = vAngularVelocityShoulderVertAbductionNew;
            AngleShoulderVertAbduction = vAngleShoulderVertAbductionNew;

            //calculate the Shoulder Abduction Horizontal angle
            float vAngleShoulderHorAbductionNew = Vector3.Angle(vTorsoAxisForward, Vector3.ProjectOnPlane(vShoulderAxisRight, vTorsoAxisUp));
            float vAngularVelocityShoulderHorAbductionNew = (vAngleShoulderHorAbductionNew - Mathf.Abs(AngleShoulderHorAbduction)) / DeltaTime;
            AngularAccelerationShoulderHorAbduction = (vAngularVelocityShoulderHorAbductionNew - AngularVelocityShoulderHorAbduction) / DeltaTime;
            AngularVelocityShoulderHorAbduction = vAngularVelocityShoulderHorAbductionNew;
            AngleShoulderHorAbduction = vAngleShoulderHorAbductionNew;

            //calculate the Shoulder Rotation angle
            float vAngleShoulderRotationNew = 180 - Mathf.Abs(180 - UpArTransform.rotation.eulerAngles.x);
            float vAngularVelocityShoulderRotationNew = (vAngleShoulderRotationNew - Mathf.Abs(AngleShoulderRotation)) / DeltaTime;
            AngularAccelerationShoulderRotation = (vAngularVelocityShoulderRotationNew - AngularVelocityShoulderRotation) / DeltaTime;
            AngularVelocityShoulderRotation = vAngularVelocityShoulderRotationNew;
            AngleShoulderRotation = vAngleShoulderRotationNew; //*/

            //Calculate angle from reference
            AngleShoulderReference = Vector3.Angle(vShoulderAxisRight, ReferenceVector);
            AngleShoulderReferenceXY = Vector3.Angle(Vector3.ProjectOnPlane(vShoulderAxisRight, Vector3.forward), Vector3.ProjectOnPlane(ReferenceVector, Vector3.forward));
            AngleShoulderReferenceXZ = Vector3.Angle(Vector3.ProjectOnPlane(vShoulderAxisRight, Vector3.up), Vector3.ProjectOnPlane(ReferenceVector, Vector3.up));
            AngleShoulderReferenceYZ = Vector3.Angle(Vector3.ProjectOnPlane(vShoulderAxisRight, Vector3.right), Vector3.ProjectOnPlane(ReferenceVector, Vector3.right));
        }
}
}
