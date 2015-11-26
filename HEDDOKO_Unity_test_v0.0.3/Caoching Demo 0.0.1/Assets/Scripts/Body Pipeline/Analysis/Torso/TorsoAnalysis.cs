
using System; 
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis.Torso
{
   public class TorsoAnalysis: SegmentAnalysis
   {
        /**
        * FunctionName(object args)
        * @brief Performs x function 
        * @param object args: the parameters necessary for this
        * function to perform
        * @note Please not that this will throw an exception if
        * y requirements are not met with the given parameter
        * @return returns an arbitrary value
        */
        //  public delegate void TorsoOrientationUpdatedDelegate(float[,] vNewOrientation);

        // public event TorsoOrientationUpdatedDelegate TorsoUpdatedEvent;
        private float[,] mTorsoOrientation = new float[3,3];
        private float mAngleTorsoFlexion;
        private float mAngularAccelerationTorsoFlection;
        private float mAngularVelocityTorsoFlexion;
        private float mAngleTorsoLateral;
        private float mAngularAccelerationTorsoLateral;
        private float mAngularVelocityTorsoLateral;
        private float mAngleTorsoRotation;
        private float mAngleIntegrationTurns;
        private int mNumberOfTurns;
        private int mNumberOfFlips;
        private float mAngularAccelerationTorsoRotation;
        private float mAngularVelocityTorsoRotation;
        private float mAngleTorsoVertical;
        private float mAngleIntegrationFlips;
        private float mAngularAccelerationTorsoVertical;
        private float mAngularVelocityTorsoVertical;

        /// <summary>
        /// The main torso orientation. On set, all listeners will be notified of new orientation
        /// </summary>
        public   float[,] TorsoOrientation
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

            //====================================== Begin Angle extraction ==============================================//
            //=============================calculate the Torso Flection angle ============================================//

            // Axes 1 to 4 are intermediate variables used to calculate angles. 
            //	with appropriate matrix calculations each angle and angular velocities are calculated in the first step
            // In the second step the sign of these angles will be determined and the angles will be updated
            //====================================== step 1 ==============================================================//
            Vector3 vAxis1 = new Vector3(TorsoOrientation[0, 1], TorsoOrientation[1, 1], TorsoOrientation[2, 1]);
            Vector3 vAxis2 = new Vector3(TorsoOrientation[0, 0], TorsoOrientation[1, 0], TorsoOrientation[2, 0]);
            Vector3 vAxis3 = new Vector3(0, 1, 0);
            Vector3 vAxis4 = Vector3.Cross(vAxis2, vAxis3);
            vAxis4.Normalize();
            vAxis3 = vAxis1 - (Vector3.Dot(vAxis1, vAxis4)) * vAxis4;
            vAxis3.Normalize();
            vAxis2.Set(0, 1, 0);
            float vAngleTorsoFlectionNew = Vector3.Angle(vAxis3, vAxis2);
            vAxis1.Set(TorsoOrientation[0, 0], 0, TorsoOrientation[2, 0]);
            float vAngularVelocityTorsoFlectionNew = (vAngleTorsoFlectionNew - Math.Abs(mAngleTorsoFlexion)) / vTimeDifference;
            //====================================== end of step 1 ==============================================================//

            //======================================  beginning of step 2 ==============================================================//
            if ((Vector3.Dot(vAxis3, vAxis1) * Vector3.Dot(vAxis3, vAxis2)) > 0)
            {
                vAngleTorsoFlectionNew = -vAngleTorsoFlectionNew;
                vAngularVelocityTorsoFlectionNew = -vAngularVelocityTorsoFlectionNew;
            }

            mAngularAccelerationTorsoFlection = (vAngularVelocityTorsoFlectionNew - mAngularVelocityTorsoFlexion) / vTimeDifference;
            mAngularVelocityTorsoFlexion = vAngularVelocityTorsoFlectionNew;
            mAngleTorsoFlexion = vAngleTorsoFlectionNew;

            //======================================  end of step 2 ==============================================================//

            //======================================  begin calculate the Torso lateral angle =========================================//

            vAxis1.Set(mTorsoOrientation[0, 1], mTorsoOrientation[1, 1], mTorsoOrientation[2, 1]);
            vAxis2.Set(mTorsoOrientation[0, 2], mTorsoOrientation[1, 2], mTorsoOrientation[2, 2]);
            vAxis3.Set(0, 1, 0);
            vAxis4 = Vector3.Cross(vAxis2, vAxis3);
            vAxis4.Normalize();
            vAxis3 = vAxis1 - (Vector3.Dot(vAxis1, vAxis4)) * vAxis4;
            vAxis3.Normalize();
            vAxis2.Set(0, 1, 0);
            float vAngleTorsoLateralNew = Vector3.Angle(vAxis3, vAxis2);
            float vAngularVelocityTorsoLateralNew = (vAngleTorsoLateralNew - Math.Abs(mAngleTorsoLateral)) / vTimeDifference;
            vAxis1.Set(mTorsoOrientation[0, 2], 0, mTorsoOrientation[2, 2]);


            //========================================================step 2==================================================//
            if ((Vector3.Dot(vAxis3, vAxis1) * Vector3.Dot(vAxis3, vAxis2)) < 0)
            {
                vAngleTorsoLateralNew = -vAngleTorsoLateralNew;
                vAngularVelocityTorsoLateralNew = -vAngularVelocityTorsoLateralNew;
            }

            mAngularAccelerationTorsoLateral = (vAngularVelocityTorsoLateralNew - mAngularVelocityTorsoLateral) / vTimeDifference;
            mAngularVelocityTorsoLateral = vAngularVelocityTorsoLateralNew;
            mAngleTorsoLateral = vAngleTorsoLateralNew;


            //=================================== calculate the Torso Rotational angle==================================================//

            //================================================step 1==================================================//
            vAxis1.Set(mTorsoOrientation[0, 2], 0, mTorsoOrientation[2, 2]);
            vAxis2.Set(0, 0, 1);
            float vAngleTorsoRotationNew = Vector3.Angle(vAxis1, vAxis2);
            float vAngularVelocityTorsoRotationNew = (vAngleTorsoRotationNew - Mathf.Abs(mAngleTorsoRotation)) / vTimeDifference;

            //================================================step 2==================================================//
            if (mTorsoOrientation[0, 2] < 0)
            {
                vAngleTorsoRotationNew = -vAngleTorsoRotationNew;
                vAngularVelocityTorsoRotationNew = -vAngularVelocityTorsoRotationNew;
            }
            //======================================  end calculate the Torso lateral angle ===========================================//
            //===========================================Turn detection===============================================================//
            if (Math.Abs(vAngleTorsoRotationNew) < 3)
            {
                mAngleIntegrationTurns = 0;
            }
            else
            {
                mAngleIntegrationTurns += (vAngularVelocityTorsoRotationNew * vTimeDifference);
            } 
            if (Math.Abs(mAngleIntegrationTurns) > 330)
            { 
                mAngleIntegrationTurns = 0;
                mNumberOfTurns++; 
            }
             
            //====================================End of turn detection=========================================//

            
            mAngularAccelerationTorsoRotation = (vAngularVelocityTorsoRotationNew - mAngularVelocityTorsoRotation) / vTimeDifference;
            mAngularVelocityTorsoRotation = vAngularVelocityTorsoRotationNew;
            mAngleTorsoRotation = vAngleTorsoRotationNew;
 

            //==============================calculate the Torso Vertical angle ============================//

            //=====================================step 1 ==========================================//
            vAxis1.Set(mTorsoOrientation[0, 1], mTorsoOrientation[1, 1], mTorsoOrientation[2, 1]);
            vAxis2.Set(0, 1, 0);
            float vAngleTorsoVerticalNew = Vector3.Angle(vAxis1, vAxis2);
            float vAngularVelocityTorsoVerticalNew = (vAngleTorsoVerticalNew - Math.Abs(mAngleTorsoVertical)) / vTimeDifference;



            if (mTorsoOrientation[1, 0] < 0)
            {
                vAngleTorsoVerticalNew = -vAngleTorsoVerticalNew;
                vAngularVelocityTorsoVerticalNew = -vAngularVelocityTorsoVerticalNew;
            } 
            //============================================beginning of Flip detection ===============================================//
            if (Math.Abs(vAngleTorsoVerticalNew) < 3)
            {
                mAngleIntegrationFlips = 0;
            }
            else
            {
                mAngleIntegrationFlips += (vAngularVelocityTorsoVerticalNew * vTimeDifference);
            }
            if (Math.Abs(mAngleIntegrationFlips) > 330)
            { 
                mNumberOfFlips++;
                mAngleIntegrationFlips = 0; 
            } 
            //===============================================End of Flip detection ===============================================//

            ///step 2///
            mAngularAccelerationTorsoVertical = (vAngularVelocityTorsoVerticalNew - mAngularVelocityTorsoVertical) / vTimeDifference;
            mAngularVelocityTorsoVertical = vAngularVelocityTorsoVerticalNew;
            mAngleTorsoVertical = vAngleTorsoVerticalNew; 
            //====================================== End Angle extraction ==============================================//

        }
        }
}
