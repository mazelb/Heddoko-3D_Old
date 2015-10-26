using UnityEngine;
using System;
using System.Collections;
//using Nod;
using System.IO;

///// joint code for the Right leg
public class NodJointLegRight : NodJoint
{

    // current Hips and Knee joints orientation, 
    public static float[,] HipOrientation = new float[3, 3];
    public static float[,] KneeOrientation = new float[3, 3];

    //	/**
    //* LegOrientation()
    //*	@ This function calculates and updates the current orientation of the Hips and Knee
    //	*	@param Vector3 NodHip, Hip Euler Angles Inputs
    //	*	@param Vector3  InitNodHip, this is the information of the initial frame for Hip Nod sensor
    //	*	@param Vector3 NodKnee, knee Euler Angles Inputs
    //	*	@paramVector3  InitNodKnee, this is the information of the initial frame for Knee nod sensor
    //*	@return void
    //*/
    public void LegOrientation(Vector3 NodHip, Vector3 InitNodHip, Vector3 NodKnee, Vector3 InitNodKnee)
    {
        //Intermediate arrays until achieve final orientation for hip and knee, 
        //they are Tagged with F (forward rotation) and B (Backward rotation) and are numbered consecutively

        float[,] HipF1 = new float[3, 3];
        float[,] HipF2 = new float[3, 3];
        float[,] HipFi = new float[3, 3];


        float[,] HipB1 = new float[3, 3];
        float[,] HipB2 = new float[3, 3];
        float[,] HipB3 = new float[3, 3];
        float[,] HipB4 = new float[3, 3];
        float[,] HipB5 = new float[3, 3];
        float[,] HipB6 = new float[3, 3];
        float[,] HipB7 = new float[3, 3];
        float[,] HipB8 = new float[3, 3];
        float[,] HipBi = new float[3, 3];

        float[,] KneeF1 = new float[3, 3];
        float[,] KneeF2 = new float[3, 3];
        float[,] KneeFi = new float[3, 3];


        float[,] KneeB1 = new float[3, 3];
        float[,] KneeB2 = new float[3, 3];
        float[,] KneeB3 = new float[3, 3];
        float[,] KneeB4 = new float[3, 3];
        float[,] KneeB5 = new float[3, 3];
        float[,] KneeB6 = new float[3, 3];
        float[,] KneeB7 = new float[3, 3];
        float[,] KneeB8 = new float[3, 3];
        float[,] KneeBi = new float[3, 3];

        float[,] CurrentKneeOrientation = new float[3, 3];

        /// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
        /// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
        /// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
        /// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
        /// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
        /// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
        /// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////
        /// //////////////////////////////////////////////////  tracking /////////////////////////////////////////////////////////////////////

        /////////// Converting to rotation matrix ///////////////////

        HipB1 = RotationLocal(NodHip.z, NodHip.x, NodHip.y);
        HipF1 = RotationGlobal(NodHip.z, NodHip.x, NodHip.y);

        KneeB1 = RotationLocal(NodKnee.z, NodKnee.x, NodKnee.y);
        KneeF1 = RotationGlobal(NodKnee.z, NodKnee.x, NodKnee.y);

        HipBi = RotationLocal(InitNodHip.z, InitNodHip.x, InitNodHip.y);
        HipFi = RotationGlobal(InitNodHip.z, InitNodHip.x, InitNodHip.y);

        KneeBi = RotationLocal(InitNodKnee.z, InitNodKnee.x, InitNodKnee.y);
        KneeFi = RotationGlobal(InitNodKnee.z, InitNodKnee.x, InitNodKnee.y);



        /////////// Initial Frame Adjustments ///////////////////

        //HipB2 = multi(HipFi, HipB1);
        //KneeB2 = multi(KneeFi, KneeB1);
        HipB4 = multi(HipFi, HipB1);
        KneeB4 = multi(KneeFi, KneeB1);

        ///	/////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////

        ////////////////// setting Hip to Final Body orientation ///////////////////////////////

        Vector3 u = new Vector3(HipB4[0, 1], HipB4[1, 1], HipB4[2, 1]);
        CurrentKneeOrientation = RVector(u, (float)3.1415);

        HipB5 = multi(CurrentKneeOrientation, HipB4);



        u.Set(HipB5[0, 2], HipB5[1, 2], HipB5[2, 2]);
        CurrentKneeOrientation = RVector(u, (float)3.1415);

        HipB6 = multi(CurrentKneeOrientation, HipB5);

        u.Set(0, 0, 1);
        CurrentKneeOrientation = RVector(u, (float)3.1415);

        HipB7 = multi(CurrentKneeOrientation, HipB6);

        u.Set(0, 1, 0);
        CurrentKneeOrientation = RVector(u, (float)3.1415);

        HipOrientation = multi(CurrentKneeOrientation, HipB7);

        ////////////////// setting Knee to Final Body orientation ///////////////////////////////

        Vector3 u2 = new Vector3(KneeB4[0, 1], KneeB4[1, 1], KneeB4[2, 1]);
        CurrentKneeOrientation = RVector(u2, (float)3.1415);

        KneeB5 = multi(CurrentKneeOrientation, KneeB4);



        u2.Set(KneeB5[0, 2], KneeB5[1, 2], KneeB5[2, 2]);
        CurrentKneeOrientation = RVector(u2, (float)3.1415);

        KneeB6 = multi(CurrentKneeOrientation, KneeB5);

        u2.Set(0, 0, 1);
        CurrentKneeOrientation = RVector(u2, (float)3.1415);

        KneeB7 = multi(CurrentKneeOrientation, KneeB6);

        u2.Set(0, 1, 0);
        CurrentKneeOrientation = RVector(u2, (float)3.1415);

        KneeOrientation = multi(CurrentKneeOrientation, KneeB7);

    }

