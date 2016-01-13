/** 
* @file TorsoAnalysis.cs
* @brief TorsoAnalysis class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System; 
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis.Torso
{
    [Serializable]
    public class TorsoAnalysis: SegmentAnalysis
    {
        //current torso orientation
        private Quaternion mTorsoOrientation = Quaternion.identity;

        //Angles extracted
        [SerializeField]
        private float mAngleTorsoFlexion;
        [SerializeField]
        private float mAngleTorsoLateral;
        [SerializeField]
        private float mAngleTorsoRotation;

        //Accelerations and velocities
        private float mAngularAccelerationTorsoFlection;
        private float mAngularVelocityTorsoFlexion;
        private float mAngularAccelerationTorsoLateral;
        private float mAngularVelocityTorsoLateral;
        private float mAngularAccelerationTorsoRotation;
        private float mAngularVelocityTorsoRotation;

        //Flips and turns detections
        public float AngleIntegrationTurns { get; private set; }
        public float AngleIntegrationFlips { get; private set; }
        public int NumberOfTurns { get; private set; }
        public  int NumberOfFlips { get; private set; }


        /// <summary>
        /// The main torso orientation. On set, all listeners will be notified of new orientation
        /// </summary>
        public Quaternion TorsoOrientation
        {
            get
            {
                return mTorsoOrientation;
            }
            set
            {
                mTorsoOrientation = value; 
            }
        }

        /// <summary>
        /// Extract angles of torso
        /// </summary>
        public override void AngleExtraction()
        {
            float vTimeDifference = Time.time - mLastTimeCalled;
            if(  vTimeDifference == 0)
            {
                return;
            }
            mLastTimeCalled = Time.time;

            // calculate the Torso Flexion angle 
            float vAngleTorsoFlexionNew = TorsoOrientation.eulerAngles.x;
            float vAngularVelocityTorsoFlexionNew = (vAngleTorsoFlexionNew - Math.Abs(mAngleTorsoFlexion)) / vTimeDifference;
            mAngularAccelerationTorsoFlection = (vAngularVelocityTorsoFlexionNew - mAngularVelocityTorsoFlexion) / vTimeDifference;
            mAngularVelocityTorsoFlexion = vAngularVelocityTorsoFlexionNew;
            mAngleTorsoFlexion = vAngleTorsoFlexionNew;

            //  calculate the Torso lateral angle 
            float vAngleTorsoLateralNew = TorsoOrientation.eulerAngles.z;
            float vAngularVelocityTorsoLateralNew = (vAngleTorsoLateralNew - Math.Abs(mAngleTorsoLateral)) / vTimeDifference;
            mAngularAccelerationTorsoLateral = (vAngularVelocityTorsoLateralNew - mAngularVelocityTorsoLateral) / vTimeDifference;
            mAngularVelocityTorsoLateral = vAngularVelocityTorsoLateralNew;
            mAngleTorsoLateral = vAngleTorsoLateralNew;

            // calculate the Torso Rotational angle 
            float vAngleTorsoRotationNew = TorsoOrientation.eulerAngles.y;
            float vAngularVelocityTorsoRotationNew = (vAngleTorsoRotationNew - Mathf.Abs(mAngleTorsoRotation)) / vTimeDifference;
            mAngularAccelerationTorsoRotation = (vAngularVelocityTorsoRotationNew - mAngularVelocityTorsoRotation) / vTimeDifference;
            mAngularVelocityTorsoRotation = vAngularVelocityTorsoRotationNew;
            mAngleTorsoRotation = vAngleTorsoRotationNew;

            // Turn detection 
            if (Math.Abs(vAngleTorsoRotationNew) < 3)
            {
                AngleIntegrationTurns = 0;
            }
            else
            {
                AngleIntegrationTurns += (vAngularVelocityTorsoRotationNew * vTimeDifference);
            } 
            if (Math.Abs(AngleIntegrationTurns) > 330)
            { 
                AngleIntegrationTurns = 0;
                NumberOfTurns++; 
            }

            // Flip detection 
            if (Math.Abs(vAngularVelocityTorsoFlexionNew) < 3)
            {
                AngleIntegrationFlips = 0;
            }
            else
            {
                AngleIntegrationFlips += (mAngularVelocityTorsoFlexion * vTimeDifference);
            }

            if (Math.Abs(AngleIntegrationFlips) > 330)
            { 
                NumberOfFlips++;
                AngleIntegrationFlips = 0; 
            } 
        }
    }
}
