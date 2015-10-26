/*
using Assets.Scripts.Body_Pipeline.Joints;
using Assets.Scripts.Utils.Mathtools;
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Tracking
{
    public class LeftArmTracker : Tracker
    {
        public LeftArmJointModel LeftArmJoint { get; set; }
        // current Upperarm (shoulder) and lower arm (Elbow) joints orientation, UpAr stands for upper arm and LoAr stands for Lower arm (forearm) in this code
        public float[,] UpArOrientation = new float[3, 3];
        public float[,] LoArOrientation = new float[3, 3];

        //	/**
        //* ArmOrientation()
        //*	@ This function calculates and updates the current orientation of the shoulder and elbow joints
        //	*	@param Vector3 NodUpAr, UpperArm Euler Angles Inputs
        //	*	@paramVector3  InitNodUpAr, this is the information of the initial frame for Upper Arm joint
        //	*	@param Vector3 NodLoAr, forearm Euler Angles Inputs
        //	*	@paramVector3  InitNodLoAr, this is the information of the initial frame for Torso joint
        //*	@return void
        //#1#
        public void ArmOrientation(Vector3 NodUpAr, Vector3 InitNodUpAr, Vector3 NodLoAr, Vector3 InitNodLoAr)
        {
            //Intermediate arrays until achieve final orientation for shoulder and elbow
            //they are Tagged with F (forward rotation) and B (Backward rotation) and are numbered consecutively
            // UpAr stands for upper arm sensor orientation and lower arm stands for lower arm (forearm) orientation

            float[,] UpArF1 = new float[3, 3];
            float[,] UpArF2 = new float[3, 3];
            float[,] UpArFi = new float[3, 3];


            float[,] UpArB1 = new float[3, 3];
            float[,] UpArB2 = new float[3, 3];
            float[,] UpArB3 = new float[3, 3];
            float[,] UpArB4 = new float[3, 3];
            float[,] UpArB5 = new float[3, 3];
            float[,] UpArB6 = new float[3, 3];
            float[,] UpArB7 = new float[3, 3];
            float[,] UpArBi = new float[3, 3];

            float[,] LoArF1 = new float[3, 3];
            float[,] LoArF2 = new float[3, 3];
            float[,] LoArFi = new float[3, 3];


            float[,] LoArB1 = new float[3, 3];
            float[,] LoArB2 = new float[3, 3];
            float[,] LoArB3 = new float[3, 3];
            float[,] LoArB4 = new float[3, 3];
            float[,] LoArB5 = new float[3, 3];
            float[,] LoArB6 = new float[3, 3];
            float[,] LoArB7 = new float[3, 3];
            float[,] LoArBi = new float[3, 3];

            float[,] CurrentLoArOrientation = new float[3, 3];

            /// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
            /// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////

            /////////// Converting to orientation matrix ///////////////////

            UpArB1 = MatrixHelper.RotationLocal(NodUpAr.z, NodUpAr.x, NodUpAr.y);
            UpArF1 = MatrixHelper.RotationGlobal(NodUpAr.z, NodUpAr.x, NodUpAr.y);

            LoArB1 = MatrixHelper.RotationLocal(NodLoAr.z, NodLoAr.x, NodLoAr.y);
            LoArF1 = MatrixHelper.RotationGlobal(NodLoAr.z, NodLoAr.x, NodLoAr.y);

            UpArBi = MatrixHelper.RotationLocal(InitNodUpAr.z, InitNodUpAr.x, InitNodUpAr.y);
            UpArFi = MatrixHelper.RotationGlobal(InitNodUpAr.z, InitNodUpAr.x, InitNodUpAr.y);

            LoArBi = MatrixHelper.RotationLocal(InitNodLoAr.z, InitNodLoAr.x, InitNodLoAr.y);
            LoArFi = MatrixHelper.RotationGlobal(InitNodLoAr.z, InitNodLoAr.x, InitNodLoAr.y);



            /////////// Initial Frame Adjustments ///////////////////

            //UpArB2 = multi(UpArFi, UpArB1);
            //LoArB2 = multi(LoArFi, LoArB1);
            UpArB3 = MatrixHelper.Multiply(UpArFi, UpArB1);
            LoArB4 = MatrixHelper.Multiply(LoArFi, LoArB1);

        }
    }
}
*/