    /**
* RightLegMovement()
*	@This Function Anchors feet to the ground (enables sitting and squats)- will be called in NodJointTorso
*	@param float vUpperRightLegLength,is the Upper Leg Length of the person
*	@param float vLowerRightLegLength, is the Lower Leg Length
*	@return vRightLegHeight : Right leg vertical movement 
*/
    public static float RightLegMovement(float vUpperLegLength, float vLowerLegLength)
    {

        float vRightLegHeight = KneeOrientation[1, 1] * 0.5f * vLowerLegLength + HipOrientation[1, 1] * 0.5f * vUpperLegLength;

        return vRightLegHeight;

    }


    public override void UpdateJoint()
    {
        for (int ndx = 0; ndx < mNodSensors.Length; ndx++)
        {
            mNodSensors[ndx].UpdateSensor();
        }

        Vector3 vNodRawEuler1 = mNodSensors[0].curRotationRawEuler;
        Vector3 vNodRawEuler2 = mNodSensors[1].curRotationRawEuler;
        vNodRawEuler1 = new Vector3(vNodRawEuler1.x, vNodRawEuler1.y, vNodRawEuler1.z);
        vNodRawEuler2 = new Vector3(vNodRawEuler2.x, vNodRawEuler2.y, vNodRawEuler2.z);

        Vector3 NodIniEuler1 = mNodSensors[0].initRotationEuler;
        Vector3 NodIniEuler2 = mNodSensors[1].initRotationEuler;
        NodIniEuler1 = new Vector3(NodIniEuler1.x, NodIniEuler1.y, NodIniEuler1.z);
        NodIniEuler2 = new Vector3(NodIniEuler2.x, NodIniEuler2.y, NodIniEuler2.z);

        // getting the current Torso orientation for Hip angle extraction
        float[,] vaTorsoOrientation = new float[3, 3];
        vaTorsoOrientation = NodContainer.GetTorsoOrientation();

        // call LegOrientation function to calculate current orientation of leg joints
        LegOrientation(vNodRawEuler1, NodIniEuler1, vNodRawEuler2, NodIniEuler2);

        // convert leg orientations from 3*3 matrix to quaternion
        NodQuaternionOrientation vNodRawQuat = MatToQuat(HipOrientation);
        Quaternion vNodQuat = new Quaternion(vNodRawQuat.x, vNodRawQuat.y, vNodRawQuat.z, vNodRawQuat.w);
        Quaternion vJointQuat = inverseInitRotation * vNodQuat * Quaternion.Inverse(Quaternion.Euler(quaternionFactor));
        NodQuaternionOrientation vNodRawQuat2 = MatToQuat(KneeOrientation);
        Quaternion vNodQuat2 = new Quaternion(vNodRawQuat2.x, vNodRawQuat2.y, vNodRawQuat2.z, vNodRawQuat2.w);
        Quaternion vJointQuat2 = inverseInitRotation * vNodQuat2 * Quaternion.Inverse(Quaternion.Euler(quaternionFactor));

        if (jointTransform != null)
        {
            jointTransform.rotation = vJointQuat;
            jointTransform2.rotation = vJointQuat2;
        }

    }

}
